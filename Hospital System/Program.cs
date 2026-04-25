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
//HospitalData.ExtractPharmacists();
////PharmacyStaff p1 = new PharmacyStaff("ahmed", 19, GenderType.Male, "30701181200491", "01025920863", "ahataba@gmail.com", "komombo", 1500, 1, "pharmacist");
//HospitalData.SavePharmacists();

//foreach (var p in PharmacyStaff.pharmacists)
//{
//	Console.WriteLine($"{p.Name} is a {p.Role}");
//}
HospitalData.ExtractDoctors();
//new Doctor("Sarah Kamel", 42, GenderType.Female, "28502021234567", "01122223333", "sarah.k@neurai.com", "Giza", 28000m, 14.0, "Pulmonology", "A+", "MED-PUL-002", 600m, 20, "Room 105", Doctor.DoctorRank.Senior);
//new Doctor("Mona Samir", 55, GenderType.Female, "27011111234567", "01177778888", "mona.s@neurai.com", "Zayed", 40000m, 25.0, "Neurology", "A-", "MED-NEU-006", 1000m, 10, "Room 305", Doctor.DoctorRank.Consultant);
//HospitalData.SaveDoctors();
foreach (var d in Doctor.doctors)
{
	Console.WriteLine($"{d.Name} has no  {d.MedicalLicenseNumber}");
}
//HospitalData.ExtractEmployees ();
////Security p1 = new Security("ahmed", "19", "Day");
//HospitalData.SaveEmployees();

//foreach (var p in Employee.employees)
//{
//	Console.WriteLine($"{p.Name} salary {p.NetSalary}");
//}