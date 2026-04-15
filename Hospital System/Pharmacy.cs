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
                // 1. Cardiology
                categories["Cardiology"] = new List<Medicine>
            {
                new Medicine(1, "Concor", 90, 25, 4, 10),
                new Medicine(2, "Aspirin", 15, 100, 3, 10),
                new Medicine(7, "Plavix", 120, 30, 2, 14),
                new Medicine(8, "Lanoxin", 45, 15, 6, 10),
                new Medicine(9, "Aldactone", 60, 40, 5, 10)
            };
                // 2. Chest
                categories["Chest"] = new List<Medicine>
            {
                new Medicine(3, "Ventolin", 95, 40, 1, 1),
                new Medicine(4, "qibrion", 40, 70, 1, 1),
                new Medicine(10, "Foradil", 110, 20, 2, 10),
                new Medicine(11, "Pulmicort", 150, 10, 1, 1),
                new Medicine(12, "Singulair", 85, 50, 4, 7)
            };

                // 3. Pediatrics
                categories["Pediatrics"] = new List<Medicine>
            {
                new Medicine(5, "Calpol", 25, 80, 1, 1),
                new Medicine(6, "Antiflu Kids", 30, 60, 1, 1),
                new Medicine(13, "Brufen Syrup", 45, 90, 1, 1),
                new Medicine(14, "Zyrtec Drops", 55, 35, 1, 1),
                new Medicine(15, "Vi-Daylin", 40, 100, 1, 1)
            };

                // 4. Painkillers
                categories["Painkillers"] = new List<Medicine>
            {
                new Medicine(16, "Panadol", 20, 100, 2, 12),
                new Medicine(17, "Adol", 25, 80, 2, 10),
                new Medicine(18, "Brufen", 35, 60, 3, 10),
                new Medicine(19, "Cataflam", 40, 50, 2, 10),
                new Medicine(20, "Nurofen", 45, 70, 2, 12)
            };

                // 5. General
                categories["General"] = new List<Medicine>
            {
                new Medicine(21, "Vitamin C", 30, 100, 1, 20),
                new Medicine(22, "Multivitamin", 55, 90, 1, 30),
                new Medicine(23, "Zinc", 40, 80, 2, 10),
                new Medicine(24, "Magnesium", 60, 70, 2, 15),
                new Medicine(25, "Iron Tablets", 50, 60, 3, 10)
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
        }
    }


