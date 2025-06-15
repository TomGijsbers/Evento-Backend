using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Data;

public class ApplicationDbContext : DbContext   // <-- naam exact zo
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Location> Locations { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    
    public DbSet<EventFeedback> EventFeedbacks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // unieke Auth0-ID
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Auth0Id)
            .IsUnique();

        // relatie Feedback → Event
        modelBuilder.Entity<EventFeedback>()
            .HasOne(f => f.Event)
            .WithMany()                // of .WithMany(e => e.Feedbacks)
            .HasForeignKey(f => f.EventId);

        // relatie Feedback → User
        modelBuilder.Entity<EventFeedback>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId);
        
          // ╭──── Unique sleutel: 1 inschrijving pp event+user ───╮
            modelBuilder.Entity<EventRegistration>()
              .HasIndex(r => new { r.EventId, r.UserId })
              .IsUnique();
    }

    

}