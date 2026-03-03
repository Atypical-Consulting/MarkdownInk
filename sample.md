# Welcome to mdink

This is a **terminal markdown renderer** built with .NET 10.

## Features

Here's what mdink can render with *beautiful colors*:

### Text Formatting

You can use **bold text**, *italic text*, and ~~strikethrough text~~.
Combine them: ***bold and italic***, or even **bold with ~~strikethrough~~**.

Inline `code` looks distinct, like `Console.WriteLine("Hello!")`.

### Links and Images

Visit [Spectre.Console](https://spectreconsole.net) for the rendering engine.
Check out [Markdig](https://github.com/xoofx/markdig) for the parser.

![Alt text](https://example.com/image.png)

---

## Code Blocks

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello from mdink!");
    }
}
```

```json
{
  "name": "mdink",
  "version": "1.0.0",
  "description": "Terminal markdown renderer"
}
```

## Lists

### Unordered List

- First item
- Second item with **bold**
- Third item
  - Nested item A
  - Nested item B
    - Deep nested

### Ordered List

1. Step one
2. Step two
3. Step three

### Task List

- [x] Parse markdown with Markdig
- [x] Render with Spectre.Console
- [ ] Add more themes
- [ ] Publish to NuGet

## Blockquotes

> This is a blockquote.
> It can span multiple lines.

> Nested quotes work too:
> > This is a nested blockquote.
> > With **formatted text** inside.

## Tables

| Feature       | Status   | Priority |
|:--------------|:--------:|---------:|
| Headings      | Done     | High     |
| Bold/Italic   | Done     | High     |
| Code blocks   | Done     | High     |
| Tables        | Done     | Medium   |
| Footnotes     | Done     | Low      |

## Footnotes

Here is a sentence with a footnote[^1]. And another one[^2].

[^1]: This is the first footnote.
[^2]: This is the second footnote with **bold** text.

---

#### H4 Heading
##### H5 Heading
###### H6 Heading

That's all folks!
