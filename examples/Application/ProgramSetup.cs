using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Authorization;
using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Application.Orders;
using SampleApp.Domain;

var builder = WebApplication.CreateBuilder(args);

// JSON: serialize strongly-typed IDs as GUID strings instead of {"value":"..."}
builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory()));

// User context from JWT claims
builder.Services.AddUserContext();

// MediatR — seeds behaviors first, then your own handlers
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromApplicationSeeds();
    config.RegisterServicesFromAssemblyContaining<Program>();
});

// Authorization — compose existing policies with AND/OR
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageOrders", policy =>
        policy.AddRequirements(MultiplePoliciesRequirement.ForAnyOf("Admin", "OrderManager")));
});
builder.Services.AddSingleton<IAuthorizationHandler, MultiplePoliciesHandler>();

// Hangfire background jobs
builder.Services.AddHangfireRequestScheduler();

var app = builder.Build();

// GET /orders?offset=0&limit=20&sort=-createdAt&filter=total:gt:100
app.MapGet("/orders", async (HttpContext ctx, IMediator mediator, CancellationToken ct) =>
{
    var request = await ctx.ToQueryRequest<GetOrdersQuery>(
        (offset, limit, sort, filter) => new GetOrdersQuery(offset, limit, sort, filter));

    if (request is null) return Results.BadRequest();

    var result = await mediator.Send(request, ct);
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error.ToProblemDetails();
});

// POST /orders — ValidationFilter handles 422 before the handler runs
app.MapPost("/orders", async (CreateOrderCommand command, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(command, ct);
    return result.IsSuccess
        ? Results.Created($"/orders/{result.Value}", result.Value)
        : result.Error.ToProblemDetails();
})
.AddEndpointFilter<ValidationFilter<CreateOrderCommand>>()
.RequireAuthorization("CanManageOrders");

app.Run();
