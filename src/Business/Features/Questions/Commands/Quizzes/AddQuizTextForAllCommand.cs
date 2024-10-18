using Business.Services.QuestionService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Commands.Quizzes;

public class AddQuizTextForAllCommand : IRequest, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class AddQuizTextForAllCommandHandler(IQuestionService questionService) : IRequestHandler<AddQuizTextForAllCommand>
{
    public async Task Handle(AddQuizTextForAllCommand request, CancellationToken cancellationToken)
    {
        await questionService.AddQuizText(true, cancellationToken);
    }
}