namespace ApartmentManagementApp.Models
{
    public abstract class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public Person() { }

        public Person(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public virtual string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}