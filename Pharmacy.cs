namespace Hospital_System
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            new Medicine(3, "Plavix", 120, 30, 2, 14, 2026, 6)
        };

            // 2. Pulmonology
            categories["Pulmonology"] = new List<Medicine>
        {
            new Medicine(4, "Ventolin Inhaler", 150, 50, 1, 1, 2025, 8),
            new Medicine(5, "Symbicort", 300, 20, 1, 1, 2026, 12)
        };

            // 3. General Medicine
            categories["General Medicine"] = new List<Medicine>
        {
            new Medicine(6, "Calpol", 25, 80, 1, 1, 2026, 1),
            new Medicine(7, "Antiflu Kids", 30, 60, 1, 1, 2025, 10)
        };

            // 4. Orthopedics
            categories["Orthopedics"] = new List<Medicine>
        {
            new Medicine(8, "Panadol", 20, 100, 2, 12, 2026, 4),
            new Medicine(9, "Adol", 25, 80, 2, 10, 2025, 6),
            new Medicine(10, "Voltaren", 45, 60, 2, 10, 2026, 2)
        };

            // 5. Internal Medicine
            categories["Internal Medicine"] = new List<Medicine>
        {
            new Medicine(11, "Metformin", 35, 100, 3, 10, 2027, 1),
            new Medicine(12, "Omeprazole", 60, 50, 2, 14, 2025, 11)
        };

            // 6. Neurology
            categories["Neurology"] = new List<Medicine>
        {
            new Medicine(13, "Gabapentin", 180, 40, 3, 10, 2026, 7),
            new Medicine(14, "Keppra", 250, 30, 2, 10, 2027, 3)
        };

            // 7. Ophthalmology
            categories["Ophthalmology"] = new List<Medicine>
        {
            new Medicine(15, "Systane Drops", 120, 40, 1, 1, 2025, 5),
            new Medicine(16, "Tobradex", 85, 35, 1, 1, 2026, 9)
        };

            // 8. Physical Medicine
            categories["Physical Medicine"] = new List<Medicine>
        {
            new Medicine(17, "Celebrex", 210, 25, 2, 10, 2026, 4),
            new Medicine(18, "Mobic", 95, 40, 3, 10, 2025, 10)
        };

            // 9. Otolaryngology (ENT)
            categories["Otolaryngology (ENT)"] = new List<Medicine>
        {
            new Medicine(19, "Augmentin", 140, 50, 2, 7, 2025, 12),
            new Medicine(20, "Zyrtec", 45, 80, 2, 10, 2027, 2)
        };

            // 10. Dermatology
            categories["Dermatology"] = new List<Medicine>
        {
            new Medicine(21, "Fucidin Cream", 35, 70, 1, 1, 2026, 6),
            new Medicine(22, "Roaccutane", 400, 15, 3, 10, 2025, 8)
        };

            // 11. General Surgery
            categories["General Surgery"] = new List<Medicine>
        {
            new Medicine(23, "Tramadol", 150, 30, 2, 10, 2025, 1),
            new Medicine(24, "Cefepime IV", 80, 100, 1, 1, 2026, 11)
        };
        }

        public void ShowCategory(string category)
        {
            if (categories.ContainsKey(category))
            {
                // Console.WriteLine($"\n--- Medicines in {category} ---");
                // Console.WriteLine("ID | Name | Price | Qty | Strips | Tablets | Expiry | Status");
                // Console.WriteLine("---------------------------------------------------------------");

                foreach (var m in categories[category])
                {
                    // Console.WriteLine(m.ToString());
                }
            }
            else
            {
                // Console.WriteLine("Category not found!");
            }
        }

        public Medicine FindMedicine(string name)
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

        public string GetCategoryNameByIndex(int index)
        {
            var keys = new List<string>(categories.Keys);
            if (index > 0 && index <= keys.Count)
                return keys[index - 1];
            return string.Empty;
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

                // Console.WriteLine("Medicine Added Successfully");
            }
            else
            {
                // Console.WriteLine("Category not found");
            }
        }

        public void ShowAll()
        {
            // Console.WriteLine("\n=== Available Categories ===");
            int index = 1;
            foreach (var cat in categories.Keys)
            {
                // Console.WriteLine($"{index}. {cat}");
                index++;
            }
        }

        internal static void RunPharmacyMenu(Hospital neurai)
        {
            // This console menu is disabled in the Windows Forms application.
            // All pharmacy management is handled through the MainForm UI.
        }
    }
}