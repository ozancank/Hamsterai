namespace Business.Features.Questions.Models.Similars;

public class GetSimilarModel : IResponseModel
{
    public Guid Id { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public byte LessonId { get; set; }
    public string QuestionPictureFileName { get; set; }
    public string QuestionPictureExtension { get; set; }
    public string ResponseQuestion { get; set; }
    public string ResponseQuestionFileName { get; set; }
    public string ResponseQuestionExtension { get; set; }
    public string ResponseAnswer { get; set; }
    public string ResponseAnswerFileName { get; set; }
    public string ResponseAnswerExtension { get; set; }
    public QuestionStatus Status { get; set; }

    public string LessonName { get; set; }
}