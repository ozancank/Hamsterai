namespace Domain.Constants;

public enum QuestionType : byte
{
    None = 0,
    Question = 1,
    FindMistake = 2,
    MakeDescription = 3,
    MakeSummary = 4,
    MakeDescriptionWithText = 5,
    MakeSummaryWithText = 6,
}