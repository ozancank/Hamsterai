namespace Domain.Constants;

public class AppOptions
{
    public static string ProfilePicturePath { get; set; }
    public static string QuestionPicturePath { get; set; }
    public static string AnswerPicturePath { get; set; }
    public static string SimilarQuestionPicturePath { get; set; }
    public static string SimilarAnswerPicturePath { get; set; }
    public static string QuizQuestionPicturePath { get; set; }
    public static string QuizAnswerPicturePath { get; set; }
    public static string HomeworkPath { get; set; }
    public static string HomeworkAnswerPath { get; set; }
    public static int AITryCount { get; set; }
    public static int AISendSecond { get; set; }
    public static string AI_Default { get; set; }
    public static string AI_Kazim1 { get; set; }
    public static string AI_Kazim2 { get; set; }
    public static string AI_Kazim3 { get; set; }
    public static string ForgetPasswordUrl { get; set; }
    public static int QuestionLimitForStudent { get; set; }
    public static int SimilarLimitForStudent { get; set; }
    public static int QuizMinimumQuestionLimit { get; set; }
    public static string DefaultPassword { get; set; }
    public static int SenderCapacity { get; set; }
    public static string OCR_Url { get; set; }

    public static string ProfilePictureFolderPath { get; set; }
    public static string QuestionPictureFolderPath { get; set; }
    public static string AnswerPictureFolderPath { get; set; }
    public static string SimilarQuestionPictureFolderPath { get; set; }
    public static string SimilarAnswerPictureFolderPath { get; set; }
    public static string QuizQuestionPictureFolderPath { get; set; }
    public static string QuizAnswerPictureFolderPath { get; set; }
    public static string HomeworkFolderPath { get; set; }
    public static string HomeworkAnswerFolderPath { get; set; }

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