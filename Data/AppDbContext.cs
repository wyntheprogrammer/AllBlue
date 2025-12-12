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
    public DbSet<AccountType> AccountType { get; set; }
    public DbSet<UserAccount> UserAccount { get; set; }

    public DbSet<Order> Order { get; set; }
    public DbSet<Payment> Payment { get; set; }

    public DbSet<Stock> Stock { get; set; }
    public DbSet<StockIn> StockIn { get; set; }
    public DbSet<StockOut> StockOut { get; set; }


    public DbSet<Expense> Expense { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategory { get; set; }

    public DbSet<Settings> Settings{ get; set; }

}
