using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// By Ahmed Hataba
namespace Hospital_System
{
    internal class HospitalData
    {


        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

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
            string json = JsonSerializer.Serialize(Employee.employees, JsonOptions);
            File.WriteAllText(DataStore.Employees, json);
        }

        public static void ExtractEmployees()
        {
            if (File.Exists(DataStore.Employees))
            {
                string json = File.ReadAllText(DataStore.Employees);
                Employee.employees = JsonSerializer.Deserialize<List<Employee>>(json, JsonOptions)
                                     ?? new List<Employee>();
                if (Employee.employees.Any())
                    Employee.employeeid_counter = Employee.employees.Max(e => e.EmployeeId) + 1;
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
        }

        public static void SaveDoctors()
        {
            string json = JsonSerializer.Serialize(Doctor.doctors, JsonOptions);
            File.WriteAllText(DataStore.Doctors, json);
        }

        public static void ExtractDoctors()
        {
            if (File.Exists(DataStore.Doctors))
            {
                string json = File.ReadAllText(DataStore.Doctors);
                Doctor.doctors = JsonSerializer.Deserialize<List<Doctor>>(json, JsonOptions)
                                 ?? new List<Doctor>();
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
            string json = JsonSerializer.Serialize(Nurse.nurses, JsonOptions);
            File.WriteAllText(DataStore.Nurses, json);
        }

        public static void ExtractNurses()
        {
            if (File.Exists(DataStore.Nurses))
            {
                string json = File.ReadAllText(DataStore.Nurses);
                Nurse.nurses = JsonSerializer.Deserialize<List<Nurse>>(json, JsonOptions)
                               ?? new List<Nurse>();
            }
        }

        // Pharmacists json files ---------------------
        public static void SavePharmacists()
        {
            string json = JsonSerializer.Serialize(PharmacyStaff.pharmacists, JsonOptions);
            File.WriteAllText(DataStore.Pharmacists, json);
        }

        public static void ExtractPharmacists()
        {
            if (File.Exists(DataStore.Pharmacists))
            {
                string json = File.ReadAllText(DataStore.Pharmacists);
                PharmacyStaff.pharmacists = JsonSerializer.Deserialize<List<PharmacyStaff>>(json, JsonOptions)
                                            ?? new List<PharmacyStaff>();
                if (PharmacyStaff.pharmacists.Any())
                    PharmacyStaff.pharmacyStaffIdCounter = PharmacyStaff.pharmacists.Max(p => p.PharmacyStaffId) + 1;
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
            string json = JsonSerializer.Serialize(Security.securities, JsonOptions);
            File.WriteAllText(DataStore.SecurityStaff, json);
        }

        public static void ExtractSecurity()
        {
            if (File.Exists(DataStore.SecurityStaff))
            {
                string json = File.ReadAllText(DataStore.SecurityStaff);
                Security.securities = JsonSerializer.Deserialize<List<Security>>(json, JsonOptions)
                                      ?? new List<Security>();
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
