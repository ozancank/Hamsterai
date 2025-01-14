namespace Domain.Constants;

public class AppOptions
{
    public static string ProfilePicturePath { get; set; } = string.Empty;
    public static string QuestionPicturePath { get; set; } = string.Empty;
    public static string QuestionSmallPicturePath { get; set; } = string.Empty;
    public static string QuestionThumbnailPath { get; set; } = string.Empty;
    public static string AnswerPicturePath { get; set; } = string.Empty;
    public static string SimilarQuestionPicturePath { get; set; } = string.Empty;
    public static string SimilarAnswerPicturePath { get; set; } = string.Empty;
    public static string QuizQuestionPicturePath { get; set; } = string.Empty;
    public static string QuizAnswerPicturePath { get; set; } = string.Empty;
    public static string PackagePicturePath { get; set; } = string.Empty;
    public static string HomeworkPath { get; set; } = string.Empty;
    public static string HomeworkAnswerPath { get; set; } = string.Empty;
    public static string? ForgetPasswordUrl { get; set; } = string.Empty;
    public static int QuestionMonthLimitForStudent { get; set; } = 300;
    public static int SimilarMonthLimitForStudent { get; set; } = 300;
    public static int QuizMinimumQuestionLimit { get; set; } = 10;
    public static string DefaultPassword { get; set; } = string.Empty;
    public static DateTime ChangeDate { get; set; }
    public static int AITryCount { get; set; } = 10;
    public static int AITimeoutSecond { get; set; } = 60;
    public static int AIQuestionSendSecond { get; set; } = 40;
    public static int AIQuestionSenderCapacity { get; set; } = 10;
    public static int AIQuestionQueueCapacity { get; set; } = 30;
    public static int AISimilarSendSecond { get; set; } = 15;
    public static int AISimilarSenderCapacity { get; set; } = 10;
    public static int AISimilarQueueCapacity { get; set; } = 30;
    public static int AIGainSendSecond { get; set; } = 20;
    public static int AIGainSenderCapacity { get; set; } = 1;
    public static int AIGainQueueCapacity { get; set; } = 1;
    public static int QuestionMaxLimit { get; set; } = 30;
    public static int QuizMaxLimit { get; set; } = 30;

    public static string[] AIDefaultUrls { get; set; } = [];

    public static string ProfilePictureFolderPath { get; set; } = string.Empty;
    public static string QuestionPictureFolderPath { get; set; } = string.Empty;
    public static string QuestionSmallPictureFolderPath { get; set; } = string.Empty;
    public static string QuestionThumbnailFolderPath { get; set; } = string.Empty;
    public static string AnswerPictureFolderPath { get; set; } = string.Empty;
    public static string SimilarQuestionPictureFolderPath { get; set; } = string.Empty;
    public static string SimilarAnswerPictureFolderPath { get; set; } = string.Empty;
    public static string QuizQuestionPictureFolderPath { get; set; } = string.Empty;
    public static string QuizAnswerPictureFolderPath { get; set; } = string.Empty;
    public static string PackagePictureFolderPath { get; set; } = string.Empty;
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

        QuestionSmallPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuestionSmallPicturePath);
        if (!Directory.Exists(QuestionSmallPictureFolderPath)) Directory.CreateDirectory(QuestionSmallPictureFolderPath);

        QuestionThumbnailFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuestionThumbnailPath);
        if (!Directory.Exists(QuestionThumbnailFolderPath)) Directory.CreateDirectory(QuestionThumbnailFolderPath);

        SimilarQuestionPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SimilarQuestionPicturePath);
        if (!Directory.Exists(SimilarQuestionPictureFolderPath)) Directory.CreateDirectory(SimilarQuestionPictureFolderPath);

        SimilarAnswerPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SimilarAnswerPicturePath);
        if (!Directory.Exists(SimilarAnswerPictureFolderPath)) Directory.CreateDirectory(SimilarAnswerPictureFolderPath);

        QuizQuestionPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuizQuestionPicturePath);
        if (!Directory.Exists(QuizQuestionPictureFolderPath)) Directory.CreateDirectory(QuizQuestionPictureFolderPath);

        QuizAnswerPictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, QuizAnswerPicturePath);
        if (!Directory.Exists(QuizAnswerPictureFolderPath)) Directory.CreateDirectory(QuizAnswerPictureFolderPath);

        PackagePictureFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PackagePicturePath);
        if (!Directory.Exists(PackagePictureFolderPath)) Directory.CreateDirectory(PackagePictureFolderPath);

        HomeworkFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HomeworkPath);
        if (!Directory.Exists(HomeworkFolderPath)) Directory.CreateDirectory(HomeworkFolderPath);

        HomeworkAnswerFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HomeworkAnswerPath);
        if (!Directory.Exists(HomeworkAnswerFolderPath)) Directory.CreateDirectory(HomeworkAnswerFolderPath);
    }
}