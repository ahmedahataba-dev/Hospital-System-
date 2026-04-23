using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
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

            // 1. Cardiology
            categories["Cardiology"] = new List<Medicine>
            {
                new Medicine(1, "Concor", 90, 25, 4, 10, 2027, 5),
                new Medicine(2, "Aspirin", 15, 100, 3, 10, 2024, 1),
                new Medicine(7, "Plavix", 120, 30, 2, 14, 2026, 6),
                new Medicine(8, "Lanoxin", 45, 15, 6, 10, 2025, 12),
                new Medicine(9, "Aldactone", 60, 40, 5, 10, 2023, 5)
            };

            // 2. Pulmonology
            categories["Pulmonology"] = new List<Medicine>
            {
                new Medicine(3, "Ventolin", 95, 40, 1, 1, 2026, 3),
                new Medicine(4, "qibrion", 40, 70, 1, 1, 2025, 7),
                new Medicine(10, "Foradil", 110, 20, 2, 10, 2027, 1),
                new Medicine(11, "Pulmicort", 150, 10, 1, 1, 2024, 2),
                new Medicine(12, "Singulair", 85, 50, 4, 7, 2026, 8)
            };

            // 3. General Medicine
            categories["General Medicine"] = new List<Medicine>
            {
                new Medicine(5, "Calpol", 25, 80, 1, 1, 2026, 1),
                new Medicine(6, "Antiflu Kids", 30, 60, 1, 1, 2025, 10),
                new Medicine(13, "Brufen Syrup", 45, 90, 1, 1, 2024, 3),
                new Medicine(14, "Zyrtec Drops", 55, 35, 1, 1, 2027, 2),
                new Medicine(15, "Vi-Daylin", 40, 100, 1, 1, 2026, 9)
            };

            // 4. Orthopedics
            categories["Orthopedics"] = new List<Medicine>
            {
                new Medicine(16, "Panadol", 20, 100, 2, 12, 2026, 4),
                new Medicine(17, "Adol", 25, 80, 2, 10, 2025, 6),
                new Medicine(18, "Brufen", 35, 60, 3, 10, 2027, 3),
                new Medicine(19, "Cataflam", 40, 50, 2, 10, 2024, 5),
                new Medicine(20, "Nurofen", 45, 70, 2, 12, 2026, 11)
            };

            // 5. Internal Medicine
            categories["Internal Medicine"] = new List<Medicine>
            {
                new Medicine(21, "Vitamin C", 30, 100, 1, 20, 2027, 1),
                new Medicine(22, "Multivitamin", 55, 90, 1, 30, 2026, 5),
                new Medicine(23, "Zinc", 40, 80, 2, 10, 2025, 3),
                new Medicine(24, "Magnesium", 60, 70, 2, 15, 2024, 8),
                new Medicine(25, "Iron Tablets", 50, 60, 3, 10, 2026, 2)
            };

            // 6. Neurology
            categories["Neurology"] = new List<Medicine>
            {
                new Medicine(26, "Gabapentin", 80, 40, 2, 14, 2026, 7),
                new Medicine(27, "Levetiracetam", 120, 30, 1, 10, 2025, 9),
                new Medicine(28, "Topiramate", 95, 25, 2, 14, 2027, 4)
            };

            // 7. Ophthalmology
            categories["Ophthalmology"] = new List<Medicine>
            {
                new Medicine(29, "Latanoprost Drops", 65, 50, 1, 1, 2026, 6),
                new Medicine(30, "Timolol", 45, 60, 1, 2, 2024, 6),
                new Medicine(31, "Systane Tears", 30, 100, 4, 1, 2027, 1)
            };

            // 8. Physical Medicine
            categories["Physical Medicine"] = new List<Medicine>
            {
                new Medicine(32, "Cyclobenzaprine", 50, 40, 3, 10, 2025, 8),
                new Medicine(33, "Methocarbamol", 45, 35, 2, 14, 2026, 10),
                new Medicine(34, "Voltaren Gel", 60, 80, 1, 1, 2024, 9)
            };

            // 9. ENT
            categories["Otolaryngology (ENT)"] = new List<Medicine>
            {
                new Medicine(35, "Amoxicillin-Clav", 85, 60, 2, 10, 2026, 12),
                new Medicine(36, "Fluticasone Spray", 70, 40, 1, 1, 2025, 4),
                new Medicine(37, "Ciprofloxacin Drops", 55, 30, 3, 7, 2024, 7)
            };

            // 10. Dermatology
            categories["Dermatology"] = new List<Medicine>
            {
                new Medicine(38, "Hydrocortisone", 25, 90, 1, 1, 2026, 3),
                new Medicine(39, "Isotretinoin", 200, 15, 1, 30, 2027, 5),
                new Medicine(40, "Ketoconazole Cream", 40, 50, 2, 14, 2025, 11)
            };

            // 11. General Surgery
            categories["General Surgery"] = new List<Medicine>
{
    new Medicine(41, "Morphine Sulfate", 150, 20, 1, 5),
    new Medicine(42, "Cefazolin", 110, 40, 3, 7),
    new Medicine(43, "Ondansetron", 75, 50, 2, 5)
};
        }

        // Show all categories
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

        // Show medicines in category
        public void ShowCategory(string category)
        {
            if (categories.ContainsKey(category))
            {
                Console.WriteLine($"\n--- Medicines in {category} ---");
                Console.WriteLine("ID | Name | Price | Qty | Strips | Tablets");
                Console.WriteLine("----------------------------------------");

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

        // Find medicine by name
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

        // Get category by index
        public string? GetCategoryNameByIndex(int index)
        {
            var keys = new List<string>(categories.Keys);

            if (index > 0 && index <= keys.Count)
                return keys[index - 1];

            return null;
        }


        public void AddMedicineToCategory(string category, string name, double price, int quantity, int strips, int tablets)
        {
            if (categories.ContainsKey(category))
            {
                int newId = categories.SelectMany(c => c.Value).Max(m => m.MedicineId) + 1;

                Medicine newMed = new Medicine(newId, name, price, quantity, strips, tablets);

                categories[category].Add(newMed);

                Console.WriteLine("Medicine Added Successfully ");
            }
            else
            {
                Console.WriteLine("Category not found ");
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
                Console.WriteLine("1: View All Medicines in Hospital");
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
}


