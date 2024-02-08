using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Application.Extensions;

/// <summary>
///     Factory delegate to create a <see cref="IRequestQuery" />
/// </summary>
/// <typeparam name="TRequest">Request type implementing <see cref="IRequestQuery" /></typeparam>
public delegate TRequest? RequestQueryFactory<out TRequest>(int arg1, int arg2, string[] arg3, string[] arg4)
    where TRequest : IRequestQuery;

/// <summary>
///     HttpContext extension methods
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    ///     Method to decode the request query and returns a <see cref="IRequestQuery" />
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestQueryFactory">Factory delegate to create a <see cref="IRequestQuery" /></param>
    /// <typeparam name="TRequest">Request type implementing <see cref="IRequestQuery" /></typeparam>
    /// <returns></returns>
    public static ValueTask<TRequest?> ToQueryRequest<TRequest>(this HttpContext context,
        RequestQueryFactory<TRequest> requestQueryFactory) where TRequest : IRequestQuery
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requestQueryFactory);

        const string offsetKey = nameof(IRequestQuery.Offset);
        const string limitKey = nameof(IRequestQuery.Limit);
        const string sortKey = nameof(IRequestQuery.Sort);
        const string filterKey = nameof(IRequestQuery.Filter);

        var hasOffset = int.TryParse(context.Request.Query[offsetKey], out var offset);
        var hasLimit = int.TryParse(context.Request.Query[limitKey], out var limit);
        var sort = context.Request.Query[sortKey];
        var filter = context.Request.Query[filterKey];

        var result = requestQueryFactory(
            hasOffset ? offset : QueryParams.DefaultOffset,
            hasLimit ? limit : QueryParams.DefaultLimit,
            sort!,
            filter!);

        return ValueTask.FromResult(result);
    }
}
