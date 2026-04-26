using NeurAI_Hospital;
using System;
using System.Linq;

namespace Hospital_System
{
    class Program
    {
        static HospitalService hospitalService = new HospitalService();
        static ClinicService clinicService = new ClinicService();

        static void Main(string[] args)
        {
            hospitalService.LoadData();
            ShowMainMenu();
        }

        
        //                  MAIN MENU
        

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  ╔═══════════════════════════════════════════════════╗");
                Console.WriteLine("  ║           Welcome to NeurAI Hospital              ║");
                Console.WriteLine("  ╚═══════════════════════════════════════════════════╝");
                Console.ResetColor();
                Skip();
                Console.WriteLine("  [1] Staff Login (Employees)");
                Console.WriteLine("  [2] Patient Portal (Users)");
                Console.WriteLine("  [3] Hospital Info");
                Console.WriteLine("  [4] Exit");
                Skip();
                Console.Write("  Your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": StaffLogin(); break;
                    case "2": PatientPortal(); break;
                    case "3": ShowHospitalInfo(); break;
                    case "4": Environment.Exit(0); break;
                    default: ShowError("Invalid choice."); Pause(); break;
                }
            }
        }

        
        //                  STAFF SECTION

        static void StaffLogin()
        {
            Console.Clear();
            Console.WriteLine("--- STAFF LOGIN ---");
            Console.Write("Enter Staff ID: ");
            string sid = Console.ReadLine();

            if (sid == "123") StaffDashboard();
            else { ShowError("Access Denied. Invalid Staff ID."); Pause(); }
        }

        static void StaffDashboard()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"--- STAFF DASHBOARD | Today: {DateTime.Now:dd/MM/yyyy} ---");
                Console.WriteLine("1. View All Bookings\n2. Search by Code/ID\n3. Logout");
                string c = Console.ReadLine();

                if (c == "1")
                {
                    Console.WriteLine("\nAll Records:");
                    foreach (var b in hospitalService.AllBookings)
                        Console.WriteLine(
                            $"[At: {b.BookingDateTime}] ID: {b.PatientID} | " +
                            $"Clinic: {b.ClinicName} | Visit: {b.Day} | Code: {b.Code}");
                    Pause();
                }
                else if (c == "2")
                {
                    Console.Write("Enter Code or Patient ID to Search: ");
                    string query = Console.ReadLine();
                    var results = hospitalService.SearchBookings(query);

                    if (!results.Any()) Console.WriteLine("No results found.");
                    foreach (var r in results)
                        Console.WriteLine(
                            $"Found: {r.ClinicName} - Visit: {r.Day} - Booked on: {r.BookingDateTime}");
                    Pause();
                }
                else if (c == "3") break;
            }
        }

        // 
        //                 PATIENT SECTION
        // 

        static void PatientPortal()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- PATIENT PORTAL ---");
                Console.WriteLine("1. Login\n2. New Registration\n3. Back");
                string c = Console.ReadLine();

                if (c == "1") ShowLogin();
                else if (c == "2") HandleSignUpProcess();
                else if (c == "3") break;
            }
        }

        static void HandleSignUpProcess()
        {
            Console.Clear();
            Console.WriteLine("--- NEW REGISTRATION (Type '0' to cancel) ---");

            Console.Write("Full Name: "); string name = Console.ReadLine(); if (name == "0") return;
            Console.Write("National ID (14 digits): "); string id = Console.ReadLine(); if (id == "0") return;
            Console.Write("Phone (11 digits): "); string phone = Console.ReadLine(); if (phone == "0") return;
            Console.Write("Address: "); string addr = Console.ReadLine(); if (addr == "0") return;

            string error = hospitalService.CheckSignUp(name, id, phone, addr);
            if (error != "") { ShowError(error); Pause(); return; }

            var newPatient = new OnlineRegistration
            {
                NameEnglish = name,
                NationalID = id,
                Phone = phone,
                Address = addr
            };

            Console.Write("Create Password (min 8 chars + number): "); string p1 = Console.ReadLine();
            Console.Write("Confirm Password: "); string p2 = Console.ReadLine();

            string passError = hospitalService.SetPassword(newPatient, p1, p2);
            if (passError != "") { ShowError(passError); Pause(); return; }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nRegistration Successful! You can login now.");
            Console.ResetColor();
            Pause();
        }

        static void ShowLogin()
        {
            Console.Clear();
            Console.WriteLine("--- LOGIN ---");
            Console.Write("National ID: "); string id = Console.ReadLine();
            Console.Write("Password: "); string pass = Console.ReadLine();

            var patient = hospitalService.FindPatient(id, pass);
            if (patient != null) PatientDashboard(patient);
            else { ShowError("Invalid ID or Password."); Pause(); }
        }

        static void PatientDashboard(OnlineRegistration patient)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {patient.NameEnglish} | Today: {DateTime.Now:dd/MM/yyyy}");
                Console.WriteLine("1. Book Appointment\n2. My Bookings\n3. Logout");
                string c = Console.ReadLine();

                if (c == "1")
                {
                    HandleBooking(patient);
                }
                else if (c == "2")
                {
                    Console.WriteLine("\nYour Appointments:");
                    var myBookings = hospitalService.GetPatientBookings(patient.NationalID);

                    if (!myBookings.Any()) Console.WriteLine("You have no bookings.");
                    foreach (var b in myBookings)
                        Console.WriteLine(
                            $"- {b.ClinicName} on {b.Day} [Code: {b.Code}] (Booked at: {b.BookingDateTime})");
                    Pause();
                }
                else if (c == "3") break;
            }
        }

        static void HandleBooking(OnlineRegistration patient)
        {
            Console.Clear();
            Console.WriteLine("--- SELECT CLINIC (Type '0' to cancel) ---");

            foreach (var kv in clinicService.Clinics)
                Console.WriteLine($"{kv.Key}. {kv.Value.NameEnglish}");

            string input = Console.ReadLine();
            if (input == "0") return;

            if (!int.TryParse(input, out int idx) || !clinicService.Clinics.ContainsKey(idx))
            {
                ShowError("Invalid Choice.");
                Pause();
                return;
            }

            Clinic chosen = clinicService.Clinics[idx];
            Console.WriteLine("\nAvailable Days: " + string.Join(", ", chosen.AvailableDays));
            Console.Write("Choose Day for visit: ");
            string day = Console.ReadLine();

            Booking booking = hospitalService.CreateBooking(patient.NationalID, chosen, day);

            Console.WriteLine($"\nConfirmed! Code: {booking.Code}");
            Console.WriteLine($"Time of Booking: {booking.BookingDateTime}");
            Pause();
        }

    
        //               HOSPITAL INFO
        
        static void ShowHospitalInfo()
        {
            Console.Clear();
            Console.WriteLine("--- NEURAI HOSPITAL INFO & CLINIC SCHEDULE ---");
            foreach (var kv in clinicService.Clinics)
                Console.WriteLine(
                    $"{kv.Value.NameEnglish.PadRight(20)} | Days: {string.Join(", ", kv.Value.AvailableDays)}");
            Pause();
        }

        
        //                   HELPERS

        static void Skip() => Console.WriteLine();
        static void Pause() { Console.WriteLine("\nPress Enter to continue..."); Console.ReadLine(); }
        static void ShowError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + msg);
            Console.ResetColor();
        }
    }
}
