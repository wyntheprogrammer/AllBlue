using Microsoft.EntityFrameworkCore;
using AllBlue.Models; 

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    public DbSet<Customer> Customer { get; set; }
    public DbSet<City> City { get; set; }
    public DbSet<Barangay> Barangay { get; set; }
    public DbSet<Item> Item { get; set; }

}
