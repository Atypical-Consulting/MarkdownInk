# MarkdownInk (mdink)

<!-- mcp-name: io.github.Atypical-Consulting/markdownink-mcp -->

**Markdown, rendered beautifully in your terminal** — as the `mdink` CLI, or as an MCP server so AI
agents can render it for you.

[![CI](https://github.com/Atypical-Consulting/MarkdownInk/actions/workflows/ci.yml/badge.svg)](https://github.com/Atypical-Consulting/MarkdownInk/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)

🌐 **[atypical-consulting.github.io/MarkdownInk](https://atypical-consulting.github.io/MarkdownInk/)**

![MarkdownInk — Markdown, rendered beautifully in your terminal](https://atypical-consulting.github.io/MarkdownInk/og.png)

## The problem

Markdown is written to be *rendered* — but your terminal shows it raw. `cat README.md` and you get a
wall of `#`, `**`, backticks, and `| pipe | tables |`. Fenced code blocks have no syntax
highlighting. Links are naked URLs. The one place developers live all day is the one place Markdown
looks worst.

It's worse with AI coding agents: when an agent prints Markdown to your terminal, you read the
*source*, not the document.

## The solution

MarkdownInk parses Markdown with [Markdig](https://github.com/xoofx/markdig) and paints it with
[Spectre.Console](https://spectreconsole.net/) + [TextMateSharp](https://github.com/danipen/TextMateSharp):
colored headings, syntax-highlighted code (30+ languages, VS Code's Dark+ theme), bordered tables,
task lists, nested block quotes, footnotes, and clickable links.

One rendering engine, two ways to use it:

- **`mdink` CLI** — render a file or stdin straight to your terminal.
- **`MarkdownInk.Mcp` server** — the same engine over [MCP](https://modelcontextprotocol.io), so any
  AI agent can show Markdown as rich terminal output instead of raw source.

## Quick start (CLI)

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
# Build and install the mdink global tool from source
dotnet pack src/MarkdownInk -o nupkg
dotnet tool install -g --add-source nupkg MarkdownInk
```

```bash
# Render a file
mdink README.md

# …or pipe from stdin
cat notes.md | mdink
```

## Use it with an AI agent (MCP)

`MarkdownInk.Mcp` is a stdio [Model Context Protocol](https://modelcontextprotocol.io) server. Point
any MCP client at it and the agent gains two read-only tools:

| Tool | What it does |
| --- | --- |
| `render_markdown` | Render a Markdown **string** to terminal-ready output. |
| `render_markdown_file` | Read a Markdown **file** from disk and render it. |

Both take `ansi` (default `true` — include ANSI color/style codes; `false` for plain text with the
same layout) and `width` (default `100` columns). Every call returns a uniform envelope —
`{ "ok": true, "data": { "output", "ansi", "width", "lineCount", "path"? } }` — or `ok: false` with
a typed `error`. Neither tool writes to disk.

Add it to your client config (launched on demand via `dnx`, the .NET tool runner):

```jsonc
{
  "mcpServers": {
    "markdownink": {
      "command": "dnx",
      "args": ["MarkdownInk.Mcp", "--yes"]
    }
  }
}
```

## Install

| Method | How |
| --- | --- |
| **mdink CLI** | `dotnet pack src/MarkdownInk -o nupkg && dotnet tool install -g --add-source nupkg MarkdownInk` |
| **MCP · dnx** | Add the `mcpServers` config above (needs the .NET 10 SDK). |
| **MCP · Docker** | `docker build -t markdownink-mcp .` then `docker run -i --rm markdownink-mcp` |
| **From source** | `dotnet run --project src/MarkdownInk -- README.md` (CLI) · `dotnet run --project src/MarkdownInk.Mcp` (MCP server) |

## Features

- **Headings** — H1/H2 as full-width rules, H3–H6 as colored text
- **Code blocks** — syntax highlighting (30+ languages) via TextMateSharp with VS Code's Dark+ theme, in rounded panels with language labels
- **Inline formatting** — bold, italic, strikethrough, inline code
- **Links** — clickable hyperlinks (OSC 8) in supported terminals
- **Tables** — rounded borders and column alignment
- **Lists** — ordered, unordered, nested, and task lists with checkboxes
- **Blockquotes** — nested, with colored vertical bars
- **Footnotes** — numbered references with a dedicated section
- **Thematic breaks** — clean horizontal rules

## Supported languages

Syntax highlighting works for C#, JavaScript, TypeScript, Python, JSON, XML, HTML, CSS, YAML, Bash,
SQL, Rust, Go, Java, Ruby, C/C++, PHP, Swift, Kotlin, F#, PowerShell, Markdown, Dockerfile,
Makefile, and more.

## Built with

- [Markdig](https://github.com/xoofx/markdig) — Markdown parsing
- [Spectre.Console](https://spectreconsole.net/) — terminal rendering
- [TextMateSharp](https://github.com/danipen/TextMateSharp) — syntax highlighting
- [ModelContextProtocol](https://github.com/modelcontextprotocol/csharp-sdk) — the MCP server SDK

## License

MIT — see [LICENSE](LICENSE).
