using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class SedussRequestModel : IRequestModel
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("ders")]
    public string Ders { get; set; }
}