namespace Ordering.Application.Orders.Queries.GetOrdersByName;

public class GetOrdersByNameHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
{
    public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
    {
        // get orders by name using dbContext
        // return result

        var orders = await orderRepository.GetOrdersByNameAsync(query.Name, cancellationToken);
        return new GetOrdersByNameResult(orders.ToOrderDtoList());
    }
}

//public class GetOrdersByNameHandler(IApplicationDbContext dbContext)
//    : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
//{
//    public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
//    {
//        // get orders by name using dbContext
//        // return result

//        var orders = await dbContext.Orders
//                .Include(o => o.OrderItems)
//                .AsNoTracking()
//                .Where(o => o.OrderName.Value.Contains(query.Name))
//                .OrderBy(o => o.OrderName.Value)
//                .ToListAsync(cancellationToken);

//        return new GetOrdersByNameResult(orders.ToOrderDtoList());
//    }
//}
