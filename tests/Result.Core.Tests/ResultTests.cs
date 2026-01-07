using System;
using Result.Core;
using Xunit;

namespace Result.Core.Tests;

/// <summary>
/// Extensive tests for Result&lt;T,E&gt; based on the current implementation.
/// </summary>
public class ResultTests
{
    // --------------------------------------------------------------------
    // 1. Construction & basic flags
    // --------------------------------------------------------------------

    [Fact]
    public void Success_Should_Create_Success_Result()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = Result<int, string>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Equal(default, result.Error);
    }

    [Fact]
    public void Failure_Should_Create_Failure_Result()
    {
        // Arrange
        const string error = "something went wrong";

        // Act
        var result = Result<int, string>.Failure(error);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(default, result.Value); // default(int) == 0
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Implicit_Conversion_From_Value_Should_Create_Success_Result()
    {
        // Arrange
        const int value = 42;

        // Act
        Result<int, string> result = value; // eindeutig: T=int, E=string

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Equal(default, result.Error);
    }


    [Fact]
    public void Implicit_Conversion_From_Error_Should_Create_Failure_Result()
    {
        // Arrange
        const string error = "error";

        // Act
        Result<int, string> result = error; // eindeutig: kommt von E=string

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(default, result.Value);
        Assert.Equal(error, result.Error);
    }


    // --------------------------------------------------------------------
    // 2. Match(Func<T, Result<T,E>>, Func<E, Result<T,E>>)
    // --------------------------------------------------------------------

    [Fact]
    public void Match_ResultToResult_Should_Invoke_OnSuccess_When_Success()
    {
        // Arrange
        var initial = Result<int, string>.Success(10);

        var successCalled = false;
        var failureCalled = false;

        // Act
        var final = initial.Match(
            onSuccess: v =>
            {
                successCalled = true;
                return Result<int, string>.Success(v + 1);
            },
            onFailure: e =>
            {
                failureCalled = true;
                return Result<int, string>.Failure(e);
            });

        // Assert
        Assert.True(successCalled);
        Assert.False(failureCalled);

        Assert.True(final.IsSuccess);
        Assert.Equal(11, final.Value);
        Assert.Equal(default, final.Error);
    }

    [Fact]
    public void Match_ResultToResult_Should_Invoke_OnFailure_When_Failure()
    {
        // Arrange
        var error = "not found";
        var initial = Result<int, string>.Failure(error);

        var successCalled = false;
        var failureCalled = false;

        // Act
        var final = initial.Match(
            onSuccess: v =>
            {
                successCalled = true;
                return Result<int, string>.Success(v);
            },
            onFailure: e =>
            {
                failureCalled = true;
                return Result<int, string>.Failure(e + "!");
            });

        // Assert
        Assert.False(successCalled);
        Assert.True(failureCalled);

        Assert.True(final.IsFailure);
        Assert.Equal(default, final.Value);
        Assert.Equal(error + "!", final.Error);
    }

    // --------------------------------------------------------------------
    // 3. Match(Func<T,T>, Func<E,Exception>)
    // --------------------------------------------------------------------

    [Fact]
    public void Match_ToValue_Should_Return_Transformed_Value_On_Success()
    {
        // Arrange
        var result = Result<int, string>.Success(5);

        // Act
        var final = result.Match(
            onSuccess: v => v * 2,
            onFailure: e => new InvalidOperationException(e));

        // Assert
        Assert.Equal(10, final);
    }

    [Fact]
    public void Match_ToValue_Should_Throw_Exception_From_OnFailure_When_Failure()
    {
        // Arrange
        const string error = "boom";
        var result = Result<int, string>.Failure(error);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() =>
            result.Match(
                onSuccess: v => v,
                onFailure: e => new InvalidOperationException($"error: {e}")));

        // Assert
        Assert.Contains("error: boom", ex.Message);
    }

    // --------------------------------------------------------------------
    // 4. Unwrap()
    // --------------------------------------------------------------------

    [Fact]
    public void Unwrap_Should_Return_Value_On_Success()
    {
        // Arrange
        var result = Result<string, string>.Success("ok");

        // Act
        var value = result.Unwrap();

        // Assert
        Assert.Equal("ok", value);
    }

    [Fact]
    public void Unwrap_Should_Throw_InvalidOperationException_On_Failure()
    {
        // Arrange
        const string error = "failed";
        var result = Result<string, string>.Failure(error);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => result.Unwrap());

        // Assert
        Assert.Contains("Unwrap failed", ex.Message);
        Assert.Contains(error, ex.Message);
    }

    // --------------------------------------------------------------------
    // 5. UnwrapOrDefault()
    // --------------------------------------------------------------------

    [Fact]
    public void UnwrapOrDefault_Should_Return_Value_On_Success()
    {
        // Arrange
        var result = Result<int, string>.Success(99);

        // Act
        var value = result.UnwrapOrDefault(-1);

        // Assert
        Assert.Equal(99, value);
    }

    [Fact]
    public void UnwrapOrDefault_Should_Return_Default_On_Failure()
    {
        // Arrange
        var result = Result<int, string>.Failure("nope");

        // Act
        var value = result.UnwrapOrDefault(123);

        // Assert
        Assert.Equal(123, value);
    }

    // --------------------------------------------------------------------
    // 6. TryUnwrap()
    // --------------------------------------------------------------------

    [Fact]
    public void TryUnwrap_Should_Return_True_And_Set_Value_On_Success()
    {
        // Arrange
        var result = Result<int, string>.Success(7);

        // Act
        var success = result.TryUnwrap(out var value, out var error);

        // Assert
        Assert.True(success);
        Assert.Equal(7, value);
        Assert.Equal(default, error);
    }

    [Fact]
    public void TryUnwrap_Should_Return_False_And_Set_Error_On_Failure()
    {
        // Arrange
        var result = Result<int, string>.Failure("err");

        // Act
        var success = result.TryUnwrap(out var value, out var error);

        // Assert
        Assert.False(success);
        Assert.Equal(default, value);
        Assert.Equal("err", error);
    }

    // --------------------------------------------------------------------
    // 7. Map()
    // --------------------------------------------------------------------

    [Fact]
    public void Map_Should_Transform_Value_On_Success()
    {
        // Arrange
        var result = Result<int, string>.Success(3);

        // Act
        var mapped = result.Map(v => $"Value: {v}");

        // Assert
        Assert.True(mapped.IsSuccess);
        Assert.Equal("Value: 3", mapped.Value);
        Assert.Equal(default, mapped.Error);
    }

    [Fact]
    public void Map_Should_Propagate_Error_On_Failure()
    {
        // Arrange
        var result = Result<int, string>.Failure("error");

        // Act
        var mapped = result.Map(v => $"Value: {v}");

        // Assert
        Assert.True(mapped.IsFailure);
        Assert.Equal(default, mapped.Value);
        Assert.Equal("error", mapped.Error);
    }

    // --------------------------------------------------------------------
    // 8. Bind()
    // --------------------------------------------------------------------

    [Fact]
    public void Bind_Should_Invoke_Binder_On_Success()
    {
        // Arrange
        var result = Result<int, string>.Success(10);

        // Act
        var bound = result.Bind(v =>
        {
            if (v > 5)
                return Result<string, string>.Success("big");
            return Result<string, string>.Failure("too small");
        });

        // Assert
        Assert.True(bound.IsSuccess);
        Assert.Equal("big", bound.Value);
    }

    [Fact]
    public void Bind_Should_Skip_Binder_And_Propagate_Error_On_Failure()
    {
        // Arrange
        var result = Result<int, string>.Failure("original error");
        var binderCalled = false;

        // Act
        var bound = result.Bind(v =>
        {
            binderCalled = true;
            return Result<string, string>.Success("should not happen");
        });

        // Assert
        Assert.False(binderCalled);
        Assert.True(bound.IsFailure);
        Assert.Equal(default, bound.Value);
        Assert.Equal("original error", bound.Error);
    }

    // --------------------------------------------------------------------
    // 9. OnSuccess / OnFailure
    // --------------------------------------------------------------------

    [Fact]
    public void OnSuccess_Should_Execute_Action_On_Success_And_Return_Same_Instance()
    {
        // Arrange
        var result = Result<int, string>.Success(5);
        var called = false;

        // Act
        var returned = result.OnSuccess(v => { called = (v == 5); });

        // Assert
        Assert.True(called);
        Assert.Same(result, returned);
    }

    [Fact]
    public void OnSuccess_Should_Not_Execute_Action_On_Failure()
    {
        // Arrange
        var result = Result<int, string>.Failure("error");
        var called = false;

        // Act
        var returned = result.OnSuccess(v => { called = true; });

        // Assert
        Assert.False(called);
        Assert.Same(result, returned);
    }

    [Fact]
    public void OnFailure_Should_Execute_Action_On_Failure_And_Return_Same_Instance()
    {
        // Arrange
        var result = Result<int, string>.Failure("boom");
        var called = false;

        // Act
        var returned = result.OnFailure(e => { called = (e == "boom"); });

        // Assert
        Assert.True(called);
        Assert.Same(result, returned);
    }

    [Fact]
    public void OnFailure_Should_Not_Execute_Action_On_Success()
    {
        // Arrange
        var result = Result<int, string>.Success(1);
        var called = false;

        // Act
        var returned = result.OnFailure(e => { called = true; });

        // Assert
        Assert.False(called);
        Assert.Same(result, returned);
    }

    // --------------------------------------------------------------------
    // 10. Integration with Failure as error type
    // --------------------------------------------------------------------

    [Fact]
    public void Result_With_Failure_ErrorType_Should_Work_EndToEnd()
    {
        // Arrange
        var failure = CommonFailureTypes.NoDataFound.CreateWithArgs(null, "User-123");

        // Act
        var result = Result<int, Failure>.Failure(failure);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(default, result.Value);
        Assert.Equal(failure, result.Error);
    }
}
