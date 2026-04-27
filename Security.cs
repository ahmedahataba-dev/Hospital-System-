using System.Collections.Generic;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Security : Employee
    {
        int badgeNumber;
        string shift; //morning, evening, night
        private string _securityName;

        public static List<Security> securities = new List<Security>();

        public Security() : base() { }

        public new string Name
        {
            get => _securityName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _securityName = "Unknown";
                }
                _securityName = value;
                base.Name = value;
            }
        }

        public string Shift
        {
            get { return shift; }
            set { shift = value; }
        }
        public int BadgeNumber
        {
            get { return badgeNumber; }
            set { badgeNumber = value; }
        }

        public Security(string name, string badgeNumber, string shift, int age, GenderType gender, string Nationalid, string phoneNumber, string email, string address, decimal salary, double experienceyears)
        : base(name, age, gender, Nationalid, phoneNumber, email, address, salary, experienceyears)
        {
            this.Name = name;
            this.BadgeNumber = int.Parse(badgeNumber.Replace("SEC-", ""));
            this.Shift = shift;
        }

        public Security(string name, string autoBadgeString, string shiftTime)
            : base(name, 0, GenderType.Male, "0", "0", "0", "0", 0, 0)
        {
            this.Name = name;
            this.BadgeNumber = int.Parse(autoBadgeString.Replace("SEC-", ""));
            this.Shift = shiftTime;
        }

    }
}