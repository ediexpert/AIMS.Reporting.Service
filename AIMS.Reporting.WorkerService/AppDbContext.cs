using AIMS.Dashboards.Data;
using Microsoft.EntityFrameworkCore;

namespace AIMS.Reporting.WorkerService
{
    public class AppDbContext: DbContext
    {
        static DbContextOptions<AppDbContext> _options;
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<EventSection>()
                .HasOne(es => es.Event)
                .WithMany(a => a.EventSections)
                .HasForeignKey(es => es.EventId);
        }

        public DbSet<Hazri> Hazris { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<HazriNigraniData> HazriNigraniDatas { get; set; }
        public DbSet<EventSection> EventSections { get; set; }

    }
}

