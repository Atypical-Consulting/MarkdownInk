using System.Text.RegularExpressions;
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
public sealed partial class MarkdownRenderService : IMarkdownRenderService
{
    // The pipeline is immutable and safe to share across calls; it mirrors MarkdownInk's CLI
    // (Program.cs) and its test helper.
    private readonly MarkdownPipeline _pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

    // Matches CSI sequences (SGR colors/decorations, cursor moves) and OSC sequences (e.g. OSC-8
    // hyperlinks, BEL- or ST-terminated) — every escape MarkdownInk can emit. Used to guarantee the
    // ansi:false contract.
    [GeneratedRegex(@"\x1B\[[0-9;?]*[ -/]*[@-~]|\x1B\][^\x07\x1B]*(?:\x07|\x1B\\)")]
    private static partial Regex AnsiEscape();

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

        var output = writer.ToString();

        // Guarantee the plain-text contract. Spectre's Ansi=No proved environment-dependent — on
        // some hosts (CI runners) it still emits SGR decoration escapes (e.g. bold) even with
        // NoColors — so strip any residual escape sequences ourselves. No-op where Spectre already
        // produced plain text; preserves layout (Unicode box-drawing, spacing) and removes only escapes.
        return ansi ? output : AnsiEscape().Replace(output, string.Empty);
    }
}
