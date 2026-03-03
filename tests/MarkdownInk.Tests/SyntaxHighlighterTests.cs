using MarkdownInk.Rendering;
using Shouldly;

namespace MarkdownInk.Tests;

public class SyntaxHighlighterTests
{
    private readonly SyntaxHighlighter _highlighter = new();

    [Fact]
    public void Null_Language_Returns_Null()
    {
        _highlighter.Highlight("code", null).ShouldBeNull();
    }

    [Fact]
    public void Empty_Language_Returns_Null()
    {
        _highlighter.Highlight("code", "").ShouldBeNull();
    }

    [Fact]
    public void Whitespace_Language_Returns_Null()
    {
        _highlighter.Highlight("code", "   ").ShouldBeNull();
    }

    [Fact]
    public void Unknown_Language_Returns_Null()
    {
        _highlighter.Highlight("code", "not-a-real-language-xyz").ShouldBeNull();
    }

    [Fact]
    public void CSharp_Code_Returns_Highlighted_Markup()
    {
        var result = _highlighter.Highlight("public class Foo { }", "csharp");
        result.ShouldNotBeNull();
        result.ShouldContain("public");
        result.ShouldContain("class");
        result.ShouldContain("Foo");
        // Should contain hex color markup from the theme
        result.ShouldContain("#");
    }

    [Fact]
    public void JavaScript_Alias_Works()
    {
        var result = _highlighter.Highlight("const x = 42;", "js");
        result.ShouldNotBeNull();
        result.ShouldContain("const");
    }

    [Fact]
    public void Python_Alias_Works()
    {
        var result = _highlighter.Highlight("def hello():\n    pass", "py");
        result.ShouldNotBeNull();
        result.ShouldContain("def");
    }

    [Fact]
    public void Multiline_Code_Preserves_Lines()
    {
        var code = "var a = 1;\nvar b = 2;\nvar c = 3;";
        var result = _highlighter.Highlight(code, "csharp");
        result.ShouldNotBeNull();
        result.ShouldContain("a");
        result.ShouldContain("b");
        result.ShouldContain("c");
    }

    [Fact]
    public void Empty_Code_Returns_Empty_Markup()
    {
        var result = _highlighter.Highlight("", "csharp");
        result.ShouldNotBeNull();
        result.Length.ShouldBe(0);
    }

    [Fact]
    public void Special_Characters_In_Code_Are_Preserved()
    {
        var result = _highlighter.Highlight("var x = \"<hello>\";", "csharp");
        result.ShouldNotBeNull();
        // Spectre markup brackets are escaped, but angle brackets are literal text
        result.ShouldContain("<hello>");
        result.ShouldNotContain("[[");
    }
}
