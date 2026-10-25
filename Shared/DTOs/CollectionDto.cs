namespace Shared.DTOs;

public sealed record CollectionDto(
    Guid Id,
    string Name,
    List<QuizDto> Quizzes,
    List<CollectionDto> Collections
);