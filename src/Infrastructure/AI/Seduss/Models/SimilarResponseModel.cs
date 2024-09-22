using OCK.Core.Interfaces;

namespace Infrastructure.AI.Seduss.Models;

public class SimilarResponseModel : IModel
{
    public string Soru_OCR { get; set; }
    public string Benzer_Soru_Text { get; set; }
    public string Benzer_Image { get; set; }
    public string Cevap_Text { get; set; }
    public string Cevap_Image { get; set; }
    public string Kazanim { get; set; }
    public string Cevap { get; set; }
}