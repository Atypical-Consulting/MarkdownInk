using Shouldly;

namespace MarkdownInk.Tests;

public class BlockRendererTests
{
    [Fact]
    public void H1_Heading_Is_Rendered()
    {
        var output = TestHelper.Render("# Hello World");
        output.ShouldContain("Hello World");
    }

    [Fact]
    public void H2_Heading_Is_Rendered()
    {
        var output = TestHelper.Render("## Section Title");
        output.ShouldContain("Section Title");
    }

    [Fact]
    public void H3_Through_H6_Are_Rendered_Without_Hash_Prefix()
    {
        var output = TestHelper.Render("### Level 3");
        output.ShouldContain("Level 3");
        output.ShouldNotContain("### Level 3");

        output = TestHelper.Render("#### Level 4");
        output.ShouldContain("Level 4");
        output.ShouldNotContain("#### Level 4");

        output = TestHelper.Render("##### Level 5");
        output.ShouldContain("Level 5");
        output.ShouldNotContain("##### Level 5");

        output = TestHelper.Render("###### Level 6");
        output.ShouldContain("Level 6");
        output.ShouldNotContain("###### Level 6");
    }

    [Fact]
    public void Paragraph_Text_Is_Rendered()
    {
        var output = TestHelper.Render("Hello, this is a paragraph.");
        output.ShouldContain("Hello, this is a paragraph.");
    }

    [Fact]
    public void Code_Block_Shows_Content()
    {
        var markdown = """
            ```
            var x = 42;
            ```
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("var x = 42;");
    }

    [Fact]
    public void Fenced_Code_Block_Shows_Language_Label()
    {
        var markdown = """
            ```csharp
            Console.WriteLine("Hello");
            ```
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("csharp");
        output.ShouldContain("Console.WriteLine");
    }

    [Fact]
    public void Thematic_Break_Produces_Output()
    {
        var output = TestHelper.Render("Above\n\n---\n\nBelow");
        output.ShouldContain("Above");
        output.ShouldContain("Below");
    }

    [Fact]
    public void Blockquote_Contains_Text()
    {
        var output = TestHelper.Render("> This is a quote.");
        output.ShouldContain("This is a quote.");
    }

    [Fact]
    public void Nested_Blockquote_Contains_Text()
    {
        var markdown = """
            > Outer quote
            > > Inner quote
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Outer quote");
        output.ShouldContain("Inner quote");
    }

    [Fact]
    public void Fenced_Code_Block_CSharp_Has_Syntax_Highlighting()
    {
        var markdown = """
            ```csharp
            public class Foo { }
            ```
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("public");
        output.ShouldContain("class");
        output.ShouldContain("Foo");
    }

    [Fact]
    public void Fenced_Code_Block_Unknown_Language_Falls_Back_To_Monochrome()
    {
        var markdown = """
            ```obscurelang
            some code here
            ```
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("some code here");
        output.ShouldContain("obscurelang");
    }

    [Fact]
    public void Unfenced_Code_Block_Renders_Without_Crash()
    {
        var markdown = "    indented code block\n    second line\n";
        var output = TestHelper.Render(markdown);
        output.ShouldContain("indented code block");
    }

    [Fact]
    public void Empty_Document_Produces_No_Output()
    {
        var output = TestHelper.Render("");
        output.ShouldBeEmpty();
    }
}
