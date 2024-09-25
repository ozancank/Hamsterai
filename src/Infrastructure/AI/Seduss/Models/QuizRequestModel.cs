using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuizRequestModel : IRequestModel
{
    [JsonPropertyName("content")]
    public List<string> QuestionImages { get; set; }


    [JsonPropertyName("ders")]
    public string LessonName { get; set; }
}