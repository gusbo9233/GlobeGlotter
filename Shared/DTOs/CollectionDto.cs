public sealed record CollectionDto 
(
    Guid Id,
    string Name,
    IReadOnlyList<QuizDto> Quizzes
);