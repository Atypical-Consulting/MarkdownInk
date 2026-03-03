namespace MarkdownInk.Rendering;

internal static class ColorScheme
{
    // Headings (h1..h6 in descending brightness)
    public const string H1 = "bold cyan";
    public const string H2 = "bold deepskyblue1";
    public const string H3 = "bold dodgerblue1";
    public const string H4 = "bold steelblue1";
    public const string H5 = "bold cornflowerblue";
    public const string H6 = "bold slateblue1";

    // Inline styles
    public const string Bold = "bold white";
    public const string Italic = "italic grey93";
    public const string InlineCode = "bold yellow on grey23";
    public const string Strikethrough = "strikethrough grey62";
    public const string Link = "underline deepskyblue2";
    public const string LinkUrl = "grey50";

    // Structural
    public const string CodeBlockBorder = "grey35";
    public const string CodeBlockText = "chartreuse3";
    public const string BlockquoteBorder = "dodgerblue3";
    public const string BlockquoteText = "italic grey78";
    public const string Rule = "grey50";
    public const string TaskChecked = "green";
    public const string TaskUnchecked = "grey50";
    public const string FootnoteRef = "bold yellow";
    public const string ListNumber = "white";
    public const string TableBorder = "grey35";
    public const string TableHeader = "bold cornflowerblue";
}
