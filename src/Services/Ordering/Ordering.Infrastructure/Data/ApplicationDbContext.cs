using Ordering.Application.Data;
using System.Reflection;

namespace Ordering.Infrastructure.Data;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    void IApplicationDbContext.Add<TEntity>(TEntity entity) where TEntity : class
    {
        Set<TEntity>().Add(entity);
    }

    void IApplicationDbContext.Update<TEntity>(TEntity entity)
    {
        Set<TEntity>().Update(entity);
    }

    void IApplicationDbContext.Remove<TEntity>(TEntity entity)
    {
        Set<TEntity>().Remove(entity);
    }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class
    {
        return Set<TEntity>().AsQueryable();
    }
}