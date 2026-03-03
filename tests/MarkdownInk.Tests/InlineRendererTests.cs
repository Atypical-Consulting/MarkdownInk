using Shouldly;

namespace MarkdownInk.Tests;

public class InlineRendererTests
{
    [Fact]
    public void Bold_Text_Is_Rendered()
    {
        var output = TestHelper.Render("This is **bold** text.");
        output.ShouldContain("bold");
    }

    [Fact]
    public void Italic_Text_Is_Rendered()
    {
        var output = TestHelper.Render("This is *italic* text.");
        output.ShouldContain("italic");
    }

    [Fact]
    public void Strikethrough_Text_Is_Rendered()
    {
        var output = TestHelper.Render("This is ~~deleted~~ text.");
        output.ShouldContain("deleted");
    }

    [Fact]
    public void Inline_Code_Is_Rendered()
    {
        var output = TestHelper.Render("Use `Console.WriteLine()` here.");
        output.ShouldContain("Console.WriteLine()");
    }

    [Fact]
    public void Link_Shows_Label_Text()
    {
        var output = TestHelper.Render("[Click here](https://example.com)");
        output.ShouldContain("Click here");
    }

    [Fact]
    public void Image_Shows_Alt_Text_And_Url()
    {
        var output = TestHelper.Render("![Logo](https://example.com/img.png)");
        output.ShouldContain("image:");
        output.ShouldContain("Logo");
        output.ShouldContain("https://example.com/img.png");
    }

    [Fact]
    public void Special_Characters_Are_Escaped()
    {
        var output = TestHelper.Render("Text with [brackets] inside.");
        output.ShouldContain("[brackets]");
    }

    [Fact]
    public void Bold_And_Italic_Combined()
    {
        var output = TestHelper.Render("This is ***bold italic*** text.");
        output.ShouldContain("bold italic");
    }
}
