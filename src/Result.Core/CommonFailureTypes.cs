namespace Result.Core;

/// <summary>
/// Common, cross-domain failure templates that are universally useful.
/// ID range 1-999 is reserved for these shared failures.
/// </summary>
public static class CommonFailureTypes
{
    // -----------------------------------------
    // 1–99: General errors
    // -----------------------------------------

    /// <summary>
    /// Generic unknown error for unexpected failures.
    /// </summary>
    public static readonly FailureTemplate Unknown = new(
        new FailureCode(1, "COMMON.UNKNOWN"),
        "An unknown error occurred.",
        SelfCreated: false
    );

    /// <summary>
    /// Generic internal server error.
    /// </summary>
    public static readonly FailureTemplate InternalError = new(
        new FailureCode(2, "COMMON.INTERNAL_ERROR"),
        "An internal server error occurred."
    );

    /// <summary>
    /// Generic validation error without further detail.
    /// </summary>
    public static readonly FailureTemplate OperationFailed = new(
        new FailureCode(3, "COMMON.OPERATION_FAILED"),
        "The requested operation could not be completed."
    );


    // -----------------------------------------
    // 100–199: Validation
    // -----------------------------------------

    /// <summary>
    /// Indicates that a value is outside of the allowed range.
    /// </summary>
    public static readonly FailureTemplate ValidationError = new(
        new FailureCode(100, "COMMON.VALIDATION_ERROR"),
        "Validation failed."
    );

    /// <summary>
    /// Indicates that a value has an invalid format.
    /// </summary>
    public static readonly FailureTemplate InvalidFormat = new(
        new FailureCode(101, "COMMON.INVALID_FORMAT"),
        "The value '{0}' has an invalid format."
    );

    /// <summary>
    /// Indicates that a value is outside of the allowed range.
    /// </summary>
    public static readonly FailureTemplate ValueOutOfRange = new(
        new FailureCode(102, "COMMON.VALUE_OUT_OF_RANGE"),
        "The value '{0}' is out of the allowed range."
    );

    /// <summary>
    /// Indicates that a required field is missing.
    /// </summary>
    public static readonly FailureTemplate RequiredFieldMissing = new(
        new FailureCode(103, "COMMON.REQUIRED_FIELD_MISSING"),
        "The required field '{0}' is missing."
    );

    /// <summary>
    /// Indicates that a provided value is not a valid enum option.
    /// </summary>
    public static readonly FailureTemplate InvalidEnumValue = new(
        new FailureCode(104, "COMMON.INVALID_ENUM_VALUE"),
        "The value '{0}' is not a valid option."
    );


    // -----------------------------------------
    // 200–299: Authentication & Authorization
    // -----------------------------------------

    /// <summary>
    /// The caller is not authorized to perform the requested action.
    /// </summary>
    public static readonly FailureTemplate NotAuthorized = new(
        new FailureCode(200, "COMMON.NOT_AUTHORIZED"),
        "You are not authorized to perform this action."
    );

    /// <summary>
    /// The caller must be authenticated before accessing this resource.
    /// </summary>
    public static readonly FailureTemplate NotAuthenticated = new(
        new FailureCode(201, "COMMON.NOT_AUTHENTICATED"),
        "You must be authenticated to perform this action."
    );

    /// <summary>
    /// Access to the requested resource is forbidden.
    /// </summary>
    public static readonly FailureTemplate Forbidden = new(
        new FailureCode(202, "COMMON.FORBIDDEN"),
        "You do not have permission to access this resource."
    );

    /// <summary>
    /// The authentication token has expired.
    /// </summary>
    public static readonly FailureTemplate TokenExpired = new(
        new FailureCode(203, "COMMON.TOKEN_EXPIRED"),
        "The authentication token has expired."
    );

    /// <summary>
    /// The provided credentials are invalid.
    /// </summary>
    public static readonly FailureTemplate InvalidCredentials = new(
        new FailureCode(204, "COMMON.INVALID_CREDENTIALS"),
        "The provided credentials are invalid."
    );


    // -----------------------------------------
    // 300–399: Data & Storage
    // -----------------------------------------

    /// <summary>
    /// No data was found for the given key or query.
    /// </summary>
    public static readonly FailureTemplate NoDataFound = new(
        new FailureCode(300, "COMMON.NO_DATA_FOUND"),
        "No data found for '{0}'."
    );

    /// <summary>
    /// The specified parameter was not found.
    /// </summary>
    public static readonly FailureTemplate ParameterNotFound = new(
        new FailureCode(301, "COMMON.PARAMETER_NOT_FOUND"),
        "The parameter '{0}' was not found."
    );

    /// <summary>
    /// A concurrency conflict occurred while updating the resource.
    /// </summary>
    public static readonly FailureTemplate ConcurrencyConflict = new(
        new FailureCode(302, "COMMON.CONCURRENCY_CONFLICT"),
        "A concurrency conflict occurred while updating the resource."
    );

    /// <summary>
    /// The resource already exists and cannot be created again.
    /// </summary>
    public static readonly FailureTemplate AlreadyExists = new(
        new FailureCode(303, "COMMON.ALREADY_EXISTS"),
        "The resource '{0}' already exists."
    );

    /// <summary>
    /// A value that must be unique is already in use.
    /// </summary>
    public static readonly FailureTemplate NotUnique = new(
        new FailureCode(304, "COMMON.NOT_UNIQUE"),
        "The value '{0}' must be unique."
    );

    /// <summary>
    /// Stored data appears to be corrupted or inconsistent.
    /// </summary>
    public static readonly FailureTemplate DataCorrupted = new(
        new FailureCode(305, "COMMON.DATA_CORRUPTED"),
        "The stored data appears to be corrupted."
    );


    // -----------------------------------------
    // 400–499: External services / network
    // -----------------------------------------

    /// <summary>
    /// An external service is currently unavailable.
    /// </summary>
    public static readonly FailureTemplate ExternalServiceUnavailable = new(
        new FailureCode(400, "COMMON.EXTERNAL_SERVICE_UNAVAILABLE"),
        "The external service '{0}' is currently unavailable."
    );

    /// <summary>
    /// A call to an external service failed.
    /// </summary>
    public static readonly FailureTemplate ExternalCallFailed = new(
        new FailureCode(401, "COMMON.EXTERNAL_CALL_FAILED"),
        "The call to '{0}' failed with error: {1}."
    );

    /// <summary>
    /// The operation exceeded the configured timeout.
    /// </summary>
    public static readonly FailureTemplate Timeout = new(
        new FailureCode(402, "COMMON.TIMEOUT"),
        "The operation timed out."
    );

    /// <summary>
    /// A network error occurred while accessing a remote resource.
    /// </summary>
    public static readonly FailureTemplate NetworkError = new(
        new FailureCode(403, "COMMON.NETWORK_ERROR"),
        "A network error occurred while accessing '{0}'."
    );


    // -----------------------------------------
    // 500–599: Business logic / domain-general
    // -----------------------------------------

    /// <summary>
    /// The action has already been performed and cannot be repeated.
    /// </summary>
    public static readonly FailureTemplate ActionAlreadyPerformed = new(
        new FailureCode(500, "COMMON.ACTION_ALREADY_PERFORMED"),
        "The action has already been performed and cannot be repeated."
    );

    /// <summary>
    /// One or more preconditions for executing the action were not met.
    /// </summary>
    public static readonly FailureTemplate PreconditionsNotMet = new(
        new FailureCode(501, "COMMON.PRECONDITIONS_NOT_MET"),
        "One or more preconditions for executing this action were not met."
    );

    /// <summary>
    /// The resource is in an invalid state for the requested operation.
    /// </summary>
    public static readonly FailureTemplate InvalidState = new(
        new FailureCode(502, "COMMON.INVALID_STATE"),
        "The resource is in an invalid state for this operation."
    );

    /// <summary>
    /// The selected resource is temporarily locked and cannot be modified.
    /// </summary>
    public static readonly FailureTemplate ResourceLocked = new(
        new FailureCode(503, "COMMON.RESOURCE_LOCKED"),
        "The resource '{0}' is temporarily locked."
    );
}
