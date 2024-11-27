using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class SimilarResponseModel : IResponseModel
{
    [JsonPropertyName("similar")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("gain")]
    public string? GainName { get; set; }

    [JsonPropertyName("answer")]
    public string? RightOption { get; set; }
}