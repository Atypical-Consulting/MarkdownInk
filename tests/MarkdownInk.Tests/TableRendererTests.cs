using Shouldly;

namespace MarkdownInk.Tests;

public class TableRendererTests
{
    [Fact]
    public void Table_Renders_Header_And_Cells()
    {
        var markdown = """
            | Name  | Age |
            |-------|-----|
            | Alice | 30  |
            | Bob   | 25  |
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Name");
        output.ShouldContain("Age");
        output.ShouldContain("Alice");
        output.ShouldContain("30");
        output.ShouldContain("Bob");
        output.ShouldContain("25");
    }

    [Fact]
    public void Table_With_Alignment()
    {
        var markdown = """
            | Left | Center | Right |
            |:-----|:------:|------:|
            | A    | B      | C     |
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Left");
        output.ShouldContain("Center");
        output.ShouldContain("Right");
        output.ShouldContain("A");
        output.ShouldContain("B");
        output.ShouldContain("C");
    }

    [Fact]
    public void Table_With_Special_Characters_In_Cells()
    {
        var markdown = """
            | Key   | Value       |
            |-------|-------------|
            | items | [1, 2, 3]   |
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Key");
        output.ShouldContain("Value");
        output.ShouldContain("[1, 2, 3]");
    }
}
