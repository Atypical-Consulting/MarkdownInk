using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using MarkdownInk.Mcp.Models;

namespace MarkdownInk.Mcp.Tools;

/// <summary>
/// The closed, stable set of machine-readable error <c>type</c> values returned by every MCP tool.
/// Raw CLR exception type names are never surfaced directly — every failure is classified into one
/// of these categories so callers can branch on a documented contract.
/// </summary>
internal static class ToolErrorTypes
{
    /// <summary>Caller-supplied input was missing, malformed, or otherwise invalid.</summary>
    public const string Validation = "ValidationError";

    /// <summary>The requested file could not be located.</summary>
    public const string NotFound = "NotFoundError";

    /// <summary>An unexpected, unclassified failure occurred. Full detail is logged server-side.</summary>
    public const string Internal = "InternalError";
}

/// <summary>
/// Per-invocation correlation/logging context created at the start of each MCP tool method (via
/// <see cref="ToolExecutionHelper.BeginInvocation"/>) and disposed when the method returns. Every
/// call gets a correlation ID (echoed into error responses) and a logger scope carrying it.
/// </summary>
internal sealed class ToolInvocation : IDisposable
{
    private readonly IDisposable? _logScope;

    /// <summary>Per-invocation correlation ID, generated once at the start of the tool call.</summary>
    public string CorrelationId { get; } = Guid.NewGuid().ToString("n");

    /// <summary>Logger for this tool invocation, scoped with the correlation ID.</summary>
    public ILogger? Logger { get; }

    public ToolInvocation(string toolName, ILoggerFactory? loggerFactory)
    {
        Logger = loggerFactory?.CreateLogger($"MarkdownInk.Mcp.Tools.{toolName}");
        _logScope = Logger?.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = CorrelationId,
            ["Tool"] = toolName,
        });
    }

    public void Dispose() => _logScope?.Dispose();
}

/// <summary>
/// Shared helper used by every MCP tool method to start the per-invocation correlation context and
/// build consistent typed failure envelopes (<see cref="ToolResult{T}"/>). Tools never throw to the
/// MCP protocol layer.
/// </summary>
internal static class ToolExecutionHelper
{
    /// <summary>Starts the per-invocation correlation/logging context for a tool call.</summary>
    public static ToolInvocation BeginInvocation(string toolName, ILoggerFactory? loggerFactory) =>
        new(toolName, loggerFactory);

    /// <summary>Builds the typed failure envelope for a caller-input validation failure.</summary>
    public static ToolResult<T> ValidationError<T>(string message, string correlationId, string? hint = null) =>
        ToolResult<T>.Failure(new ToolError
        {
            Type = ToolErrorTypes.Validation,
            Message = message,
            Hint = hint,
            CorrelationId = correlationId,
        });

    /// <summary>Builds the typed failure envelope for a missing target (e.g. a file that does not exist).</summary>
    public static ToolResult<T> NotFound<T>(string message, string correlationId, string? hint = null) =>
        ToolResult<T>.Failure(new ToolError
        {
            Type = ToolErrorTypes.NotFound,
            Message = message,
            Hint = hint,
            CorrelationId = correlationId,
        });

    /// <summary>
    /// Builds the standard typed failure envelope for an unhandled exception, classifying it into
    /// the closed <see cref="ToolErrorTypes"/> set. Internal-class failures are logged in full and
    /// reported with a short, user-safe message so no raw exception text or stack trace leaks.
    /// </summary>
    public static ToolResult<T> Error<T>(
        Exception ex,
        string correlationId,
        ILogger? logger = null,
        [CallerMemberName] string? toolName = null)
    {
        var type = Classify(ex);
        string message;

        if (type == ToolErrorTypes.Internal)
        {
            logger?.LogError(ex, "Unhandled internal error in {Tool}", toolName);
            message = "An unexpected internal error occurred. Check the server logs for details.";
        }
        else
        {
            message = ex.Message;
        }

        return ToolResult<T>.Failure(new ToolError
        {
            Type = type,
            Message = message,
            CorrelationId = correlationId,
        });
    }

    private static string Classify(Exception ex) => ex switch
    {
        ArgumentException or FormatException => ToolErrorTypes.Validation,
        FileNotFoundException or DirectoryNotFoundException => ToolErrorTypes.NotFound,
        _ => ToolErrorTypes.Internal,
    };
}
