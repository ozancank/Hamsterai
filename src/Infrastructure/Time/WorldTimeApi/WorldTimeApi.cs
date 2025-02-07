using Domain.Constants;
using Newtonsoft.Json;
using OCK.Core.Exceptions.CustomExceptions;

namespace Infrastructure.Time.WorldTimeApi;

public sealed record WorldTimeApiModel(DateTime DateTime);

public sealed class WorldTimeApi(HttpClient httpClient) : ITimeApi
{
    public async Task<DateTime> GetTime()
    {
        var apiUrl = "http://worldtimeapi.org/api/ip";
        var response = await httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<WorldTimeApiModel>(content) ?? throw new ExternalApiException(Strings.DynamicNotNull, nameof(content));
        return result.DateTime;
    }
}