using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class SimilarRequestModel : IRequestModel
{
    [JsonPropertyName("soru")]
    public string? QuestionText { get; set; }

    [JsonPropertyName("paket")]
    public string? PackageName { get; set; }
}