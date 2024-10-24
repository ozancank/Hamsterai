using ExcelDataReader;
using System.Net.Http.Json;

internal class Program
{
    private static async Task Main()
    {
        // Excel dosyasını okuyacağımız path
        string excelPath = "Hamster Kayıt.xlsx"; // Buraya dosyanın yolunu girin
        string url = "https://api.hamsterai.com.tr/v1/Student/AddStudent"; // POST isteği yapılacak URL

        // Excel verilerini listeye oku
        List<Student> students = ReadExcelData(excelPath);

        // Her bir öğrenci için POST isteği gönder
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiw5Z6ZWwgRGXEn2VyIEtvbGVqaSBFxJ9pdGltIEt1cnVtbGFyxLEgIiwiZW1haWwiOiJzZW5lbG9tZXJmYXJ1a0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1IiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy91c2VybmFtZSI6IjY4NTA1MzUwNDYiLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL211c3RjaGFuZ2VwYXNzd29yZCI6dHJ1ZSwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy91c2VyVHlwZSI6MiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy9zY2hvb2xJZCI6IjMiLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL2Nvbm5lY3Rpb25JZCI6IiIsImh0dHA6Ly91c2Vyc3dpdGhvdXRpZGVudGl0eS9jbGFpbXMvZ3JvdXBJZCI6IiIsIm5iZiI6MTcyOTcxMjEwNywiZXhwIjoxNzMyMTEyMTA3LCJpc3MiOiJhbnkiLCJhdWQiOiJhbnkifQ.SX4GmxddZ427otJSIYf14eMfic9gBrDUKD6716Z-9HM");

        client.DefaultRequestHeaders.Add("X-Api-Key", "HaMsTerAI-Security");

        for (int i = 0; i < students.Count; i++)
        {
            var student = students[i];
            var response = await client.PostAsJsonAsync(url, student);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"{i + 1}");
            }
            else
            {
                Console.WriteLine($"Veri gönderme başarısız: {i + 1} {student.Name} {student.Surname}");
            }
        }
    }

    // Excel dosyasını okuyan method
    private static List<Student> ReadExcelData(string filePath)
    {
        List<Student> students = [];

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using var reader = ExcelReaderFactory.CreateReader(stream);
            // İlk satırı başlık satırı olarak kabul et
            reader.Read();

            while (reader.Read())
            {
                var student = new Student
                {
                    Name = reader.GetValue(0)?.ToString(),
                    Surname = reader.GetValue(1)?.ToString(),
                    StudentNo = reader.GetValue(2)?.ToString(),
                    TcNo = reader.GetValue(3)?.ToString(),
                    Phone = reader.GetValue(4)?.ToString(),
                    Email = reader.GetValue(5)?.ToString(),
                    ClassRoomId = int.Parse(reader.GetValue(6)?.ToString())
                };

                students.Add(student);
            }
        }

        return students;
    }
}

// Öğrenci model sınıfı
internal class Student
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string StudentNo { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int ClassRoomId { get; set; }
}