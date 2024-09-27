using Bogus;
using DrifterApps.Seeds.Testing.Infrastructure;

namespace DrifterApps.Seeds.Testing.Tests.Builders;

internal class ApiResourceBuilder : FakerBuilder<ApiResource>
{
    protected override Faker<ApiResource> Faker { get; } = CreatePrivateFaker()
        .RuleFor(x => x.HttpMethod, f => f.PickRandom(new List<HttpMethod>
            {HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch, HttpMethod.Delete}))
        .RuleFor(x => x.EndpointTemplate, f => f.Internet.UrlRootedPath());

    public ApiResourceBuilder WithHttpMethod(HttpMethod httpMethod)
    {
        Faker.RuleFor(x => x.HttpMethod, httpMethod);

        return this;
    }

    public ApiResourceBuilder WithEndpointTemplate(string endpointTemplate)
    {
        Faker.RuleFor(x => x.EndpointTemplate, endpointTemplate);

        return this;
    }
}
