using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Authorization;
using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.Infrastructure;
using FluentValidation;
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

// Handlers and validators
builder.Services.AddScoped<GetOrdersHandler>();
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderValidator>();
builder.Services.AddScoped<IValidator<GetOrdersQuery>, GetOrdersQueryValidator>();

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
app.MapGet("/orders", async (HttpContext ctx, GetOrdersHandler handler, CancellationToken ct) =>
{
    var request = await ctx.ToQueryRequest<GetOrdersQuery>(
        (offset, limit, sort, filter) => new GetOrdersQuery(offset, limit, sort, filter));

    if (request is null) return Results.BadRequest();

    var result = await handler.HandleAsync(request, ct);
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error.ToProblemDetails(StatusCodes.Status400BadRequest);
});

// POST /orders — ValidationFilter answers 400/422 before the handler runs,
// UnitOfWorkFilter wraps it in a transaction
app.MapPost("/orders", async (CreateOrderCommand command, CreateOrderHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(command, ct);
    return result.IsSuccess
        ? Results.Created($"/orders/{result.Value}", result.Value)
        : result.Error.ToProblemDetails(StatusCodes.Status400BadRequest);
})
.AddEndpointFilter<ValidationFilter<CreateOrderCommand>>()
.AddEndpointFilter<UnitOfWorkFilter>()
.RequireAuthorization("CanManageOrders");

app.Run();
