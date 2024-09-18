namespace Business.Features.Questions.Models.Questions;

public class GetQuestionModel : IResponseModel
{
    public Guid Id { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public byte LessonId { get; set; }
    public string QuestionPictureFileName { get; set; }
    public string QuestionPictureExtension { get; set; }
    public string AnswerText { get; set; }
    public QuestionStatus Status { get; set; }

    public string LessonName { get; set; }
}