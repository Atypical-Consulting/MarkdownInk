using Markdig;
using MarkdownInk.Rendering;
using Spectre.Console.Testing;

namespace MarkdownInk.Tests;

internal static class TestHelper
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static string Render(string markdown)
    {
        var console = new TestConsole();
        var renderer = new SpectreRenderer(console);
        var document = Markdown.Parse(markdown, Pipeline);
        renderer.Render(document);
        return console.Output;
    }
}
