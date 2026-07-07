using MarkdownInk.Mcp.Models;
using MarkdownInk.Mcp.Services;
using MarkdownInk.Mcp.Tools;
using Shouldly;

namespace MarkdownInk.Mcp.Tests.Tools;

/// <summary>
/// Direct C# invocation of the static tool methods against a real <see cref="MarkdownRenderService"/>.
/// Faster than the protocol tests and asserts directly on the <see cref="ToolResult{T}"/> envelope.
/// </summary>
public class RenderToolDirectTests
{
    private static readonly IMarkdownRenderService Service = new MarkdownRenderService();

    [Fact]
    public void RenderMarkdown_Ansi_Succeeds_With_Escape_Codes()
    {
        var result = RenderMarkdownTool.RenderMarkdown(Service, "# Title\n\n**bold**");

        result.Ok.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data!.Ansi.ShouldBeTrue();
        result.Data.Width.ShouldBe(100);
        result.Data.Output.ShouldContain("Title");
        result.Data.Output.ShouldContain('\x1b');
        result.Data.LineCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void RenderMarkdown_PlainText_Has_No_Escape_Codes()
    {
        var result = RenderMarkdownTool.RenderMarkdown(Service, "# Title", ansi: false);

        result.Ok.ShouldBeTrue();
        result.Data!.Output.ShouldContain("Title");
        result.Data.Output.ShouldNotContain('\x1b');
    }

    [Fact]
    public void RenderMarkdown_Empty_Input_Is_ValidationError()
    {
        var result = RenderMarkdownTool.RenderMarkdown(Service, "");

        result.Ok.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.Type.ShouldBe("ValidationError");
    }

    [Fact]
    public void RenderMarkdown_NonPositive_Width_Is_ValidationError()
    {
        var result = RenderMarkdownTool.RenderMarkdown(Service, "# Hi", width: 0);

        result.Ok.ShouldBeFalse();
        result.Error!.Type.ShouldBe("ValidationError");
    }

    [Fact]
    public async Task RenderMarkdownFile_Missing_File_Is_NotFound()
    {
        var result = await RenderMarkdownFileTool.RenderMarkdownFile(Service, "nope-missing.md");

        result.Ok.ShouldBeFalse();
        result.Error!.Type.ShouldBe("NotFoundError");
    }

    [Fact]
    public async Task RenderMarkdownFile_Existing_File_Renders_And_Sets_Path()
    {
        var path = Path.Combine(Path.GetTempPath(), $"mdink-direct-{Guid.NewGuid():n}.md");
        await File.WriteAllTextAsync(path, "# File heading\n");
        try
        {
            var result = await RenderMarkdownFileTool.RenderMarkdownFile(Service, path, ansi: false);

            result.Ok.ShouldBeTrue();
            result.Data!.Path.ShouldBe(path);
            result.Data.Output.ShouldContain("File heading");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("one line", 1)]
    [InlineData("a\nb", 2)]
    [InlineData("a\nb\n", 2)]
    public void CountLines_Counts_Correctly(string input, int expected)
    {
        RenderMarkdownTool.CountLines(input).ShouldBe(expected);
    }
}
