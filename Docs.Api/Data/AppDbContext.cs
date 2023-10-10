using Docs.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Docs.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {}

        public DbSet<Document> Documents { get; set; }
    }
}
