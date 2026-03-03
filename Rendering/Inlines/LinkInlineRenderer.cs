using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Spectre.Console;

namespace mdink.Rendering.Inlines;

internal class LinkInlineRenderer : MarkdownObjectRenderer<SpectreRenderer, LinkInline>
{
    protected override void Write(SpectreRenderer renderer, LinkInline obj)
    {
        if (obj.IsImage)
        {
            renderer.InlineBuffer.Append($"[{ColorScheme.LinkUrl}]");
            renderer.InlineBuffer.Append(Markup.Escape("[image: "));
            renderer.WriteChildren(obj);
            renderer.InlineBuffer.Append(Markup.Escape($" -> {obj.Url ?? ""}]"));
            renderer.InlineBuffer.Append("[/]");
        }
        else
        {
            renderer.InlineBuffer.Append($"[{ColorScheme.Link}]");
            renderer.WriteChildren(obj);
            renderer.InlineBuffer.Append("[/]");
            if (!string.IsNullOrEmpty(obj.Url))
            {
                renderer.InlineBuffer.Append(
                    $" [{ColorScheme.LinkUrl}]({Markup.Escape(obj.Url)})[/]");
            }
        }
    }
}
