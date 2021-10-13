using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class TowerContext : DbContext
    {
        public virtual DbSet<FlightModel> Flights { get; set; }
        public virtual DbSet<StationModel> Stations { get; set; }
        public virtual DbSet<TrafficHistory> History { get; set; }
        public TowerContext(DbContextOptions<TowerContext> opts) : base(opts) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}
