using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using static Hospital_System.PharmacyStaff;

// By Ahmed Hataba
namespace Hospital_System
{
	internal class HospitalData
	{
		private const string employeeFileName = "employees.json";
		private const string doctorFileName = "doctors.json";
		private const string nurseFileName = "nurses.json";
		private const string pharmacistsFileName = "pharmacists.json";
		private const string securityFileName = "security.json";

		// Employees Json implementation --------------------
		public static void AddEmployee(Employee newemp)
		{
			if (!Employee.employees.Any(e => e.NationalId == newemp.NationalId))
			{
				Employee.employees.Add(newemp);
				newemp.EmployeeId = Employee.employeeid_counter++;
				SaveEmployees();
			}
		}

		public static void SaveEmployees()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string serializedemployeeslist = JsonSerializer.Serialize(Employee.employees, options);
			File.WriteAllText(employeeFileName, serializedemployeeslist);
		}

		public static void ExtractEmployees()
		{
			if (File.Exists(employeeFileName))
			{
				string deserializedemployeeslist = File.ReadAllText(employeeFileName);
				Employee.employees = JsonSerializer.Deserialize<List<Employee>>(deserializedemployeeslist);

				if (Employee.employees != null && Employee.employees.Any())
				{
					int max_id = Employee.employees.Max(e => e.EmployeeId);
					Employee.employeeid_counter = max_id + 1;
				}
			}
		}

		// Doctors Json file implementation --------------------
		public static void AddDoctor(Doctor newdoc)
		{
			if (!Doctor.doctors.Any(d => d.MedicalLicenseNumber == newdoc.MedicalLicenseNumber))
			{
				Doctor.doctors.Add(newdoc);
				SaveDoctors();
			}
			else
			{
				Console.WriteLine($"Warning: Doctor With Medical License {newdoc.MedicalLicenseNumber} Already Exists.");
			}
		}

		public static void SaveDoctors()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string serializeddoctorslist = JsonSerializer.Serialize(Doctor.doctors, options);
			File.WriteAllText(doctorFileName, serializeddoctorslist);
		}

		public static void ExtractDoctors()
		{
			if (File.Exists(doctorFileName))
			{
				var deserializeddoctorslist = File.ReadAllText(doctorFileName);
				Doctor.doctors = JsonSerializer.Deserialize<List<Doctor>>(deserializeddoctorslist);
			}
		}

		// Nurses json file ------------------------------------
		public static void AddNurse(Nurse newnurse)
		{
			if (!Nurse.nurses.Any(n => n.LicenseNumber == newnurse.LicenseNumber))
			{
				Nurse.nurses.Add(newnurse);
				SaveNurses();
			}
			else
			{
				Console.WriteLine($"Warning: Nurse With Medical License {newnurse.LicenseNumber} Already Exists.");
			}
		}

		public static void SaveNurses()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string serializednurseslist = JsonSerializer.Serialize(Nurse.nurses, options);
			File.WriteAllText(nurseFileName, serializednurseslist);
		}

		public static void ExtractNurses()
		{
			if (File.Exists(nurseFileName))
			{
				var deserializednurseslist = File.ReadAllText(nurseFileName);
				Nurse.nurses = JsonSerializer.Deserialize<List<Nurse>>(deserializednurseslist);
			}
		}

		// Pharmacists json files ---------------------
		public static void SavePharmacists()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string serializedpharmacistslist = JsonSerializer.Serialize(PharmacyStaff.pharmacists, options);
			File.WriteAllText(pharmacistsFileName, serializedpharmacistslist);
		}

		public static void ExtractPharmacists()
		{
			if (File.Exists(pharmacistsFileName))
			{
				var deserializedpharmacistslist = File.ReadAllText(pharmacistsFileName);
				PharmacyStaff.pharmacists = JsonSerializer.Deserialize<List<PharmacyStaff>>(deserializedpharmacistslist);
				if (PharmacyStaff.pharmacists != null && PharmacyStaff.pharmacists.Any())
				{
					int max_id = PharmacyStaff.pharmacists.Max(p => p.PharmacyStaffId);
					PharmacyStaff.pharmacyStaffIdCounter = max_id + 1;
				}
			}
		}

		public static void AddPharmacists(PharmacyStaff newpharmacist)
		{
			if (!PharmacyStaff.pharmacists.Any(p => p.NationalId == newpharmacist.NationalId))
			{
				PharmacyStaff.pharmacists.Add(newpharmacist);
				SavePharmacists();
			}
			else
			{
				Console.WriteLine($"Warning: Pharmacist With National ID: {newpharmacist.NationalId} Already Exists.");
			}
		}

		// Security db implementation --------------------------
		public static void SaveSecurity()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string serializedsecuritylist = JsonSerializer.Serialize(Security.securities, options);
			File.WriteAllText(securityFileName, serializedsecuritylist);
		}

		public static void ExtractSecurity()
		{
			if (File.Exists(securityFileName))
			{
				var deserializedsecuritylist = File.ReadAllText(securityFileName);
				Security.securities = JsonSerializer.Deserialize<List<Security>>(deserializedsecuritylist);
			}
		}

		public static void AddSecurity(Security newsecurity)
		{
			if (!Security.securities.Any(p => p.NationalId == newsecurity.NationalId))
			{
				Security.securities.Add(newsecurity);
				SaveSecurity();
			}
			else
			{
				Console.WriteLine($"Warning: Security Guy With Badge Number: {newsecurity.BadgeNumber} Already Exists.");
			}
		}
	}
}