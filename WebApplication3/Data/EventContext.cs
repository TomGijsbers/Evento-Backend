using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Data;


public class ApplicationDbContext : DbContext  
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
        base.OnModelCreating(modelBuilder); // Roep de basis implementatie aan.

       
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Auth0Id)
            .IsUnique();

       
        modelBuilder.Entity<EventFeedback>()
            .HasOne(f => f.Event)
            .WithMany()                // of .WithMany(e => e.Feedbacks)
            .HasForeignKey(f => f.EventId);


        modelBuilder.Entity<EventFeedback>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId);
        
          // ╭──── Unique sleutel: 1 inschrijving pp event+user ───╮
          // Configureert een samengestelde unieke sleutel voor EventRegistration.
          // Dit zorgt ervoor dat een gebruiker zich slechts één keer kan registreren voor een specifiek evenement.
          // De combinatie van EventId en UserId moet uniek zijn.
            modelBuilder.Entity<EventRegistration>()
              .HasIndex(r => new { r.EventId, r.UserId })
              .IsUnique();
    }

    

}
