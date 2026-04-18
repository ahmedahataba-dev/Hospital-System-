using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Hospital_System
{
    internal class Patient : Person
    {
#nullable enable
        public int PatientId { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalCase { get; set; }
        public BloodGroup BloodType { get; set; }
        public string? MedicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? Risk { get; set; }
        public string? PaymentMethods { get; set; }
        public DateTime? LastSurgeryDate { get; set; } 
        public DateTime? NextScheduledProcedure { get; set; } 
        public string? PreviousSurgeriesLog { get; set; } 



        public virtual void Display()
        {
            Console.WriteLine($"\n[ID: {PatientId}] | Name: {Name} | Age: {Age} | Gender: {Gender}");
            Console.WriteLine($"National ID: {NationalId} | Phone: {PhoneNumber}");
            Console.WriteLine($"Case: {MedicalCase} \nRisk: {Risk} \nBlood: {BloodType}");
            Console.WriteLine($"Height: {Height}m \nWeight: {Weight}kg");
            Console.WriteLine($"Medical History: {MedicalHistory}");
            Console.WriteLine($"Last Surgery: {(LastSurgeryDate.HasValue ? LastSurgeryDate.Value.ToShortDateString() : "None")}");
            Console.WriteLine($"Next Scheduled: {(NextScheduledProcedure.HasValue ? NextScheduledProcedure.Value.ToShortDateString() : "None")}");
            Console.WriteLine($"Full Surgical History: {PreviousSurgeriesLog}");
            Console.WriteLine($"Family History: {FamilyHistory}");
            Console.WriteLine($"Allergies: {Allergies} \nPayment: {PaymentMethods}");

        }


    }

}



