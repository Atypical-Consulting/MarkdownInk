using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Spectre.Console;

namespace MarkdownInk.Rendering.Inlines;

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
            var url = obj.Url ?? "";
            // URLs with [ or ] would break Spectre markup parsing in link= attributes
            if (url.Length > 0 && !url.Contains('[') && !url.Contains(']'))
            {
                renderer.InlineBuffer.Append($"[{ColorScheme.Link} link={url}]");
                renderer.WriteChildren(obj);
                renderer.InlineBuffer.Append("[/]");
            }
            else
            {
                renderer.InlineBuffer.Append($"[{ColorScheme.Link}]");
                renderer.WriteChildren(obj);
                renderer.InlineBuffer.Append("[/]");
            }
        }
    }
}
