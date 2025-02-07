namespace Application.Features.Books.Dto;

public class OptionDto : IDto
{
    public byte QuestionNumber { get; set; }
    public char? Option { get; set; }

    public OptionDto()
    {
    }

    public OptionDto(byte questionNumber, char? option)
    {
        QuestionNumber = questionNumber;
        Option = option != null ? char.ToUpperInvariant(option.Value) : null;
    }
}