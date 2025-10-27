namespace ShiftManager.Web.Data;

using Microsoft.EntityFrameworkCore;
using ShiftManager.Web.Data.Entities;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Affiliate> Affiliates { get; set; }

    protected ApplicationDbContext()
    {
    }
}