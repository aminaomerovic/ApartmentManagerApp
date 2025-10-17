using Microsoft.EntityFrameworkCore;
using ApartmentManagementApp.Models;

namespace ApartmentManagementApp.Data
{
    public class ApartmentDbContext : DbContext
    {
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public ApartmentDbContext() { }

        public ApartmentDbContext(DbContextOptions<ApartmentDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=localhost;Database=ApartmentManagement;User=root;Password=Ertanertan2@;";
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                optionsBuilder.UseMySql(connectionString, serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Guest>().HasKey(g => g.GuestId);
            modelBuilder.Entity<Apartment>().HasKey(a => a.ApartmentId);
            modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Guest)
                .WithMany(g => g.Reservations)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Apartment)
                .WithMany(a => a.Reservations)
                .HasForeignKey(r => r.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}