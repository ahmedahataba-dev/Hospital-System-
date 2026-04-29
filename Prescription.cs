namespace Hospital_System
{
    public class PrescriptionItem
    {
        public Medicine Medicine { get; set; }

        public int Quantity { get; set; }
        public int TimesPerDay { get; set; }
        public int NumberOfDays { get; set; }
        public PrescriptionItem() { }

        public PrescriptionItem(Medicine medicine, int qty, int times, int days)
        {
            Medicine = medicine;
            Quantity = qty;
            TimesPerDay = times;
            NumberOfDays = days;
        }


        public double TotalPrice()
        {
            return Medicine.Price * Quantity;
        }
        public class Prescription
        {
            private List<PrescriptionItem> _items = new List<PrescriptionItem>();

            public void AddItem(Pharmacy pharmacy, string medicineName, int qty, int times, int days)
            {
                Medicine med = null;
                foreach (var kv in pharmacy.categories)
                {
                    med = kv.Value.FirstOrDefault(m =>
                        m.Name.IndexOf(medicineName, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (med != null) break;
                }

                if (med == null)
                {
                    Console.WriteLine($"[!] Medicine '{medicineName}' not found.");
                    return;
                }

                if (med.IsExpired())
                {
                    Console.WriteLine($"[!] '{med.Name}' is expired.");
                    return;
                }

                // ── لو موجود بالفعل → update الـ qty بس ──
                var existing = _items.FirstOrDefault(i =>
                    i.Medicine.Name.Equals(med.Name, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    existing.Quantity = qty;
                    existing.TimesPerDay = times;
                    existing.NumberOfDays = days;
                    return;
                }

                _items.Add(new PrescriptionItem(med, qty, times, days));
            }

            public void RemoveItem(string medicineName)
            {
                var item = _items.FirstOrDefault(i =>
                    i.Medicine.Name.Equals(medicineName, StringComparison.OrdinalIgnoreCase));

                if (item == null)
                    throw new Exception($"'{medicineName}' not found in prescription.");

                // ── مش بنرجع الـ stock عشان AddItem مش بتخصم ──
                _items.Remove(item);
            }

            public double GetTotalPrice()
            {
                return _items.Sum(i => i.TotalPrice());
            }

            public void UpdateQuantity(string medicineName, int newQty)
            {
                if (newQty <= 0)
                {
                    Console.WriteLine("[!] Quantity must be greater than zero.");
                    return;
                }

                var item = _items.FirstOrDefault(i =>
                    i.Medicine.Name.IndexOf(medicineName, StringComparison.OrdinalIgnoreCase) >= 0);

                if (item == null)
                {
                    Console.WriteLine("[!] Medicine not found in prescription.");
                    return;
                }

                item.Quantity = newQty;
            }

            public void DisplayPrescription()
            {
                if (_items.Count == 0)
                {
                    Console.WriteLine("[!] Prescription is empty.");
                    return;
                }

                Console.WriteLine("\n===== Prescription =====");
                Console.WriteLine($"{"Medicine",-20} {"Qty",5} {"Times/Day",10} {"Days",6} {"Price",10}");
                Console.WriteLine(new string('-', 55));

                foreach (var item in _items)
                    Console.WriteLine($"{item.Medicine.Name,-20} {item.Quantity,5} " +
                                      $"{item.TimesPerDay,10} {item.NumberOfDays,6} " +
                                      $"{item.TotalPrice(),10:F2}");

                Console.WriteLine(new string('-', 55));
                Console.WriteLine($"{"Total",-20} {GetTotalPrice(),43:F2}");
            }

            public List<PrescriptionItem> GetItems() => _items;

            public void Clear()
            {

                _items.Clear();
            }
        }
    }
}

