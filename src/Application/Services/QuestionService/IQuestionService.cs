using Application.Features.Questions.Models.Quizzes;
using DataAccess.EF;
using Infrastructure.AI.Seduss.Dtos;
using Infrastructure.AI.Seduss.Models;

namespace Application.Services.QuestionService;

public interface IQuestionService : IBusinessService
{
    Task SendQuestions(CancellationToken cancellationToken);

    Task<bool> UpdateQuestion(QuestionResponseModel model, UpdateQuestionDto dto);

    Task SendSimilar(CancellationToken cancellationToken);

    Task<bool> AddSimilarQuestion(SimilarResponseModel model, UpdateQuestionDto dto);

    Task<string> AddQuiz(AddQuizModel model, HamsteraiDbContext? context , CancellationToken cancellationToken);

    //Task<bool> UpdateSimilarAnswer(SimilarResponseModel model, UpdateQuestionDto dto);

    //Task<bool> AddQuiz(bool timePass = false, CancellationToken cancellationToken = default);
}