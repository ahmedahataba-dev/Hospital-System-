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
        
        internal static void RunDoctorMenu(Hospital neurai )
        {

            bool doctorMenu = true;
            while (doctorMenu)
            {
                Console.WriteLine("1: View All Doctors by Department");
                Console.WriteLine("2: Assign a Doctor to a Room");
                Console.WriteLine("3: Hire a New Doctor (Set Rank & Dept)");
                Console.WriteLine("0: Return to Main Menu");

                string? docChoice = Console.ReadLine();

                if (docChoice == "1")
                {
                    Console.WriteLine("\n--- Hospital Roster ---");
                    foreach (var dept in neurai.ActiveDepartments)
                    {
                        Console.WriteLine($"\n>> {dept.DeptName} <<");
                        if (dept.Doctors.Count == 0) Console.WriteLine("  No doctors assigned yet.");

                        foreach (var doc in dept.Doctors)
                        {
                            Console.WriteLine($"  Dr. {doc.Name} | Rank: {doc.Rank} | Room: {doc.AssignedRoom}");
                        }
                    }
                }
                else if (docChoice == "2")
                {
                    Console.Write("\nEnter the exact name of the Doctor (e.g., Ahmed Hassan): ");
                    string? docName = Console.ReadLine();

                    Doctor? foundDoctor = null;
                    Department? foundDept = null;
                    foreach (var dept in neurai.ActiveDepartments)
                    {
                        foundDoctor = dept.Doctors.Find(d => d.Name.Equals(docName, StringComparison.OrdinalIgnoreCase));
                        if (foundDoctor != null)
                        {
                            foundDept = dept;
                            break;
                        }
                    }

                    if (foundDoctor == null || foundDept == null)
                    {
                        Console.WriteLine("\n[!] Doctor not found in the hospital registry.");
                        continue;
                    }

                    Console.WriteLine($"\n>>> Dr. {foundDoctor.Name} is registered to the {foundDept.DeptName} department.");

                    Console.WriteLine("Available rooms in this department:");
                    foreach (var Room in foundDept.Rooms)
                    {
                        Console.Write($"[{Room.RoomNumber}");
                    }

                    Console.Write("\n\nEnter Room Number from the list above to assign: ");
                    if (int.TryParse(Console.ReadLine(), out int roomNum))
                    {
                        var targetRoom = foundDept.Rooms.Find(r => r.RoomNumber == roomNum);

                        if (targetRoom != null)
                        {
                            foundDoctor.AssignedRoom = targetRoom.RoomNumber.ToString();
                            Console.WriteLine($"\n[+] SUCCESS: Dr. {foundDoctor.Name} has been securely assigned to Room {targetRoom.RoomNumber}.");
                        }
                        else
                        {  
                            Console.WriteLine($"\n[!] ERROR: Room {roomNum} does not belong to the {foundDept.DeptName} department!");
                            Console.WriteLine("Assignment blocked. Doctors can only be assigned to their own department's rooms.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n[!] Invalid room number format.");
                    }
                   
                }
                else if (docChoice == "3")
                {
                    Console.WriteLine("\n=== HIRE A NEW DOCTOR ===");
                    Console.Write("Enter Doctor's Name (e.g., Ahmed Mohammed): ");
                    string? rawName = Console.ReadLine();
                    string name = "Unknown";

                    if (!string.IsNullOrWhiteSpace(rawName))
                    {
                        name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawName.Trim().ToLower());//check language and culture for name formatting
                    }
                    bool isDuplicate = false;
                    foreach (var dept in neurai.ActiveDepartments)
                    {
                        if (dept.Doctors.Exists(d =>
                            d.Name.Replace(" ", "").Equals(name.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (isDuplicate)
                    {
                        Console.WriteLine($"\n[!] HIRING BLOCKED: A doctor named Dr. {name} already works at the hospital.");
                        continue; 
                    }
                    // ==========================================
                    Console.WriteLine("\nAvailable Departments: Cardiology, Pulmonology, General Medicine, Orthopedics, Internal Medicine, Neurology, Ophthalmology, Physical Medicine, Otolaryngology (ENT), Dermatology, General Surgery");
                    Console.Write("Enter Exact Department Name: ");
                    string? deptName = Console.ReadLine();

                    Console.Write("Enter Blood Type (e.g., A+, O-): ");
                    string? rawBloodType = Console.ReadLine();
                    string bloodType = "";
                    if (!string.IsNullOrWhiteSpace(rawBloodType))
                    {
                        bloodType = rawBloodType.Trim().ToUpper();
                    }

                    // 3. RANK SELECTION
                    Console.WriteLine("\nSelect Doctor Rank:");
                    Console.WriteLine("1: Trainee");
                    Console.WriteLine("2: Junior");
                    Console.WriteLine("3: Senior");
                    Console.WriteLine("4: Consultant");
                    Console.Write("Selection: ");
                    string? rankChoice = Console.ReadLine();

                    Doctor.DoctorRank selectedRank = Doctor.DoctorRank.Trainee;
                    if (rankChoice == "2") selectedRank = Doctor.DoctorRank.Junior;
                    else if (rankChoice == "3") selectedRank = Doctor.DoctorRank.Senior;
                    else if (rankChoice == "4") selectedRank = Doctor.DoctorRank.Consultant;

                    // 4. CREATION & HIRING
                    try
                    {
                        Doctor newDoctor = new Doctor(
                            name, 30, GenderType.Male, "29999999999999", "01000000000",
                            "new.doc@neurai.com", "Unassigned", 15000m, 1.0, deptName??"Unassigned",
                            bloodType, "MED-NEW-999", 300m, 20, "Unassigned Room", selectedRank
                        );

                        Department? targetDept = neurai.ActiveDepartments.Find(d =>
                            d.DeptName.Trim().Equals(deptName?.Trim() ?? "Unassigned", StringComparison.OrdinalIgnoreCase));

                        if (targetDept != null)
                        {
                            targetDept.Doctors.Add(newDoctor);
                            Console.WriteLine($"\n[+] SUCCESS: Dr. {newDoctor.Name} has been officially hired as a {selectedRank} in {targetDept.DeptName}!");
                        }
                        else
                        {
                            Console.WriteLine($"\n[!] ERROR: Department '{deptName}' does not exist.");
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"\n[!] HIRING FAILED: {ex.Message}");
                    }
                }


                else if (docChoice == "0")
                {
                    doctorMenu = false;
                    Console.WriteLine("\nLogging out of Doctor Terminal...");
                }
                else
                {
                    Console.WriteLine("\n[!] Invalid selection. Please try again.");
                }
            }
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
                Department? targetDept = neurai.ActiveDepartments.Find(d =>
                    d.DeptName.Equals(doc.Department, StringComparison.OrdinalIgnoreCase));

                if (targetDept != null)
                {
                    targetDept.Doctors.Add(doc);
                }
            }
        }
    }
}