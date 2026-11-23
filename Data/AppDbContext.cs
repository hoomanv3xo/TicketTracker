using Microsoft.EntityFrameworkCore;
using TicketTracker.Models;

namespace TicketTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Ticket>().Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        b.Entity<Ticket>().Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
        b.Entity<AppUser>().Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        b.Entity<AppUser>().HasData(
    new AppUser { Id = 1, Name = "Alex Requester", Role = UserRole.Requester },
    new AppUser { Id = 2, Name = "Sam Technician", Role = UserRole.Technician });
    }
}