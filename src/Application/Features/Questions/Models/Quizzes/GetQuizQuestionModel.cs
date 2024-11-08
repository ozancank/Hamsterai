namespace Application.Features.Questions.Models.Quizzes;

public class GetQuizQuestionModel : IResponseModel
{
    public string? Id { get; set; }
    public short SortNo { get; set; }
    public string? QuizId { get; set; }
    public string? Question { get; set; }
    public string? QuestionPictureFileName { get; set; }
    public string? QuestionPictureExtension { get; set; }
    public string? Answer { get; set; }
    public string? AnswerPictureFileName { get; set; }
    public string? AnswerPictureExtension { get; set; }
    public char RightOption { get; set; }
    public char? AnswerOption { get; set; }
    public byte OptionCount { get; set; }
    public int GainId { get; set; }

    public string? GainName { get; set; }
}