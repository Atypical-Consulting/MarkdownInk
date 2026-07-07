export type ToolKind = 'read' | 'write';

export interface Tool {
  name: string;        // MCP wire name (snake_case)
  title: string;       // human-readable Title advertised to clients
  group: string;
  kind: ToolKind;
  summary: string;
  params: string;
  returns: string;     // shape of the `data` payload (nested inside the ToolResult envelope)
}

export const groups = ['Rendering'] as const;

export const tools: Tool[] = [
  {
    name: 'render_markdown', title: 'Render Markdown', group: 'Rendering', kind: 'read',
    summary:
      'Render a Markdown string to beautiful, terminal-ready output — headings, syntax-highlighted code blocks, tables, lists, task lists, block quotes, footnotes, and clickable links. Read-only: operates purely on the provided string and never touches the filesystem.',
    params: 'markdown, ansi = true, width = 100',
    returns: 'output (the rendered text — ANSI escape codes when ansi:true), ansi, width, lineCount',
  },
  {
    name: 'render_markdown_file', title: 'Render Markdown File', group: 'Rendering', kind: 'read',
    summary:
      'Read a Markdown file from disk and render it to beautiful, terminal-ready output — the same rich formatting as render_markdown. Read-only: reads the file but never modifies it.',
    params: 'path, ansi = true, width = 100',
    returns: 'output, ansi, width, lineCount, path',
  },
];
