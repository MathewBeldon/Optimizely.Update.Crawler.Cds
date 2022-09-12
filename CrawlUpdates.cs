using HtmlAgilityPack;
using Optimizely.Update.Crawler.Cds.Models;
using System.Web;

namespace Optimizely.Update.Crawler.Cds
{
    internal static class CrawlUpdates
    {
        static readonly HttpClient client = new HttpClient();
        internal static async Task<IEnumerable<Models.Update>> GetUntilAsync(int lastUpdate)
        {
            List<Models.Update> Updates = new List<Models.Update>();
            bool finished = false;

            while (!finished)
            {
                var update = new Models.Update();
                try
                {
                    update.UpdateNumber = lastUpdate;
                    string responseBody = string.Empty;
                    string url = string.Empty;

                    // the url seems to have a random number of '-' in it, can improve by crawling urls from RSS feed
                    for (int i = 0; i < 5; i++)
                    {
                        url = $"https://world.optimizely.com/releases/optimizely{new String('-', i + 1)}update-{lastUpdate}";
                        try
                        {
                            responseBody = await client.GetStringAsync(url);
                        }
                        catch (HttpRequestException e)
                        {
                            continue;
                        }
                        break;
                    }
                    update.UpdateUrl = url;
                    if (string.Empty == responseBody)
                    {
                        throw new Exception();
                    }
                    Console.WriteLine($"Captured update {lastUpdate++}");
                    HtmlDocument htmlSnippet = new HtmlDocument();
                    htmlSnippet.LoadHtml(responseBody);

                    var data = new List<RowData>();
                    var htmlTable = htmlSnippet.DocumentNode.SelectNodes("//table");
                    if (htmlTable is not null)
                    {
                        foreach (HtmlNode table in htmlTable)
                        {
                            foreach (HtmlNode row in table.SelectNodes("tr"))
                            {
                                var rowData = new RowData();
                                int counter = 0;
                                foreach (HtmlNode cell in row.SelectNodes("td"))
                                {
                                    switch (counter)
                                    {
                                        case 1:
                                            rowData.Id = cell.InnerText.Trim();
                                            if (rowData.Id.Contains("CMS"))
                                            {
                                                rowData.Area = "Content Cloud";
                                            }
                                            else if (rowData.Id.Contains("FIND"))
                                            {
                                                rowData.Area = "Find";
                                            }
                                            else if (rowData.Id.Contains("HAPI"))
                                            {
                                                rowData.Area = "Content Delivery";
                                            }
                                            else if (rowData.Id.Contains("AFORM") || rowData.Id.Contains("MAI"))
                                            {
                                                rowData.Area = "Apps & Integrations";
                                            }
                                            else if (rowData.Id.Contains("PAAS"))
                                            {
                                                rowData.Area = "DXP";
                                            }
                                            else if (rowData.Id.Contains("LM"))
                                            {
                                                rowData.Area = "Personalization";
                                            }
                                            else if (rowData.Id.Contains("COM"))
                                            {
                                                update.CommerceUpdateCount++;
                                            }
                                            else
                                            {
                                                rowData.Area = rowData.Id;
                                            }

                                            break;
                                        case 2:
                                            rowData.Type = cell.ChildNodes[1].Attributes[1].Value;
                                            break;
                                        case 3:
                                            rowData.DescriptionTitle = HttpUtility.HtmlDecode(cell.SelectNodes("div").First().InnerText
                                                .Replace("&nbsp;", string.Empty).Trim());
                                            rowData.Description = cell.SelectNodes("div/p").Select(x => HttpUtility.HtmlDecode(x.InnerText.Trim()));
                                            break;
                                        case 4:
                                            rowData.Released = DateTime.Parse(cell.InnerText.Trim());
                                            break;
                                    }
                                    counter++;
                                }
                                data.Add(rowData);
                            }
                        }
                        update.UpdateDate = data.FirstOrDefault().Released;
                    }
                    update.RowData = data;
                    Updates.Add(update);
                }
                catch (Exception e)
                {
                    finished = true;                    
                }
            }
            return Updates;
        }
    }
}
