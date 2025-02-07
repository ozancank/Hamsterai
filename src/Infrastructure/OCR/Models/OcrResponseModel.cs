using OCK.Core.Interfaces;

namespace Infrastructure.OCR.Models;

public sealed class OcrResponseModel : IResponseModel
{
    public string? Text { get; set; }
}