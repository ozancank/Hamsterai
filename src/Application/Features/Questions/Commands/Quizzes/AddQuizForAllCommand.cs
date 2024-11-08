using Application.Features.Questions.Models.Quizzes;
using Application.Services.QuestionService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Commands.Quizzes;

public class AddQuizForAllCommand : IRequest, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class AddQuizForAllCommandHandler(IQuestionService questionService) : IRequestHandler<AddQuizForAllCommand>
{
    public async Task Handle(AddQuizForAllCommand request, CancellationToken cancellationToken)
    {
        await questionService.AddQuiz(true, cancellationToken);
    }
}