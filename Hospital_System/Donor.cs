using System;
using System.Collections.Generic;

namespace Hospital_System
{
    public class Donor
    {
        public int DonorId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string BloodType { get; set; }
        public DateTime LastDonationDate { get; set; } //اخر مره اتبرع فيها امتى
        public List<DonationLog> DonationHistory { get; set; } // فيها تاريخ التبرع بتاع الشخص 

        public Donor()
        {
            DonationHistory = new List<DonationLog>();
        }

        public void Display()
        {
            Console.WriteLine($"\n[Donor ID: {DonorId}] {Name} | Age: {Age} | Blood: {BloodType} | Phone: {PhoneNumber}");
            Console.WriteLine($"Last Donation: {LastDonationDate.ToShortDateString()}");
            Console.WriteLine($"Total Donations: {DonationHistory.Count}"); //بيشوف الليسته فيها كم مره هو اتبرع فيها وبيحسبهم 
        }
    }

    public class DonationLog 
    {
        public DateTime Date { get; set; }
        public int Bags { get; set; }
        public string BloodType { get; set; }
    }
}