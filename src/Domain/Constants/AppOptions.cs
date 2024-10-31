namespace Domain.Constants;

public class AppOptions
{
    public static string ProfilePicturePath { get; set; } = string.Empty;
    public static string QuestionPicturePath { get; set; } = string.Empty;
    public static string AnswerPicturePath { get; set; } = string.Empty;
    public static string SimilarQuestionPicturePath { get; set; } = string.Empty;
    public static string SimilarAnswerPicturePath { get; set; } = string.Empty;
    public static string QuizQuestionPicturePath { get; set; } = string.Empty;
    public static string QuizAnswerPicturePath { get; set; } = string.Empty;
    public static string HomeworkPath { get; set; } = string.Empty;
    public static string HomeworkAnswerPath { get; set; } = string.Empty;
    public static int AITryCount { get; set; } = 3;
    public static int AISendSecond { get; set; } = 3;
    public static string? AI_Default { get; set; } = string.Empty;
    public static string? AI_Kazim1 { get; set; } = string.Empty;
    public static string? AI_Kazim2 { get; set; } = string.Empty;
    public static string? AI_Kazim3 { get; set; } = string.Empty;
    public static string? ForgetPasswordUrl { get; set; } = string.Empty;
    public static int QuestionLimitForStudent { get; set; } = 20;
    public static int SimilarLimitForStudent { get; set; } = 3;
    public static int QuizMinimumQuestionLimit { get; set; } = 10;
    public static string DefaultPassword { get; set; } = string.Empty;
    public static int SenderCapacity { get; set; } = 3;
    public static string OCR_Url { get; set; } = string.Empty;

    public static string ProfilePictureFolderPath { get; set; } = string.Empty;
    public static string QuestionPictureFolderPath { get; set; } = string.Empty;
    public static string AnswerPictureFolderPath { get; set; } = string.Empty;
    public static string SimilarQuestionPictureFolderPath { get; set; } = string.Empty;
    public static string SimilarAnswerPictureFolderPath { get; set; } = string.Empty;
    public static string QuizQuestionPictureFolderPath { get; set; } = string.Empty;
    public static string QuizAnswerPictureFolderPath { get; set; } = string.Empty;
    public static string HomeworkFolderPath { get; set; } = string.Empty;
    public static string HomeworkAnswerFolderPath { get; set; } = string.Empty;

    public static void CreateFolder()
    {
        ProfilePictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfilePicturePath);
        if (!Directory.Exists(ProfilePictureFolderPath)) Directory.CreateDirectory(ProfilePictureFolderPath);

        AnswerPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AnswerPicturePath);
        if (!Directory.Exists(AnswerPictureFolderPath)) Directory.CreateDirectory(AnswerPictureFolderPath);

        QuestionPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuestionPicturePath);
        if (!Directory.Exists(QuestionPictureFolderPath)) Directory.CreateDirectory(QuestionPictureFolderPath);

        SimilarQuestionPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SimilarQuestionPicturePath);
        if (!Directory.Exists(SimilarQuestionPictureFolderPath)) Directory.CreateDirectory(SimilarQuestionPictureFolderPath);

        SimilarAnswerPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SimilarAnswerPicturePath);
        if (!Directory.Exists(SimilarAnswerPictureFolderPath)) Directory.CreateDirectory(SimilarAnswerPictureFolderPath);

        QuizQuestionPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuizQuestionPicturePath);
        if (!Directory.Exists(QuizQuestionPictureFolderPath)) Directory.CreateDirectory(QuizQuestionPictureFolderPath);

        QuizAnswerPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuizAnswerPicturePath);
        if (!Directory.Exists(QuizAnswerPictureFolderPath)) Directory.CreateDirectory(QuizAnswerPictureFolderPath);

        HomeworkFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HomeworkPath);
        if (!Directory.Exists(HomeworkFolderPath)) Directory.CreateDirectory(HomeworkFolderPath);

        HomeworkAnswerFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HomeworkAnswerPath);
        if (!Directory.Exists(HomeworkAnswerFolderPath)) Directory.CreateDirectory(HomeworkAnswerFolderPath);
    }
}