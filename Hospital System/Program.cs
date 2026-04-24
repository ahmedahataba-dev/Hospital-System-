using Hospital_System;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
// [STAThread] // You only need this if you are running a Windows Forms UI
// ==========================================
// Ahmed Hataba's Tests (Console Environment)
// ==========================================
//Employee e1 = new Employee("Youssef", 19, GenderType.Male, "30704211200695", "01029611625", "youssef@example.com", "Mansoura", 15000, 5); e1.CheckInandOut(e1);

Console.WriteLine("\n-----------------------------------\n");

// ==========================================
// Youssef's Tests (Hospital & Rooms Environment)
// ==========================================

//Initialize the hospital
//Hospital neurai = new Hospital("NeurAi Medical Center");
//Doctor.HireInitialDoctors(neurai);
//Hospital.RunMainMenu(neurai);

////-------------------------------------------------------------------\\
// ==========================================
Console.WriteLine("Loading hospital database...");
HospitalData.ExtractEmployees();
HospitalData.ExtractDoctors();
HospitalData.ExtractNurses();
HospitalData.ExtractPharmacists();
HospitalData.ExtractSecurity();
Console.WriteLine("Databases loaded successfully!\n");
Hospital neurai = new Hospital("NeurAi Medical Center");
Doctor.HireInitialDoctors(neurai);
if(Doctor.doctors.Count == 0)
{
    List<Doctor> initialDoctors = new List<Doctor>()
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
    foreach (var doc in initialDoctors)
    {
        HospitalData.AddDoctor(doc);
    }
}
foreach (var doc in Doctor.doctors)
{
    var targetDept = neurai.ActiveDepartments.FirstOrDefault(d => d.DeptName.Equals(doc.Department, StringComparison.OrdinalIgnoreCase));

    if (targetDept != null)
    {
        if (!targetDept.Doctors.Any(d => d.MedicalLicenseNumber == doc.MedicalLicenseNumber))
        {
            targetDept.Doctors.Add(doc);
        }
    }
    else
    {
        Console.WriteLine($"[Warning] Could not find department '{doc.Department}' for Dr. {doc.Name}");
    }
}
Hospital.RunMainMenu(neurai);
Console.WriteLine("Saving all data before exit...");
HospitalData.SaveEmployees();
HospitalData.SaveDoctors();
HospitalData.SaveNurses();
HospitalData.SavePharmacists();
HospitalData.SaveSecurity();