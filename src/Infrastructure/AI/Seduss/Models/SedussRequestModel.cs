using OCK.Core.Interfaces;

namespace Infrastructure.AI.Seduss.Models;

public class SedussRequestModel : IRequestModel
{
    public string Base64 { get; set; }
    public string LessonName { get; set; }
}
