namespace Ordering.Application.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        void Update(Order order);
        void Remove(Order order);
        Task<Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);

        // New methods for pagination and count
        Task<long> GetTotalOrdersCountAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<Order>> GetPaginatedOrdersAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Order>> GetOrdersByNameAsync(string name, CancellationToken cancellationToken);
    }
}
