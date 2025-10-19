using ApartmentManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementApp.Data
{
    public class ReservationService
    {
        private readonly ApartmentDbContext _context;

        public event EventHandler<ReservationCreatedEventArgs> ReservationCreated;
        public event EventHandler<GuestAddedEventArgs> GuestAdded;

        public ReservationService(ApartmentDbContext context)
        {
            _context = context;
        }

        // GUEST OPERATIONS
        public async Task<List<Guest>> GetAllGuestsAsync()
        {
            return await _context.Guests.ToListAsync();
        }

        public async Task<Guest> GetGuestByIdAsync(int guestId)
        {
            return await _context.Guests.FindAsync(guestId);
        }

        public async Task<Guest> AddGuestAsync(string firstName, string lastName, string email, string phoneNumber)
        {
            var guest = new Guest(firstName, lastName, email, phoneNumber);
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            OnGuestAdded(new GuestAddedEventArgs { Guest = guest });
            return guest;
        }

        public async Task UpdateGuestAsync(int guestId, string firstName, string lastName, string email, string phoneNumber)
        {
            var guest = await GetGuestByIdAsync(guestId);
            if (guest != null)
            {
                guest.FirstName = firstName;
                guest.LastName = lastName;
                guest.Email = email;
                guest.PhoneNumber = phoneNumber;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteGuestAsync(int guestId)
        {
            var guest = await GetGuestByIdAsync(guestId);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
            }
        }

        // APARTMENT OPERATIONS
        public async Task<List<Apartment>> GetAllApartmentsAsync()
        {
            return await _context.Apartments.ToListAsync();
        }

        public async Task<Apartment> GetApartmentByIdAsync(int apartmentId)
        {
            return await _context.Apartments.FindAsync(apartmentId);
        }

        public async Task<Apartment> AddApartmentAsync(string name, int capacity, string location, string description, decimal pricePerNight)
        {
            var apartment = new Apartment(name, capacity, location, description, pricePerNight);
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            return apartment;
        }

        public async Task UpdateApartmentAsync(int apartmentId, string name, int capacity, string location, string description, decimal pricePerNight)
        {
            var apartment = await GetApartmentByIdAsync(apartmentId);
            if (apartment != null)
            {
                apartment.Name = name;
                apartment.Capacity = capacity;
                apartment.Location = location;
                apartment.Description = description;
                apartment.PricePerNight = pricePerNight;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteApartmentAsync(int apartmentId)
        {
            var apartment = await GetApartmentByIdAsync(apartmentId);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();
            }
        }

        // RESERVATION OPERATIONS
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Apartment)
                .ToListAsync();
        }

        public async Task<Reservation> CreateReservationAsync(int guestId, int apartmentId, DateTime checkInDate, DateTime checkOutDate)
        {
            // LINQ: overlapping reservations
            var hasOverlap = _context.Reservations
                .Any(r => r.ApartmentId == apartmentId &&
                          r.CheckInDate < checkOutDate &&
                          r.CheckOutDate > checkInDate);

            if (hasOverlap)
                throw new InvalidOperationException("Apartman nije dostupan za izabrane datume.");

            int nights = (int)(checkOutDate - checkInDate).TotalDays;
            var apartment = await GetApartmentByIdAsync(apartmentId);
            decimal totalPrice = nights * apartment.PricePerNight;

            var reservation = new Reservation(guestId, apartmentId, checkInDate, checkOutDate, totalPrice);
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            OnReservationCreated(new ReservationCreatedEventArgs { Reservation = reservation });
            return reservation;
        }

        public async Task<List<Reservation>> SearchReservationsAsync(int? apartmentId = null, int? guestId = null, DateTime? date = null)
        {
            // LINQ: Search reservations with filters
            var query = _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Apartment)
                .AsQueryable();

            if (apartmentId.HasValue)
                query = query.Where(r => r.ApartmentId == apartmentId.Value);

            if (guestId.HasValue)
                query = query.Where(r => r.GuestId == guestId.Value);

            if (date.HasValue)
                query = query.Where(r => r.CheckInDate <= date.Value && r.CheckOutDate >= date.Value);

            return await query.ToListAsync();
        }

        public async Task DeleteReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        // EVENTS
        protected virtual void OnReservationCreated(ReservationCreatedEventArgs e)
        {
            ReservationCreated?.Invoke(this, e);
        }

        protected virtual void OnGuestAdded(GuestAddedEventArgs e)
        {
            GuestAdded?.Invoke(this, e);
        }
    }

    // EVENT ARGUMENT CLASSES
    public class ReservationCreatedEventArgs : EventArgs
    {
        public Reservation Reservation { get; set; }
    }

    public class GuestAddedEventArgs : EventArgs
    {
        public Guest Guest { get; set; }
    }
}