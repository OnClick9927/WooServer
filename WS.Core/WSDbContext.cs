using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WS.Core;

//Add-Migration InitialCreate
//Update-Database
public class WSDbContext<T> : DbContext where T : WSDbContext<T>
{
    protected WSDbContext(DbContextOptions<T> options) : base(options) { }

 
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

    }
    public async Task Delete<TEntity>(TEntity entity) where TEntity : class
    {
        this.Set<TEntity>().Remove(entity);
        await SaveChangesAsync();
    }
    public async Task DeleteRange<TEntity>(IEnumerable<TEntity> ie) where TEntity : class
    {
        foreach (TEntity entity in ie)
            this.Set<TEntity>().Remove(entity);
        await SaveChangesAsync();
    }
   
}
public class WSDbModel
{
    public int Id { get; set; }

    //public bool need_delete;
}

