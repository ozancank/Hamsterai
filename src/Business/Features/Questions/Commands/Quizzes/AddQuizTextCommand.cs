using Business.Features.Questions.Models.Quizzes;
using Business.Services.QuestionService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Questions.Commands.Quizzes;

public class AddQuizTextCommand : IRequest<GetQuizModel>, ISecuredRequest<UserTypes>
{
    public AddQuizModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
}

public class AddQuizTextCommandHandler(IMapper mapper,
                                       IQuizDal quizDal,
                                       IQuestionService questionService) : IRequestHandler<AddQuizTextCommand, GetQuizModel>
{
    public async Task<GetQuizModel> Handle(AddQuizTextCommand request, CancellationToken cancellationToken)
    {
        var quizId = await questionService.AddQuizText(request.Model, cancellationToken);

        var result = await quizDal.GetAsyncAutoMapper<GetQuizModel>(
            enableTracking: false,
            predicate: x => x.Id == quizId,
            include: x => x.Include(u => u.QuizQuestions),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AddQuizTextCommandValidator : AbstractValidator<AddQuizTextCommand>
{
    public AddQuizTextCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.UserId).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);

        RuleFor(x => x.Model.Base64List).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Question]);
    }
}