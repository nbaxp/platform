namespace Wta.Infrastructure.Data;

public interface IDbSeeder<TContext> where TContext : DbContext
{
    void Seed(TContext context);
}
