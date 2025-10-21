using Microsoft.EntityFrameworkCore;
using AllBlue.Models; 

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

}
