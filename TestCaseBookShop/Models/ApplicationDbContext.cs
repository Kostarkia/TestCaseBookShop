using Microsoft.EntityFrameworkCore;
using TestCaseBookShop.Models.Data;
using TestCaseBookShop.Models.Data.Auth;

namespace TestCaseBookShop.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
