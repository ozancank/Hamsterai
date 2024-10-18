using OCK.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Infrastructure.AI.Seduss.Models;

public class QuizTextResponseModel : IResponseModel
{
    [JsonPropertyName("results")]
    public List<SimilarTextResponseModel> Questions { get; set; }
}