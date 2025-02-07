using Domain.Constants;
using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class QuestionRequestModel : IRequestModel
{
    [JsonPropertyName("content")]
    public string? Question { get; set; }

    [JsonPropertyName("ders")]
    public string? LessonName { get; set; }

    [JsonPropertyName("question_type")]
    public QuestionType QuestionType { get; set; }
}