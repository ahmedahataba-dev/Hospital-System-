using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Doctors : Employee
    {
        string department;//ICU(intensive care unit),Er(emergency room),Surgery, etc.
        string bloodType;//to determine the blood type of the doctor for medical purposes
        readonly string[] validTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };//blood types >_>
        public enum doctorRank { Trainee, Junior, Senior, Consultant };
        string medicalLicenseNumber;//to track the doctor's medical license information
        decimal consultationFee;//to determine the fee charged by the doctor for consultations and medical services
        int maxPatientsPerDay;//to limit the number of patients a doctor can see in a day to ensure quality care and prevent burnout
        string assignedRoom;
        public string Department
        {
            get { return department; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Department cannot be empty.");
                }
                department = value;
            }
        }
        public string BloodType
        {
            get { return bloodType; }
            set
            {
                string formattedValue = value.Trim().ToUpper();
                if (System.Linq.Enumerable.Contains(validBloodTypes, formattedValue))//LINQ:gives arrays "extra brainpower," like the ability to search, sort, or filter.
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
                {
                    throw new ArgumentException("Medical license number cannot be empty.");
                }
                medicalLicenseNumber = value;
            }
        }
        public decimal ConsultationFee
        {
            get { return consultationFee; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Consultation fee cannot be negative.");
                }
                consultationFee = value;
            }
        }
        public int MaxPatientsPerDay
        {
            get { return maxPatientsPerDay; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Max patients per day cannot be negative.");
                }
                if (value > 30)
                {
                    throw new ArgumentException("One doctor cannot see more than 30 patients per day.");
                }
                maxPatientsPerDay = value;
            }
        }
        public string AssignedRoom
        {
            get { return assignedRoom; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Assigned room cannot be empty.");
                }
                assignedRoom = value;
            }
        }
        public Doctors(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email
            , string address, decimal salary, double arrivaltime, double departuretime, double experienceyears, string department, string bloodType, string medicalLicenseNumber, decimal consultationFee, int maxPatientsPerDay, string assignedRoom)
            : base(name, age, gender, nationalId, phoneNumber, email, address, salary, arrivaltime, departuretime, experienceyears)
        {
            Department = department;
            BloodType = bloodType;
            MedicalLicenseNumber = medicalLicenseNumber;
            ConsultationFee = consultationFee;
            MaxPatientsPerDay = maxPatientsPerDay;
            AssignedRoom = assignedRoom;
        }
    }
}
