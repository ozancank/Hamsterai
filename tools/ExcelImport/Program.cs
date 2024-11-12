using ExcelDataReader;
using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json;

internal class Program
{
    private const string BASE_URL = "https://testapi.hamsterai.com.tr";

    private static async Task Main()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiw5Z6ZWwgRGXEn2VyIEtvbGVqaSBFxJ9pdGltIEt1cnVtbGFyxLEgIiwiZW1haWwiOiJzZW5lbG9tZXJmYXJ1a0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1IiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy91c2VybmFtZSI6IjY4NTA1MzUwNDYiLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL211c3RjaGFuZ2VwYXNzd29yZCI6dHJ1ZSwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy91c2VyVHlwZSI6MiwiaHR0cDovL3VzZXJzd2l0aG91dGlkZW50aXR5L2NsYWltcy9zY2hvb2xJZCI6IjMiLCJodHRwOi8vdXNlcnN3aXRob3V0aWRlbnRpdHkvY2xhaW1zL2Nvbm5lY3Rpb25JZCI6IiIsImh0dHA6Ly91c2Vyc3dpdGhvdXRpZGVudGl0eS9jbGFpbXMvcGFja2FnZUlkIjoiU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWMuTGlzdGAxW0RvbWFpbi5FbnRpdGllcy5QYWNrYWdlVXNlcl0iLCJuYmYiOjE3MzE0MzE3MDQsImV4cCI6MTczMzgzMTcwNCwiaXNzIjoiYW55IiwiYXVkIjoiYW55In0.MQEJYEt3ywBvWE_nCN46DR81I0ymTkh7ODfhpJ_AqOg");
        client.DefaultRequestHeaders.Add("X-Api-Key", "HaMsTerAI-Security");

        await AddTeachers(client);
        //await AddStudents(client);

        Console.WriteLine("İşlem tamamlandı.");
        Console.ReadLine();
    }

    private static async Task AddTeachers(HttpClient client)
    {
        string excelPath = "HamsterKayıt2.xlsx";

        string urlCreate = $"{BASE_URL}/v1/School/AddTeacher";
        string urlUpdate = $"{BASE_URL}/v1/School/UpdateTeacher";
        string urlGet = $"{BASE_URL}/v1/School/GetTeachersDynamic?Page=0&PageSize=0";

        List<Teacher> teachers = [];

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
        {
            using var reader = ExcelReaderFactory.CreateReader(stream);
            reader.Read();

            while (reader.Read())
            {
                var teacher = new Teacher
                {
                    Name = reader.GetValue(0)?.ToString(),
                    Surname = reader.GetValue(1)?.ToString(),
                    Phone = reader.GetValue(2)?.ToString(),
                    TcNo = null,
                    Email = reader.GetValue(3)?.ToString(),
                    Branch = reader.GetValue(4)?.ToString()
                };

                teachers.Add(teacher);
            }
        }

        for (int i = 0; i < teachers.Count; i++)
        {
            var teacher = teachers[i];

            var response = await client.PostAsJsonAsync(urlGet, new
            {
                Filter = new
                {
                    Field = "Phone",
                    Operator = "eq",
                    Value = teacher.Phone,
                }
            });
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic dynamicContent = JsonSerializer.Deserialize<ExpandoObject>(content);
                if (dynamicContent.count is JsonElement countElement && countElement.ValueKind == JsonValueKind.Number && countElement.GetInt32() == 0)
                {
                    var responseCreate = await client.PostAsJsonAsync(urlCreate, teacher);
                    if (responseCreate.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Eklendi: {i + 1}");
                    }
                    else
                    {
                        Console.WriteLine($"Veri ekleme başarısız: {i + 1} {teacher.Name} {teacher.Surname}");
                    }
                }
                else
                {
                    if (dynamicContent.items is JsonElement itemsElement && itemsElement.ValueKind == JsonValueKind.Array)
                    {
                        var firstItem = itemsElement[0];

                        if (firstItem.TryGetProperty("id", out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number)
                        {
                            teacher.Id = idElement.GetInt32();
                        }
                    }
                    var responseUpdate = await client.PostAsJsonAsync(urlUpdate, teacher);
                    if (responseUpdate.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Güncellendi: {i + 1}");
                    }
                    else
                    {
                        Console.WriteLine($"Veri güncelleme başarısız: {i + 1} {teacher.Name} {teacher.Surname}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Veri getirme başarısız: {i + 1} {teacher.Name} {teacher.Surname}");
            }
        }
    }

    private static async Task AddStudents(HttpClient client)
    {
        string excelPath = "HamsterKayıt.xlsx";

        string urlCreate = $"{BASE_URL}/v1/Student/AddStudent";
        string urlUpdate = $"{BASE_URL}/v1/Student/UpdateStudent";
        string urlGet = $"{BASE_URL}/v1/Student/GetStudentsDynamic?Page=0&PageSize=0";

        List<Student> students = [];

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
        {
            using var reader = ExcelReaderFactory.CreateReader(stream);
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

        for (int i = 0; i < students.Count; i++)
        {
            var student = students[i];

            var response = await client.PostAsJsonAsync(urlGet, new
            {
                Filter = new
                {
                    Field = "StudentNo",
                    Operator = "eq",
                    Value = student.StudentNo,
                }
            });
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic dynamicContent = JsonSerializer.Deserialize<ExpandoObject>(content);
                if (dynamicContent.count is JsonElement countElement && countElement.ValueKind == JsonValueKind.Number && countElement.GetInt32() == 0)
                {
                    var responseCreate = await client.PostAsJsonAsync(urlCreate, student);
                    if (responseCreate.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Eklendi: {i + 1}");
                    }
                    else
                    {
                        Console.WriteLine($"Veri ekleme başarısız: {i + 1} {student.Name} {student.Surname}");
                    }
                }
                else
                {
                    if (dynamicContent.items is JsonElement itemsElement && itemsElement.ValueKind == JsonValueKind.Array)
                    {
                        var firstItem = itemsElement[0];

                        if (firstItem.TryGetProperty("id", out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number)
                        {
                            student.Id = idElement.GetInt32();
                        }
                    }
                    var responseUpdate = await client.PostAsJsonAsync(urlUpdate, student);
                    if (responseUpdate.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Güncellendi: {i + 1}");
                    }
                    else
                    {
                        Console.WriteLine($"Veri güncelleme başarısız: {i + 1} {student.Name} {student.Surname}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Veri getirme başarısız: {i + 1} {student.Name} {student.Surname}");
            }
        }
    }
}

internal class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Branch { get; set; }
}

internal class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string StudentNo { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int ClassRoomId { get; set; }
}