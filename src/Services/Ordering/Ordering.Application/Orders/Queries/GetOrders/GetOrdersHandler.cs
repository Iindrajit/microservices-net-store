using BuildingBlocks.Pagination;

namespace Ordering.Application.Orders.Queries.GetOrders;

public class GetOrdersHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        // get orders with pagination
        // return result

        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await orderRepository.GetTotalOrdersCountAsync(cancellationToken);

        var orders = await orderRepository.GetPaginatedOrdersAsync(pageIndex, pageSize, cancellationToken);

        return new GetOrdersResult(
            new PaginatedResult<OrderDto>(
                pageIndex,
                pageSize,
                totalCount,
                orders.ToOrderDtoList()));
    }
}

//public class GetOrdersHandler(IApplicationDbContext dbContext)
//    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
//{
//    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
//    {
//        // get orders with pagination
//        // return result

//        var pageIndex = query.PaginationRequest.PageIndex;
//        var pageSize = query.PaginationRequest.PageSize;

//        var totalCount = await dbContext.Orders.LongCountAsync(cancellationToken);

//        //var orders = await dbContext.Orders
//        //               .Include(o => o.OrderItems)
//        //               .OrderBy(o => o.OrderName.Value)
//        //               .Skip(pageSize * pageIndex)
//        //               .Take(pageSize)
//        //               .ToListAsync(cancellationToken);

//        var orders = await dbContext.Orders
//                      .Include(o => o.OrderItems)
//                  //    .ThenInclude(oi => oi.)
//                      .OrderBy(o => o.OrderName.Value)
//                      .Skip(pageSize * pageIndex)
//                      .Take(pageSize)
//                      .ToListAsync(cancellationToken);

//        return new GetOrdersResult(
//            new PaginatedResult<OrderDto>(
//                pageIndex,
//                pageSize,
//                totalCount,
//                orders.ToOrderDtoList()));
//    }
//}
