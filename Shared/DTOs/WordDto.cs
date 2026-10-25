namespace Shared.DTOs;

public sealed record WordDto 
(
    Guid Id,
    string Original,
    string Translation
);