namespace Result.Core;

/// <summary>
/// Defines a reusable message template for generating concrete <see cref="Failure"/> instances.
/// Use this to declare shared failure types like "NO_DATA_FOUND" or "NOT_AUTHORIZED".
/// </summary>
public sealed record FailureTemplate(
    FailureCode Code,
    string MessageTemplate,
    bool SelfCreated = true)
{
    /// <summary>
    /// Creates a <see cref="Failure"/> without any formatting arguments.
    /// The <see cref="MessageTemplate"/> is used as-is.
    /// </summary>
    /// <param name="trace">Optional trace or diagnostic information.</param>
    public Failure Create(string? trace = null)
        => new(Code, MessageTemplate, trace, SelfCreated);

    /// <summary>
    /// Creates a <see cref="Failure"/> using a custom message instead of the template.
    /// This is useful when the error message is built elsewhere, but you still want
    /// to keep a consistent failure code.
    /// </summary>
    /// <param name="message">The final message to use instead of the template.</param>
    /// <param name="trace">Optional trace or diagnostic information.</param>
    public Failure CreateWithMessage(string message, string? trace = null)
        => new(Code, message, trace, SelfCreated);

    /// <summary>
    /// Creates a <see cref="Failure"/> by formatting the template with
    /// <see cref="string.Format(string,object?[])"/> using the given arguments.
    /// </summary>
    /// <param name="trace">Optional trace or diagnostic information.</param>
    /// <param name="args">Arguments used to format the message template.</param>
    public Failure CreateWithArgs(string? trace = null, params object?[] args)
    {
        var message = (args is null || args.Length == 0)
            ? MessageTemplate
            : string.Format(MessageTemplate, args);

        return new Failure(Code, message, trace, SelfCreated);
    }
}
