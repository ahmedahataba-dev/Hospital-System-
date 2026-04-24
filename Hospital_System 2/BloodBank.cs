using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Hospital_System
{

    public partial class program
    {
        public class BloodBank
        {
            private Dictionary<string, int> _bloodStocks;
            private const string bloodFilePath = "blood_inventory.json"; // الملف اللي هيتحفظ فيه

            public BloodBank()
            {
                if (!LoadBloodData())
                {

                    _bloodStocks = new Dictionary<string, int>
                {
                    {"A+", 15}, {"A-", 7}, {"B+", 12}, {"B-", 4},
                    {"AB+", 5}, {"AB-", 2}, {"O+", 20}, {"O-", 25}
                };
                    SaveBloodData();
                }
            }

            public void DonateBlood(string type, int bags)
            {
                _bloodStocks[type] += bags;
                Console.WriteLine($"\n[SUCCESS] Added {bags} bags of {type}. Total stock: {_bloodStocks[type]}");
                SaveBloodData();
            }

            public bool WithdrawBlood(string type, int bags)
            {
                if (_bloodStocks[type] >= bags)
                {
                    _bloodStocks[type] -= bags;
                    Console.WriteLine($"\n[APPROVED] Issued {bags} bags of {type}. Remaining: {_bloodStocks[type]}");
                    SaveBloodData();

                    if (_bloodStocks[type] < 5)
                    {
                        Console.WriteLine($"[CRITICAL WARNING] {type} stock is very low ({_bloodStocks[type]} bags left)!");
                    }
                    return true;
                }

                Console.WriteLine($"\n[DENIED] Insufficient stock for {type}. Only {_bloodStocks[type]} bags available.");
                return false;
            }

            public void PrintInventoryReport()
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("       BLOOD BANK INVENTORY REPORT      ");
                Console.WriteLine("========================================");
                Console.WriteLine("{0,-15} | {1,-10} | {2,-10}", "Blood Type", "Quantity", "Status");
                Console.WriteLine("----------------------------------------");

                foreach (var stock in _bloodStocks)
                {
                    string status = stock.Value < 5 ? "CRITICAL" : "STABLE";
                    Console.WriteLine("{0,-15} | {1,-10} | {2,-10}", stock.Key, stock.Value, status);
                }
                Console.WriteLine("========================================\n");
            }

            private void SaveBloodData()
            {
                try
                {
                    string json = JsonConvert.SerializeObject(_bloodStocks, Formatting.Indented);
                    File.WriteAllText(bloodFilePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving blood data: " + ex.Message);
                }
            }


            private bool LoadBloodData()
            {
                try
                {
                    if (File.Exists(bloodFilePath))
                    {
                        string json = File.ReadAllText(bloodFilePath);
                        _bloodStocks = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
                        return _bloodStocks != null;
                    }
                }
                catch { }
                return false;
            }
        }
    }
}
















































