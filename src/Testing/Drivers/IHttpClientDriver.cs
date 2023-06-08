using System.Net;
using DrifterApps.Seeds.Testing.Infrastructure;

namespace DrifterApps.Seeds.Testing.Drivers;

public interface IHttpClientDriver
{
    HttpResponseMessage? ResponseMessage { get; }
    void ShouldBeUnauthorized();
    void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus);
    void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate);
    void ShouldNotHaveResponseWithOneOfStatuses(params HttpStatusCode[] httpStatuses);
    T? DeserializeContent<T>();
    void AuthenticateUser(Guid userId);
    void UnAuthenticate();
    public Task SendGetRequest(ApiResource apiResource, string? query = null);
    public Task SendGetRequest(ApiResource apiResource, params object[] parameters);
    public Task SendPostRequest(ApiResource apiResource, string? body = null);
    public Task SendDeleteRequest(ApiResource apiResource, params object[] parameters);
}
