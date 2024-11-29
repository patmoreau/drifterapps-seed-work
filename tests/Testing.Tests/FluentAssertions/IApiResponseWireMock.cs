using Refit;

namespace DrifterApps.Seeds.Testing.Tests.FluentAssertions;

internal interface IApiResponseWireMock
{
    internal const string WithCorrelationId = $"/{nameof(WithCorrelationId)}";
    internal const string WithoutCorrelationId = $"/{nameof(WithoutCorrelationId)}";
    internal const string IsSuccessful = $"/{nameof(IsSuccessful)}";
    internal const string IsNotSuccessful = $"/{nameof(IsNotSuccessful)}";
    internal const string IsFailure = $"/{nameof(IsFailure)}";
    internal const string IsNotFailure = $"/{nameof(IsNotFailure)}";
    internal const string IsAuthorized = $"/{nameof(IsAuthorized)}";
    internal const string IsNotAuthorized = $"/{nameof(IsNotAuthorized)}";
    internal const string IsForbidden = $"/{nameof(IsForbidden)}";
    internal const string IsNotForbidden = $"/{nameof(IsNotForbidden)}";
    internal const string IsUnauthorized = $"/{nameof(IsUnauthorized)}";
    internal const string IsNotUnauthorized = $"/{nameof(IsNotUnauthorized)}";
    internal const string IsWithStatusCode = $"/{nameof(IsWithStatusCode)}";
    internal const string IsNotWithStatusCode = $"/{nameof(IsNotWithStatusCode)}";
    internal const string IsWithError = $"/{nameof(IsWithError)}";

    [Get(WithCorrelationId)]
    Task<IApiResponse> GetWithCorrelationIdAsync([Header("X-Correlation-Id")] Guid correlationId);

    [Get(WithoutCorrelationId)]
    Task<IApiResponse> GetWithoutCorrelationIdAsync();

    [Get(IsSuccessful)]
    Task<IApiResponse> GetIsSuccessfulAsync();

    [Get(IsNotSuccessful)]
    Task<IApiResponse> GetIsNotSuccessfulAsync();

    [Get(IsFailure)]
    Task<IApiResponse> GetIsFailureAsync();

    [Get(IsNotFailure)]
    Task<IApiResponse> GetIsNotFailureAsync();

    [Get(IsAuthorized)]
    Task<IApiResponse> GetIsAuthorizedAsync();

    [Get(IsNotAuthorized)]
    Task<IApiResponse> GetIsNotAuthorizedAsync();

    [Get(IsForbidden)]
    Task<IApiResponse> GetIsForbiddenAsync();

    [Get(IsNotForbidden)]
    Task<IApiResponse> GetIsNotForbiddenAsync();

    [Get(IsUnauthorized)]
    Task<IApiResponse> GetIsUnauthorizedAsync();

    [Get(IsNotUnauthorized)]
    Task<IApiResponse> GetIsNotUnauthorizedAsync();

    [Get(IsWithStatusCode)]
    Task<IApiResponse> GetIsWithStatusCodeAsync();

    [Get(IsNotWithStatusCode)]
    Task<IApiResponse> GetIsNotWithStatusCodeAsync();

    [Get(IsWithError)]
    Task<IApiResponse> GetIsWithErrorAsync();
}
