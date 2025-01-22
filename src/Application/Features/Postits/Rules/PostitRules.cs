using Application.Features.Postits.Models;

namespace Application.Features.Postits.Rules;

public class PostitRules(IPostitDal postitDal) : IBusinessRule
{
    internal static Task PostitShouldExists(object postit)
    {
        if (postit == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Postit);
        return Task.CompletedTask;
    }

    internal static Task PostitShouldExistsAndActive(GetPostitModel postitModel)
    {
        PostitShouldExists(postitModel);
        if (!postitModel.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Postit);
        return Task.CompletedTask;
    }

    internal static Task PostitShouldExistsAndActive(Postit postit)
    {
        PostitShouldExists(postit);
        if (!postit.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Postit);
        return Task.CompletedTask;
    }

    internal async Task PostitShouldExists(Guid id)
    {
        var exists = await postitDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        await PostitShouldExists(exists);
    }

    internal async Task PostitShouldExistsAndActive(Guid id)
    {
        var postit = await postitDal.GetAsync(predicate: x => x.Id == id, enableTracking: false);
        await PostitShouldExistsAndActive(postit);
    }

    internal static Task OnlyOneShouldBeFilled(string? title, string? description, string? pictureBase64)
    {
        int filledCount = 0;
        if (title.IsNotEmpty()) filledCount++;
        if (description.IsNotEmpty()) filledCount++;
        if (pictureBase64.IsNotEmpty()) filledCount++;
        if (filledCount == 0)
        {
            throw new BusinessException(Strings.DynamicNotEmpty, $"{Strings.Title}, {Strings.Description}, {Strings.Picture}");
        }
        return Task.CompletedTask;
    }
}