using Application.Features.Questions.Models.Quizzes;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Application.Services.QuestionService;

public interface IQuestionService : IBusinessService
{
    Task<bool> UpdateAnswer(QuestionITOResponseModel model, UpdateQuestionDto dto);

    Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto);

    Task SendQuestions(CancellationToken cancellationToken);

    Task<string> AddQuiz(AddQuizModel model, CancellationToken cancellationToken);

    Task<bool> AddQuiz(bool timePass = false, CancellationToken cancellationToken = default);
}