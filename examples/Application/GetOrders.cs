using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.FluentResult;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domain;

namespace SampleApp.Application.Orders;

// The query record implements IRequestQuery so ASP.NET Core can bind and validate it.
public record GetOrdersQuery(int Offset, int Limit, string[] Sort, string[] Filter)
    : IRequestQuery, IRequest<Result<QueryResult<OrderDto>>>;

public record OrderDto(OrderId Id, CustomerId CustomerId, decimal Total, DateTimeOffset CreatedAt);

// Inheriting QueryValidatorRoot pre-wires offset >= 0 and limit > 0.
public class GetOrdersQueryValidator : QueryValidatorRoot<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(q => q.Sort)
            .Must(s => s.All(v => v is "total" or "-total" or "createdAt" or "-createdAt"))
            .When(q => q.Sort.Length > 0)
            .WithMessage("Allowed sort fields: total, createdAt");
    }
}

public class GetOrdersHandler(AppDbContext dbContext)
    : IRequestHandler<GetOrdersQuery, Result<QueryResult<OrderDto>>>
{
    public async Task<Result<QueryResult<OrderDto>>> Handle(
        GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var paramsResult = QueryParams.Create(query);
        if (paramsResult.IsFailure) return paramsResult.Error;

        var queryable = dbContext.Orders.AsNoTracking();
        var total = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .Query(paramsResult.Value)
            .Select(o => new OrderDto(o.Id, o.CustomerId, o.Total, o.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<QueryResult<OrderDto>>.Success(new QueryResult<OrderDto>(total, items));
    }
}
