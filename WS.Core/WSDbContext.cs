using Microsoft.EntityFrameworkCore;

namespace WS.Core;

//Add-Migration InitialCreate
//Update-Database
public class WSDbContext<T> : DbContext where T : WSDbContext<T>
{
    public WSDbContext(DbContextOptions<T> options) : base(options) { }

    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

    }
}
