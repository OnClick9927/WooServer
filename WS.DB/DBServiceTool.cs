
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WS.Core.Tool;

namespace WS.DB
{
    //Add-Migration InitialCreate
    //Update-Database
    public static class DBServiceTool
    {
        public static void AddWSDataContexts(this IServiceCollection services, Action<DbContextOptionsBuilder, Type> config)
        {
            typeof(IWSDbContext).GetSubTypes().Where(x => !x.IsAbstract && !x.IsGenericType).ToList().ForEach(type =>
            {

                Console.WriteLine(type);
                var baseType = type.BaseType;
                Type genType = baseType.GetGenericArguments()[0];
                DbContextOptionsBuilder builder = typeof(DbContextOptionsBuilder<>).MakeGenericType(genType).CreateInstance<object>() as DbContextOptionsBuilder;

                config?.Invoke(builder, type);
                var obj = Activator.CreateInstance(type, builder.Options);
                services.AddSingleton(type, obj);
            });

        }
    }
}
