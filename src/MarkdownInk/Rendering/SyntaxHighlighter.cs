using System.Text;
using Spectre.Console;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace MarkdownInk.Rendering;

internal class SyntaxHighlighter
{
    private static readonly TimeSpan TokenizeTimeout = TimeSpan.FromSeconds(5);

    private readonly RegistryOptions _options;
    private readonly Registry _registry;
    private readonly Theme _theme;

    public SyntaxHighlighter()
    {
        _options = new RegistryOptions(ThemeName.DarkPlus);
        _registry = new Registry(_options);
        _theme = _registry.GetTheme();
    }

    public string? Highlight(string code, string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return null;

        var scope = ResolveScope(language);
        if (scope is null)
            return null;

        var grammar = _registry.LoadGrammar(scope);
        if (grammar is null)
            return null;

        var sb = new StringBuilder();
        var lines = code.ReplaceLineEndings("\n").Split('\n');
        IStateStack? ruleStack = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var result = grammar.TokenizeLine(line, ruleStack, TokenizeTimeout);
            ruleStack = result.RuleStack;

            foreach (var token in result.Tokens)
            {
                int startIndex = Math.Min(token.StartIndex, line.Length);
                int endIndex = Math.Min(token.EndIndex, line.Length);
                if (startIndex >= endIndex) continue;

                var tokenText = Markup.Escape(line[startIndex..endIndex]);

                int foreground = -1;
                FontStyle fontStyle = FontStyle.NotSet;

                foreach (var themeRule in _theme.Match(token.Scopes))
                {
                    if (foreground == -1 && themeRule.foreground > 0)
                        foreground = themeRule.foreground;
                    if (fontStyle == FontStyle.NotSet && themeRule.fontStyle > 0)
                        fontStyle = themeRule.fontStyle;
                }

                if (foreground > 0)
                {
                    var hex = _theme.GetColor(foreground)?.TrimStart('#');
                    if (hex is not null && hex.Length >= 6)
                    {
                        var style = BuildStyleMarkup(hex, fontStyle);
                        sb.Append($"[{style}]{tokenText}[/]");
                        continue;
                    }
                }

                sb.Append(tokenText);
            }

            if (i < lines.Length - 1)
                sb.AppendLine();
        }

        return sb.ToString();
    }

    private string? ResolveScope(string language)
    {
        try
        {
            var scope = _options.GetScopeByLanguageId(language);
            if (scope is not null) return scope;
        }
        catch { }

        var ext = language.ToLowerInvariant() switch
        {
            "csharp" or "cs" => ".cs",
            "javascript" or "js" => ".js",
            "typescript" or "ts" => ".ts",
            "python" or "py" => ".py",
            "json" => ".json",
            "xml" => ".xml",
            "html" => ".html",
            "css" => ".css",
            "yaml" or "yml" => ".yaml",
            "bash" or "sh" or "shell" => ".sh",
            "sql" => ".sql",
            "rust" or "rs" => ".rs",
            "go" => ".go",
            "java" => ".java",
            "ruby" or "rb" => ".rb",
            "cpp" or "c++" => ".cpp",
            "c" => ".c",
            "php" => ".php",
            "swift" => ".swift",
            "kotlin" or "kt" => ".kt",
            "fsharp" or "fs" => ".fs",
            "powershell" or "ps1" => ".ps1",
            "markdown" or "md" => ".md",
            "dockerfile" => ".dockerfile",
            "makefile" => ".makefile",
            _ => $".{language}"
        };

        try
        {
            return _options.GetScopeByExtension(ext);
        }
        catch
        {
            return null;
        }
    }

    private static string BuildStyleMarkup(string hex, FontStyle fontStyle)
    {
        var style = $"#{hex}";
        if (fontStyle == FontStyle.NotSet)
            return style;

        if ((fontStyle & FontStyle.Bold) != 0) style += " bold";
        if ((fontStyle & FontStyle.Italic) != 0) style += " italic";
        if ((fontStyle & FontStyle.Underline) != 0) style += " underline";
        return style;
    }
}
