using Markdig.Extensions.TaskLists;
using Markdig.Renderers;
using Spectre.Console;

namespace MarkdownInk.Rendering.Inlines;

internal class TaskListRenderer : MarkdownObjectRenderer<SpectreRenderer, TaskList>
{
    protected override void Write(SpectreRenderer renderer, TaskList obj)
    {
        if (obj.Checked)
            renderer.InlineBuffer.Append($"[{ColorScheme.TaskChecked}]{Markup.Escape("[x]")}[/] ");
        else
            renderer.InlineBuffer.Append($"[{ColorScheme.TaskUnchecked}]{Markup.Escape("[ ]")}[/] ");
    }
}
