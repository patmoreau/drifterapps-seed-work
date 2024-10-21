using DrifterApps.Seeds.FluentResult;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Application.Extensions;

public static class ResultErrorExtensions
{
    public static IResult ToProblemDetails(this ResultError error, int statusCode)
    {
        ArgumentNullException.ThrowIfNull(error);
        return Results.Problem(statusCode: statusCode, detail: error.Description);
    }

    public static IResult ToValidationProblemDetails(this ResultValidationError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return Results.ValidationProblem(error.ValidationErrors.ToDictionary(entry => entry.Key, entry => entry.Value),
            error.Description);
    }
}
