using DrifterApps.Seeds.Testing.Infrastructure;

namespace DrifterApps.Seeds.Testing.Drivers;

/// <summary>
///     Represents a class which is used as a HttpClient driver.
/// </summary>
public interface IHttpClientDriver
{
    HttpResponseMessage? ResponseMessage { get; }
    T? DeserializeContent<T>();
    void AuthenticateUser(string userId);
    void UnAuthenticate();
    public Task SendGetRequestAsync(ApiResource apiResource, string? query = null);
    public Task SendGetRequestAsync(ApiResource apiResource, params object[] parameters);
    public Task SendPostRequestAsync(ApiResource apiResource, string? body = null);
    public Task SendDeleteRequestAsync(ApiResource apiResource, params object[] parameters);
}
