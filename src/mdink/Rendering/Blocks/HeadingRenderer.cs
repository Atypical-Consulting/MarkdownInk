using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace mdink.Rendering.Blocks;

internal class HeadingRenderer : MarkdownObjectRenderer<SpectreRenderer, HeadingBlock>
{
    private static readonly string[] HeadingColors =
    [
        ColorScheme.H1, ColorScheme.H2, ColorScheme.H3,
        ColorScheme.H4, ColorScheme.H5, ColorScheme.H6
    ];

    protected override void Write(SpectreRenderer renderer, HeadingBlock obj)
    {
        renderer.WriteChildren(obj.Inline!);
        var text = renderer.DrainInlineBuffer();

        var color = HeadingColors[Math.Clamp(obj.Level - 1, 0, 5)];

        if (obj.Level <= 2)
        {
            var rule = new Rule($"[{color}]{Markup.Escape(text)}[/]");
            rule.RuleStyle(Style.Parse(ColorScheme.Rule));
            if (obj.Level == 2)
                rule.Justification = Justify.Left;
            AnsiConsole.Write(rule);
        }
        else
        {
            var prefix = new string('#', obj.Level) + " ";
            AnsiConsole.MarkupLine($"[{color}]{Markup.Escape(prefix + text)}[/]");
        }

        AnsiConsole.WriteLine();
    }
}
