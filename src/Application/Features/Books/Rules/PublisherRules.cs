using DataAccess.EF;

namespace Application.Features.Books.Rules;

public class PublisherRules(IPublisherDal publisherDal) : IBusinessRule
{
    internal static Task PublisherShouldExists(object publisher)
    {
        if (publisher == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Publisher);
        return Task.CompletedTask;
    }

    internal async Task PublisherShouldNotExistsById(short id)
    {
        var control = await publisherDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, [Strings.Publisher]);
    }

    internal async Task PublisherShouldExistsAndActive(short id)
    {
        var control = await publisherDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        if (!control) throw new BusinessException(Strings.DynamicNotFoundOrActive, [Strings.Publisher]);
    }

    internal async Task PublisherNameCanNotBeDuplicated(string name, short? publisherId = null)
    {
        var publisher = await publisherDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name));
        if (publisherId == null && publisher != null) throw new BusinessException(Strings.DynamicExists, name);
        if (publisherId != null && publisher != null && publisher.Id != publisherId) throw new BusinessException(Strings.DynamicExists, name);
    }
}