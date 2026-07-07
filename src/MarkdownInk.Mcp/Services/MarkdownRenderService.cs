using Markdig;
using MarkdownInk.Rendering;
using Spectre.Console;

namespace MarkdownInk.Mcp.Services;

/// <summary>
/// Default <see cref="IMarkdownRenderService"/>. Reuses MarkdownInk's exact Markdig pipeline and
/// public <see cref="SpectreRenderer"/>, but injects a recording <see cref="IAnsiConsole"/> (backed
/// by a <see cref="StringWriter"/>) so the rendered result is returned as a string rather than
/// written to the process's terminal.
/// </summary>
public sealed class MarkdownRenderService : IMarkdownRenderService
{
    // The pipeline is immutable and safe to share across calls; it mirrors MarkdownInk's CLI
    // (Program.cs) and its test helper.
    private readonly MarkdownPipeline _pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

    /// <inheritdoc />
    public string Render(string markdown, bool ansi, int width)
    {
        // A fresh writer + console + renderer per call: SpectreRenderer holds mutable per-render
        // state (indent level, inline buffer), so instances must never be shared across calls.
        var writer = new StringWriter();
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = ansi ? AnsiSupport.Yes : AnsiSupport.No,
            ColorSystem = ColorSystemSupport.TrueColor,
            Interactive = InteractionSupport.No,
            Out = new AnsiConsoleOutput(writer),
        });

        // There is no real TTY behind the StringWriter, so pin the width explicitly rather than
        // letting Spectre fall back to a default/detected size.
        console.Profile.Width = width;

        var document = Markdown.Parse(markdown, _pipeline);
        new SpectreRenderer(console).Render(document);

        return writer.ToString();
    }
}
