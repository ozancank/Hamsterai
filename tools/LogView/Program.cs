using System.Timers;

class Program
{
    private static readonly HttpClient client = new();
    private static readonly System.Timers.Timer timer = new(10000);

    private static void Main()
    {
        client.DefaultRequestHeaders.Add("SyncKey", "b83d6619-a723-4bba-8495-8124e81404e8-573aa71c-28dc-2727-8f3a-e512cdacd455");
        client.DefaultRequestHeaders.Add("X-Api-Key", "HaMsTerAI-Security");

        timer.Elapsed += async (sender, e) => await FetchAndDisplayLogs();
        timer.Start();

        Console.ReadLine();
    }

    private static async Task FetchAndDisplayLogs()
    {
        try
        {
            string url = "https://api.hamsterai.com.tr/v1/Common/GetLogs?onlyError=true";
            var response = await client.GetStringAsync(url);
            var logs = response.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(logs);

            Console.Clear();
            foreach (var log in logs)
            {
                Console.WriteLine(log.Length > 300 ? log[..300] : log);
            }
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }
}
