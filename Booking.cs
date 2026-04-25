namespace Hospital_System
{
    public class Booking
    {
        public string PatientID;
        public string ClinicName;
        public string Day;
        public string Code;
        public string BookingDateTime;
        public bool IsPaid = false;

        public string ToFileLine() =>
            $"{PatientID},{ClinicName},{Day},{Code},{IsPaid},{BookingDateTime}";
    }
}