using Shouldly;

namespace MarkdownInk.Tests;

public class EdgeCaseTests
{
    [Fact]
    public void Whitespace_Only_Input_Produces_No_Output()
    {
        var output = TestHelper.Render("   \n  \n   ");
        output.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Empty_Code_Block_Renders_Panel()
    {
        var markdown = """
            ```
            ```
            """;
        var output = TestHelper.Render(markdown);
        // Should not crash; panel border is still rendered
        output.ShouldNotBeEmpty();
    }

    [Fact]
    public void Empty_Table_With_Headers_Only()
    {
        var markdown = """
            | A | B |
            |---|---|
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("A");
        output.ShouldContain("B");
    }

    [Fact]
    public void Table_With_Single_Column()
    {
        var markdown = """
            | Only |
            |------|
            | Cell |
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Only");
        output.ShouldContain("Cell");
    }

    [Fact]
    public void Nested_Blockquote_With_Bold_And_Italic()
    {
        var markdown = """
            > **Bold quote**
            > > *Nested italic*
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Bold quote");
        output.ShouldContain("Nested italic");
    }

    [Fact]
    public void Blockquote_With_Inline_Code()
    {
        var output = TestHelper.Render("> Use `var x = 1;` here.");
        output.ShouldContain("var x = 1;");
    }

    [Fact]
    public void Link_With_Brackets_In_Url_Does_Not_Crash()
    {
        var output = TestHelper.Render("[docs](https://example.com/page[1])");
        output.ShouldContain("docs");
    }

    [Fact]
    public void Link_With_Empty_Url()
    {
        var output = TestHelper.Render("[label]()");
        output.ShouldContain("label");
    }

    [Fact]
    public void Hard_Line_Break_Produces_Newline()
    {
        // Two trailing spaces before newline = hard break
        var output = TestHelper.Render("Line one  \nLine two");
        output.ShouldContain("Line one");
        output.ShouldContain("Line two");
    }

    [Fact]
    public void Ordered_List_Preserves_Start_Number()
    {
        var markdown = """
            3. Third
            4. Fourth
            5. Fifth
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("3.");
        output.ShouldContain("Third");
        output.ShouldContain("5.");
        output.ShouldContain("Fifth");
    }

    [Fact]
    public void Deeply_Nested_List_Does_Not_Crash()
    {
        var markdown = """
            - Level 1
              - Level 2
                - Level 3
                  - Level 4
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Level 1");
        output.ShouldContain("Level 4");
    }

    [Fact]
    public void Mixed_Ordered_And_Unordered_Lists()
    {
        var markdown = """
            1. First
            2. Second
               - Sub A
               - Sub B
            3. Third
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("1.");
        output.ShouldContain("First");
        output.ShouldContain("Sub A");
        output.ShouldContain("3.");
        output.ShouldContain("Third");
    }

    [Fact]
    public void Code_Block_With_Empty_Language_Specifier()
    {
        // Fenced block with ``` and nothing after
        var markdown = "```\nvar x = 1;\n```";
        var output = TestHelper.Render(markdown);
        output.ShouldContain("var x = 1;");
    }

    [Fact]
    public void Multiple_Headings_At_Different_Levels()
    {
        var markdown = """
            # Title
            ## Section
            ### Subsection
            Paragraph text.
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Title");
        output.ShouldContain("Section");
        output.ShouldContain("Subsection");
        output.ShouldContain("Paragraph text.");
    }

    [Fact]
    public void Multiple_Paragraphs_Separated()
    {
        var markdown = """
            First paragraph.

            Second paragraph.

            Third paragraph.
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("First paragraph.");
        output.ShouldContain("Second paragraph.");
        output.ShouldContain("Third paragraph.");
    }

    [Fact]
    public void Inline_Code_With_Special_Characters()
    {
        var output = TestHelper.Render("Use `<div class=\"foo\">` here.");
        output.ShouldContain("<div");
    }

    [Fact]
    public void Image_With_Empty_Alt_Text()
    {
        var output = TestHelper.Render("![](https://example.com/img.png)");
        output.ShouldContain("image:");
        output.ShouldContain("https://example.com/img.png");
    }

    [Fact]
    public void Thematic_Break_Variants()
    {
        // All three valid thematic break syntaxes
        TestHelper.Render("---").ShouldNotBeEmpty();
        TestHelper.Render("***").ShouldNotBeEmpty();
        TestHelper.Render("___").ShouldNotBeEmpty();
    }

    [Fact]
    public void Consecutive_Code_Blocks_Render_Independently()
    {
        var markdown = """
            ```python
            print("hello")
            ```

            ```javascript
            console.log("hello")
            ```
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("python");
        output.ShouldContain("print");
        output.ShouldContain("javascript");
        output.ShouldContain("console.log");
    }

    [Fact]
    public void Large_Document_With_Mixed_Elements()
    {
        var markdown = """
            # Project Title

            A paragraph with **bold**, *italic*, and `code`.

            ## Features

            - Item one
            - Item two
              - Nested

            ```csharp
            var x = 42;
            ```

            > A quote

            | Col1 | Col2 |
            |------|------|
            | A    | B    |

            ---

            Final paragraph.
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Project Title");
        output.ShouldContain("bold");
        output.ShouldContain("italic");
        output.ShouldContain("Features");
        output.ShouldContain("Item one");
        output.ShouldContain("Nested");
        output.ShouldContain("var x = 42;");
        output.ShouldContain("A quote");
        output.ShouldContain("Col1");
        output.ShouldContain("Final paragraph.");
    }
}
