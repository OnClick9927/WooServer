using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WS.Core.Tool;

namespace WS.DB
{
    //Add-Migration InitialCreate
    //Update-Database
    interface IDbContext { }
    public class WSDbContext<T> : DbContext, IDbContext where T : WSDbContext<T>
    {
        public WSDbContext(DbContextOptions<T> options) : base(options) { }

    }
    public static class DBServiceTool
    {
        public static void AddDBContexts(this IServiceCollection services, Action<DbContextOptionsBuilder, Type> config)
        {
            var types = typeof(IDbContext).GetSubTypes();
            types.Where(x => !x.IsAbstract && !x.IsGenericType).ToList().ForEach(type =>
            {

                var baseType = type.BaseType;
                DbContextOptionsBuilder builder = typeof(DbContextOptionsBuilder<>).MakeGenericType(type)
                    .CreateInstance<object>() as DbContextOptionsBuilder;

                config?.Invoke(builder, type);
                var obj = Activator.CreateInstance(type, builder.Options);
                services.AddSingleton(type, obj);
            });
        }
    }

}
