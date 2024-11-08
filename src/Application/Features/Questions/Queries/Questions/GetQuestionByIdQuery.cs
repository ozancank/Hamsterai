using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Questions;

public class GetQuestionByIdQuery : IRequest<GetQuestionModel>, ISecuredRequest<UserTypes>
{
    public Guid Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetQuestionByIdHandler(IMapper mapper,
                                    IQuestionDal questionDal,
                                    ICommonService commonService) : IRequestHandler<GetQuestionByIdQuery, GetQuestionModel>
{
    public async Task<GetQuestionModel> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await questionDal.GetAsyncAutoMapper<GetQuestionModel>(
            predicate: x => x.Id == request.Id && x.CreateUser == commonService.HttpUserId && x.IsActive,
            enableTracking: request.Tracking,
            include: x => x.Include(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await QuestionRules.QuestionShouldExists(question);
        return question;
    }
}