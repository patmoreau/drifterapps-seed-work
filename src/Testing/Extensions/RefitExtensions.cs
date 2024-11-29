using DrifterApps.Seeds.FluentScenario;
using Refit;

namespace DrifterApps.Seeds.Testing.Extensions;

/// <summary>
///     Provides extension methods for persisting and retrieving API responses in the context.
/// </summary>
public static class RefitExtensions
{
    private const string DefaultApiCall = "latest";
    private const string ContextApiResponse = $"{nameof(Refit)}-api-response";

    /// <summary>
    ///     Persists the API response in the context with the specified API call key.
    /// </summary>
    /// <typeparam name="TApiResponse">The type of the API response.</typeparam>
    /// <param name="response">The API response to persist.</param>
    /// <param name="context">The context in which to persist the response.</param>
    /// <param name="apiCall">The key for the API call. Defaults to "latest".</param>
    /// <returns>The persisted API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the response or context is null.</exception>
    public static TApiResponse Persist<TApiResponse>(this TApiResponse response, IRunnerContext context,
        string apiCall = DefaultApiCall)
        where TApiResponse : IApiResponse
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextData(BuildContextKey(apiCall), response);

        return response;
    }

    /// <summary>
    ///     Asynchronously persists the API response in the context with the specified API call key.
    /// </summary>
    /// <typeparam name="TApiResponse">The type of the API response.</typeparam>
    /// <param name="taskResponse">The task representing the API response to persist.</param>
    /// <param name="context">The context in which to persist the response.</param>
    /// <param name="apiCall">The key for the API call. Defaults to "latest".</param>
    /// <returns>A task representing the persisted API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the taskResponse or context is null.</exception>
    public static async Task<TApiResponse> Persist<TApiResponse>(this Task<TApiResponse> taskResponse,
        IRunnerContext context, string apiCall = DefaultApiCall)
        where TApiResponse : IApiResponse
    {
        ArgumentNullException.ThrowIfNull(taskResponse);
        ArgumentNullException.ThrowIfNull(context);

        var response = await taskResponse.ConfigureAwait(false);

        return response.Persist(context, apiCall);
    }

    /// <summary>
    ///     Retrieves the persisted API response from the context with the specified API call key.
    /// </summary>
    /// <typeparam name="TApiResponse">The type of the API response.</typeparam>
    /// <param name="context">The context from which to retrieve the response.</param>
    /// <param name="apiCall">The key for the API call. Defaults to "latest".</param>
    /// <returns>The retrieved API response.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the context is null.</exception>
    public static TApiResponse ApiResponse<TApiResponse>(this IRunnerContext context, string apiCall = DefaultApiCall)
        where TApiResponse : IApiResponse
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.GetContextData<TApiResponse>(BuildContextKey(apiCall));
    }

    /// <summary>
    ///     Builds the context key for the specified API call.
    /// </summary>
    /// <param name="apiCall">The key for the API call.</param>
    /// <returns>The built context key.</returns>
    private static string BuildContextKey(string apiCall) => $"{ContextApiResponse}-{apiCall}";
}
