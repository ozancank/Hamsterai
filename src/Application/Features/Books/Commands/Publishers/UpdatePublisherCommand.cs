using Application.Features.Books.Models.Publisher;
using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Publishers;

public class UpdatePublisherCommand : IRequest<GetPublisherModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdatePublisherModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdatePublisherCommandHandler(IMapper mapper,
                                           IPublisherDal publisherDal,
                                           ICommonService commonService,
                                           PublisherRules publisherRules) : IRequestHandler<UpdatePublisherCommand, GetPublisherModel>
{
    public async Task<GetPublisherModel> Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
    {
        request.Model.Name = request.Model.Name!.Trim();
        var publisher = await publisherDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await publisherRules.PublisherNameCanNotBeDuplicated(request.Model.Name, request.Model.Id);

        mapper.Map(request.Model, publisher);
        publisher.UpdateUser = commonService.HttpUserId;
        publisher.UpdateDate = DateTime.Now;

        var updated = await publisherDal.UpdateAsyncCallback(publisher, cancellationToken: cancellationToken);
        var result = mapper.Map<GetPublisherModel>(updated);
        return result;
    }
}

public class UpdatePublisherCommandValidator : AbstractValidator<UpdatePublisherModel>
{
    public UpdatePublisherCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);
    }
}