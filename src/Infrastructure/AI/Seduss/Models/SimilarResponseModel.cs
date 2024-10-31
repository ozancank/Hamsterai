using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class SimilarResponseModel : IModel
{
    [JsonPropertyName("Soru_OCR")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("Benzer_Soru_Text")]
    public string? SimilarQuestionText { get; set; }

    [JsonPropertyName("Benzer_Image")]
    public string? SimilarImage { get; set; }

    [JsonPropertyName("Cevap_Text")]
    public string? AnswerText { get; set; }

    [JsonPropertyName("Cevap_Image")]
    public string? AnswerImage { get; set; }

    [JsonPropertyName("Kazanim")]
    public string? GainName { get; set; }

    [JsonPropertyName("Cevap")]
    public string? RightOption { get; set; }

    [JsonPropertyName("opsiyon")]
    public int OptionCount { get; set; }
}