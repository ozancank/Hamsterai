using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuestionRequestModel : IRequestModel
{
    [JsonPropertyName("content")]
    public string? QuestionImage { get; set; }


    [JsonPropertyName("ders")]
    public string? LessonName { get; set; }
}