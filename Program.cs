using Optimizely.Update.Crawler.Cds;

internal class Program
{
    static async Task Main()
    {
        Console.Write("Last update: ");
        var lastUpdate = int.Parse(Console.ReadLine() ?? throw new ArgumentNullException());

        var updates = await CrawlUpdates.GetUntilAsync(lastUpdate);
        var filePath = GenerateDoc.Create(updates);

        Console.WriteLine($"Created file path: {filePath}");
    }
}