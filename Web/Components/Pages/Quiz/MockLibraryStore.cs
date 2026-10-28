
using Shared.DTOs;

namespace Web.Components.Quiz;

public sealed class MockLibraryStore
{
    public const int MaxDepth = 5;

    public List<CollectionDto> RootCollections { get; } = [];
    public List<QuizDto> RootQuizzes { get; } = [];

    public MockLibraryStore()
    {
        RootQuizzes.AddRange(
        [
            new QuizDto(
                Guid.NewGuid(),
                "Common Verbs",
                [
                    new WordDto(Guid.NewGuid(), "Comer", "To eat"),
                    new WordDto(Guid.NewGuid(), "Beber", "To drink")
                ]),
            new QuizDto(
                Guid.NewGuid(),
                "Daily Phrases",
                [
                    new WordDto(Guid.NewGuid(), "Hola", "Hello"),
                    new WordDto(Guid.NewGuid(), "Gracias", "Thank you")
                ])
        ]);

        RootCollections.AddRange(
        [
            new CollectionDto(
                Guid.NewGuid(),
                "Spanish",
                [
                    new QuizDto(
                        Guid.NewGuid(),
                        "Spanish Basics",
                        [
                            new WordDto(Guid.NewGuid(), "Gato", "Cat"),
                            new WordDto(Guid.NewGuid(), "Perro", "Dog")
                        ])
                ],
                [
                    new CollectionDto(
                        Guid.NewGuid(),
                        "Grammar",
                        [
                            new QuizDto(
                                Guid.NewGuid(),
                                "Present Tense",
                                [
                                    new WordDto(Guid.NewGuid(), "Soy", "I am"),
                                    new WordDto(Guid.NewGuid(), "Tengo", "I have")
                                ])
                        ],
                        [])
                ]),
            new CollectionDto(
                Guid.NewGuid(),
                "French",
                [
                    new QuizDto(
                        Guid.NewGuid(),
                        "French Basics",
                        [new WordDto(Guid.NewGuid(), "Bonjour", "Hello")])
                ],
                [])
        ]);
    }

    // A path length of 1 means a level-1 collection.
    public List<CollectionDto>? GetPath(Guid collectionId)
    {
        List<CollectionDto> path = [];
        return TryFindPath(RootCollections, collectionId, path)
            ? path
            : null;
    }

    private static bool TryFindPath(
        IEnumerable<CollectionDto> collections,
        Guid id,
        List<CollectionDto> path)
    {
        foreach (var collection in collections)
        {
            path.Add(collection);

            if (collection.Id == id ||
                TryFindPath(collection.Collections, id, path))
            {
                return true;
            }

            path.RemoveAt(path.Count - 1);
        }

        return false;
    }

    public bool AddCollection(Guid? parentId, string name)
    {
        name = name.Trim();
        if (name.Length is 0 or > 100)
            return false;

        List<CollectionDto> target = RootCollections;

        if (parentId is Guid id)
        {
            var path = GetPath(id);
            if (path is null || path.Count >= MaxDepth)
                return false;

            target = path[^1].Collections;
        }

        target.Add(new CollectionDto(Guid.NewGuid(), name, [], []));
        return true;
    }

    // Removing the parent also removes its entire in-memory subtree.
    public bool DeleteCollection(Guid collectionId) =>
        RemoveCollection(RootCollections, collectionId);

    private static bool RemoveCollection(
        List<CollectionDto> collections,
        Guid id)
    {
        var match = collections.FirstOrDefault(c => c.Id == id);
        if (match is not null)
            return collections.Remove(match);

        foreach (var collection in collections)
        {
            if (RemoveCollection(collection.Collections, id))
                return true;
        }

        return false;
    }

    public bool AddQuiz(Guid? collectionId, string name)
    {
        name = name.Trim();
        if (name.Length is 0 or > 100)
            return false;

        List<QuizDto> target = RootQuizzes;

        if (collectionId is Guid id)
        {
            var path = GetPath(id);
            if (path is null)
                return false;

            target = path[^1].Quizzes;
        }

        // QuizDto.Words is IReadOnlyList<WordDto>.
        target.Add(new QuizDto(Guid.NewGuid(), name, []));
        return true;
    }

    public bool DeleteQuiz(Guid? collectionId, Guid quizId)
    {
        List<QuizDto> target = RootQuizzes;

        if (collectionId is Guid id)
        {
            var path = GetPath(id);
            if (path is null)
                return false;

            target = path[^1].Quizzes;
        }

        var match = target.FirstOrDefault(q => q.Id == quizId);
        return match is not null && target.Remove(match);
    }

    // Supports the optional quiz detail page.
    public QuizDto? FindQuiz(Guid quizId)
    {
        var rootMatch = RootQuizzes.FirstOrDefault(q => q.Id == quizId);
        return rootMatch ?? FindQuizInCollections(RootCollections, quizId);
    }

    private static QuizDto? FindQuizInCollections(
        IEnumerable<CollectionDto> collections,
        Guid quizId)
    {
        foreach (var collection in collections)
        {
            var match = collection.Quizzes.FirstOrDefault(q => q.Id == quizId);
            if (match is not null)
                return match;

            var nested = FindQuizInCollections(collection.Collections, quizId);
            if (nested is not null)
                return nested;
        }

        return null;
    }


    public bool AddWord(Guid id, string word, string translation)
    {
        if (string.IsNullOrWhiteSpace(word) ||
            string.IsNullOrWhiteSpace(translation))
            return false;

        var quiz = FindQuiz(id);

        if (quiz is null)
            return false;

        var newWord = new WordDto(
            Guid.NewGuid(),
            word.Trim(),
            translation.Trim()
        );

        var updatedQuiz = quiz with
        {
            Words = quiz.Words.Append(newWord).ToList()
        };

        return UpdateQuiz(updatedQuiz);
    }

    private bool UpdateQuiz(QuizDto updatedQuiz)
    {
        var index = RootQuizzes.FindIndex(
            q => q.Id == updatedQuiz.Id);

        if (index >= 0)
        {
            RootQuizzes[index] = updatedQuiz;
            return true;
        }

        return UpdateQuizInCollections(
            RootCollections, updatedQuiz);
    }

    private static bool UpdateQuizInCollections(
        IEnumerable<CollectionDto> collections,
        QuizDto updatedQuiz)
    {
        foreach (var collection in collections)
        {
            var index = collection.Quizzes.FindIndex(
                q => q.Id == updatedQuiz.Id);

            if (index >= 0)
            {
                collection.Quizzes[index] = updatedQuiz;
                return true;
            }

            if (UpdateQuizInCollections(
                collection.Collections, updatedQuiz))
            {
                return true;
            }
        }

        return false;
    }

    public bool DeleteWord(Guid quizId, Guid wordId)
    {
        var quiz = FindQuiz(quizId);

        if (quiz is null ||
            !quiz.Words.Any(w => w.Id == wordId))
            return false;

        var updatedQuiz = quiz with
        {
            Words = quiz.Words
                .Where(w => w.Id != wordId)
                .ToList()
        };

        return UpdateQuiz(updatedQuiz);
    }

    public bool UpdateWord(
        Guid quizId,
        Guid wordId,
        string original,
        string translation)
    {
        if (string.IsNullOrWhiteSpace(original) ||
            string.IsNullOrWhiteSpace(translation))
            return false;

        var quiz = FindQuiz(quizId);

        if (quiz is null ||
            !quiz.Words.Any(w => w.Id == wordId))
            return false;

        var updatedQuiz = quiz with
        {
            Words = quiz.Words.Select(w =>
                w.Id == wordId
                    ? w with
                    {
                        Original = original.Trim(),
                        Translation = translation.Trim()
                    }
                    : w
            ).ToList()
        };

        return UpdateQuiz(updatedQuiz);
    }


}
