namespace ApartmentManagementApp.Models
{
    public class Apartment
    {
        public int ApartmentId { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public Apartment() { }

        public Apartment(string name, int capacity, string location, string description, decimal pricePerNight)
        {
            Name = name;
            Capacity = capacity;
            Location = location;
            Description = description;
            PricePerNight = pricePerNight;
        }
    }
}