using System.Text;
using DrifterApps.Seeds.Testing.Infrastructure;
using static System.Globalization.CultureInfo;

namespace DrifterApps.Seeds.Testing.Scenarios;

public abstract partial class Scenario
{
    /// <summary>
    /// </summary>
    /// <param name="apiResource"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="sorts"></param>
    /// <param name="filters"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task QueryEndpoint(ApiResource apiResource, int? offset = null, int? limit = null,
        string? sorts = null, string? filters = null, params object[] parameters)
    {
        StringBuilder sb = new("?");
        if (offset is not null)
        {
            sb.Append(InvariantCulture, $"offset={offset}&");
        }

        if (limit is not null)
        {
            sb.Append(InvariantCulture, $"limit={limit}&");
        }

        if (!string.IsNullOrWhiteSpace(sorts))
        {
            foreach (var sort in sorts.Split(';'))
            {
                sb.Append(InvariantCulture, $"sort={sort}&");
            }
        }

        if (!string.IsNullOrWhiteSpace(filters))
        {
            foreach (var filter in filters.Split(';'))
            {
                sb.Append(InvariantCulture, $"filter={filter}&");
            }
        }

        var args = parameters.Concat([sb.Length == 1 ? string.Empty : sb.Remove(sb.Length - 1, 1).ToString()])
            .ToArray();
        return HttpClientDriver.SendRequestAsync(apiResource, args);
    }
}
