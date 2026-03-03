using System.Text;
using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace MarkdownInk.Rendering.Blocks;

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

        // Try syntax highlighting, fall back to monochrome
        var highlighted = renderer.Highlighter.Value.Highlight(code, lang);
        IRenderable codeContent;

        if (highlighted is not null)
        {
            codeContent = new Markup(highlighted);
        }
        else
        {
            codeContent = new Markup($"[{ColorScheme.CodeBlockText}]{Markup.Escape(code)}[/]");
        }

        var panel = new Panel(codeContent)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Grey))
            .Padding(1, 0);

        if (lang is not null)
            panel.Header($"[{ColorScheme.CodeBlockBorder}]{Markup.Escape(lang)}[/]");

        renderer.Console.Write(panel);
        renderer.Console.WriteLine();
    }
}
