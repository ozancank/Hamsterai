using Application.Features.Books.Dto;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Rules;
using Application.Services.CommonService;
using FluentValidation.Validators;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using System.Text.Json;

namespace Application.Features.Books.Commands.BookQuizzes;

public class UpdateBookQuizUserCommand : IRequest<GetBookQuizUserModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateBookQuizUserModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Student];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateBookQuizUserCommandHandler(IMapper mapper,
                                              ICommonService commonService,
                                              IBookQuizDal bookQuizDal,
                                              IBookQuizUserDal bookQuizUserDal,
                                              BookRules bookRules) : IRequestHandler<UpdateBookQuizUserCommand, GetBookQuizUserModel>
{
    public async Task<GetBookQuizUserModel> Handle(UpdateBookQuizUserCommand request, CancellationToken cancellationToken)
    {
        await BookQuizRules.BookQuizUserShouldBePausedOrCompleted(request.Model.Status);

        var bookQuiz = await bookQuizDal.GetAsync(x => x.Id == request.Model.BookQuizId, cancellationToken: cancellationToken);

        await BookQuizRules.BookQuizShouldExistsAndActive(bookQuiz);

        await bookRules.CanAccessBook(bookQuiz.BookId);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;

        var bookQuizUser = await bookQuizUserDal.GetAsync(
            predicate: x => x.IsActive
                            && x.BookQuizId == bookQuiz.Id
                            && x.UserId == userId,
            cancellationToken: cancellationToken);
        var firstRecord = bookQuizUser == null;

        var rightAnswers = JsonSerializer.Deserialize<OptionDto[]>(bookQuiz.Answers ?? "[]") ?? [];

        if (bookQuizUser == null)
        {
            bookQuizUser = new BookQuizUser
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                CreateDate = date,
                CreateUser = commonService.HttpUserId,
                UpdateDate = date,
                UpdateUser = commonService.HttpUserId,
                BookQuizId = bookQuiz.Id,
                UserId = commonService.HttpUserId,
                Status = QuizStatus.Started,
                CorrectCount = 0,
                WrongCount = 0,
                EmptyCount = 0,
                SuccessRate = 0
            };

            List<OptionDto> options = [];
            for (byte i = 1; i <= bookQuiz.QuestionCount; i++)
                options.Add(new OptionDto(i, null));

            bookQuizUser.Answers = JsonSerializer.Serialize(options);
        }

        await BookQuizRules.BookQuizShouldBeNotCompleted(bookQuizUser);

        var answers = JsonSerializer.Deserialize<OptionDto[]>(bookQuizUser.Answers ?? "[]") ?? [];

        foreach (var userAnswer in request.Model.UserAnswers)
        {
            var answer = answers.First(x => x.QuestionNumber == userAnswer.QuestionNumber);
            answer.Option = userAnswer.Option;
        }

        bookQuizUser.Answers = JsonSerializer.Serialize(answers);

        if (request.Model.Status == QuizStatus.Completed)
        {
            bookQuizUser.CorrectCount = bookQuizUser.WrongCount = bookQuizUser.EmptyCount = 0;
            foreach (var answer in answers)
            {
                var rightAnswer = rightAnswers.First(x => x.QuestionNumber == answer.QuestionNumber);
                if (answer.Option == rightAnswer.Option)
                    bookQuizUser.CorrectCount++;
                else if (answer.Option.HasValue && answer.Option != rightAnswer.Option)
                    bookQuizUser.WrongCount++;
                else if (!answer.Option.HasValue)
                    bookQuizUser.EmptyCount++;
            }

            bookQuizUser.SuccessRate = Math.Round(bookQuizUser.CorrectCount * 100.0 / bookQuiz.QuestionCount, 2);
        }

        bookQuizUser.Status = request.Model.Status;

        if (firstRecord)
            await bookQuizUserDal.AddAsync(bookQuizUser, cancellationToken: cancellationToken);
        else
            await bookQuizUserDal.UpdateAsync(bookQuizUser, cancellationToken: cancellationToken);

        var result = mapper.Map<GetBookQuizUserModel>(bookQuizUser);
        result.UserAnswers = JsonSerializer.Deserialize<OptionDto[]>(bookQuizUser.Answers ?? "[]") ?? [];

        return result;
    }
}

public class UpdateBookQuizUserCommandValidator : AbstractValidator<UpdateBookQuizUserCommand>
{
    public UpdateBookQuizUserCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.BookQuizId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Book} {Strings.OfQuiz}"]);

        RuleFor(x => x.Model.UserAnswers).SetValidator(new OptionDtoValidator());

        //RuleFor(x=>x.Model.Status)..WithMessage(Strings.DynamicNotEmpty, [Strings.Status]);
    }

    internal class OptionDtoValidator : IPropertyValidator<UpdateBookQuizUserCommand, OptionDto[]>
    {
        private static readonly char?[] ValidOptions = [null, 'A', 'B', 'C', 'D', 'E'];

        public bool IsValid(ValidationContext<UpdateBookQuizUserCommand> context, OptionDto[] value)
        {
            if (value == null) return context.AddErrorMessage(Strings.DynamicNotNull.Format(Strings.RightOption));

            foreach (var option in value)
            {
                if (option.Option != null && !ValidOptions.Contains(option.Option.Value)) return context.AddErrorMessage(Strings.DynamicBetween.Format($"{Strings.RightOption}", "A", "E"));
                if (value.Count(x => x.QuestionNumber == option.QuestionNumber) != 1) return context.AddErrorMessage(Strings.DynamicUnique.Format($"{Strings.Question} {Strings.Number}"));
            }

            return true;
        }

        public string GetDefaultMessageTemplate(string errorCode)
        {
            return $"Validation failed: {errorCode}";
        }

        public string Name => nameof(OptionDtoValidator);
    }
}