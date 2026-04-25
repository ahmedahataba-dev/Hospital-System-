using System;
using System.Linq;
using static Hospital_System.program;
using static Hospital_System.HospitalEngine;

namespace Hospital_System
{
    internal class UIMission
    {
        public static void Run(HospitalEngine engine)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("     HOSPITAL MANAGEMENT SYSTEM     ");
                Console.WriteLine("\n1. Add Inpatient");
                Console.WriteLine("2. Add Outpatient");
                Console.WriteLine("3. View All Patients");
                Console.WriteLine("4. Search for Patient (Name/National ID)");
                Console.WriteLine("5. Update Patient Data");
                Console.WriteLine("6. Delete Patient");
                Console.WriteLine("7. Blood Bank System Menu");
                Console.WriteLine("8. Operating System Menu");
                Console.WriteLine("9. Exit");
                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": engine.AddInpatient(); break;
                    case "2": engine.AddOutpatient(); break;
                    case "3": engine.ViewByCategory(); break;
                    case "4": engine.Search(); break;
                    case "5": engine.UpdatePatient(); break;
                    case "6": engine.DeletePatient(); break;
                    case "7": BloodBankMenu(); break;
                    case "8": OperationMenu(engine); break;
                    case "9":
                        Console.WriteLine("Exiting... Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void BloodBankMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("BLOOD BANK MANAGEMENT");
                Console.WriteLine(" \nDonor Management");
                Console.WriteLine("  1. Register New Donor");
                Console.WriteLine("  2. View All Donors");
                Console.WriteLine("  3. Filter Donors by Blood Type");
                Console.WriteLine("  4. Record Donation");
                Console.WriteLine(" \nPatient / Transfer ");
                Console.WriteLine("   5. Process Blood Transfer");
                Console.WriteLine("   6. View Transfer Records");
                Console.WriteLine(" \nInventory");
                Console.WriteLine("   7. View Inventory");
                Console.WriteLine("   8. Withdraw Blood (Manual)");
                Console.WriteLine(" \nReports");
                Console.WriteLine("   9. Dashboard");
                Console.WriteLine("   0. Back to Main Menu");
                Console.WriteLine("-------------------------------------------");
                Console.Write("Choose: ");
                string c = Console.ReadLine();

                switch (c)
                {
                    case "1": HospitalEngine.myBank.RegisterDonor(); Console.ReadKey(); break;
                    case "2": HospitalEngine.myBank.ViewDonors(); Console.ReadKey(); break;
                    case "3":
                        string[] types = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                        Console.WriteLine("1.A+ 2.A- 3.B+ 4.B- 5.AB+ 6.AB- 7.O+ 8.O-");
                        int ch = InputHelper.ReadInt("Choice: ");
                        if (ch < 1) ch = 1; if (ch > 8) ch = 8;
                        HospitalEngine.myBank.ViewDonors(types[ch - 1]);
                        Console.ReadKey();
                        break;
                    case "4": HospitalEngine.myBank.RecordDonation(); Console.ReadKey(); break;
                    case "5": HospitalEngine.myBank.ProcessTransfer(); Console.ReadKey(); break;
                    case "6": HospitalEngine.myBank.ViewTransfers(); Console.ReadKey(); break;
                    case "7": HospitalEngine.myBank.PrintInventoryReport(); Console.ReadKey(); break;
                    case "8": Withdrawal(HospitalEngine.myBank); break;
                    case "9": HospitalEngine.myBank.ShowDashboard(); Console.ReadKey(); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("Invalid choice!"); Console.ReadKey(); break;
                }
            }
        }

        static void Withdrawal(BloodBank bank)
        {
            string type = GetBloodTypeFromMenu();
            int amount = GetValidAmount();
            bank.WithdrawBlood(type, amount);
            Console.ReadKey();
        }

        static string GetBloodTypeFromMenu()
        {
            string[] types = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            Console.WriteLine("\nSelect Type: \n1. A+ \n2. A- \n3. B+ \n4. B- \n5. AB+ \n6. AB- \n7. O+ \n8. O-");
            int choice = InputHelper.ReadInt("Choice (1-8): ");
            if (choice < 1) choice = 1;
            if (choice > 8) choice = 8;
            return types[choice - 1];
        }

        static int GetValidAmount() => InputHelper.ReadInt("Enter number of bags: ");

        public static void OperationMenu(HospitalEngine engine)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Operating System Management ---");
                Console.WriteLine("\n1. Schedule New Operation");
                Console.WriteLine("2. View Today's Schedule");
                Console.WriteLine("3. EMERGENCY CASE");
                Console.WriteLine("4. Check Rooms Availability");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddOperation(false, engine); break;
                    case "2": ShowOperations(); break;
                    case "3": AddOperation(true, engine); break;
                    case "4": CheckRooms(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice!"); break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void AddOperation(bool isEmergency, HospitalEngine engine)
        {
            Console.Clear();
            Operation op = new Operation();
            op.IsEmergency = isEmergency;

            if (isEmergency)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("!!! EMERGENCY MODE ACTIVATED !!!");
                Console.ResetColor();

                Console.Write("Patient Name (or Unknown): ");
                op.PatientName = Console.ReadLine();
                op.OperationType = "Urgent Surgery";
                op.SurgeryDate = DateTime.Now;
                op.Time = DateTime.Now;

                int freeRoom = Array.IndexOf(HospitalEngine.roomsStatus, false);
                if (freeRoom != -1)
                {
                    HospitalEngine.roomsStatus[freeRoom] = true;
                    op.RoomNumber = (freeRoom + 1).ToString();
                    Console.WriteLine($"\n SYSTEM ALERT: Room {op.RoomNumber} Reserved Immediately!");
                }
                else
                {
                    op.RoomNumber = "WAITING (All Rooms Full)";
                }
            }
            else
            {
                Console.WriteLine("--- Schedule New Operation ---");
                Console.Write("Is this patient already registered? (y/n): ");
                string registered = Console.ReadLine().ToLower();

                if (registered == "y")
                {
                    int id = InputHelper.ReadInt("Enter Patient ID: ");
                    var patient = HospitalEngine.patients.FirstOrDefault(p => p.PatientId == id);

                    if (patient != null)
                    {
                        op.PatientName = patient.Name;
                        patient.NextScheduledProcedure = op.SurgeryDate;
                        Console.WriteLine($"Patient found: {patient.Name}");
                    }
                    else
                    {
                        Console.WriteLine("ID not found! You must register the patient first.");
                        engine.AddInpatient();
                        op.PatientName = HospitalEngine.patients.Last().Name;
                    }
                }
                else
                {
                    Console.WriteLine("Registering new patient...");
                    engine.AddInpatient();
                    op.PatientName = HospitalEngine.patients.Last().Name;
                }

                Console.Write("Surgery Type: "); op.OperationType = Console.ReadLine();
                Console.Write("Surgeon Name: "); op.DoctorName = Console.ReadLine();
                Console.Write("Surgery Date: ");
                op.SurgeryDate = DateTime.Parse(Console.ReadLine());
                Console.Write("Time: ");
                op.Time = DateTime.Parse(Console.ReadLine());
                Console.Write("Assign Room (1-9): "); op.RoomNumber = Console.ReadLine();

                if (int.TryParse(op.RoomNumber, out int rNum) && rNum <= 9 && rNum > 0)
                    HospitalEngine.roomsStatus[rNum - 1] = true;
            }

            operationsList.Add(op);
            engine.SaveData(); 
            Console.WriteLine("\nDone! Data Saved Successfully.");
        }

        static void ShowOperations()
        {
            Console.Clear();
            Console.WriteLine("--- Current Surgery Schedule ---");
            Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-10}", "Patient", "Surgery", "Date", "Time", "Room");
            Console.WriteLine("-------------------------------------------------------------------------------------");

            foreach (var op in operationsList)
            {
                if (op.IsEmergency) Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-10}",
                    op.PatientName, op.OperationType,
                    op.SurgeryDate.ToShortDateString(),
                    op.Time.ToShortTimeString(),
                    op.RoomNumber);
                Console.ResetColor();
            }
        }

        static void CheckRooms()
        {
            Console.Clear();
            Console.WriteLine("--- Operating Rooms Status ---");
            for (int i = 0; i < HospitalEngine.roomsStatus.Length; i++)
            {
                string status = HospitalEngine.roomsStatus[i] ? "OCCUPIED " : "AVAILABLE ";
                Console.WriteLine($"Room #{i + 1}: {status}");
            }
        }
    }
}