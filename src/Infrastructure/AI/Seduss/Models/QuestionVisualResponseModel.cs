using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuestionVisualResponseModel : IResponseModel
{
    [JsonPropertyName("Soru_OCR")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("Cevap_Text")]
    public string? AnswerText { get; set; }

    [JsonPropertyName("Cevap_Image")]
    public string? AnswerImage { get; set; }
}