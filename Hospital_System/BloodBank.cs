using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hospital_System
{
   
    
        public class BloodBank
        {
            private Dictionary<string, int> _bloodStocks;
            private const string bloodFilePath = "blood_inventory.json";
            private List<Donor> _donors;
            private List<BloodTransfer> _transfers;
            private int _nextDonorId = 1;
            private int _nextTransferId = 1;
            private const string donorsFilePath = "donors_data.json";
            private const string transfersFilePath = "transfers_data.json";

            public BloodBank()
            {
               
                _donors = new List<Donor>();
                _transfers = new List<BloodTransfer>();

                if (!LoadBloodData())
                {
                    _bloodStocks = new Dictionary<string, int>
                    {
                        {"A+", 15}, {"A-", 7}, {"B+", 12}, {"B-", 4},
                        {"AB+", 5}, {"AB-", 2}, {"O+", 20}, {"O-", 25}
                    };
                    SaveBloodData();
                }

               
                LoadDonors();
                LoadTransfers();
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
                        Console.WriteLine($"[CRITICAL WARNING] {type} stock is very low ({_bloodStocks[type]} bags left)!");

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

          

            public void RegisterDonor()
            {
                Donor d = new Donor();
                d.DonorId = _nextDonorId++;
                d.Name = InputHelper.ReadLettersOnly("Donor Name: ");
                d.Age = InputHelper.ReadInt("Age: ");
                d.PhoneNumber = InputHelper.ReadString("Phone (11 digits): ");

                string[] types = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                Console.WriteLine("Blood Type: 1.A+ 2.A- 3.B+ 4.B- 5.AB+ 6.AB- 7.O+ 8.O-");
                int ch = InputHelper.ReadInt("Choice: ");
                if (ch < 1) ch = 1;
                if (ch > 8) ch = 8;
                d.BloodType = types[ch - 1];

                d.LastDonationDate = DateTime.Now;
                _donors.Add(d);
                SaveDonors(); 
                Console.WriteLine($"\n[SUCCESS] Donor registered with ID: {d.DonorId}");
            }

            public void ViewDonors(string filterBloodType = null)
            {
                Console.WriteLine("\n========== DONORS LIST ==========");
                bool found = false;
                foreach (var d in _donors)
                {
                    if (filterBloodType == null || d.BloodType == filterBloodType)
                    {
                        d.Display();
                        found = true;
                    }
                }
                if (!found) Console.WriteLine("No donors found.");
                Console.WriteLine("==================================");
            }

            public void RecordDonation()
            {
                int id = InputHelper.ReadInt("Enter Donor ID: ");
                Donor donor = null;
                foreach (var d in _donors)
                    if (d.DonorId == id) { donor = d; break; }

                if (donor == null) { Console.WriteLine("Donor not found!"); return; }

                int bags = InputHelper.ReadInt("Number of bags donated: ");

                _bloodStocks[donor.BloodType] += bags;

                donor.DonationHistory.Add(new DonationLog
                {
                    Date = DateTime.Now,
                    Bags = bags,
                    BloodType = donor.BloodType
                });
                donor.LastDonationDate = DateTime.Now;

                SaveBloodData(); 
                SaveDonors();   
                Console.WriteLine($"\n[SUCCESS] Recorded {bags} bags from {donor.Name}. Stock updated.");
            }

           

            public void ProcessTransfer()
            {
                int donorId = InputHelper.ReadInt("Enter Donor ID: ");
                Donor donor = null;
                foreach (var d in _donors)
                    if (d.DonorId == donorId) { donor = d; break; }

                if (donor == null) { Console.WriteLine("Donor not found!"); return; }

                string patientName = InputHelper.ReadString("Patient Name: ");

                Console.WriteLine($"Donor blood type: {donor.BloodType}");
                Console.WriteLine("Confirm blood type match? (y/n): ");
                if (Console.ReadLine().ToLower() != "y")
                {
                    Console.WriteLine("Transfer cancelled.");
                    return;
                }

                int bags = InputHelper.ReadInt("Number of bags to transfer: ");

                if (_bloodStocks[donor.BloodType] < bags)
                {
                    Console.WriteLine($"[DENIED] Only {_bloodStocks[donor.BloodType]} bags available.");
                    return;
                }

                _bloodStocks[donor.BloodType] -= bags;

                BloodTransfer transfer = new BloodTransfer
                {
                    TransferId = _nextTransferId++,
                    DonorName = donor.Name,
                    PatientName = patientName,
                    BloodType = donor.BloodType,
                    Bags = bags,
                    Date = DateTime.Now,
                    Status = "Completed"
                };
                _transfers.Add(transfer);

                SaveBloodData();  
                SaveTransfers();  

                Console.WriteLine($"\n[SUCCESS] Transfer #{transfer.TransferId} completed.");

                if (_bloodStocks[donor.BloodType] < 5)
                    Console.WriteLine($"[WARNING] {donor.BloodType} stock is now critically low!");
            }

            public void ViewTransfers()
            {
                Console.WriteLine("\n========== TRANSFER RECORDS ==========");
                if (_transfers.Count == 0) { Console.WriteLine("No transfers recorded."); return; }
                foreach (var t in _transfers) t.Display();
                Console.WriteLine("Total transfers: " + _transfers.Count);
            }

        

            public void ShowDashboard()
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("         BLOOD BANK DASHBOARD          ");
                Console.WriteLine("========================================");
                Console.WriteLine($"Total Donors     : {_donors.Count}");
                Console.WriteLine($"Total Transfers  : {_transfers.Count}");

                int totalBags = 0;
                foreach (var t in _transfers) totalBags += t.Bags;
                Console.WriteLine($"Total Bags Moved : {totalBags}");

                Console.WriteLine("\n--- Inventory Summary ---");
                foreach (var stock in _bloodStocks)
                {
                    string status = stock.Value < 5 ? "!! CRITICAL" : "OK";
                    Console.WriteLine($"  {stock.Key,-5}: {stock.Value} bags  [{status}]");
                }
                Console.WriteLine("========================================");
            }

           

            private void SaveDonors()
            {
                try
                {
                    string json = JsonConvert.SerializeObject(_donors, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(donorsFilePath, json);
                }
                catch (Exception ex) { Console.WriteLine("Error saving donors: " + ex.Message); }
            }

            private void LoadDonors()
            {
                try
                {
                    if (File.Exists(donorsFilePath))
                    {
                        string json = File.ReadAllText(donorsFilePath);
                        var loaded = JsonConvert.DeserializeObject<List<Donor>>(json);
                        if (loaded != null)
                        {
                            _donors = loaded;
                            if (_donors.Count > 0)
                                _nextDonorId = _donors[_donors.Count - 1].DonorId + 1;
                        }
                    }
                }
                catch { }
            }

          

            private void SaveTransfers()
            {
                try
                {
                    string json = JsonConvert.SerializeObject(_transfers, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(transfersFilePath, json);
                }
                catch (Exception ex) { Console.WriteLine("Error saving transfers: " + ex.Message); }
            }

            private void LoadTransfers()
            {
                try
                {
                    if (File.Exists(transfersFilePath))
                    {
                        string json = File.ReadAllText(transfersFilePath);
                        var loaded = JsonConvert.DeserializeObject<List<BloodTransfer>>(json);
                        if (loaded != null)
                        {
                            _transfers = loaded;
                            if (_transfers.Count > 0)
                                _nextTransferId = _transfers[_transfers.Count - 1].TransferId + 1;
                        }
                    }
                }
                catch { }
            }
        }
    }
