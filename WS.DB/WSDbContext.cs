
using Microsoft.EntityFrameworkCore;

namespace WS.DB
{
    public class WSDbContext<T> : DbContext, IWSDbContext where T : WSDbContext<T>
    {
        protected WSDbContext(DbContextOptions<T> options) : base(options) { }

    }
}
