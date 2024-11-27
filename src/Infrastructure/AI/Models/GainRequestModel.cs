using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class GainRequestModel : IRequestModel
{
    [JsonPropertyName("content")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("ders")]
    public string? LessonName { get; set; }
}