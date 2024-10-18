using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.OCR.Gemini.Models;

public class GeminiOcrRequestModel : IRequestModel
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
}