using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.Constants;

public sealed class InfrastructureDelegates
{
    public delegate Task<bool> UpdateQuestionOcrImageDelegate(QuestionITOResponseModel model, UpdateQuestionDto dto);

    public delegate Task<bool> UpdateSimilarDelegate(SimilarResponseModel model, UpdateQuestionDto dto);

    public static UpdateQuestionOcrImageDelegate? UpdateQuestionOcrImage { get; set; }

    public static UpdateSimilarDelegate? UpdateSimilarAnswer { get; set; }
}