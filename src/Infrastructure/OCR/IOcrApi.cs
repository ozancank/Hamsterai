using Infrastructure.OCR.Models;
using OCK.Core.Interfaces;

namespace Infrastructure.OCR;

public interface IOcrApi : IExternalApi
{
    Task<OcrResponseModel> GetTextFromImage(OcrRequestModel model);
}