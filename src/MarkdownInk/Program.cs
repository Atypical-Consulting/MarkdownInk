using Markdig;
using MarkdownInk.Rendering;

string markdown;

if (args.Length > 0)
{
    var path = args[0];
    if (!File.Exists(path))
    {
        Console.Error.WriteLine($"Error: File not found: {path}");
        return 1;
    }
    markdown = await File.ReadAllTextAsync(path);
}
else if (Console.IsInputRedirected)
{
    markdown = await Console.In.ReadToEndAsync();
}
else
{
    Console.Error.WriteLine("Usage: mdink <file.md>  or  cat file.md | mdink");
    return 1;
}

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var document = Markdown.Parse(markdown, pipeline);
var renderer = new SpectreRenderer();
renderer.Render(document);

return 0;
