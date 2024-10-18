using OCK.Core.Interfaces;

namespace Infrastructure.OCR.Gemini.Models;

public class GeminiOcrResponseModel : IResponseModel
{
    public string Message { get; set; }
}