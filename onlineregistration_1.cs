namespace Hospital_System
{
    public class OnlineRegistration
    {
        public string NameEnglish;
        public string NationalID;
        public string Phone;
        public string Address;
        public string Password;
        public bool IsRegistered = false;

        public string ToFileLine() =>
            $"{NationalID},{NameEnglish},{Phone},{Address},{Password},{IsRegistered}";
    }
}