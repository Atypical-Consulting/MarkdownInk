using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace mdink.Rendering.Inlines;

internal class EmphasisInlineRenderer : MarkdownObjectRenderer<SpectreRenderer, EmphasisInline>
{
    protected override void Write(SpectreRenderer renderer, EmphasisInline obj)
    {
        string tag;

        if (obj.DelimiterChar == '~')
        {
            tag = ColorScheme.Strikethrough;
        }
        else if (obj.DelimiterCount >= 2)
        {
            tag = ColorScheme.Bold;
        }
        else
        {
            tag = ColorScheme.Italic;
        }

        renderer.InlineBuffer.Append($"[{tag}]");
        renderer.WriteChildren(obj);
        renderer.InlineBuffer.Append("[/]");
    }
}
