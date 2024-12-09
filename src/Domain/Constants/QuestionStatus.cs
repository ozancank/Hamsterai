namespace Domain.Constants;

public enum QuestionStatus : byte
{
    None = 0,
    Waiting = 1,
    Answered = 2,
    Error = 3,
    SendAgain = 4,
    OcrError = 5,
    ConnectionError = 6,
    Timeout = 7,
    NotFoundImage = 8,
}