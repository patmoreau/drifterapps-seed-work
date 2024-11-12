using Microsoft.AspNetCore.Mvc;
using Refit;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace DrifterApps.Seeds.Infrastructure;

public static class RefitExtensions
{
    public static Task<ProblemDetails?> ToProblemDetails(this ApiException self)
    {
        ArgumentNullException.ThrowIfNull(self);

        return self.GetContentAsAsync<ProblemDetails>();
    }

    public static Task<ValidationProblemDetails?> ToValidationProblemDetails(this ApiException self)
    {
        ArgumentNullException.ThrowIfNull(self);

        return self.GetContentAsAsync<ValidationProblemDetails>();
    }
}
