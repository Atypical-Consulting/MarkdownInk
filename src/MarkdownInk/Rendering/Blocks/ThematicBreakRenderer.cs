using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace MarkdownInk.Rendering.Blocks;

internal class ThematicBreakRenderer : MarkdownObjectRenderer<SpectreRenderer, ThematicBreakBlock>
{
    protected override void Write(SpectreRenderer renderer, ThematicBreakBlock obj)
    {
        var rule = new Rule();
        rule.RuleStyle(Style.Parse(ColorScheme.Rule));
        renderer.Console.Write(rule);
        renderer.Console.WriteLine();
    }
}
