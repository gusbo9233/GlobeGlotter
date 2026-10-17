public sealed record QuizDto
(
    Guid int Id,
    string Name,
    IReadOnlyList<Word> Words
);