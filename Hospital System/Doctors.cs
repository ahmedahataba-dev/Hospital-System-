using System;
using System.Collections.Generic;
using System.Text;
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

        public enum DoctorRank { Trainee, Junior, Senior, Consultant };
        private decimal consultationFee;
        private int maxPatientsPerDay;
        public string Department
        {
            get { return department; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Department cannot be empty.");
                department = value;
            }
        }

        public string BloodType
        {
            get { return bloodType; }
            set
            {
                // We only need ONE version of this property!
                string formattedValue = value.Trim().ToUpper();
                if (validBloodTypes.Contains(formattedValue))
                {
                    bloodType = formattedValue;
                }
                else
                {
                    throw new ArgumentException("Invalid blood type. Valid types are: " + string.Join(", ", validBloodTypes));
                }
            }
        }

        public string MedicalLicenseNumber
        {
            get { return medicalLicenseNumber; }
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
                if (value < 0)
                    throw new ArgumentException("Consultation fee cannot be negative.");
                consultationFee = value;
            }
        }

        public int MaxPatientsPerDay
        {
            get { return maxPatientsPerDay; }
            set
            {
                if (value < 0 || value > 30)
                    throw new ArgumentException("Max patients per day must be between 0 and 30.");
                maxPatientsPerDay = value;
            }
        }

        public string AssignedRoom
        {
            get { return assignedRoom; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Assigned room cannot be empty.");
                assignedRoom = value;
            }
        }

    

        public Doctor(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email,
                      string address, decimal salary, double arrivaltime, double departuretime, double experienceyears,
                      string department, string bloodType, string medicalLicenseNumber, decimal consultationFee,
                      int maxPatientsPerDay, string assignedRoom)
            : base(name, age, gender, nationalId, phoneNumber, email, address, salary, arrivaltime, departuretime, experienceyears)
        {
            this.Department = department;
            this.BloodType = bloodType;
            this.MedicalLicenseNumber = medicalLicenseNumber;
            this.ConsultationFee = consultationFee;
            this.MaxPatientsPerDay = maxPatientsPerDay;
            this.AssignedRoom = assignedRoom;
        }
    }
}