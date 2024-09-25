using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class OcrResponseModel : IResponseModel
{
    [JsonPropertyName("text")]
    public string QuestionText { get; set; }
}