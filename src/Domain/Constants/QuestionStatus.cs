namespace Domain.Constants;

public enum QuestionStatus : byte
{
    Unsend = 0,
    Waiting = 1,
    Answered = 2,
    Error = 3,
}