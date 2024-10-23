using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Infrastructure.Constants;

public class InfrastructureDelegates
{
    public delegate Task<bool> UpdateQuestionOcrImageDelegate(QuestionITOResponseModel model, UpdateQuestionDto dto);

    public delegate Task<bool> UpdateSimilarDelegate(SimilarResponseModel model, UpdateQuestionDto dto);

    public delegate Task<bool> UpdateQuestionTextDelegate(QuestionTextResponseModel model, UpdateQuestionDto dto);

    public delegate Task<bool> UpdateSimilarTextDelegate(SimilarTextResponseModel model, UpdateQuestionDto dto);

    public delegate Task<bool> UpdateQuestionVisualDelegate(QuestionVisualResponseModel model, UpdateQuestionDto dto);

    public static UpdateQuestionOcrImageDelegate UpdateQuestionOcrImage { get; set; }

    public static UpdateSimilarDelegate UpdateSimilarAnswer { get; set; }

    public static UpdateQuestionTextDelegate UpdateQuestionText { get; set; }

    public static UpdateSimilarTextDelegate UpdateSimilarText { get; set; }

    public static UpdateQuestionVisualDelegate UpdateQuestionVisual { get; set; }
}