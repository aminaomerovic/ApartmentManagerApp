namespace ApartmentManagementApp.Models
{
    public class Guest : Person
    {
        public int GuestId { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public Guest() { }

        public Guest(string firstName, string lastName, string email, string phoneNumber)
            : base(firstName, lastName, email, phoneNumber)
        {
        }

        public override string GetFullName()
        {
            return base.GetFullName();
        }
    }
}