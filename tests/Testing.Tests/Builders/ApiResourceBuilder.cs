using Bogus;
using DrifterApps.Seeds.Testing.Infrastructure;

namespace DrifterApps.Seeds.Testing.Tests.Builders;

internal class ApiResourceBuilder : FakerBuilder<ApiResource>
{
    private string? _endpointTemplate;
    private HttpMethod? _httpMethod;

    protected override Faker<ApiResource> ConfigureRules(Faker<ApiResource> fakerBuilder) =>
        fakerBuilder.CustomInstantiator(f =>
            ApiResource.DefineApi(
                _endpointTemplate ?? f.Internet.UrlRootedPath(),
                _httpMethod ?? f.PickRandom(new List<HttpMethod>
                    {HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch, HttpMethod.Delete})
            ));

    public ApiResourceBuilder WithHttpMethod(HttpMethod httpMethod)
    {
        _httpMethod = httpMethod;

        return this;
    }

    public ApiResourceBuilder WithEndpointTemplate(string endpointTemplate)
    {
        _endpointTemplate = endpointTemplate;

        return this;
    }
}
