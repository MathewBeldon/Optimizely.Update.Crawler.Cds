using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Optimizely.Update.Crawler.Cds
{
    internal static class GenerateDoc
    {
        internal static string Create(IEnumerable<Models.Update> updates)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".docx"); 
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new Document();

                Body body = mainPart.Document.AppendChild(new Body(
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial" },
                        new FontSize() { Val = "20" }
                    )
                ));

                foreach (var update in updates)
                {
                    wordDocument.MainDocumentPart.Document.Body.Append(new Paragraph(
                        new Run(
                            new RunProperties(new Italic(), new FontSize() { Val = "24" }),
                            new Text($"Update {update.UpdateNumber}")
                        )
                    ));

                    wordDocument.MainDocumentPart.Document.Body.Append(new Paragraph(
                        new Run(
                            new Text(update.UpdateDate.ToString("d MMMM yyyy"))
                        )
                    ));

                    wordDocument.MainDocumentPart.Document.Body.Append(new Paragraph(
                        new Run(
                            new Text() { Text = "Link to update: ", Space = SpaceProcessingModeValues.Preserve },
                            new Hyperlink(
                                new Run(
                                    new RunProperties(
                                        new RunStyle { Val = "Hyperlink" },
                                        new Underline { Val = UnderlineValues.Single },
                                        new Color { ThemeColor = ThemeColorValues.Hyperlink }),
                                    new Text(update.UpdateUrl)))
                            {
                                Id = update.UpdateUrl
                            }
                        )
                    ));

                    if (update.CommerceUpdateCount > 0)
                    {
                        wordDocument.MainDocumentPart.Document.Body.Append(new Paragraph(
                            new Run(
                                new Text($"Includes {update.CommerceUpdateCount} updates to Commerce which have not been included as not relevant.")
                            )
                        ));
                    }

                    if (update.RowData.Count == 0 || update.RowData.Count(x => x.Id.Contains("COM")) == update.RowData.Count)
                    {
                        wordDocument.MainDocumentPart.Document.Body.Append(new Paragraph(
                            new Run(
                                new Text("No Updates in table")
                            )
                        ));
                        continue;
                    }

                    Table table = new Table();

                    TableRow th = new TableRow();

                    th.Append(new TableCell(
                        new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }),
                        new Paragraph(new Run(new Text("Area")))
                    ));

                    th.Append(new TableCell(
                        new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }),
                        new Paragraph(new Run(new Text("Type")))
                    ));

                    th.Append(new TableCell(
                        new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }),
                        new Paragraph(new Run(new Text("Detail")))
                    ));

                    table.Append(th);

                    foreach (var data in update.RowData)
                    {
                        if (data.Id.Contains("COM"))
                        {
                            continue;
                        }

                        TableRow td = new TableRow();

                        td.Append(new TableCell(
                            new Paragraph(new Run(new Text(data.Area)))
                        ));

                        td.Append(new TableCell(
                           new Paragraph(new Run(new Text(data.Type)))
                        ));

                        TableCell tc = new TableCell();
                        tc.Append(new Paragraph(
                            new Run(
                                new RunProperties(new Bold()),
                                new Run(new Text(data.DescriptionTitle)))
                        ));
                        foreach (var description in data.Description)
                        {
                            tc.Append(new Paragraph(new Run(new Text(description))));
                        }
                        tc.Append(new Paragraph());
                        tc.Append(new Paragraph(new Run(new Text("https://world.optimizely.com/documentation/Release-Notes/ReleaseNote/?releaseNoteId=" + data.Id))));
                        td.Append(tc);

                        table.Append(td);
                    }

                    wordDocument.MainDocumentPart.Document.Body.Append(table);

                    wordDocument.MainDocumentPart.Document.Body.Append(new Paragraph(
                        new Run(
                            new RunProperties(
                                new Italic(), 
                                new Color() { Val = "1ba39c" },
                                new FontSize() { Val = "22" }
                            ),
                            new Text("CDS Analysis")
                        )
                    ));
                }
            }
            return path;
        }
    }
}
