using OCK.Core.Interfaces;

namespace Infrastructure.AI.Seduss.Models;

public class SimilarModel : IModel
{
    public string Soru_OCR { get; set; }
    public string Benzer_Soru_Text { get; set; }
    public string Benzer_Image { get; set; }
    public string Cevap_Text { get; set; }
    public string Cevap_Image { get; set; }
}