# Result.Core

A lightweight, dependency-free `Result<T, E>` library for modern .NET. Inspired by Rust's `Result`, it stays idiomatic to C# with structured failure metadata and reusable templates.

---

## Why Result.Core?

- Replace exceptions for **expected** control-flow paths with explicit successes and failures.
- Keep domain logic clear in application services, APIs, authentication flows, and data access layers.
- Share consistent failure templates across bounded contexts (e.g., multiple microservices).
- Small, focused API with no runtime dependencies.

---

## Installation

Use the NuGet package name `Result.Core`:

```bash
dotnet add package Result.Core
```

Or reference it in your project file:

```xml
<ItemGroup>
  <PackageReference Include="Result.Core" Version="1.0.0" />
</ItemGroup>
```

---

## Core types

### Result<T, E>
A discriminated-union-like type that represents either:

- a successful value `Value` of type `T`, or
- an error `Error` of type `E`.

Key members:

- Flags: `IsSuccess`, `IsFailure`
- Factories: `Result<T, E>.Success(T)`, `Result<T, E>.Failure(E)`
- Implicit conversions from `T` and `E`
- Operations: `Map`, `Bind` (a.k.a. `FlatMap`), `Match`, `Unwrap`, `UnwrapOrDefault`, `TryUnwrap`, `OnSuccess`, `OnFailure`

### Failure model

- **FailureCode**: `int Id`, `string Code`; equality and hashing are based on `Id`.
- **FailureTemplate**: reusable definition that produces `Failure` instances via `Create`, `CreateWithMessage`, and `CreateWithArgs`.
- **Failure**: concrete error instance (typically used as `E` in `Result<T, E>`), with `FailureCode Code`, `string Message`, optional `string? Trace`, and `bool SelfCreated`.
- **CommonFailureTypes**: curated `FailureTemplate` catalog for general, validation, authentication/authorization, data/storage, external/network, and business/domain failures.

---

## Quick start
The following example shows a downstream authentication call that can:

- return `1` → authenticated user (success),
- return `0` → unauthenticated (maps to a `NotAuthenticated` failure),
- throw an exception → wrapped into a `Failure` with trace information.

```csharp
using Result.Core;

Result<int, Failure> VerifyUser(string token)
{
    try
    {
        int status = ExternalAuthProvider.Check(token); // can be 1, 0, or throw

        return status switch
        {
            1 => status, // implicit Success
            0 => CommonFailureTypes.NotAuthenticated.CreateWithMessage(
                "Token is missing or expired."),
            _ => Result<int, Failure>.Failure(new Failure(
                new FailureCode(999, "AUTH.UNEXPECTED_STATUS"),
                $"Unexpected authentication status '{status}'."))
        };
    }
    catch (Exception ex)
    {
        // Wrap the exception into a Failure with trace info
        return Result<int, Failure>.Failure(new Failure(
            new FailureCode(400, "AUTH.EXTERNAL_ERROR"),
            "External authentication failed.",
            trace: ex.ToString()));
    }
}

var authResult = VerifyUser("Bearer 123");

// Explicit branching without throwing:
if (authResult.TryUnwrap(out var userId, out var authFailure))
{
    Console.WriteLine($"User authenticated: {userId}");
}
else
{
    Console.WriteLine($"Auth failed: {authFailure!.Message}");
}

// Mapping to HTTP responses in a controller (Minimal API example)
IResult actionResult = authResult.IsSuccess
    ? Results.Ok()
    : authResult.Error!.Code.Code switch
    {
        "COMMON.NOT_AUTHENTICATED" => Results.Unauthorized(),
        "AUTH.EXTERNAL_ERROR"      => Results.StatusCode(503),
        _                          => Results.StatusCode(500)
    };
```

---

## Working with results

### Transform and chain
```csharp
using Result.Core;

var userResult = Result<int, Failure>.Success(1)
    .Bind(id => id == 1
        ? "Ruffallow"
        : CommonFailureTypes.NoDataFound.CreateWithArgs(id))
    .Map(name => name.ToUpperInvariant());

if (userResult.TryUnwrap(out var user, out var failure))
{
    Console.WriteLine(user);
}
else
{
    Console.WriteLine(failure!.Message);
}

// Unwrap throws on failure; UnwrapOrDefault returns a fallback:
string forced  = userResult.Unwrap();
string fallback = userResult.UnwrapOrDefault("UNKNOWN");
```

### Side effects via OnSuccess / OnFailure
```csharp
using Result.Core;

Result<string, Failure> LoadUserName(int id)
{
    return Result<int, Failure>.Success(id)
        .Bind(userId => userId == 7
            ? "Captain"
            : CommonFailureTypes.NoDataFound.CreateWithArgs(userId))
        .OnSuccess(name => Console.WriteLine($"Loaded {name}"))
        .OnFailure(err => Console.Error.WriteLine($"Error loading user: {err.Message}"));
}

var nameResult = LoadUserName(7)
    .Map(name => name.ToUpperInvariant());

// Convert failures to exceptions only when explicitly desired:
string finalName = nameResult.Match(
    onSuccess: n => n,
    onFailure: err => new InvalidOperationException(err.Message));
```

---

## Creating failures

### Using built-in templates
```csharp
using Result.Core;

// Simple authorization error
var unauthorized = CommonFailureTypes.NotAuthorized.Create();

// Parameter not found with formatted message
var missingParam = CommonFailureTypes.ParameterNotFound.CreateWithArgs("userId");
```

### One-off failures
```csharp
var paymentFailure = new Failure(
    code:    new FailureCode(450, "PAYMENT.REJECTED"),
    message: "Payment was rejected by issuer."
);
```

### Your own reusable templates
```csharp
using Result.Core;

static class CheckoutFailureTypes
{
    public static readonly FailureTemplate StockTooLow = new(
        new FailureCode(701, "CHECKOUT.STOCK_TOO_LOW"),
        "Not enough stock for item '{0}'."
    );
}

Result<string, Failure> paymentResult = Result<string, Failure>.Failure(
    CheckoutFailureTypes.StockTooLow.CreateWithArgs("ITEM-123")
);
```

### Wrap exceptions into failures
```csharp
using Result.Core;

Result<int, Failure> SafeParseInt(string input)
{
    try
    {
        return int.Parse(input); // implicit Success
    }
    catch (Exception ex)
    {
        return new Failure(
            new FailureCode(101, "VALIDATION.INVALID_NUMBER"),
            $"Could not parse '{input}': {ex.Message}"
        );
    }
}
```

---

## Common failures catalog
CommonFailureTypes reserves well-known ID ranges for cross-domain use:

- **1–99** General: Unknown, InternalError, OperationFailed
- **100–199** Validation: ValidationError, InvalidFormat, ValueOutOfRange, RequiredFieldMissing, InvalidEnumValue
- **200–299** Authentication/Authorization: NotAuthorized, NotAuthenticated, Forbidden, TokenExpired, InvalidCredentials
- **300–399** Data/Storage: NoDataFound, ParameterNotFound, ConcurrencyConflict, AlreadyExists, NotUnique, DataCorrupted
- **400–499** External/Network: ExternalServiceUnavailable, ExternalCallFailed, Timeout, NetworkError
- **500–599** Business: ActionAlreadyPerformed, PreconditionsNotMet, InvalidState, ResourceLocked

---

## Project structure
```text
Result.Core/
  src/Result.Core/           # Library project
  tests/Result.Core.Tests/   # xUnit test project
  LICENSE
  README.md
  Result.Core.sln
```

## Contributing
Issues and pull requests are welcome. If you add new common failure templates, please document their ID ranges and intended usage.

## Support
If you find this library useful, please consider starring the repository. It helps others discover the project and motivates further improvements.
