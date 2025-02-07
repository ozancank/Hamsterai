using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Models;

public class GainResponseModel : IResponseModel
{
    [JsonPropertyName("gain")]
    public string? GainName { get; set; }
}