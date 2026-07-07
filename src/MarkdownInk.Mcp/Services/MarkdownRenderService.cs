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
            // Force capabilities explicitly so the output is a function of the requested settings
            // ONLY — never of the server's own environment (TTY, locale, NO_COLOR, CI). This server
            // renders for the *client's* terminal, so ansi:true must always emit escapes and
            // ansi:false must always be plain. Pairing Ansi=No with NoColors guarantees no SGR
            // escapes leak into plain-text output (Ansi=No alone proved environment-dependent in CI).
            Ansi = ansi ? AnsiSupport.Yes : AnsiSupport.No,
            ColorSystem = ansi ? ColorSystemSupport.TrueColor : ColorSystemSupport.NoColors,
            Interactive = InteractionSupport.No,
            Out = new AnsiConsoleOutput(writer),
        });

        // There is no real TTY behind the StringWriter, so pin the width explicitly rather than
        // letting Spectre fall back to a default/detected size.
        console.Profile.Width = width;

        // Pin Unicode on so box-drawing (heading rules, table borders) renders identically
        // everywhere — without this, Spectre falls back to ASCII on hosts it reads as non-Unicode
        // (e.g. CI runners), producing different output than a developer sees locally.
        console.Profile.Capabilities.Unicode = true;

        var document = Markdown.Parse(markdown, _pipeline);
        new SpectreRenderer(console).Render(document);

        return writer.ToString();
    }
}
