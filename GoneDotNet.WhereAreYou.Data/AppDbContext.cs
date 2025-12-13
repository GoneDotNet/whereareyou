using Microsoft.EntityFrameworkCore;

namespace GoneDotNet.WhereAreYou.Data;

public class AppDbContext : DbContext
{
    public AppDbContext() : base() {}
    public AppDbContext(DbContextOptions options) : base(options) {}
    
    public DbSet<UserCheckin> UserCheckins => this.Set<UserCheckin>();
}
