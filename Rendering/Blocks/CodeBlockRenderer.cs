using System.Text;
using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace mdink.Rendering.Blocks;

internal class CodeBlockRenderer : MarkdownObjectRenderer<SpectreRenderer, CodeBlock>
{
    protected override void Write(SpectreRenderer renderer, CodeBlock obj)
    {
        var sb = new StringBuilder();
        var lines = obj.Lines.Lines;
        if (lines is not null)
        {
            for (int i = 0; i < obj.Lines.Count; i++)
            {
                var slice = lines[i].Slice;
                sb.AppendLine(slice.ToString());
            }
        }
        var code = sb.ToString().TrimEnd('\r', '\n');

        string? lang = null;
        if (obj is FencedCodeBlock fenced && !string.IsNullOrWhiteSpace(fenced.Info))
            lang = fenced.Info.Trim();

        var codeMarkup = new Markup($"[{ColorScheme.CodeBlockText}]{Markup.Escape(code)}[/]");
        var panel = new Panel(codeMarkup)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Grey))
            .Padding(1, 0);

        if (lang is not null)
            panel.Header($"[{ColorScheme.CodeBlockBorder}]{Markup.Escape(lang)}[/]");

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}
