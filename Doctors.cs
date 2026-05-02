using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
        //
        //ahmed
        [JsonIgnore]
        public static List<Doctor> doctors = new List<Doctor>();

        // Parameterless constructor required for System.Text.Json deserialization
        public Doctor() : base() { }
        //
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
            HospitalData.AddDoctor(this);
        }

        public bool AcceptPatient(string patientName)
        {
            if (CurrentPatients.Count >= MaxPatientsPerDay)
            {
                // Console.WriteLine($"Dr. {Name} is at full capacity!");
                return false;
            }

            CurrentPatients.Add(patientName);
            // Console.WriteLine($"Patient {patientName} assigned to Dr. {Name}.");
            return true;
        }
        public string GetProfessionalProfile()
        {
            return $"Dr. {Name} ({Rank})\nDept: {Department}\nExperience: {ExperienceYears} years\nFee: {ConsultationFee:C}";
        }

        internal static void RunDoctorMenu(Hospital neurai)
        {
            // This console menu is disabled in the Windows Forms application.
            // All doctor management is handled through the MainForm UI.
        }

        internal static void HireInitialDoctors(Hospital neurai)
        {
            List<Doctor> hospitalDoctors = new List<Doctor>//some doctors 
    {
        new Doctor("Ahmed Hassan", 50, GenderType.Male, "29010101234567", "01011112222", "ahmed.h@neurai.com", "Cairo", 35000m, 20.5, "Cardiology", "O+", "MED-CAR-001", 800m, 15, "Room 101", Doctor.DoctorRank.Consultant),
        new Doctor("Sarah Kamel", 42, GenderType.Female, "28502021234567", "01122223333", "sarah.k@neurai.com", "Giza", 28000m, 14.0, "Pulmonology", "A+", "MED-PUL-002", 600m, 20, "Room 105", Doctor.DoctorRank.Senior),
        new Doctor("Omar Youssef", 35, GenderType.Male, "29505051234567", "01233334444", "omar.y@neurai.com", "Alexandria", 18000m, 8.0, "General Medicine", "B+", "MED-GEN-003", 300m, 30, "Room 201", Doctor.DoctorRank.Junior),
        new Doctor("Khaled Tarek", 48, GenderType.Male, "27808081234567", "01555556666", "khaled.t@neurai.com", "Cairo", 32000m, 18.0, "Orthopedics", "O-", "MED-ORT-004", 750m, 12, "Room 205", Doctor.DoctorRank.Consultant),
        new Doctor("Laila Mahmoud", 40, GenderType.Female, "28804041234567", "01066667777", "laila.m@neurai.com", "Maadi", 26000m, 12.5, "Internal Medicine", "AB+", "MED-INT-005", 550m, 25, "Room 301", Doctor.DoctorRank.Senior),
        new Doctor("Mona Samir", 55, GenderType.Female, "27011111234567", "01177778888", "mona.s@neurai.com", "Zayed", 40000m, 25.0, "Neurology", "A-", "MED-NEU-006", 1000m, 10, "Room 305", Doctor.DoctorRank.Consultant),
        new Doctor("Tarek Zaki", 38, GenderType.Male, "29012121234567", "01288889999", "tarek.z@neurai.com", "Nasr City", 22000m, 10.0, "Ophthalmology", "O+", "MED-OPH-007", 450m, 22, "Room 401", Doctor.DoctorRank.Senior),
        new Doctor("Rania Farid", 33, GenderType.Female, "29509091234567", "01599990000", "rania.f@neurai.com", "Dokki", 15000m, 5.0, "Physical Medicine", "B-", "MED-PHY-008", 350m, 18, "Room 405", Doctor.DoctorRank.Junior),
        new Doctor("Hassan Ali", 45, GenderType.Male, "28003031234567", "01012341234", "hassan.a@neurai.com", "Heliopolis", 29000m, 16.0, "Otolaryngology (ENT)", "A+", "MED-ENT-009", 600m, 25, "Room 501", Doctor.DoctorRank.Consultant),
        new Doctor("Nada Ibrahim", 31, GenderType.Female, "29807071234567", "01123452345", "nada.i@neurai.com", "Shorouk", 12000m, 2.5, "Dermatology", "O+", "MED-DER-010", 400m, 30, "Room 505", Doctor.DoctorRank.Trainee),
        new Doctor("Youssef Essam", 47, GenderType.Male, "27810101234567", "01234563456", "youssef.e@neurai.com", "Cairo", 45000m, 22.0, "General Surgery", "AB-", "MED-SUR-011", 1200m, 8, "Operating Theater 1", Doctor.DoctorRank.Consultant)
    };
            foreach (var doc in hospitalDoctors)
            {
                Department targetDept = neurai.ActiveDepartments.Find(d =>
                    d.DeptName.Equals(doc.Department, StringComparison.OrdinalIgnoreCase));

                if (targetDept != null)
                {
                    targetDept.Doctors.Add(doc);
                }
            }
        }
    }
}