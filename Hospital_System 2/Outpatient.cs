using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Hospital_System
{
#nullable enable
    class Outpatient : Patient
    {
        public string? ClinicName { get; set; }
        public double ConsultationFee { get; set; }
        public string Complaint { get; set; } = "Not Specified";

        public override void Display()
        {
            base.Display();
            Console.WriteLine("      Outpatient Specific Info      ");
            Console.WriteLine($"Clinic: {ClinicName} \nFee: {ConsultationFee} EGP");
            Console.WriteLine($"Complaint: {Complaint}");
            Console.WriteLine("--------------------------------------------------");
        }

    }
}


