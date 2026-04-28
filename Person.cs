namespace Hospital_System
{
    public enum GenderType { Male, Female }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public GenderType Gender { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public Person() { }

        public Person(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email, string address)
        {
            Name = name;
            Age = age;
            Gender = gender;
            NationalId = nationalId;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
        }
    }
}