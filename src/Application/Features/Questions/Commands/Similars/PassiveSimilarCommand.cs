﻿using Application.Features.Questions.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Questions.Commands.Similars;

public class PassiveSimilarCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public Guid QuestionId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassiveSimilarCommandHandler(ISimilarDal similarQuestionDal,
                                          ICommonService commonService) : IRequestHandler<PassiveSimilarCommand, bool>
{
    public async Task<bool> Handle(PassiveSimilarCommand request, CancellationToken cancellationToken)
    {
        var question = await similarQuestionDal.GetAsync(predicate: x => x.Id == request.QuestionId, cancellationToken: cancellationToken);
        await SimilarRules.SimilarQuestionShouldExists(question);

        question.IsActive = false;
        question.UpdateUser = commonService.HttpUserId;
        question.UpdateDate = DateTime.Now;

        await similarQuestionDal.UpdateAsync(question, cancellationToken: cancellationToken);

        return true;
    }
}