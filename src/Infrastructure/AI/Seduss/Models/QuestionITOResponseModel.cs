using OCK.Core.Interfaces;

namespace Infrastructure.AI.Seduss.Models;

public class QuestionITOResponseModel : IResponseModel
{
    public string Soru_OCR { get; set; }
    public string Cevap_Text { get; set; }
    public string Cevap_Image { get; set; }
    public string Kazanim { get; set; }
    public string Cevap { get; set; }
}