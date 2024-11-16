namespace Application.Features.Questions.Models;

public class AddManuelModel : IRequestModel
{
    public QuestionType QuestionType { get; set; }
    public long UserId { get; set; }
    public short LessonId { get; set; }
    public List<AddManuelQuestionModel> Questions { get; set; } = [];
    public bool ExcludeQuiz { get; set; }
    public bool ExistsVisualContent { get; set; }
}

public class AddManuelQuestionModel : IRequestModel
{
    public string? QuestionText { get; set; }
    public string? AnswerText { get; set; }
    public int? GainId { get; set; }
    public char? RightOption { get; set; }
    public byte OptionCount { get; set; }
}