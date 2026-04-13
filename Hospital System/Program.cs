using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital_System
{
	class Program
	{
		static List<Patient> patients = new List<Patient>();

		static void Main(string[] args)
		{
			while (true)
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("      WELCOME TO HOSPITAL MANAGEMENT      ");
				Console.ResetColor();
				Console.WriteLine("1. Add Inpatient ");
				Console.WriteLine("2. Add Outpatient ");
				Console.WriteLine("3. View All Patients");
				Console.WriteLine("4. Search by ID");
				Console.WriteLine("5. Exit");
				Console.Write("\nChoose an option: ");

				string choice = Console.ReadLine();
				switch (choice)
				{
					case "1": AddInpatient(); break;
					case "2": AddOutpatient(); break;
					case "3": ViewByCategory(); break;
					case "4": SearchByCategory(); break;
					case "5": return;
					default:
						Console.WriteLine("Invalid choice! Press any key...");
						Console.ReadKey();
						break;
				}
			}
		}

		// Logic for adding Inpatients
		static void AddInpatient()
		{
			Console.WriteLine("\n--- Add New Inpatient ---");
			string name = ReadLettersOnly("Enter Name: ");
			int age = ReadInt("Enter Age: ");
			GenderType gender = ReadGender();
			string nId = ReadString("Enter National ID: ");
			string phone = ReadString("Enter Phone Number: ");

			Inpatient p = new Inpatient(name, age, gender, nId, phone);
			FillPatientDetails(p);
			p.WardName = ReadString("Ward Name: ");
			p.RoomNumber = ReadInt("Room Number: ");
			p.AttendingPhysician = ReadString("Attending Physician: ");
			p.HasOperations = ReadBool("Does the patient have surgery?");
			if (p.HasOperations) p.OperationType = ReadString("Operation Type: ");

			patients.Add(p);
			Console.WriteLine("\nSaved successfully!");
			Console.ReadKey();
		}

		// Logic for adding Outpatients
		static void AddOutpatient()
		{
			Console.WriteLine("\n--- Add New Outpatient ---");
			string name = ReadLettersOnly("Enter Name: ");
			int age = ReadInt("Enter Age: ");
			GenderType gender = ReadGender();
			string nId = ReadString("Enter National ID: ");
			string phone = ReadString("Enter Phone Number: ");

			Outpatient p = new Outpatient(name, age, gender, nId, phone);
			FillPatientDetails(p);
			p.ClinicName = ReadString("Clinic Name: ");
			p.ConsultationFee = ReadDouble("Consultation Fee: ");
			p.ChiefComplaint = ReadString("Complaint: ");

			patients.Add(p);
			Console.WriteLine("\nSaved successfully!");
			Console.ReadKey();
		}

		// --- Helper Methods ---
		static void FillPatientDetails(Patient p)
		{
			p.PatientId = patients.Count == 0 ? 1 : patients.Max(x => x.PatientId) + 1;
			p.Height = ReadDouble("Height (m): ");
			p.Weight = ReadDouble("Weight (kg): ");
			p.BloodType = ReadBloodType();
			p.MedicalCase = ReadString("Medical Case: ");
			p.Allergies = ReadString("Allergies: ");
			p.Risk = ReadString("Risk Level: ");
			p.PaymentMethods = ReadString("Payment Method: ");
		}

		static GenderType ReadGender()
		{
			while (true)
			{
				Console.Write("Enter Gender (1 for Male, 2 for Female): ");
				string input = Console.ReadLine();
				if (input == "1") return GenderType.Male;
				if (input == "2") return GenderType.Female;
			}
		}

		static BloodGroup ReadBloodType()
		{
			while (true)
			{
				Console.WriteLine("\nChoose Blood Type (1-8):");
				var types = Enum.GetValues(typeof(BloodGroup));
				for (int i = 0; i < types.Length; i++)
					Console.WriteLine($"{i + 1}. {types.GetValue(i)}");
				if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 8)
					return (BloodGroup)(choice - 1);
			}
		}

		static void ViewByCategory()
		{
			patients.ForEach(p => p.Display());
			Console.ReadKey();
		}

		static void SearchByCategory()
		{
			int id = ReadInt("Enter Patient ID: ");
			var p = patients.FirstOrDefault(x => x.PatientId == id);
			if (p != null) p.Display(); else Console.WriteLine("Not found!");
			Console.ReadKey();
		}

		static string ReadString(string prompt) { Console.Write(prompt); return Console.ReadLine(); }
		static string ReadLettersOnly(string prompt) { Console.Write(prompt); return Console.ReadLine(); }
		static int ReadInt(string prompt) { Console.Write(prompt); int.TryParse(Console.ReadLine(), out int r); return r; }
		static double ReadDouble(string prompt) { Console.Write(prompt); double.TryParse(Console.ReadLine(), out double r); return r; }
		static bool ReadBool(string prompt) { Console.Write(prompt + " (y/n): "); return Console.ReadLine().ToLower() == "y"; }
	}
}