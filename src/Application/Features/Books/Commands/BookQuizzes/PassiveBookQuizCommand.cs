using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.BookQuizzes;

public class PassiveBookQuizCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required Guid BookQuizId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveBookQuizCommandHandler(IBookQuizDal bookQuizDal,
                                           ICommonService commonService) : IRequestHandler<PassiveBookQuizCommand, bool>
{
    public async Task<bool> Handle(PassiveBookQuizCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var userType = commonService.HttpUserType;
        var schoolId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var bookQuiz = await bookQuizDal.GetAsync(predicate: x => x.Id == request.BookQuizId && (userType == UserTypes.Administator || x.Book != null && x.Book.SchoolId == schoolId), cancellationToken: cancellationToken);
        await BookQuizRules.BookQuizShouldExists(bookQuiz);

        bookQuiz.UpdateUser = userId;
        bookQuiz.UpdateDate = date;
        bookQuiz.IsActive = false;

        await bookQuizDal.UpdateAsync(bookQuiz, cancellationToken: cancellationToken);

        return true;
    }
}