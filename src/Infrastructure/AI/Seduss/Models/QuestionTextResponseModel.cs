using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuestionTextResponseModel : IResponseModel
{
    
    [JsonPropertyName("Cevap_Text")]
    public string AnswerText { get; set; }

    [JsonPropertyName("Cevap_Image")]
    public string AnswerImage { get; set; }

    [JsonPropertyName("Kazanim")]
    public string GainName { get; set; }

    [JsonPropertyName("Cevap")]
    public string RightOption { get; set; }

    [JsonPropertyName("opsiyon")]
    public int OptionCount { get; set; }
}