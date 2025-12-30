using LondonStockApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Data
{
    public class TradingDbContext : DbContext
    {
        public TradingDbContext(DbContextOptions<TradingDbContext> options)
       : base(options)
        {
        }

        public DbSet<Trade> Trades => Set<Trade>();
        public DbSet<Stock> Stocks => Set<Stock>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(t => t.StockId)
                      .IsRequired();

                entity.Property(t => t.Price)
                      .HasPrecision(18, 4)
                      .IsRequired();

                entity.Property(t => t.Quantity)
                      .HasPrecision(18, 4)
                      .IsRequired();

                entity.Property(t => t.BrokerId)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(t => t.TradedAt)
                      .IsRequired();

                entity.HasIndex(t => t.StockId);

                entity.HasOne<Stock>()
                      .WithMany()
                      .HasForeignKey(t => t.StockId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Trades_Stocks");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.TickerSymbol)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.HasIndex(s => s.TickerSymbol)
                      .IsUnique();

                entity.Property(s => s.AveragePrice)
                      .HasPrecision(18, 4)
                      .IsRequired();

                entity.Property(s => s.TotalVolume)
                      .HasPrecision(18, 4)
                      .IsRequired();

                entity.Property(s => s.LastUpdated)
                      .IsRequired();
            });
        }
    }
}
