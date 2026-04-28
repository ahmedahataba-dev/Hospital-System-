using System;

namespace Hospital_System
{
    public class BloodTransfer
    {
        public int TransferId { get; set; } //رقم عملية النقل
        public string DonorName { get; set; } //اسم المتبرع
        public string PatientName { get; set; } //اسم المريض اللي بيتبرعله
        public string BloodType { get; set; } //فصيلة الدم
        public int Bags { get; set; } //عدد اكياس الدم
        public DateTime Date { get; set; } //وقت حصول التبرع
        public string Status { get; set; } 

        public void Display()
        {
            Console.WriteLine($"\n[Transfer #{TransferId}] {Date.ToShortDateString()}");
            Console.WriteLine($"From Donor : {DonorName}");
            Console.WriteLine($"To Patient : {PatientName}");
            Console.WriteLine($"Blood Type : {BloodType} | Bags: {Bags}");
            Console.WriteLine($"Status     : {Status}");
            Console.WriteLine("--------------------------------------------------");
        }
    }
}