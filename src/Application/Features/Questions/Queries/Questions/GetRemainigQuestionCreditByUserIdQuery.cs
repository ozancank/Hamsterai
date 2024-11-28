using Application.Features.Users.Rules;
using Application.Services.CommonService;
using Application.Services.UserService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Questions;

public class GetRemainigQuestionCreditByUserIdQuery : IRequest<int>, ISecuredRequest<UserTypes>
{
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetRemainigQuestionCreditByUserIdQueryHandler(ICommonService commonService,
                                                           IUserService userService,
                                                           UserRules userRules) : IRequestHandler<GetRemainigQuestionCreditByUserIdQuery, int>
{
    public async Task<int> Handle(GetRemainigQuestionCreditByUserIdQuery request, CancellationToken cancellationToken)
    {
        await userRules.UserTypeAllowed(commonService.HttpUserType, request.UserId, true);

        var credit = await userService.RemainigQuestionCredit(request.UserId);

        return credit;
    }
}