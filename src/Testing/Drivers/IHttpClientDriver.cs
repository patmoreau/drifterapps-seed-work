using System.Net;
using DrifterApps.Seeds.Testing.Infrastructure;

namespace DrifterApps.Seeds.Testing.Drivers;

/// <summary>
///     Represents a class which is used as a HttpClient driver.
/// </summary>
public interface IHttpClientDriver
{
    HttpStatusCode ResponseStatusCode { get; }

    string? ResponseContent { get; }

    Uri? ResponseLocation { get; }

    T? DeserializeContent<T>();

    void AuthenticateUser(string userId);

    void UnAuthenticate();

    Task SendRequestAsync(ApiResource apiResource, params object[] parameters);

    Task SendRequestWithBodyAsync(ApiResource apiResource, string body, params object[] parameters);
}
