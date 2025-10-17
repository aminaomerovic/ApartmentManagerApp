using System.Windows;
using ApartmentManagementApp.ViewModels;
using ApartmentManagementApp.Data;
using ApartmentManagementApp.Models;
using System.Linq;

namespace ApartmentManagementApp
{
    public partial class MainWindow : Window
    {
        private static ApartmentDbContext _sharedContext;

        public MainWindow()
        {
            InitializeComponent();

            if (_sharedContext == null)
            {
                _sharedContext = new ApartmentDbContext();
                _sharedContext.Database.EnsureCreated();
                SeedDataIntoContext(_sharedContext);
            }

            var reservationService = new ReservationService(_sharedContext);
            DataContext = new MainViewModel(reservationService);
        }

        private void SeedDataIntoContext(ApartmentDbContext context)
        {
            if (!context.Apartments.Any())
            {
                context.Apartments.AddRange(
                    new Apartment
                    {
                        Name = "Luksuzni Apartman Centar",
                        Capacity = 4,
                        Location = "Beograd, Centar",
                        Description = "Moderan apartman u srcu grada sa pogledom na reku",
                        PricePerNight = 12000
                    },
                    new Apartment
                    {
                        Name = "Udoban Studio",
                        Capacity = 2,
                        Location = "Novi Beograd",
                        Description = "Idealan za parove, blizu Delta City-a",
                        PricePerNight = 7000
                    },
                    new Apartment
                    {
                        Name = "Porodični Apartman",
                        Capacity = 6,
                        Location = "Vračar",
                        Description = "Prostran apartman za porodice sa decom",
                        PricePerNight = 15000
                    }
                );
            }

            if (!context.Guests.Any())
            {
                context.Guests.AddRange(
                    new Guest("Dina", "Bećirović", "dina.becirovic@email.com", "060-123-4567"),
                    new Guest("Binela", "Omerović", "binela.omerovic@email.com", "061-987-6543"),
                    new Guest("Ertan", "Zahitović", "ertan.zahitovic@email.com", "062-555-1234")
                );
            }

            context.SaveChanges();
        }
    }
}