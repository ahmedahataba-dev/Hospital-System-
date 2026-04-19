using System;
using System.Collections.Generic;
using System.Text;


namespace Hospital_System
{
   

        public class Medicine
{
    public int MedicineId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public int NumberOfStrips { get; set; }
    public int NumberOfTabletsPerStrip { get; set; }
    public int ExpiryYear { get; set; }
    public int ExpiryMonth { get; set; }

    public Medicine(int id, string name, double price, int quantity,
        int numberOfStrips, int numberOfTabletsPerStrip,
        int expiryYear, int expiryMonth)
    {
        MedicineId = id;
        Name = name;
        Price = price;
        Quantity = quantity;
        NumberOfStrips = numberOfStrips;
        NumberOfTabletsPerStrip = numberOfTabletsPerStrip;
        ExpiryYear = expiryYear;
        ExpiryMonth = expiryMonth;
    }

    public int GetTotalTablets()
    {
        return NumberOfStrips * NumberOfTabletsPerStrip;
    }

    public override string ToString()
    {
        return $"{MedicineId} | {Name} | {Price} | {Quantity} | {NumberOfStrips} | {NumberOfTabletsPerStrip} | {ExpiryMonth}/{ExpiryYear}";
    }

    public void DecreaseQuantity(int amount)
    {
        Quantity -= amount;

        if (Quantity < 0)
            Quantity = 0;
    }
}
}
