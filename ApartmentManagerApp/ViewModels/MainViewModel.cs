using ApartmentManagementApp.Data;
using ApartmentManagementApp.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ApartmentManagementApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ReservationService _reservationService;

        // Collections
        public ObservableCollection<Guest> Guests { get; set; }
        public ObservableCollection<Apartment> Apartments { get; set; }
        public ObservableCollection<Reservation> Reservations { get; set; }

        // Selected items
        private Guest _selectedGuest;
        public Guest SelectedGuest
        {
            get => _selectedGuest;
            set => SetProperty(ref _selectedGuest, value);
        }

        private Apartment _selectedApartment;
        public Apartment SelectedApartment
        {
            get => _selectedApartment;
            set => SetProperty(ref _selectedApartment, value);
        }

        private Reservation _selectedReservation;
        public Reservation SelectedReservation
        {
            get => _selectedReservation;
            set => SetProperty(ref _selectedReservation, value);
        }

        // Guest form fields
        private string _guestFirstName;
        public string GuestFirstName
        {
            get => _guestFirstName;
            set => SetProperty(ref _guestFirstName, value);
        }

        private string _guestLastName;
        public string GuestLastName
        {
            get => _guestLastName;
            set => SetProperty(ref _guestLastName, value);
        }

        private string _guestEmail;
        public string GuestEmail
        {
            get => _guestEmail;
            set => SetProperty(ref _guestEmail, value);
        }

        private string _guestPhone;
        public string GuestPhone
        {
            get => _guestPhone;
            set => SetProperty(ref _guestPhone, value);
        }

        // Apartment form fields
        private string _apartmentName;
        public string ApartmentName
        {
            get => _apartmentName;
            set => SetProperty(ref _apartmentName, value);
        }

        private int _apartmentCapacity;
        public int ApartmentCapacity
        {
            get => _apartmentCapacity;
            set => SetProperty(ref _apartmentCapacity, value);
        }

        private string _apartmentLocation;
        public string ApartmentLocation
        {
            get => _apartmentLocation;
            set => SetProperty(ref _apartmentLocation, value);
        }

        private string _apartmentDescription;
        public string ApartmentDescription
        {
            get => _apartmentDescription;
            set => SetProperty(ref _apartmentDescription, value);
        }

        private decimal _apartmentPrice;
        public decimal ApartmentPrice
        {
            get => _apartmentPrice;
            set => SetProperty(ref _apartmentPrice, value);
        }

        // Reservation form fields
        private DateTime _checkInDate = DateTime.Today;
        public DateTime CheckInDate
        {
            get => _checkInDate;
            set => SetProperty(ref _checkInDate, value);
        }

        private DateTime _checkOutDate = DateTime.Today.AddDays(1);
        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set => SetProperty(ref _checkOutDate, value);
        }

        // Search fields for reservations
        private int? _searchApartmentId;
        public int? SearchApartmentId
        {
            get => _searchApartmentId;
            set => SetProperty(ref _searchApartmentId, value);
        }

        private int? _searchGuestId;
        public int? SearchGuestId
        {
            get => _searchGuestId;
            set => SetProperty(ref _searchGuestId, value);
        }

        private DateTime? _searchDate;
        public DateTime? SearchDate
        {
            get => _searchDate;
            set => SetProperty(ref _searchDate, value);
        }

        // Commands
        public ICommand AddGuestCommand { get; }
        public ICommand UpdateGuestCommand { get; }
        public ICommand DeleteGuestCommand { get; }
        public ICommand AddApartmentCommand { get; }
        public ICommand UpdateApartmentCommand { get; }
        public ICommand DeleteApartmentCommand { get; }
        public ICommand CreateReservationCommand { get; }
        public ICommand DeleteReservationCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand SearchReservationsCommand { get; }

        public MainViewModel(ReservationService reservationService)
        {
            _reservationService = reservationService;

            Guests = new ObservableCollection<Guest>();
            Apartments = new ObservableCollection<Apartment>();
            Reservations = new ObservableCollection<Reservation>();

            // Initialize commands
            AddGuestCommand = new RelayCommand(async _ => await AddGuest());
            UpdateGuestCommand = new RelayCommand(async _ => await UpdateGuest(), _ => SelectedGuest != null);
            DeleteGuestCommand = new RelayCommand(async _ => await DeleteGuest(), _ => SelectedGuest != null);
            AddApartmentCommand = new RelayCommand(async _ => await AddApartment());
            UpdateApartmentCommand = new RelayCommand(async _ => await UpdateApartment(), _ => SelectedApartment != null);
            DeleteApartmentCommand = new RelayCommand(async _ => await DeleteApartment(), _ => SelectedApartment != null);
            CreateReservationCommand = new RelayCommand(async _ => await CreateReservation());
            DeleteReservationCommand = new RelayCommand(async _ => await DeleteReservation(), _ => SelectedReservation != null);
            LoadDataCommand = new RelayCommand(async _ => await LoadData());
            SearchReservationsCommand = new RelayCommand(async _ => await SearchReservations());

            // Subscribe to events
            _reservationService.GuestAdded += OnGuestAdded;
            _reservationService.ReservationCreated += OnReservationCreated;

            // Load initial data
            LoadData().ConfigureAwait(false);
        }

        private async Task LoadData()
        {
            var guests = await _reservationService.GetAllGuestsAsync();
            var apartments = await _reservationService.GetAllApartmentsAsync();
            var reservations = await _reservationService.GetAllReservationsAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Guests.Clear();
                foreach (var guest in guests)
                    Guests.Add(guest);

                Apartments.Clear();
                foreach (var apartment in apartments)
                    Apartments.Add(apartment);

                Reservations.Clear();
                foreach (var reservation in reservations)
                    Reservations.Add(reservation);
            });
        }

        // Guest operations
        private async Task AddGuest()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(GuestFirstName))
            {
                MessageBox.Show("Ime je obavezno.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(GuestLastName))
            {
                MessageBox.Show("Prezime je obavezno.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(GuestEmail) && !GuestEmail.Contains("@"))
            {
                MessageBox.Show("Molimo unesite validnu email adresu.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await _reservationService.AddGuestAsync(GuestFirstName, GuestLastName, GuestEmail ?? string.Empty, GuestPhone ?? string.Empty);
            ClearGuestForm();
            await LoadData();
            MessageBox.Show("Gost je uspešno dodat!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task UpdateGuest()
        {
            if (SelectedGuest == null) return;

            await _reservationService.UpdateGuestAsync(SelectedGuest.GuestId, GuestFirstName, GuestLastName, GuestEmail, GuestPhone);
            await LoadData();
        }

        private async Task DeleteGuest()
        {
            if (SelectedGuest == null) return;

            var result = MessageBox.Show($"Obrisati gosta {SelectedGuest.GetFullName()}?", "Potvrda brisanja", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await _reservationService.DeleteGuestAsync(SelectedGuest.GuestId);
                await LoadData();
            }
        }

        private void ClearGuestForm()
        {
            GuestFirstName = string.Empty;
            GuestLastName = string.Empty;
            GuestEmail = string.Empty;
            GuestPhone = string.Empty;
        }

        // Apartment operations
        private async Task AddApartment()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(ApartmentName))
            {
                MessageBox.Show("Naziv apartmana je obavezan.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ApartmentCapacity <= 0)
            {
                MessageBox.Show("Kapacitet mora biti veći od 0.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ApartmentLocation))
            {
                MessageBox.Show("Lokacija je obavezna.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ApartmentPrice <= 0)
            {
                MessageBox.Show("Cena mora biti veća od 0.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await _reservationService.AddApartmentAsync(ApartmentName, ApartmentCapacity, ApartmentLocation, ApartmentDescription ?? string.Empty, ApartmentPrice);
            ClearApartmentForm();
            await LoadData();
            MessageBox.Show("Apartman je uspešno dodat!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task UpdateApartment()
        {
            if (SelectedApartment == null) return;

            await _reservationService.UpdateApartmentAsync(SelectedApartment.ApartmentId, ApartmentName, ApartmentCapacity, ApartmentLocation, ApartmentDescription, ApartmentPrice);
            await LoadData();
        }

        private async Task DeleteApartment()
        {
            if (SelectedApartment == null) return;

            var result = MessageBox.Show($"Obrisati apartman {SelectedApartment.Name}?", "Potvrda brisanja", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await _reservationService.DeleteApartmentAsync(SelectedApartment.ApartmentId);
                await LoadData();
            }
        }

        private void ClearApartmentForm()
        {
            ApartmentName = string.Empty;
            ApartmentCapacity = 0;
            ApartmentLocation = string.Empty;
            ApartmentDescription = string.Empty;
            ApartmentPrice = 0;
        }

        // Reservation operations
        private async Task CreateReservation()
        {
            // Validation
            if (SelectedGuest == null)
            {
                MessageBox.Show("Molimo izaberite gosta.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedApartment == null)
            {
                MessageBox.Show("Molimo izaberite apartman.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CheckOutDate <= CheckInDate)
            {
                MessageBox.Show("Datum odjave mora biti posle datuma prijave.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CheckInDate < DateTime.Today)
            {
                MessageBox.Show("Datum prijave ne može biti u prošlosti.", "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _reservationService.CreateReservationAsync(SelectedGuest.GuestId, SelectedApartment.ApartmentId, CheckInDate, CheckOutDate);
                await LoadData();
                MessageBox.Show("Rezervacija je uspešno kreirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Sukob rezervacije", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteReservation()
        {
            if (SelectedReservation == null) return;

            var result = MessageBox.Show("Obrisati ovu rezervaciju?", "Potvrda brisanja", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await _reservationService.DeleteReservationAsync(SelectedReservation.ReservationId);
                await LoadData();
            }
        }

        // Event handlers
        private void OnGuestAdded(object sender, GuestAddedEventArgs e)
        {
            MessageBox.Show($"Dodat je novi gost: {e.Guest.GetFullName()}", "Gost dodat");
        }

        private void OnReservationCreated(object sender, ReservationCreatedEventArgs e)
        {
            MessageBox.Show($"Kreirana je rezervacija za {e.Reservation.Guest?.GetFullName()}", "Rezervacija kreirana");
        }

        // Search reservations
        private async Task SearchReservations()
        {
            try
            {
                var results = await _reservationService.SearchReservationsAsync(SearchApartmentId, SearchGuestId, SearchDate);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Reservations.Clear();
                    foreach (var reservation in results)
                        Reservations.Add(reservation);
                });

                MessageBox.Show($"Pronađeno rezervacija: {results.Count}", "Rezultati pretrage");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri pretrazi: {ex.Message}", "Greška");
            }
        }
    }
}
