using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.Constants;

public class InfrastructureDelegates
{
    public delegate Task<bool> UpdateQuestionOcrDelegate(QuestionTOResponseModel model, UpdateQuestionDto dto);

    public static UpdateQuestionOcrDelegate UpdateQuestionOcr { get; set; }

    public delegate Task<bool> UpdateQuestionOcrImageDelegate(QuestionITOResponseModel model, UpdateQuestionDto dto);

    public static UpdateQuestionOcrImageDelegate UpdateQuestionOcrImage { get; set; }

    public delegate Task<bool> UpdateSimilarQuestionAnswerDelegate(SimilarResponseModel model, UpdateQuestionDto dto);

    public static UpdateSimilarQuestionAnswerDelegate UpdateSimilarQuestionAnswer { get; set; }
}