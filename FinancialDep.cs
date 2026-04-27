using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital_System
{
    internal class FinancialDep
    {
        public enum DiscountType { None, Insurance, SpecialCase }
        public enum InvoiceStatus { Pending, PartiallyPaid, Paid }

        public class MedicalService
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public MedicalService(string name, decimal price) { Name = name; Price = price; }
        }

        public class Client
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Client(int id, string name) { Id = id; Name = name; }
        }

        public class Invoice
        {
            public int InvoiceId { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;
            public Client Client { get; set; } = null;
            public List<MedicalService> Services { get; set; } = new List<MedicalService>();
            public decimal SubTotal { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal Total { get; set; }
            public decimal PaidAmount { get; set; }
            public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
            public decimal Debt => Total - PaidAmount;
        }

        public class BillingSystem
        {
            public List<Invoice> Invoices { get; set; } = new List<Invoice>();

            private decimal ApplyDiscount(decimal total, DiscountType type)
            {
                switch (type)
                {
                    case DiscountType.Insurance:
                        return total * 0.70m;

                    case DiscountType.SpecialCase:
                        return total * 0.85m;

                    default:
                        return total;
                }
            }

            public Invoice CreateInvoice(int clientId, string clientName,
                                         List<MedicalService> services, DiscountType discountType)
            {
                var client = new Client(clientId, clientName);
                decimal sub = services.Sum(s => s.Price);
                decimal tot = ApplyDiscount(sub, discountType);

                var invoice = new Invoice
                {
                    InvoiceId = new Random().Next(1000, 9999),
                    Client = client,
                    Services = services,
                    SubTotal = sub,
                    DiscountAmount = sub - tot,
                    Total = tot
                };
                Invoices.Add(invoice);
                return invoice;
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
                foreach (var s in invoice.Services)
                    Console.WriteLine($"- {s.Name,-18} : {s.Price,10:C}");
                Console.WriteLine(new string('-', 35));
                Console.WriteLine($"Subtotal:    {invoice.SubTotal,10:C}");
                Console.WriteLine($"Discount:   -{invoice.DiscountAmount,10:C}");
                Console.WriteLine(new string('-', 35));
                Console.WriteLine($"TOTAL DUE:   {invoice.Total,10:C}");
                Console.WriteLine($"Status:      {invoice.Status}");
                Console.WriteLine(new string('=', 35));
            }
        }
    }
}
