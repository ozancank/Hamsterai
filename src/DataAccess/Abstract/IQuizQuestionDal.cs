namespace DataAccess.Abstract;

public interface IQuizQuestionDal : ISyncRepository<QuizQuestion>, IAsyncRepository<QuizQuestion>
{
}