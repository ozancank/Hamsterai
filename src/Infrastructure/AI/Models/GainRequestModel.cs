using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class GainRequestModel : IRequestModel
{
    [JsonPropertyName("soru")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("paket")]
    public string? LessonName { get; set; }
}