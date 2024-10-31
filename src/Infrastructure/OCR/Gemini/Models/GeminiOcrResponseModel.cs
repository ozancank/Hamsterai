using OCK.Core.Interfaces;

namespace Infrastructure.OCR.Gemini.Models;

public sealed class GeminiOcrResponseModel : IResponseModel
{
    public string Message { get; set; }
}