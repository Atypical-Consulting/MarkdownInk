export interface Feature {
  title: string;
  blurb: string;
  glyph: string; // a single mono glyph shown on the card
}

// Straight from the README feature list — one card each.
export const features: Feature[] = [
  {
    title: 'Headings',
    glyph: '#',
    blurb: 'H1/H2 as full-width rules, H3–H6 as colored text — a clear visual hierarchy in the terminal.',
  },
  {
    title: 'Syntax-highlighted code',
    glyph: '{ }',
    blurb: '30+ languages via TextMateSharp with VS Code’s Dark+ theme, in rounded panels with a language label.',
  },
  {
    title: 'Inline formatting',
    glyph: 'B',
    blurb: 'Bold, italic, strikethrough, and inline code — rendered, not left as literal Markdown syntax.',
  },
  {
    title: 'Clickable links',
    glyph: '↗',
    blurb: 'Real hyperlinks via OSC 8, so links are clickable in terminals that support it.',
  },
  {
    title: 'Tables',
    glyph: '▦',
    blurb: 'Rendered with rounded borders and per-column alignment — readable at a glance.',
  },
  {
    title: 'Lists & task lists',
    glyph: '☑',
    blurb: 'Ordered, unordered, nested, and task lists with real checkboxes.',
  },
  {
    title: 'Blockquotes',
    glyph: '❝',
    blurb: 'Nested block quotes with colored vertical bars that track the depth.',
  },
  {
    title: 'Footnotes',
    glyph: '†',
    blurb: 'Numbered references collected into a dedicated footnotes section.',
  },
  {
    title: 'Thematic breaks',
    glyph: '—',
    blurb: 'Horizontal rules that read as clean, full-width separators.',
  },
];
