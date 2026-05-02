using System;
using System.Collections.Generic;
using System.Text;



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
    }
}
