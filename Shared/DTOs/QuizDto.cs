namespace Shared.DTOs;

public sealed record QuizDto
(
    Guid Id,
    string Name,
    IReadOnlyList<WordDto> Words
);