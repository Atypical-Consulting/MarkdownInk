using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Spectre.Console;

namespace mdink.Rendering.Inlines;

internal class CodeInlineRenderer : MarkdownObjectRenderer<SpectreRenderer, CodeInline>
{
    protected override void Write(SpectreRenderer renderer, CodeInline obj)
    {
        renderer.InlineBuffer.Append(
            $"[{ColorScheme.InlineCode}]{Markup.Escape(obj.Content)}[/]");
    }
}
