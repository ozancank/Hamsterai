using OCK.Core.Extensions;
using OCK.Core.Utilities;
using System.Diagnostics;

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
    public static string BookPath { get; set; } = string.Empty;
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
    public static string[] AIDefaultUrls { get; set; } = [];
    public static int QuestionMaxLimit { get; set; } = 30;
    public static int QuizMaxLimit { get; set; } = 30;

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
    public static string BookFolderPath { get; set; } = string.Empty;

    public static void CreateFolder()
    {
        var properties = typeof(AppOptions).GetProperties()
            .Where(p => p.Name.EndsWith("FolderPath") && p.PropertyType == typeof(string));

        foreach (var property in properties)
        {
            var pathPropertyName = property.Name.Replace("FolderPath", "Path");
            var pathProperty = typeof(AppOptions).GetProperty(pathPropertyName);

            if (pathProperty != null)
            {
                var relativePath = pathProperty.GetValue(null)?.ToString() ?? string.Empty;
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                property.SetValue(null, fullPath);

                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

                if (Environment.OSVersion.IsUnix())
                {
                    UnixHelper.ExecuteBashCommand($"sudo chown -R root:root {fullPath}");
                    UnixHelper.ExecuteBashCommand($"sudo chmod -R 755 {fullPath}");
                }
            }
        }

    }
}