using Domain.Constants;
using OCK.Core.Interfaces;

namespace Infrastructure.AI.Seduss.Dtos;

public record UpdateQuestionDto(Guid QuestionId,
                                QuestionStatus Status,
                                long UserId,
                                string AIIP,
                                string? ErrorMessage = null) : IDto;