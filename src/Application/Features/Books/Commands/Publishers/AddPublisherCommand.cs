using Application.Features.Books.Models.Publisher;
using Application.Features.Books.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Books.Commands.Publishers;

public class AddPublisherCommand : IRequest<GetPublisherModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddPublisherModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddPublisherCommandHandler(IMapper mapper,
                                        ICommonService commonService,
                                        IPublisherDal publisherDal,
                                        PublisherRules publisherRules) : IRequestHandler<AddPublisherCommand, GetPublisherModel>
{
    public async Task<GetPublisherModel> Handle(AddPublisherCommand request, CancellationToken cancellationToken)
    {
        request.Model.Name = request.Model.Name!.Trim();

        await publisherRules.PublisherNameCanNotBeDuplicated(request.Model.Name);

        var publisher = mapper.Map<Publisher>(request.Model);
        publisher.Id = await publisherDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        publisher.IsActive = true;
        publisher.CreateUser = publisher.UpdateUser = commonService.HttpUserId;
        publisher.CreateDate = publisher.UpdateDate = DateTime.Now;

        var added = await publisherDal.AddAsyncCallback(publisher, cancellationToken: cancellationToken);
        var result = mapper.Map<GetPublisherModel>(added);

        return result;
    }
}

public class AddPublisherCommandValidator : AbstractValidator<AddPublisherCommand>
{
    public AddPublisherCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Name.EmptyOrTrim()).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name.EmptyOrTrim()).MaximumLength(100).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "100"]);
    }
}