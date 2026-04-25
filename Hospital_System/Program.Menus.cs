using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hospital_System
{
    public partial class program
    {
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
                    case "1": myBank.RegisterDonor(); Console.ReadKey(); break;
                    case "2": myBank.ViewDonors(); Console.ReadKey(); break;
                    case "3":
                        string[] types = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                        Console.WriteLine("1.A+ 2.A- 3.B+ 4.B- 5.AB+ 6.AB- 7.O+ 8.O-");
                        int ch = InputHelper.ReadInt("Choice: ");
                        if (ch < 1) ch = 1; if (ch > 8) ch = 8;
                        myBank.ViewDonors(types[ch - 1]);
                        Console.ReadKey();
                        break;
                    case "4": myBank.RecordDonation(); Console.ReadKey(); break;
                    case "5": myBank.ProcessTransfer(); Console.ReadKey(); break;
                    case "6": myBank.ViewTransfers(); Console.ReadKey(); break;
                    case "7": myBank.PrintInventoryReport(); Console.ReadKey(); break;
                    case "8": Withdrawal(myBank); break;
                    case "9": myBank.ShowDashboard(); Console.ReadKey(); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("Invalid choice!"); Console.ReadKey(); break;
                }
            }
        }
      
        // سحب الدم
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

        static void OperationMenu()
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
                    case "1": AddOperation(false); break;
                    case "2": ShowOperations(); break;
                    case "3": AddOperation(true); break;
                    case "4": CheckRooms(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice!"); break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void AddOperation(bool isEmergency)
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

                int freeRoom = Array.IndexOf(roomsStatus, false);
                if (freeRoom != -1)
                {
                    roomsStatus[freeRoom] = true;
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
                    var patient = patients.FirstOrDefault(p => p.PatientId == id);

                    if (patient != null)
                    {
                        op.PatientName = patient.Name;
                        patient.NextScheduledProcedure = op.SurgeryDate;
                        Console.WriteLine($"Patient found: {patient.Name}");
                    }
                    else
                    {
                        Console.WriteLine("ID not found! You must register the patient first.");
                        AddInpatient();
                        op.PatientName = patients.Last().Name;
                    }
                }
                else
                {
                    Console.WriteLine("Registering new patient...");
                    AddInpatient();
                    op.PatientName = patients.Last().Name;
                }
                Console.Write("Surgery Type: "); op.OperationType = Console.ReadLine();
                Console.Write("Surgeon Name: "); op.DoctorName = Console.ReadLine();
                Console.Write("Surgery Date: ");
                op.SurgeryDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Time: ");
                op.Time = DateTime.Parse(Console.ReadLine());
                Console.Write("Assign Room (1-9): "); op.RoomNumber = Console.ReadLine();


                if (int.TryParse(op.RoomNumber, out int rNum) && rNum <= 5 && rNum > 0)
                    roomsStatus[rNum - 1] = true;
            }

            operationsList.Add(op);
            SaveData();
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

                Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-10}", op.PatientName, op.OperationType, op.SurgeryDate.ToShortDateString(), op.Time.ToShortTimeString(), op.RoomNumber);

                Console.ResetColor();
            }
        }

        static void CheckRooms()
        {
            Console.Clear();
            Console.WriteLine("--- Operating Rooms Status ---");
            for (int i = 0; i < roomsStatus.Length; i++)
            {

                string status = roomsStatus[i] ? "OCCUPIED " : "AVAILABLE ";
                Console.WriteLine($"Room #{i + 1}: {status}");
            }
        }



    }
}


