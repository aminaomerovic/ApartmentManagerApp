namespace ApartmentManagementApp.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }

        public Reservation() { }

        public Reservation(int guestId, int apartmentId, DateTime checkInDate, DateTime checkOutDate, decimal totalPrice)
        {
            GuestId = guestId;
            ApartmentId = apartmentId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            TotalPrice = totalPrice;
        }

        public int GetNumberOfNights()
        {
            return (int)(CheckOutDate - CheckInDate).TotalDays;
        }
    }
}