using DrifterApps.Seeds.Testing.Infrastructure;

namespace DrifterApps.Seeds.Testing.Drivers;

/// <summary>
///     Represents a class which is used as a HttpClient driver.
/// </summary>
public interface IHttpClientDriver
{
    HttpResponseMessage? ResponseMessage { get; }

    Task<T?> DeserializeContentAsync<T>();

    void AuthenticateUser(string userId);

    void UnAuthenticate();

    Task SendRequestAsync(ApiResource apiResource, params object[] parameters);

    Task SendRequestWithBodyAsync(ApiResource apiResource, string body, params object[] parameters);
}
