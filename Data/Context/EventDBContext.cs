using EventSchedulePro.Data.Class;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Group = EventSchedulePro.Data.Class.Group;

namespace EventSchedulePro.Data.Context
{
    public class EventDBContext : DbContext
    {
        public EventDBContext(DbContextOptions<EventDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Staff> Staffs { get; set;}
        public DbSet<Content> Contents { get; set; }
    }
}
