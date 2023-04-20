using Microsoft.EntityFrameworkCore;
using NetworkMapLibrary;

namespace StatusAPICore
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
        public DbSet<Card> Cards { get; set; }
    }
}
