using OCK.Core.Interfaces;

namespace Infrastructure.AI.Seduss.Models;

public class OcrResponseModel : IResponseModel
{
    public string Text { get; set; }
}