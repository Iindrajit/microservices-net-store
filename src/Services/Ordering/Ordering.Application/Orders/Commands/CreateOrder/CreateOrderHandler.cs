using Ordering.Application.Repositories;

namespace Ordering.Application.Orders.Commands.CreateOrder;
public class CreateOrderHandler(
    IOrderRepository orderRepository, 
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        //create Order entity from command object
        //save to database
        //return result 

        var order = CreateNewOrder(command.Order);

        orderRepository.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id.Value);
    }

    private Order CreateNewOrder(OrderDto orderDto)
    {
        var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        var orderId = Guid.NewGuid();
        
        var newOrder = Order.Create(
            id: OrderId.Of(orderId),
            customerId: CustomerId.Of(orderDto.CustomerId),
            orderName: OrderName.Of(orderDto.OrderName),
            orderNumber: GenerateOrderNumber(orderId, orderDto.OrderName),
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            payment: Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod)
            );

        foreach (var orderItemDto in orderDto.OrderItems)
        {
            newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price);
        }
        return newOrder;
    }

    private string GenerateOrderNumber(Guid orderId, string orderName)
    {
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        string guidPart = orderId.ToString().Split('-')[0]; // Take the first part of the Guid
        string orderNamePart = orderName.Length > 3
            ? orderName.Substring(0, 3).ToUpper()
            : orderName.ToUpper(); // First 3 letters of orderName

        return $"{datePart}-{orderNamePart}-{guidPart}";
    }
}

/*
public class CreateOrderHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        //create Order entity from command object
        //save to database
        //return result 

        var order = CreateNewOrder(command.Order);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id.Value);
    }

    private Order CreateNewOrder(OrderDto orderDto)
    {
        var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        // change for OrderNum

        var orderId = Guid.NewGuid();
        //var orderNumber = 
        var newOrder = Order.Create(
            id: OrderId.Of(orderId),
            customerId: CustomerId.Of(orderDto.CustomerId),
            orderName: OrderName.Of(orderDto.OrderName),
            orderNumber: GenerateOrderNumber(orderId, orderDto.OrderName),
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            payment: Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod)
            );

        foreach (var orderItemDto in orderDto.OrderItems)
        {
            newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price);
        }
        return newOrder;
    }

    private string GenerateOrderNumber(Guid orderId, string orderName)
    {
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        string guidPart = orderId.ToString().Split('-')[0]; // Take the first part of the Guid
        string orderNamePart = orderName.Length > 3
            ? orderName.Substring(0, 3).ToUpper()
            : orderName.ToUpper(); // First 3 letters of orderName

        return $"{datePart}-{orderNamePart}-{guidPart}";
    }
}
*/
