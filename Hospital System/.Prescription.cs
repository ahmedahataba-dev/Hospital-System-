using System;
using System.Collections.Generic;

namespace Hospital_System
{
    public class Prescription
    {
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string Department { get; set; }

        public List<PrescriptionItem> Items = new List<PrescriptionItem>();

        
        public Prescription()
        {
            DoctorName = "Unknown";
            PatientName = "Unknown";
            Department = "Unknown";
        }

       
        public void AddMedicine(Medicine med, int qty, int times, int days)
        {
            //  check stock before adding
            if (med.Quantity < qty)
            {
                Console.WriteLine("Not enough stock ");
                return;
            }

           
            Items.Add(new PrescriptionItem
            {
                Medicine = med,
                Quantity = qty,
                TimesPerDay = times,
                NumberOfDays = days
            });

           
            med.Quantity -= qty;

            Console.WriteLine("Added ");

            
            if (med.Quantity < 5)
            {
                Console.WriteLine($" WARNING: {med.Name} is low in stock! Only {med.Quantity} left.");
            }
        }

       
        public void ShowPrescription()
        {
            double total = 0;

            Console.WriteLine("\n=========== PRESCRIPTION ===========");
            Console.WriteLine("Doctor: " + DoctorName);
            Console.WriteLine("Patient: " + PatientName);
            Console.WriteLine("Department: " + Department);
            Console.WriteLine("===================================");

            foreach (var item in Items)
            {
                Console.WriteLine("\n----------------------");

                Console.WriteLine("Medicine: " + item.Medicine.Name);
                Console.WriteLine("Price: " + item.Medicine.Price);
                Console.WriteLine("Quantity: " + item.Quantity);
                Console.WriteLine("Times Per Day: " + item.TimesPerDay);
                Console.WriteLine("Days: " + item.NumberOfDays);

                double itemTotal = item.TotalPrice();
                Console.WriteLine("Item Total Price: " + itemTotal);

                total += itemTotal;

                Console.WriteLine("\nDose Schedule:");
                for (int i = 1; i <= item.TimesPerDay; i++)
                {
                    Console.WriteLine("Take dose " + i);
                }
            }

            Console.WriteLine("\n===================================");
            Console.WriteLine("TOTAL PRICE: " + total);
            Console.WriteLine("===================================");
        }
    }
}