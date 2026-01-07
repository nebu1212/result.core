using System;
using System.Linq;
using System.Reflection;
using Result.Core;
using Xunit;

namespace Result.Core.Tests;

/// <summary>
/// Tests for FailureCode, FailureTemplate, Failure and CommonFailureTypes.
/// </summary>
public class FailureTests
{
    // --------------------------------------------------------------------
    // 1. FailureCode
    // --------------------------------------------------------------------

    [Fact]
    public void FailureCode_With_Same_Id_Should_Be_Equal_Even_If_Code_String_Differs()
    {
        // Arrange
        var code1 = new FailureCode(1, "COMMON.UNKNOWN");
        var code2 = new FailureCode(1, "SOME.OTHER_NAME");

        // Act & Assert
        Assert.Equal(code1, code2);
        Assert.True(code1 == code2);
        Assert.False(code1 != code2);
        Assert.Equal(code1.GetHashCode(), code2.GetHashCode());
    }

    [Fact]
    public void FailureCode_With_Different_Id_Should_Not_Be_Equal()
    {
        // Arrange
        var code1 = new FailureCode(1, "COMMON.UNKNOWN");
        var code2 = new FailureCode(2, "COMMON.INTERNAL_ERROR");

        // Act & Assert
        Assert.NotEqual(code1, code2);
        Assert.False(code1 == code2);
        Assert.True(code1 != code2);
    }

    [Fact]
    public void FailureCode_ToString_Should_Return_Code_String()
    {
        // Arrange
        var code = new FailureCode(42, "COMMON.SOMETHING");

        // Act
        var text = code.ToString();

        // Assert
        Assert.Equal("COMMON.SOMETHING", text);
    }

    // --------------------------------------------------------------------
    // 2. FailureTemplate
    // --------------------------------------------------------------------

    [Fact]
    public void FailureTemplate_Create_Should_Create_Failure_With_Same_Code_And_Template_Message()
    {
        // Arrange
        var template = new FailureTemplate(
            new FailureCode(100, "COMMON.TEST"),
            "Test message",
            SelfCreated: true);

        // Act
        var failure = template.Create();

        // Assert
        Assert.Equal(template.Code, failure.Code);
        Assert.Equal("Test message", failure.Message);
        Assert.True(failure.SelfCreated);
        Assert.Equal("n/a", ExtractTraceFromToString(failure)); // Trace war null
    }

    [Fact]
    public void FailureTemplate_CreateWithMessage_Should_Use_Custom_Message()
    {
        // Arrange
        var template = new FailureTemplate(
            new FailureCode(101, "COMMON.CUSTOM_MSG"),
            "Template message",
            SelfCreated: false);

        // Act
        var failure = template.CreateWithMessage("Custom", "Trace-123");

        // Assert
        Assert.Equal(template.Code, failure.Code);
        Assert.Equal("Custom", failure.Message);
        Assert.False(failure.SelfCreated);
        Assert.Contains("Trace-123", failure.ToString());
    }

    [Fact]
    public void FailureTemplate_CreateWithArgs_Should_Format_MessageTemplate()
    {
        // Arrange
        var template = new FailureTemplate(
            new FailureCode(102, "COMMON.INVALID_FORMAT"),
            "The value '{0}' has an invalid format.",
            SelfCreated: true);

        // Act
        var failure = template.CreateWithArgs(null, "ABC");

        // Assert
        Assert.Equal(template.Code, failure.Code);
        Assert.Contains("ABC", failure.Message);
        Assert.DoesNotContain("{0}", failure.Message);
    }

    [Fact]
    public void FailureTemplate_CreateWithArgs_With_No_Args_Should_Use_Template_As_Is()
    {
        // Arrange
        var template = new FailureTemplate(
            new FailureCode(103, "COMMON.NO_ARGS"),
            "Static message",
            SelfCreated: false);

        // Act
        var failure = template.CreateWithArgs(null);

        // Assert
        Assert.Equal("Static message", failure.Message);
        Assert.False(failure.SelfCreated);
    }

    [Fact]
    public void FailureTemplate_Should_Respect_SelfCreated_Flag()
    {
        // Arrange
        var templateSystem = new FailureTemplate(
            new FailureCode(1, "COMMON.UNKNOWN"),
            "An unknown error occurred.",
            SelfCreated: false);

        var templateUser = new FailureTemplate(
            new FailureCode(2, "COMMON.VALIDATION_ERROR"),
            "Validation failed.",
            SelfCreated: true);

        // Act
        var sysFailure = templateSystem.Create();
        var userFailure = templateUser.Create();

        // Assert
        Assert.False(sysFailure.SelfCreated);
        Assert.True(userFailure.SelfCreated);
    }

    // --------------------------------------------------------------------
    // 3. Failure (ToString / basic behaviour)
    // --------------------------------------------------------------------

    [Fact]
    public void Failure_ToString_Should_Contain_Code_Message_SelfCreated_And_Trace_When_Set()
    {
        // Arrange
        var code = new FailureCode(200, "COMMON.NOT_AUTHORIZED");
        var failure = new Failure(
            Code: code,
            Message: "You are not authorized.",
            Trace: "Some trace",
            SelfCreated: true);

        // Act
        var text = failure.ToString();

        // Assert
        Assert.Contains(code.Code, text);
        Assert.Contains("You are not authorized.", text);
        Assert.Contains("SelfCreated: True", text);
        Assert.Contains("Some trace", text);
    }

    [Fact]
    public void Failure_ToString_Should_Use_na_When_Trace_Is_Null()
    {
        // Arrange
        var code = new FailureCode(201, "COMMON.NO_TRACE");
        var failure = new Failure(
            Code: code,
            Message: "No trace here.",
            Trace: null,
            SelfCreated: false);

        // Act
        var text = failure.ToString();

        // Assert
        Assert.Contains(code.Code, text);
        Assert.Contains("No trace here.", text);
        Assert.Contains("SelfCreated: False", text);
        Assert.Contains("Trace: n/a", text);
    }

    // --------------------------------------------------------------------
    // 4. CommonFailureTypes – smoke tests & invariants
    // --------------------------------------------------------------------

    [Fact]
    public void CommonFailureTypes_Unknown_Should_Not_Be_SelfCreated()
    {
        // Act
        var failure = CommonFailureTypes.Unknown.Create();

        // Assert
        Assert.False(failure.SelfCreated);
        Assert.Equal(CommonFailureTypes.Unknown.Code, failure.Code);
    }

    [Fact]
    public void CommonFailureTypes_NotAuthorized_Should_Have_Meaningful_Message()
    {
        // Act
        var failure = CommonFailureTypes.NotAuthorized.Create();

        // Assert
        Assert.Equal(CommonFailureTypes.NotAuthorized.Code, failure.Code);
        Assert.False(string.IsNullOrWhiteSpace(failure.Message));
        Assert.Contains("authorized", failure.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommonFailureTypes_NoDataFound_Should_Format_With_Argument()
    {
        // Act
        var failure = CommonFailureTypes.NoDataFound.CreateWithArgs(null, "User-123");

        // Assert
        Assert.Equal(CommonFailureTypes.NoDataFound.Code, failure.Code);
        Assert.Contains("User-123", failure.Message);
    }

    [Fact]
    public void CommonFailureTypes_InvalidFormat_Should_Format_With_Argument()
    {
        // Act
        var failure = CommonFailureTypes.InvalidFormat.CreateWithArgs(null, "ABC");

        // Assert
        Assert.Equal(CommonFailureTypes.InvalidFormat.Code, failure.Code);
        Assert.Contains("ABC", failure.Message);
        Assert.DoesNotContain("{0}", failure.Message);
    }

    [Fact]
    public void CommonFailureTypes_ExternalCallFailed_Should_Use_Multiple_Args()
    {
        // Act
        var failure = CommonFailureTypes.ExternalCallFailed.CreateWithArgs(null, "ServiceX", "Timeout");

        // Assert
        Assert.Equal(CommonFailureTypes.ExternalCallFailed.Code, failure.Code);
        Assert.Contains("ServiceX", failure.Message);
        Assert.Contains("Timeout", failure.Message);
    }

    [Fact]
    public void CommonFailureTypes_All_Public_Static_Templates_Should_Have_Valid_Codes_And_MessageTemplates()
    {
        // Arrange
        var templates = typeof(CommonFailureTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(FailureTemplate))
            .Select(f => (FailureTemplate)f.GetValue(null)!)
            .ToList();

        // Act & Assert
        Assert.NotEmpty(templates);

        // IDs sollen > 0 sein und eindeutig
        var ids = templates.Select(t => t.Code.Id).ToList();
        Assert.All(ids, id => Assert.True(id > 0));
        Assert.Equal(ids.Count, ids.Distinct().Count());

        // Codes und MessageTemplates dürfen nicht leer sein
        foreach (var template in templates)
        {
            Assert.False(string.IsNullOrWhiteSpace(template.Code.Code));
            Assert.False(string.IsNullOrWhiteSpace(template.MessageTemplate));
        }
    }

    // --------------------------------------------------------------------
    // Helper
    // --------------------------------------------------------------------

    private static string ExtractTraceFromToString(Failure failure)
    {
        // ToString format: "[Code] Message (SelfCreated: ..., Trace: ...)"
        var text = failure.ToString();
        var traceIndex = text.IndexOf("Trace:", StringComparison.Ordinal);
        if (traceIndex < 0) return string.Empty;

        return text.Substring(traceIndex + "Trace:".Length).Trim().TrimEnd(')');
    }
}
