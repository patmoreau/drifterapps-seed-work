﻿using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Application.Extensions;

internal static class HttpContextExtensions
{
    public static ValueTask<TRequest?> ToQueryRequest<TRequest>(this HttpContext context,
        Func<int, int, string[], string[], TRequest?> createInstance) where TRequest : IRequestQuery
    {
        const string offsetKey = "offset";
        const string limitKey = "limit";
        const string sortKey = "sort";
        const string filterKey = "filter";

        var hasOffset = int.TryParse(context.Request.Query[offsetKey], out var offset);
        var hasLimit = int.TryParse(context.Request.Query[limitKey], out var limit);
        var sort = context.Request.Query[sortKey];
        var filter = context.Request.Query[filterKey];

        var result = createInstance(hasOffset ? offset : QueryParams.DefaultOffset,
            hasLimit ? limit : QueryParams.DefaultLimit, sort!, filter!);

        return ValueTask.FromResult(result);
    }
}
