using Application.Services.QuestionService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Commands.Quizzes;

public class AddQuizTextForAllCommand : IRequest, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class AddQuizTextForAllCommandHandler(IQuestionService questionService) : IRequestHandler<AddQuizTextForAllCommand>
{
    public async Task Handle(AddQuizTextForAllCommand request, CancellationToken cancellationToken)
    {
        await questionService.AddQuizText(true, cancellationToken);
    }
}