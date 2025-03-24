using Microsoft.EntityFrameworkCore;

namespace TreasureHunt.Entities
{
    public class TreasureDbContext : DbContext
    {
        public TreasureDbContext(DbContextOptions<TreasureDbContext> options) : base(options) { }

        public DbSet<TreasurePuzzle> TreasurePuzzles { get; set; }
    }
}
