using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuizResponseModel : IResponseModel
{
    [JsonPropertyName("results")]
    public List<SimilarResponseModel> Questions { get; set; }
}