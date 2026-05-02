namespace Hospital_System
{
    public class Booking
    {
        // Properties so System.Text.Json serializes them correctly
        public string PatientID       { get; set; } = "";
        public string ClinicName      { get; set; } = "";
        public string Day             { get; set; } = "";
        public string Code            { get; set; } = "";
        public string BookingDateTime { get; set; } = "";
        public bool   IsPaid          { get; set; } = false;
    }
}
