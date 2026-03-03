using Markdig.Extensions.Footnotes;
using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace MarkdownInk.Rendering.Blocks;

internal class FootnoteGroupRenderer : MarkdownObjectRenderer<SpectreRenderer, FootnoteGroup>
{
    protected override void Write(SpectreRenderer renderer, FootnoteGroup group)
    {
        var rule = new Rule($"[{ColorScheme.FootnoteRef}]Footnotes[/]");
        rule.RuleStyle(Style.Parse(ColorScheme.Rule));
        rule.Justification = Justify.Left;
        renderer.Console.Write(rule);
        renderer.Console.WriteLine();

        foreach (var child in group)
        {
            if (child is not Footnote footnote) continue;

            // Render footnote body by extracting inline content from paragraphs
            foreach (var block in footnote)
            {
                if (block is ParagraphBlock para && para.Inline is not null)
                    renderer.WriteChildren(para.Inline);
            }
            var text = renderer.DrainInlineBuffer();

            renderer.Console.MarkupLine(
                $"[{ColorScheme.FootnoteRef}]{Markup.Escape($"[{footnote.Order}]")}[/] {text}");
        }

        renderer.Console.WriteLine();
    }
}
