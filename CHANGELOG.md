# Changelog

All notable changes to MarkdownInk are documented here. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-07-07

### Added

- **`mdink` CLI** — renders Markdown beautifully in the terminal (Markdig + Spectre.Console +
  TextMateSharp): headings, syntax-highlighted code blocks (30+ languages, VS Code Dark+ theme),
  inline formatting, OSC-8 clickable links, tables, ordered/unordered/task lists, nested block
  quotes, footnotes, and thematic breaks. Reads a file argument or stdin.
- **`MarkdownInk.Mcp` MCP server** — the same rendering engine exposed as a stdio Model Context
  Protocol server. Two read-only tools, `render_markdown` and `render_markdown_file`, with an
  `ansi` toggle (default `true`) and configurable `width` (default `100`), returning a uniform
  `ToolResult` envelope. Packaged as an `McpServer` dotnet tool, launchable on demand via `dnx`.
- **Packaging** — Dockerfile (multi-arch alpine, stdio entrypoint), MCPB bundle manifest, and the
  MCP registry manifest.
- **Marketing website** — an Astro static site published to GitHub Pages.
