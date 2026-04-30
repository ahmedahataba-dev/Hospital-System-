using System;

namespace Hospital_System
{
    class Outpatient : Patient
    {
        public string ClinicName { get; set; }
        public double ConsultationFee { get; set; }
        public string Complaint { get; set; } = "Not Specified";

        //public override void Display()
        //{
        //    base.Display();
        //    Console.ForegroundColor = ConsoleColor.Yellow;
        //    Console.WriteLine("      Outpatient Specific Info      ");
        //    Console.ResetColor();
        //    Console.WriteLine($"Clinic: {ClinicName} \nFee: {ConsultationFee} EGP");
        //    Console.WriteLine($"Complaint: {Complaint}");
        //    Console.WriteLine("--------------------------------------------------");
        //}
    }
}
