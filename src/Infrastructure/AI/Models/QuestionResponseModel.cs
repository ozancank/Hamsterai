using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class QuestionResponseModel : IResponseModel
{
    [JsonPropertyName("Soru_OCR")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("Cevap_Text")]
    public string? AnswerText { get; set; }

    [JsonPropertyName("answer")]
    public string? AnswerText2 { get; set; }

    [JsonPropertyName("ai_solution")]
    public string? AnswerText3 { get; set; }

    [JsonPropertyName("Cevap_Image")]
    public string? AnswerImage { get; set; }

    [JsonPropertyName("Kazanim")]
    public string? GainName { get; set; }

    [JsonPropertyName("Cevap")]
    public string? RightOption { get; set; }

    [JsonPropertyName("ocr_method")]
    public string OcrMethod { get; set; } = string.Empty;

    [JsonPropertyName("isExistsVisualContent")]
    public bool IsExistsVisualContent { get; set; }
}