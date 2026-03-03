using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace MarkdownInk.Rendering.Blocks;

internal class QuoteBlockRenderer : MarkdownObjectRenderer<SpectreRenderer, QuoteBlock>
{
    protected override void Write(SpectreRenderer renderer, QuoteBlock obj)
    {
        var previousPrefix = renderer.QuotePrefix;
        renderer.QuotePrefix = (previousPrefix ?? "")
            + $"[{ColorScheme.BlockquoteBorder}]\u2595[/] ";

        renderer.WriteChildren(obj);

        renderer.QuotePrefix = previousPrefix;
    }
}
