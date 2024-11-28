using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static string folderPath = "C:\\Users\\Developer\\Desktop\\Seduss\\Sorular";
    private static int questionCount = 10;
    private static short lessonId = 1;
    private static byte testType = 1;
    private const string url1 = "https://api.hamsterai.com.tr/v1/Question/AddRangeQuestion";
    private const string url2 = "https://api.hamsterai.com.tr/v1/Lesson/GetLessons?Page=0&PageSize=0";

    private static async Task Main(string[] _)
    {
        try
        {
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Programdan çıkılıyor...");
                    break;
                }

                Console.WriteLine($"Test tipi [{testType}]: ");
                var lineTestType = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(lineTestType) && byte.TryParse(lineTestType, out byte type) && type > 0)
                {
                    testType = type;
                }
                else if (string.IsNullOrWhiteSpace(lineTestType))
                {
                }
                else
                {
                    Console.WriteLine("Geçersiz tip. Çıkılıyor.");
                    return;
                }

                switch (testType)
                {
                    case 1:
                        await Test1();
                        break;
                    case 2:
                        await Test2();
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }

    private static async Task Test1()
    {
        Console.WriteLine($"Klasör yolu [{folderPath}]: ");
        var linefolderPath = Console.ReadLine() ?? folderPath;
        if (!string.IsNullOrWhiteSpace(linefolderPath)) folderPath = linefolderPath;
        if (string.IsNullOrWhiteSpace(folderPath)) return;

        Console.WriteLine($"Soru sayısı [{questionCount}]: ");
        var lineQuestionCount = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(lineQuestionCount) && int.TryParse(lineQuestionCount, out int imageCount) && imageCount > 0)
        {
            questionCount = imageCount;
        }
        else if (string.IsNullOrWhiteSpace(lineQuestionCount))
        {
        }
        else
        {
            Console.WriteLine("Geçersiz sayı. Çıkılıyor.");
            return;
        }

        Console.WriteLine($"Ders id [{lessonId}]: ");
        var lineLessonId = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(lineLessonId) && short.TryParse(lineLessonId, out short id) && id > 0)
        {
            lessonId = id;
        }
        else if (string.IsNullOrWhiteSpace(lineLessonId))
        {
        }
        else
        {
            Console.WriteLine("Geçersiz id. Çıkılıyor.");
            return;
        }

        var imagePaths = await GetRandomImages();

        await PostImagesAsync(imagePaths);
    }

    private static async Task Test2()
    {
        questionCount = 1;

        Console.WriteLine($"Klasör yolu [{folderPath}]: ");
        var linefolderPath = Console.ReadLine() ?? folderPath;
        if (!string.IsNullOrWhiteSpace(linefolderPath)) folderPath = linefolderPath;
        if (string.IsNullOrWhiteSpace(folderPath)) return;

        var lessons = await GetLessonsAsync();

        foreach (var lesson in lessons.Items)
        {
            lessonId = lesson.Id;
            var imagePaths = await GetRandomImages();
            await PostImagesAsync(imagePaths);
        }
    }


    private static async Task<List<(string, string)>> GetRandomImages()
    {
        var images = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                              .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                          s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                          s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                              .ToList();

        var selectedImages = images.OrderBy(x => new Guid()).ToList().Take(questionCount).ToList();

        var base64Images = new List<(string, string)>();
        foreach (var imagePath in selectedImages)
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            var base64String = Convert.ToBase64String(imageBytes);
            var fileName = Path.GetFileName(imagePath);
            base64Images.Add((base64String, fileName));
        }

        return base64Images;
    }

    private record QuestionModel(string QuestionPictureBase64, string QuestionPictureFileName, short LessonId);

    private record LessonModel(short Id);

    private record LessonListModel(List<LessonModel> Items);

    private static async Task PostImagesAsync(List<(string, string)> imagePaths)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-API-Key", "HaMsTerAI-Security");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4gS3VsbGFuxLFjxLEiLCJlbWFpbCI6ImFkbWluQG1haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIyIiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy91c2VybmFtZSI6IkFkbWluIiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy9tdXN0Y2hhbmdlcGFzc3dvcmQiOmZhbHNlLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL3VzZXJUeXBlIjoxLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL3NjaG9vbElkIjoiIiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy9jb25uZWN0aW9uSWQiOiIiLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL3BhY2thZ2VJZCI6IlN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLkxpc3RgMVtEb21haW4uRW50aXRpZXMuUGFja2FnZVVzZXJdIiwibmJmIjoxNzMxOTE4OTg2LCJleHAiOjE3MzQzMTg5ODYsImlzcyI6ImFueSIsImF1ZCI6ImFueSJ9.ehrQlf96DNOiXtQ2wM6OVq0KW7Dir7EQK61PciUucC8");

        var list = new List<QuestionModel>();

        foreach (var base64Image in imagePaths)
        {
            var model = new QuestionModel(base64Image.Item1, base64Image.Item2, lessonId);
            list.Add(model);
        }

        var json = JsonSerializer.Serialize(list);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url1, content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("İşlem tamamlandı.");
        }
        else
        {
            Console.WriteLine($"Hata Kodu: {response.StatusCode}");
        }
    }

    private static async Task<LessonListModel> GetLessonsAsync()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-API-Key", "HaMsTerAI-Security");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4gS3VsbGFuxLFjxLEiLCJlbWFpbCI6ImFkbWluQG1haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIyIiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy91c2VybmFtZSI6IkFkbWluIiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy9tdXN0Y2hhbmdlcGFzc3dvcmQiOmZhbHNlLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL3VzZXJUeXBlIjoxLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL3NjaG9vbElkIjoiIiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy9jb25uZWN0aW9uSWQiOiIiLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL3BhY2thZ2VJZCI6IlN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLkxpc3RgMVtEb21haW4uRW50aXRpZXMuUGFja2FnZVVzZXJdIiwibmJmIjoxNzMxOTE4OTg2LCJleHAiOjE3MzQzMTg5ODYsImlzcyI6ImFueSIsImF1ZCI6ImFueSJ9.ehrQlf96DNOiXtQ2wM6OVq0KW7Dir7EQK61PciUucC8");

        var result= new LessonListModel([]);

        var response = await client.GetAsync(url2);

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<LessonListModel>();
        }
        else
        {
            Console.WriteLine($"Hata Kodu: {response.StatusCode}");
        }

        return result!;
    }

}