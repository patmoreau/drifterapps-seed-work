using System.Text;
using DrifterApps.Seeds.Testing.Infrastructure;
using static System.Globalization.CultureInfo;

namespace DrifterApps.Seeds.Testing.Scenarios;

public abstract partial class Scenario
{
    /// <summary>
    /// </summary>
    /// <param name="apiResources"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="sorts"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public Task QueryEndpoint(ApiResource apiResources, int? offset = null, int? limit = null,
        string? sorts = null, string? filters = null)
    {
        StringBuilder sb = new();
        if (offset is not null) sb.Append(InvariantCulture, $"offset={offset}&");

        if (limit is not null) sb.Append(InvariantCulture, $"limit={limit}&");

        if (!string.IsNullOrWhiteSpace(sorts))
            foreach (var sort in sorts.Split(';'))
                sb.Append(InvariantCulture, $"sort={sort}&");

        if (!string.IsNullOrWhiteSpace(filters))
            foreach (var filter in filters.Split(';'))
                sb.Append(InvariantCulture, $"filter={filter}&");

        return HttpClientDriver.SendGetRequestAsync(apiResources,
            sb.Length == 0 ? null : sb.Remove(sb.Length - 1, 1).ToString());
    }
}
