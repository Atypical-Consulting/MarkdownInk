using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;
using Spectre.Console.Rendering;
using MarkdigTable = Markdig.Extensions.Tables.Table;
using MarkdigTableRow = Markdig.Extensions.Tables.TableRow;
using MarkdigTableCell = Markdig.Extensions.Tables.TableCell;
using MarkdigTableColumnAlign = Markdig.Extensions.Tables.TableColumnAlign;

namespace MarkdownInk.Rendering.Blocks;

internal class TableRenderer : MarkdownObjectRenderer<SpectreRenderer, MarkdigTable>
{
    protected override void Write(SpectreRenderer renderer, MarkdigTable table)
    {
        var spectreTable = new Spectre.Console.Table();
        spectreTable.BorderColor(Color.Grey);
        spectreTable.Border(TableBorder.Rounded);

        bool columnsAdded = false;

        foreach (var rowObj in table)
        {
            if (rowObj is not MarkdigTableRow row) continue;

            if (row.IsHeader)
            {
                int colIdx = 0;
                foreach (var cellObj in row)
                {
                    if (cellObj is not MarkdigTableCell cell) continue;

                    var headerText = RenderCellText(renderer, cell);

                    var col = new TableColumn(
                        new Markup($"[{ColorScheme.TableHeader}]{Markup.Escape(headerText)}[/]"));

                    if (colIdx < table.ColumnDefinitions.Count)
                    {
                        col = table.ColumnDefinitions[colIdx].Alignment switch
                        {
                            MarkdigTableColumnAlign.Center => col.Centered(),
                            MarkdigTableColumnAlign.Right => col.RightAligned(),
                            _ => col.LeftAligned()
                        };
                    }

                    spectreTable.AddColumn(col);
                    colIdx++;
                }
                columnsAdded = true;
            }
            else
            {
                if (!columnsAdded)
                {
                    foreach (var cellObj in row)
                    {
                        if (cellObj is not MarkdigTableCell) continue;
                        spectreTable.AddColumn(new TableColumn(""));
                    }
                    columnsAdded = true;
                }

                var cells = new List<IRenderable>();
                foreach (var cellObj in row)
                {
                    if (cellObj is not MarkdigTableCell cell) continue;

                    var cellText = RenderCellText(renderer, cell);
                    cells.Add(new Markup(Markup.Escape(cellText)));
                }
                spectreTable.AddRow(cells.ToArray());
            }
        }

        renderer.Console.Write(spectreTable);
        renderer.Console.WriteLine();
    }

    private static string RenderCellText(SpectreRenderer renderer, MarkdigTableCell cell)
    {
        foreach (var child in cell)
        {
            if (child is ParagraphBlock para && para.Inline is not null)
                renderer.WriteChildren(para.Inline);
        }
        return renderer.DrainInlineBuffer();
    }
}
