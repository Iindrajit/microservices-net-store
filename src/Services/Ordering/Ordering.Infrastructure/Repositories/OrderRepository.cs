using Ordering.Application.Data;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository(IApplicationDbContext dbContext) : IOrderRepository
    {
        public void Add(Order order) => dbContext.Add(order);

        public async Task<Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            return await dbContext.Query<Order>()
                            .FirstOrDefaultAsync(o => o.Id.Value == orderId, cancellationToken);
        }

        public void Remove(Order order) => dbContext.Remove(order);
        public void Update(Order order) => dbContext.Update(order);

        public async Task<long> GetTotalOrdersCountAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Query<Order>().LongCountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetPaginatedOrdersAsync(
            int pageIndex, 
            int pageSize, 
            CancellationToken cancellationToken)
        {
            return await dbContext.Query<Order>()
                .Include(o => o.OrderItems)
                .OrderBy(o => o.OrderName.Value)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await dbContext.Query<Order>()
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Where(o => o.CustomerId == CustomerId.Of(customerId))
                .OrderBy(o => o.OrderName.Value)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<Order>> GetOrdersByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await dbContext.Query<Order>()
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Where(o => o.OrderName.Value.Contains(name))
                .OrderBy(o => o.OrderName.Value)
                .ToListAsync(cancellationToken);
        }
    }
}
