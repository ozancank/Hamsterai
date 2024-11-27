using Infrastructure.AI.Models;
using Infrastructure.AI.Seduss.Dtos;

namespace Infrastructure.Constants;

public sealed class InfrastructureDelegates
{
    public delegate Task<bool> UpdateQuestionOcrImageDelegate(QuestionResponseModel model, UpdateQuestionDto dto);

    public delegate Task<bool> AddSimilarDelegate(SimilarResponseModel model, UpdateQuestionDto dto);

    public static UpdateQuestionOcrImageDelegate? UpdateQuestionOcrImage { get; set; }

    public static AddSimilarDelegate? AddSimilarAnswer { get; set; }
}