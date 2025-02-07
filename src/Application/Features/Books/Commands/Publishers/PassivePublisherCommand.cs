using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Publishers;

public class PassivePublisherCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required short PublisherId { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassivePublisherCommandHandler(IPublisherDal publisherDal,
                                            ICommonService commonService) : IRequestHandler<PassivePublisherCommand, bool>
{
    public async Task<bool> Handle(PassivePublisherCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;

        var publisher = await publisherDal.GetAsync(predicate: x => x.Id == request.PublisherId, cancellationToken: cancellationToken);
        await PublisherRules.PublisherShouldExists(publisher);

        publisher.UpdateUser = commonService.HttpUserId;
        publisher.UpdateDate = date;
        publisher.IsActive = true;

        await publisherDal.UpdateAsync(publisher, cancellationToken: cancellationToken);

        return true;
    }
}