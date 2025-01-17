using Application.Features.Books.Models.Publisher;
using Application.Features.Books.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Books.Queries.Publishers;

public class GetPublisherByIdQuery : IRequest<GetPublisherModel?>, ISecuredRequest<UserTypes>
{
    public int Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
}

public class GetPublisherByIdQueryHandler(IMapper mapper,
                                          IPublisherDal publisherDal) : IRequestHandler<GetPublisherByIdQuery, GetPublisherModel?>
{
    public async Task<GetPublisherModel?> Handle(GetPublisherByIdQuery request, CancellationToken cancellationToken)
    {
        var publisher = await publisherDal.GetAsyncAutoMapper<GetPublisherModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.Books),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await PublisherRules.PublisherShouldExists(publisher);

        return publisher;
    }
}