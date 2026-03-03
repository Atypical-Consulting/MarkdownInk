using Shouldly;

namespace MarkdownInk.Tests;

public class FootnoteRendererTests
{
    [Fact]
    public void Footnote_Reference_Is_Rendered()
    {
        var markdown = """
            Text with a footnote[^1].

            [^1]: The footnote content.
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("[1]");
        output.ShouldContain("The footnote content.");
    }

    [Fact]
    public void Multiple_Footnotes_Are_Numbered()
    {
        var markdown = """
            First[^1] and second[^2].

            [^1]: Footnote one.
            [^2]: Footnote two.
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("[1]");
        output.ShouldContain("[2]");
        output.ShouldContain("Footnote one.");
        output.ShouldContain("Footnote two.");
    }

    [Fact]
    public void Footnote_Group_Has_Header()
    {
        var markdown = """
            Text[^1].

            [^1]: Note.
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Footnotes");
    }
}
