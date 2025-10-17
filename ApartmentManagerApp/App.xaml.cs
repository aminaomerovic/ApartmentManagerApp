using System.Windows;
using ApartmentManagementApp.Data;
using ApartmentManagementApp.Models;
using System.Linq;

namespace ApartmentManagementApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var context = new ApartmentDbContext())
            {
                context.Database.EnsureCreated();
                SeedData(context);
            }
        }

        private void SeedData(ApartmentDbContext context)
        {
            // Add initial apartments if database is empty
            if (!context.Apartments.Any())
            {
                context.Apartments.AddRange(
                    new Apartment
                    {
                        Name = "Luxury Downtown Apartment",
                        Capacity = 4,
                        Location = "Downtown Belgrade",
                        Description = "Modern apartment in the heart of the city",
                        PricePerNight = 120
                    },
                    new Apartment
                    {
                        Name = "Cozy Studio",
                        Capacity = 2,
                        Location = "New Belgrade",
                        Description = "Perfect for couples",
                        PricePerNight = 70
                    },
                    new Apartment
                    {
                        Name = "Family Apartment",
                        Capacity = 6,
                        Location = "Vracar",
                        Description = "Spacious apartment for families",
                        PricePerNight = 150
                    }
                );
            }

            // Add initial guests if database is empty
            if (!context.Guests.Any())
            {
                context.Guests.AddRange(
                    new Guest("John", "Doe", "john.doe@email.com", "060-123-4567"),
                    new Guest("Jane", "Smith", "jane.smith@email.com", "061-987-6543"),
                    new Guest("Marko", "Petrovic", "marko.petrovic@email.com", "062-555-1234")
                );
            }

            context.SaveChanges();
        }
    }
}