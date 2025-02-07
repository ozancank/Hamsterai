using OCK.Core.Interfaces;

namespace Infrastructure.OCR.Models;

public sealed class OcrRequestModel : IRequestModel
{
    public string? FileName { get; set; }
    public string? UserName { get; set; }
    public long UserId { get; set; }

    public OcrRequestModel()
    {
    }

    public OcrRequestModel(string? fileName, string? userName, long userId)
    {
        FileName = fileName;
        UserName = userName;
        UserId = userId;
    }
}