using MassTransit;
using System.Text.Json;

namespace Basket.API.Service
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IBus bus;
        private readonly ILogger<OutboxProcessor> logger;

        public OutboxProcessor(IServiceProvider serviceProvider, IBus bus, ILogger<OutboxProcessor> logger)
        {
            this.serviceProvider = serviceProvider;
            this.bus = bus;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

                    // Query unprocessed outbox messages
                    var outboxMessages = await session.Query<OutboxMessage>()
                        .Where(m => m.ProcessedOn == null)
                        .ToListAsync(stoppingToken);

                    foreach (var message in outboxMessages)
                    {
                        var eventType = Type.GetType(message.Type);
                        if (eventType == null)
                        {
                            logger.LogWarning("Could not resolve type: {Type}", message.Type);
                            continue;
                        }

                        var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);
                        if (eventMessage == null)
                        {
                            logger.LogWarning("Could not deserialize message: {Content}", message.Content);
                            continue;
                        }

                        await bus.Publish(eventMessage, stoppingToken);

                        message.ProcessedOn = DateTime.UtcNow;
                        session.Store(message);
                        await session.SaveChangesAsync(stoppingToken);

                        logger.LogInformation("Successfully processed outbox message with ID: {Id}", message.Id);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing outbox messages");
                }

                // Delay before polling again
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
