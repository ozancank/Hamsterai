using DataAccess.EF;

namespace Application.Features.Books.Rules;

public class BookQuizRules(IBookQuizDal bookQuizDal) : IBusinessRule
{


    internal static Task BookQuizShouldExists(object bookQuiz)
    {
        if (bookQuiz == null) throw new BusinessException(Strings.DynamicNotFound, [$"{Strings.Book} {Strings.OfQuiz}"]);
        return Task.CompletedTask;
    }

    internal static async Task BookQuizShouldExistsAndActive(BookQuiz bookQuiz)
    {
        await BookQuizShouldExists(bookQuiz);
        if (!bookQuiz.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, [$"{Strings.Book} {Strings.OfQuiz}"]);
    }

    internal async Task BookQuizShouldExistsAndActive(Guid id)
    {
        var bookQuiz = await bookQuizDal.GetAsync(x => x.Id == id, enableTracking: false);
        await BookQuizShouldExistsAndActive(bookQuiz);
    }

    internal async Task BookQuizShouldExistsAndActiveById(Guid id)
    {
        var bookQuiz = await bookQuizDal.GetAsync(x => x.Id == id, enableTracking: false);
        await BookQuizShouldExistsAndActive(bookQuiz);
    }

    internal async Task QuizNameCanNotBeDuplicated(int bookId, short lessonId, string quizName, Guid? bookQuizId = null)
    {
        var publisher = await bookQuizDal.GetAsync(predicate: x => x.BookId == bookId && x.LessonId == lessonId && PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(quizName));
        if (bookQuizId == null && publisher != null) throw new BusinessException(Strings.DynamicExists, quizName);
        if (bookQuizId != null && publisher != null && publisher.Id != bookQuizId) throw new BusinessException(Strings.DynamicExists, quizName);
    }

    internal static async Task BookQuizShouldBeNotCompleted(BookQuizUser bookQuizUser)
    {
        if (bookQuizUser.Status == QuizStatus.Completed) throw new BusinessException(Strings.DynamicAlreadyCompleted, Strings.Quiz);
        await Task.CompletedTask;
    }

    internal static async Task BookQuizUserShouldBePausedOrCompleted(QuizStatus status)
    {
        if (!status.IsIn(QuizStatus.Paused, QuizStatus.Completed)) throw new BusinessException(Strings.AnswerBePausedOrCompleted);
        await Task.CompletedTask;

    }
}