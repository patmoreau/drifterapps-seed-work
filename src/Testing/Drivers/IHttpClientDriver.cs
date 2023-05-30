using System.Net;

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
}
