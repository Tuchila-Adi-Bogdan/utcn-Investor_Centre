using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Areas.Identity.Data;
using InvestorCenter.Models;

namespace InvestorCenter.Data;

public class InvestorCenterContext : IdentityDbContext<InvestorCenterUser>
{
    public InvestorCenterContext(DbContextOptions<InvestorCenterContext> options)
        : base(options)
    {
    }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<NewsArticle> NewsArticles { get; set; }
    public DbSet<StockEffect> StockEffects { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<PortfolioItem> PortfolioItems { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
