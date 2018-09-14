using Articulate.Models;
using Microsoft.EntityFrameworkCore;

namespace Articulate.Repositories
{
    public class AttendeeContext : DbContext
    {
        protected AttendeeContext()
        {
        }

        public AttendeeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Attendee> Attendees { get; set; }
    }
}