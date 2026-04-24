using System;
using System.Collections.Generic;
using System.Linq;
using Hospital_System;

public class Pharmacy
{
	public Dictionary<string, List<Medicine>> categories = new Dictionary<string, List<Medicine>>();

	public Pharmacy()
	{
		SeedData();
	}

	private void SeedData()
	{
		categories.Clear();
		// Cardiology
		categories["Cardiology"] = new List<Medicine>
		{
			new Medicine(1, "Concor", 90, 25, 4, 10, 2027, 5),
			new Medicine(2, "Aspirin", 15, 100, 3, 10, 2024, 1),
			new Medicine(7, "Plavix", 120, 30, 2, 14, 2026, 6)
		};
		// General Medicine
		categories["General Medicine"] = new List<Medicine>
		{
			new Medicine(5, "Calpol", 25, 80, 1, 1, 2026, 1),
			new Medicine(6, "Antiflu Kids", 30, 60, 1, 1, 2025, 10)
		};
		// Orthopedics
		categories["Orthopedics"] = new List<Medicine>
		{
			new Medicine(16, "Panadol", 20, 100, 2, 12, 2026, 4),
			new Medicine(17, "Adol", 25, 80, 2, 10, 2025, 6)
		};
	}

	public void ShowCategory(string category)
	{
		if (categories.ContainsKey(category))
		{
			Console.WriteLine($"\n--- Medicines in {category} ---");
			Console.WriteLine("ID | Name | Price | Qty | Strips | Tablets | Expiry | Status");
			Console.WriteLine("---------------------------------------------------------------");

			foreach (var m in categories[category])
			{
				Console.WriteLine(m.ToString());
			}
		}
		else
		{
			Console.WriteLine("Category not found!");
		}
	}

	public Medicine? FindMedicine(string name)
	{
		foreach (var cat in categories.Values)
		{
			foreach (var med in cat)
			{
				if (med.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return med;
				}
			}
		}
		return null;
	}

	public string? GetCategoryNameByIndex(int index)
	{
		var keys = new List<string>(categories.Keys);
		if (index > 0 && index <= keys.Count)
			return keys[index - 1];
		return null;
	}

	public void AddMedicineToCategory(string category, string name, double price, int quantity, int strips, int tablets, int year, int month)
	{
		if (categories.ContainsKey(category))
		{
			// LINQ to get the next ID
			int newId = categories.SelectMany(c => c.Value).Any()
						? categories.SelectMany(c => c.Value).Max(m => m.MedicineId) + 1
						: 1;

			Medicine newMed = new Medicine(newId, name, price, quantity, strips, tablets, year, month);
			categories[category].Add(newMed);

			Console.WriteLine("Medicine Added Successfully");
		}
		else
		{
			Console.WriteLine("Category not found");
		}
	}

	public void ShowAll()
	{
		Console.WriteLine("\n=== Available Categories ===");
		int index = 1;
		foreach (var cat in categories.Keys)
		{
			Console.WriteLine($"{index}. {cat}");
			index++;
		}
	}

	internal static void RunPharmacyMenu(Hospital neurai)
	{
		Pharmacy hospitalPharmacy = neurai.CampusFacilities.HospitalPharmacy;
		bool pharmacyMenu = true;
		neurai.CampusFacilities.ShowExternalFacilities();

		while (pharmacyMenu)
		{
			Console.WriteLine("\n=== Pharmacy Terminal ===");
			Console.WriteLine("1: View All Categories");
			Console.WriteLine("2: Search for a Specific Medicine");
			Console.WriteLine("3: View Medicines by Department");
			Console.WriteLine("0: Return to Main Menu");
			Console.Write("Pharmacy Selection: ");

			string? pharmChoice = Console.ReadLine();

			if (pharmChoice == "1")
			{
				hospitalPharmacy.ShowAll();
			}
			else if (pharmChoice == "2")
			{
				Console.Write("\nEnter a medicine name to search: ");
				string? searchName = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(searchName))
				{
					Medicine? foundMed = hospitalPharmacy.FindMedicine(searchName);
					if (foundMed != null)
						Console.WriteLine($"\n>>> Found: {foundMed.Name}, Price: {foundMed.Price:C}");
					else
						Console.WriteLine("\n>>> Medicine not found.");
				}
			}
			else if (pharmChoice == "3")
			{
				Console.WriteLine("Available Departments:");
				foreach (var category in hospitalPharmacy.categories.Keys)
				{
					Console.WriteLine($"- {category}");
				}
				Console.Write("\nEnter Department Name from the list above: ");
				string? selectedDept = Console.ReadLine();

				// Assuming Department class has ShowDepartmentMedicines method
				Department? foundDept = neurai.ActiveDepartments.Find(d =>
					d.DeptName.Equals(selectedDept, StringComparison.OrdinalIgnoreCase));

				if (foundDept != null)
				{
					foundDept.ShowDepartmentMedicines(hospitalPharmacy);
				}
				else
				{
					Console.WriteLine($"[!] Could not find a department named '{selectedDept}'.");
				}
			}
			else if (pharmChoice == "0")
			{
				pharmacyMenu = false;
				Console.WriteLine("\nClosing Pharmacy Terminal...");
			}
			else
			{
				Console.WriteLine("\n[!] Invalid selection. Please try again.");
			}
		}
	}
}