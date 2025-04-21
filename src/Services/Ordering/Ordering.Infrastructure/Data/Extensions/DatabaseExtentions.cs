using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Infrastructure.Data.Extensions;
public static class DatabaseExtentions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if(context == null)
            throw new InvalidOperationException("Failed to get ApplicationDbContext.");

        context.Database.MigrateAsync().GetAwaiter().GetResult();

        await SeedAsync(context);
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedCustomerAsync(context);
        await SeedProductAsync(context);
        //await SeedOrdersWithItemsAsync(context);
    }

    private static async Task SeedCustomerAsync(ApplicationDbContext context)
    {
        if (!await context.Query<Customer>().AnyAsync())
        {
            await context.AddRangeAsync(InitialData.Customers);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    private static async Task SeedProductAsync(ApplicationDbContext context)
    {
        if (!await context.Query<Product>().AnyAsync())
        {
            await context.AddRangeAsync(InitialData.Products);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    //private static async Task SeedOrdersWithItemsAsync(ApplicationDbContext context)
    //{
    //    if (!await context.Orders.AnyAsync())
    //    {
    //        await context.Orders.AddRangeAsync(InitialData.OrdersWithItems);
    //        await context.SaveChangesAsync();
    //    }
    //}
}
