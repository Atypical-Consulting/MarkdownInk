using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace MarkdownInk.Rendering.Blocks;

internal class ListRenderer : MarkdownObjectRenderer<SpectreRenderer, ListBlock>
{
    private static readonly string[] Bullets = ["*", "-", "+", "*"];

    protected override void Write(SpectreRenderer renderer, ListBlock listBlock)
    {
        renderer.IndentLevel++;
        var indent = new string(' ', (renderer.IndentLevel - 1) * 2);
        int itemIndex = 0;

        foreach (var item in listBlock)
        {
            if (item is not ListItemBlock listItem) continue;

            string bullet;

            if (listBlock.IsOrdered)
            {
                itemIndex++;
                var num = listItem.Order > 0 ? listItem.Order : itemIndex;
                bullet = $"[{ColorScheme.ListNumber}]{num}.[/] ";
            }
            else
            {
                var bulletChar = Bullets[Math.Clamp(renderer.IndentLevel - 1, 0, Bullets.Length - 1)];
                bullet = $"[{ColorScheme.ListBullet}]{bulletChar}[/] ";
            }

            renderer.Console.Markup($"{indent}{bullet}");

            foreach (var child in listItem)
            {
                if (child is ParagraphBlock para)
                {
                    renderer.WriteChildren(para.Inline!);
                    var text = renderer.DrainInlineBuffer();
                    if (!string.IsNullOrWhiteSpace(text))
                        renderer.Console.Markup($"{text}\n");
                }
                else
                {
                    renderer.Write(child);
                }
            }
        }

        renderer.IndentLevel--;

        // Trailing blank line after top-level lists, matching other block elements
        if (renderer.IndentLevel == 0)
            renderer.Console.WriteLine();
    }
}
