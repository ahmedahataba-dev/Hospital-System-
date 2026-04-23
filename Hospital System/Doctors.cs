using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Doctor : Employee
    {
        
        private readonly string[] validBloodTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

        
        private string bloodType = string.Empty;
        private string department = string.Empty;
        private string medicalLicenseNumber = string.Empty;
        private string assignedRoom = string.Empty;
        private decimal consultationFee;
        private int maxPatientsPerDay;

        
        public List<string> CurrentPatients { get; set; } = new List<string>();

        
        public enum DoctorRank { Trainee, Junior, Senior, Consultant };
        public DoctorRank Rank { get; set; }

        public string Department
        {
            get => department;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Department cannot be empty.");
                department = value;
            }
        }

        public string BloodType
        {
            get => bloodType;
            set
            {
                string formattedValue = value.Trim().ToUpper();
                if (!validBloodTypes.Contains(formattedValue))
                    throw new ArgumentException("Invalid blood type.");
                bloodType = formattedValue;
            }
        }

        public string MedicalLicenseNumber 
        {
            get {  return medicalLicenseNumber; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Medical license number cannot be empty.");
                medicalLicenseNumber = value;
            }

        }
        public decimal ConsultationFee 
        {
            get { return consultationFee; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Invalid Consultation Fee.");
                consultationFee = value;
            }
        }
        public int MaxPatientsPerDay
        {
            get { return maxPatientsPerDay; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Invalid Max Patients Per Day .");
                maxPatientsPerDay = value;
            }
        }
        public string AssignedRoom
        {
            get { return assignedRoom; }
            set
            {
                assignedRoom = value;
            }
        }

        public Doctor(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email,
                      string address, decimal salary, /*double arrivaltime, double departuretime,*/ double experienceyears,
                      string department, string bloodType, string medicalLicenseNumber, decimal consultationFee,
                      int maxPatientsPerDay, string assignedRoom, DoctorRank rank)
            : base(name, age, gender, nationalId, phoneNumber, email, address, salary,/*, arrivaltime, departuretime,*/ experienceyears)
        {
            Department = department;
           BloodType = bloodType;
            MedicalLicenseNumber = medicalLicenseNumber;
            ConsultationFee = consultationFee;
            MaxPatientsPerDay = maxPatientsPerDay;
            AssignedRoom = assignedRoom;
            Rank = rank;
        }

        public bool AcceptPatient(string patientName)
        {
            if (CurrentPatients.Count >= MaxPatientsPerDay)
            {
                Console.WriteLine($"Dr. {Name} is at full capacity!");
                return false;
            }

            CurrentPatients.Add(patientName);
            Console.WriteLine($"Patient {patientName} assigned to Dr. {Name}.");
            return true;
        }
        public string GetProfessionalProfile()
        {
            return $"Dr. {Name} ({Rank})\nDept: {Department}\nExperience: {ExperienceYears} years\nFee: {ConsultationFee:C}";
        }
    }
}