namespace Int2Roman;

public record Mapper
{
    /// <summary>
    /// Non zero integer representation
    /// </summary>
    public required uint Number { get; init; }
    /// <summary>
    /// Roman number representation
    /// </summary>
    public required string Roman { get; init; }
}