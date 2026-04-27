using System;

namespace Hospital_System
{
    internal class Patient : Person
    {
        public int PatientId { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Allergies { get; set; }
        public string MedicalCase { get; set; }
        //public BloodGroup BloodType { get; set; }
        public string MedicalHistory { get; set; }
        public string FamilyHistory { get; set; }
        public string Risk { get; set; }
        public string PaymentMethods { get; set; }

        // Added for HospitalEngine integration
        public string BloodTypeStr { get; set; }
        public string PatientType { get; set; } = "Outpatient";
        public DateTime? LastSurgeryDate { get; set; }
        public DateTime? NextScheduledProcedure { get; set; }
        public string PreviousSurgeriesLog { get; set; }

        // Added by Ahmed Essam
        public VitalSigns LastRecordedVitals { get; set; } = new VitalSigns();

        public Patient() { }

        public virtual void Display()
        {
            Console.WriteLine($"\n[ID: {PatientId}] | Name: {Name} | Age: {Age} | Gender: {Gender}");
            Console.WriteLine($"National ID: {NationalId} | Phone: {PhoneNumber}");
            //Console.WriteLine($"Case: {MedicalCase} \nRisk: {Risk} \nBlood: {BloodType}");
            Console.WriteLine($"Height: {Height}m \nWeight: {Weight}kg");
            Console.WriteLine($"Allergies: {Allergies} \nPayment: {PaymentMethods}");
            Console.WriteLine($"Medical History: {MedicalHistory}");
            Console.WriteLine($"Family History: {FamilyHistory}");
        }
    }
}
