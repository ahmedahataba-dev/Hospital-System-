using Hospital_System;

internal class FinancialDep
{
	public enum DiscountType { None, Insurance, SpecialCase }
	public enum InvoiceStatus { Pending, PartiallyPaid, Paid }

	public class MedicalService
	{
		public string Name { get; set; }
		public decimal Price { get; set; }

		public MedicalService(string name, decimal price)
		{
			Name = name;
			Price = price;
		}
	}

	public class Client
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public Client(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class Invoice
	{
		public int InvoiceId { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
		public Client Client { get; set; }
		public List<MedicalService> Services { get; set; } = new List<MedicalService>();

		public decimal SubTotal { get; set; }       // Original price
		public decimal DiscountAmount { get; set; } // Amount saved
		public decimal Total { get; set; }          // Final price after discount
		public decimal PaidAmount { get; set; }
		public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

		public decimal Debt => Total - PaidAmount;
	}

	public class BillingSystem
	{
		public List<Invoice> Invoices { get; set; } = new List<Invoice>();

		// All-in-one interactive method
		public void RunInteractiveBilling()
		{
			Console.WriteLine("=== Medical Billing System ===");

			// 1. Client Details
			Console.Write("Enter Patient ID: ");
			int.TryParse(Console.ReadLine(), out int clientId);

			Console.Write("Enter Patient Name: ");
			string clientName = Console.ReadLine() ?? "Unknown";
			Client client = new Client(clientId, clientName);

			// 2. Services Input
			List<MedicalService> services = new List<MedicalService>();
			while (true)
			{
				Console.Write("Enter service name (or type 'done' to finish): ");
				string serviceName = Console.ReadLine() ?? "";
				if (serviceName.ToLower() == "done") break;

				Console.Write($"Enter price for ({serviceName}): ");
				decimal.TryParse(Console.ReadLine(), out decimal price);
				services.Add(new MedicalService(serviceName, price));
			}

			// 3. Discount Selection
			Console.WriteLine("\nSelect Discount Type:");
			Console.WriteLine("0 - None");
			Console.WriteLine("1 - Insurance (30% Off)");
			Console.WriteLine("2 - Special Case (15% Off)");
			int.TryParse(Console.ReadLine(), out int discountChoice);
			DiscountType type = (DiscountType)discountChoice;

			// 4. Calculations
			decimal subTotal = services.Sum(s => s.Price);
			decimal finalTotal = ApplyDiscount(subTotal, type);

			Invoice invoice = new Invoice
			{
				InvoiceId = new Random().Next(1000, 9999),
				Client = client,
				Services = services,
				SubTotal = subTotal,
				DiscountAmount = subTotal - finalTotal,
				Total = finalTotal
			};

			Invoices.Add(invoice);

			// 5. Output Results
			PrintInvoice(invoice);
		}

		private decimal ApplyDiscount(decimal total, DiscountType type)
		{
			return type switch
			{
				DiscountType.Insurance => total * 0.70m,
				DiscountType.SpecialCase => total * 0.85m,
				_ => total
			};
		}

		public void PrintInvoice(Invoice invoice)
		{
			Console.WriteLine("\n" + new string('=', 35));
			Console.WriteLine("         OFFICIAL INVOICE         ");
			Console.WriteLine(new string('=', 35));
			Console.WriteLine($"Invoice ID : {invoice.InvoiceId}");
			Console.WriteLine($"Date       : {invoice.Date:yyyy-MM-dd HH:mm}");
			Console.WriteLine($"Patient    : {invoice.Client.Name}");
			Console.WriteLine(new string('-', 35));

			Console.WriteLine("Services List:");
			foreach (var s in invoice.Services)
			{
				Console.WriteLine($"- {s.Name,-18} : {s.Price,10:C}");
			}

			Console.WriteLine(new string('-', 35));
			Console.WriteLine($"Subtotal (Gross):    {invoice.SubTotal,10:C}");
			Console.WriteLine($"Discount Applied:   -{invoice.DiscountAmount,10:C}");
			Console.WriteLine(new string('-', 35));
			Console.WriteLine($"TOTAL DUE:           {invoice.Total,10:C}");
			Console.WriteLine($"Status:              {invoice.Status}");
			Console.WriteLine(new string('=', 35));
		}
	}
	//Ahmed Ayman -------------------------------------
		public static decimal CalcNetSalary(Employee e)//method to calc net salary after the deductions 
	{
		if (e == null)
		{
			Console.WriteLine("Error: Employee object is null.");
			return 0;
		}
		return e.NetSalary;
	}
	//-------------------------------------------------
}