using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuestionTextRequestModel : IRequestModel
{
    [JsonPropertyName("content")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("ders")]
    public string? LessonName { get; set; }
}