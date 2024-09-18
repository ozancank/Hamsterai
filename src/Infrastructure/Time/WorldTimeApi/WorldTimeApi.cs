using Newtonsoft.Json;

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
        var result = JsonConvert.DeserializeObject<WorldTimeApiModel>(content);
        return result.DateTime;
    }
}