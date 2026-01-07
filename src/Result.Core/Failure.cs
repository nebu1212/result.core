namespace Result.Core;

/// <summary>
/// Represents a concrete failure instance created from a <see cref="FailureTemplate"/>.
/// This is typically used as the error type in <see cref="Result{T,E}"/>.
/// </summary>
public sealed record Failure(
    FailureCode Code,
    string Message,
    string? Trace = null,
    bool SelfCreated = false)
{
    /// <summary>
    /// A short, human-readable representation of the failure,
    /// useful for logging and debugging.
    /// </summary>
    public override string ToString()
        => $"[{Code}] {Message} (SelfCreated: {SelfCreated}, Trace: {Trace ?? "n/a"})";
}
