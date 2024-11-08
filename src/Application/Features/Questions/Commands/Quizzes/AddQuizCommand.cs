using Application.Features.Questions.Models.Quizzes;
using Application.Services.QuestionService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Commands.Quizzes;

public class AddQuizCommand : IRequest<GetQuizModel>, ISecuredRequest<UserTypes>
{
    public required AddQuizModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class AddQuizCommandHandler(IMapper mapper,
                                   IQuizDal quizDal,
                                   IQuestionService questionService) : IRequestHandler<AddQuizCommand, GetQuizModel>
{
    public async Task<GetQuizModel> Handle(AddQuizCommand request, CancellationToken cancellationToken)
    {
        var quizId = await questionService.AddQuiz(request.Model, cancellationToken);

        var result = await quizDal.GetAsyncAutoMapper<GetQuizModel>(
            enableTracking: false,
            predicate: x => x.Id == quizId,
            include: x => x.Include(u => u.QuizQuestions),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AddQuizCommandValidator : AbstractValidator<AddQuizCommand>
{
    public AddQuizCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.UserId).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.Model.QuestionList).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);
    }
}