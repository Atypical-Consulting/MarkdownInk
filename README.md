# MarkdownInk (mdink)

A .NET CLI tool that renders Markdown beautifully in the terminal.

## Installation

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
# Build and install as a global tool
dotnet pack src/MarkdownInk -o nupkg
dotnet tool install -g --add-source nupkg MarkdownInk
```

Or use the setup script:

```bash
./setup.cmd
```

## Usage

```bash
# Render a file
mdink README.md

# Pipe from stdin
cat notes.md | mdink
```

## Features

- **Headings** — H1/H2 as full-width rules, H3–H6 as colored text
- **Code blocks** — Syntax highlighting (30+ languages) via TextMateSharp with VS Code's Dark+ theme, displayed in rounded panels with language labels
- **Inline formatting** — Bold, italic, strikethrough, inline code
- **Links** — Clickable hyperlinks (OSC 8) in supported terminals
- **Tables** — Rendered with rounded borders and column alignment
- **Lists** — Ordered, unordered, nested, and task lists with checkboxes
- **Blockquotes** — Nested with colored vertical bars
- **Footnotes** — Numbered references with a dedicated section
- **Thematic breaks** — Horizontal rules

## Supported Languages

Syntax highlighting works for: C#, JavaScript, TypeScript, Python, JSON, XML, HTML, CSS, YAML, Bash, SQL, Rust, Go, Java, Ruby, C/C++, PHP, Swift, Kotlin, F#, PowerShell, Markdown, Dockerfile, Makefile, and more.

## Built With

- [Markdig](https://github.com/xoofx/markdig) — Markdown parser
- [Spectre.Console](https://spectreconsole.net/) — Terminal rendering
- [TextMateSharp](https://github.com/nickvdyck/textmatesharp) — Syntax highlighting

## License

MIT
