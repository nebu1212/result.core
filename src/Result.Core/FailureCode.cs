namespace Result.Core;

/// <summary>
/// Identifies a specific failure with a fast-to-compare numeric identifier
/// and a stable, human-readable string code.
/// </summary>
public readonly record struct FailureCode(int Id, string Code)
{
    /// <summary>
    /// Numeric identifier for fast comparisons, switching and dictionary lookups.
    /// </summary>
    public int Id { get; } = Id;

    /// <summary>
    /// Stable, human-readable failure code (e.g. "COMMON.NO_DATA_FOUND").
    /// </summary>
    public string Code { get; } = Code;

    /// <summary>
    /// Equality is intentionally based on <see cref="Id"/> only for performance.
    /// </summary>
    public bool Equals(FailureCode other) => Id == other.Id;

    /// <inheritdoc />
    public override int GetHashCode() => Id;

    /// <inheritdoc />
    public override string ToString() => Code;
}
