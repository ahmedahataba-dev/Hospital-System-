namespace Hospital_System
{
    public class Clinic
    {
        public int Id;
        public string NameEnglish;
        public string[] AvailableDays;

        public Clinic(int id, string name, string[] days)
        {
            Id = id;
            NameEnglish = name;
            AvailableDays = days;
        }
    }
}