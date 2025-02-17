using Application.Features.Books.Dto;
using Application.Features.Books.Models.BookQuizzes;
using Application.Features.Books.Rules;
using Application.Features.Lessons.Rules;
using Application.Services.CommonService;
using FluentValidation.Validators;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using System.Text.Json;

namespace Application.Features.Books.Commands.BookQuizzes;

public class UpdateBookQuizCommand : IRequest<GetBookQuizModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateBookQuizModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateBookQuizCommandHandler(IMapper mapper,
                                                   ICommonService commonService,
                                                   IBookQuizDal bookQuizDal,
                                                   IBookQuizUserDal bookQuizUserDal,
                                                   BookRules bookRules,
                                                   LessonRules lessonRules,
                                                   BookQuizRules bookQuizRules) : IRequestHandler<UpdateBookQuizCommand, GetBookQuizModel>
{
    public async Task<GetBookQuizModel> Handle(UpdateBookQuizCommand request, CancellationToken cancellationToken)
    {
        var bookQuiz = await bookQuizDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);
        await BookQuizRules.BookQuizShouldExistsAndActive(bookQuiz);

        await bookRules.CanAccessBook(request.Model.BookId);
        await lessonRules.LessonShouldExistsAndActive(request.Model.LessonId);
        await bookQuizRules.QuizNameCanNotBeDuplicated(request.Model.BookId, request.Model.LessonId, request.Model.Name!, request.Model.Id);

        var date = DateTime.Now;

        mapper.Map(request.Model, bookQuiz);
        bookQuiz.UpdateDate = date;
        bookQuiz.UpdateUser = commonService.HttpUserId;
        bookQuiz.Answers = JsonSerializer.Serialize(request.Model.RightAnswers);

        var deleteList = await bookQuizUserDal.GetListAsync(predicate: x => x.BookQuizId == bookQuiz.Id, cancellationToken: cancellationToken);

        await bookQuizDal.ExecuteWithTransactionAsync(async () =>
        {
            await bookQuizDal.UpdateAsync(bookQuiz, cancellationToken: cancellationToken);
            await bookQuizUserDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        var updated = await bookQuizDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == bookQuiz.Id,
            include: x => x.Include(u => u.Book).Include(u => u.Lesson),
            cancellationToken: cancellationToken);

        var result = mapper.Map<GetBookQuizModel>(updated);
        result.RightAnswers = JsonSerializer.Deserialize<OptionDto[]>(updated.Answers ?? "[]") ?? [];

        return result;
    }
}

public class UpdateBookQuizCommandValidator : AbstractValidator<UpdateBookQuizCommand>
{
    public UpdateBookQuizCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.Book} {Strings.OfQuiz}"]);

        RuleFor(x => x.Model.BookId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Book]);

        RuleFor(x => x.Model.LessonId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Lesson]);

        RuleFor(x => x.Model.Name.EmptyOrTrim()).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);

        RuleFor(x => x.Model.QuestionCount).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [$"{Strings.Question} {Strings.OfCount}", "1", "255"]);

        RuleFor(x => x.Model.OptionCount).InclusiveBetween((byte)3, (byte)5).WithMessage(Strings.DynamicBetween, [$"{Strings.Option} {Strings.OfCount}", "3", "5"]);

        RuleFor(x => x.Model.RightAnswers).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.RightOptions]);
        RuleFor(x => x.Model.RightAnswers.Length).Equal(x => x.Model.QuestionCount).WithMessage(Strings.DynamicEqual, [Strings.Question, Strings.RightOption]);

        RuleFor(x => x.Model.RightAnswers).SetValidator(new OptionDtoValidator());
    }

    internal class OptionDtoValidator : IPropertyValidator<UpdateBookQuizCommand, OptionDto[]>
    {
        private static readonly char?[] ValidOptions = [null, 'A', 'B', 'C', 'D', 'E'];

        public bool IsValid(ValidationContext<UpdateBookQuizCommand> context, OptionDto[] value)
        {
            if (value == null) return context.AddErrorMessage(Strings.DynamicNotNull.Format(Strings.RightOption));
            if (value.Length != context.InstanceToValidate.Model.QuestionCount) return context.AddErrorMessage(Strings.DynamicEqual.Format($"{Strings.Question} {Strings.OfCount}", $"{Strings.RightOption} {Strings.OfCount}"));
            foreach (var option in value)
            {
                if (option.Option != null && !ValidOptions.Contains(option.Option.Value)) return context.AddErrorMessage(Strings.DynamicBetween.Format($"{Strings.RightOption}", "A", "E"));
                if (!option!.QuestionNumber.Between((byte)1, context.InstanceToValidate.Model.QuestionCount)) return context.AddErrorMessage(Strings.DynamicBetween.Format($"{Strings.Question} {Strings.Number}", "1", context.InstanceToValidate.Model.QuestionCount));
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