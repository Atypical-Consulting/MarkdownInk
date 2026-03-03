using Markdig.Extensions.Footnotes;
using Markdig.Renderers;
using Spectre.Console;

namespace MarkdownInk.Rendering.Inlines;

internal class FootnoteLinkRenderer : MarkdownObjectRenderer<SpectreRenderer, FootnoteLink>
{
    protected override void Write(SpectreRenderer renderer, FootnoteLink obj)
    {
        if (!obj.IsBackLink)
        {
            var order = obj.Footnote?.Order ?? obj.Index;
            renderer.InlineBuffer.Append(
                $"[{ColorScheme.FootnoteRef}]{Markup.Escape($"[{order}]")}[/]");
        }
    }
}
