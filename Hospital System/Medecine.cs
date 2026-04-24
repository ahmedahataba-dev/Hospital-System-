using System;

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

		public Medicine() { }

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

		public bool IsExpired()
		{
			return ExpiryYear < DateTime.Now.Year ||
				  (ExpiryYear == DateTime.Now.Year && ExpiryMonth < DateTime.Now.Month);
		}

		public override string ToString()
		{
			string status = IsExpired() ? "Expired " : "Valid ";
			return $"{MedicineId} | {Name} | {Price} | {Quantity} | {NumberOfStrips} | {NumberOfTabletsPerStrip} | {ExpiryMonth}/{ExpiryYear} | {status}";
		}
	}
}