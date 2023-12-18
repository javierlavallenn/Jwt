using Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Context
{
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
