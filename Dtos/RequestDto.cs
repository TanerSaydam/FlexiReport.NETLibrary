namespace FlexiReport.Dtos;

public sealed record RequestDto(
    DateOnly StartDate,
    DateOnly EndDate,
    string? Search);
