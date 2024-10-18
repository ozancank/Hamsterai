using OCK.Core.Interfaces;

namespace Infrastructure.OCR.Models;

public class OcrRequestModel : IRequestModel
{
    public string FileName { get; set; }

    public OcrRequestModel()
    {
    }

    public OcrRequestModel(string fileName)
    {
        FileName = fileName;
    }
}