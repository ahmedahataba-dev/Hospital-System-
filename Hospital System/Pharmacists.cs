using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Pharmacists
    {
        internal class PharmacyStaff : Employee
        {
            private static int pharmacyStaffIdCounter = 1;
            private int pharmacyStaffId;

            public int PharmacyStaffId
            {
                get { return pharmacyStaffId; }
                private set { pharmacyStaffId = value; }
            }

            public string Role { get; set; }

            public PharmacyStaff(
                string name,
                int age,
                GenderType gender,
                string nationalId,
                string phoneNumber,
               string salary,
               //string arrivalTime,
               //int departureTime,
                int experienceYears,
                string role
            )
                : base(name, age, gender, nationalId, phoneNumber, salary,/* arrivalTime, departureTime,*/ experienceYears)
            {
                PharmacyStaffId = pharmacyStaffIdCounter++;
                Role = role;
            }
        }
    }
}