using Shouldly;

namespace MarkdownInk.Tests;

public class ListRendererTests
{
    [Fact]
    public void Unordered_List_Renders_Items()
    {
        var markdown = """
            - First
            - Second
            - Third
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("First");
        output.ShouldContain("Second");
        output.ShouldContain("Third");
    }

    [Fact]
    public void Ordered_List_Renders_Numbers()
    {
        var markdown = """
            1. Alpha
            2. Beta
            3. Gamma
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("1.");
        output.ShouldContain("Alpha");
        output.ShouldContain("2.");
        output.ShouldContain("Beta");
        output.ShouldContain("3.");
        output.ShouldContain("Gamma");
    }

    [Fact]
    public void Nested_List_Renders_Subitems()
    {
        var markdown = """
            - Parent
              - Child
                - Grandchild
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Parent");
        output.ShouldContain("Child");
        output.ShouldContain("Grandchild");
    }

    [Fact]
    public void Task_List_Shows_Checked_And_Unchecked()
    {
        var markdown = """
            - [x] Done
            - [ ] Not done
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("[x]");
        output.ShouldContain("Done");
        output.ShouldContain("[ ]");
        output.ShouldContain("Not done");
    }

    [Fact]
    public void List_With_Bold_Item()
    {
        var markdown = """
            - Normal item
            - **Bold item**
            """;
        var output = TestHelper.Render(markdown);
        output.ShouldContain("Normal item");
        output.ShouldContain("Bold item");
    }
}
