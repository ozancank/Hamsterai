using Application.Features.Users.Rules;
using Application.Services.CommonService;
using Application.Services.UserService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Questions;

public class GetValidTotalQuestionByUserIdQuery : IRequest<int>, ISecuredRequest<UserTypes>
{
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetValidTotalQuestionByUserIdQueryHandler(ICommonService commonService,
                                                       IUserService userService,
                                                       UserRules userRules) : IRequestHandler<GetValidTotalQuestionByUserIdQuery, int>
{
    public async Task<int> Handle(GetValidTotalQuestionByUserIdQuery request, CancellationToken cancellationToken)
    {
        await userRules.UserTypeAllowed(commonService.HttpUserType, request.UserId, true);

        var credit = await userService.ValidTotalQuestion(request.UserId);

        return credit;
    }
}