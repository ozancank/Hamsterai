using Application.Features.Questions.Models.Quizzes;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OneOf.Types;

namespace Application.Features.Questions.Queries.Quizzes;

public class GetQuizByIdQuery : IRequest<GetQuizModel>, ISecuredRequest<UserTypes>
{
    public required string Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Student, UserTypes.Person];
    public bool AllowByPass => false;
}

public class GetQuizByIdQueryHandler(IMapper mapper,
                                     IQuizDal quizDal,
                                     ICommonService commonService) : IRequestHandler<GetQuizByIdQuery, GetQuizModel>
{
    public async Task<GetQuizModel> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await quizDal.GetAsyncAutoMapper<GetQuizModel>(
            predicate: x => x.Id == request.Id && (commonService.HttpUserType == UserTypes.Administator || (x.UserId == commonService.HttpUserId && x.IsActive)),
            enableTracking: request.Tracking,
            include: x => x.Include(u => u.Lesson)
                           .Include(u => u.User).ThenInclude(u => u!.School)
                           .Include(u => u.QuizQuestions.OrderBy(x => x.SortNo)).ThenInclude(u => u.Gain),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        quiz.GainNames = [.. quiz.GainNames.Where(x => x.IsNotEmpty()).Distinct()];

        if (request.ThrowException)
        {
            await QuizRules.QuizShouldExists(quiz);
            if (quiz.Questions.Count == 0)
                throw new BusinessException(Strings.DynamicNotFound, $"{Strings.Quiz} {Strings.OfQuestion}");
        }
        return quiz;
    }
}