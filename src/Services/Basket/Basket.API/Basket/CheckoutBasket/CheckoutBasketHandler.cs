using BuildingBlocks.Messaging.Events;
using MassTransit;
using System.Text.Json;
using System.Transactions;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto)
    : ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator
    : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class CheckoutBasketCommandHandler
    (IBasketRepository repository, IDocumentSession session)//IPublishEndpoint publishEndpoint, 
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // get existing basket with total price
            // Set totalprice on basketcheckout event message
            // send basket checkout event to rabbitmq using masstransit
            // delete the basket

            var basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
            if (basket == null)
            {
                return new CheckoutBasketResult(false);
            }

            var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;

            eventMessage.OrderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                eventMessage.OrderItems.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity, Price = item.Price });
            }

            //Implementation of Outbox Pattern to fix the Dual-write Problem
            //await publishEndpoint.Publish(eventMessage, cancellationToken);

            // Add an outbox message
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(BasketCheckoutEvent).AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(eventMessage),
                OccurredOn = DateTime.UtcNow
            };

            session.Store(outboxMessage);

            await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken);

            return new CheckoutBasketResult(true);
        }
        catch
        {
            return new CheckoutBasketResult(false);
        }
    }

    //public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    //{
    //    // get existing basket with total price
    //    // Set totalprice on basketcheckout event message
    //    // send basket checkout event to rabbitmq using masstransit
    //    // delete the basket

    //    var basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
    //    if (basket == null)
    //    {
    //        return new CheckoutBasketResult(false);
    //    }

    //    var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
    //    eventMessage.TotalPrice = basket.TotalPrice;

    //    eventMessage.OrderItems = new List<OrderItem>();
    //    foreach (var item in basket.Items)
    //    {
    //        eventMessage.OrderItems.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity, Price = item.Price });
    //    }

    //    //TODO: Implementation of Outbox Pattern to fix the Dual-write Problem
    //    await publishEndpoint.Publish(eventMessage, cancellationToken);

    //    await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken);

    //    return new CheckoutBasketResult(true);
    //}
}
