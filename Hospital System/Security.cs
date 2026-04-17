using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Security: Employee
    {
        int badgeNumber;
        String? shift; //morning, evening, night
        private string? name;

        public new string? Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be empty.");
                }
                name = value;
            }
        }

        public string? Shift
        {
            get { return shift; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Shift cannot be empty.");
                }
                shift = value;
            }
        }
        public int BadgeNumber
            {
                get { return badgeNumber; }
                set
                {
                    if (value <= 0)
                    {
                        throw new ArgumentException("Badge number must be a positive integer.");
                    }
                    badgeNumber = value;
                }
        }
       
        public Security(string name, string badgeNumber, string shift):base(name) 
        {
           this.Name = name;
            this.BadgeNumber = int.Parse(badgeNumber.Replace("SEC-", ""));
            this.Shift = shift;
        }
    }
}
