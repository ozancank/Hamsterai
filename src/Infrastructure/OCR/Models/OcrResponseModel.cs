using OCK.Core.Interfaces;

namespace Infrastructure.OCR.Models;

public class OcrResponseModel : IResponseModel
{
    public string Text { get; set; }
}