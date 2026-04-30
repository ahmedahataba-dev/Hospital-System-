using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hospital_System
{
    public class BloodBank
    {
        // ── Fields ──────────────────────────────────────────
        private Dictionary<string, int> _bloodStocks;
        private List<Donor> _donors;
        private List<BloodTransfer> _transfers;

        private int _nextDonorId = 1;
        private int _nextTransferId = 1;

        private static readonly string bloodFilePath = DataStore.BloodInventory;
        private static readonly string donorsFilePath = DataStore.Donors;
        private static readonly string transfersFilePath = DataStore.Transfers;

        // ════════════════════════════════════════════════════
        //  CONSTRUCTOR & DATA LOADING
        // ════════════════════════════════════════════════════
        public BloodBank()
        {
            _donors = new List<Donor>();
            _transfers = new List<BloodTransfer>();

            if (!LoadBloodData())
            {
                _bloodStocks = new Dictionary<string, int>
                {
                    {"A+", 15}, {"A-", 7},  {"B+", 12}, {"B-", 4},
                    {"AB+", 5}, {"AB-", 2}, {"O+", 20}, {"O-", 25}
                };
                SaveBloodData();
            }

            LoadDonors();
            LoadTransfers();
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

        // ════════════════════════════════════════════════════
        //  CORE BLOOD OPERATIONS
        // ════════════════════════════════════════════════════
        public void DonateBlood(string type, int bags)
        {
            _bloodStocks[type] += bags;
            SaveBloodData();
        }

        public bool WithdrawBlood(string type, int bags)
        {
            if (_bloodStocks[type] >= bags)
            {
                _bloodStocks[type] -= bags;
                SaveBloodData();
                return true;
            }
            return false;
        }

        // ════════════════════════════════════════════════════
        //  DONOR MANAGEMENT
        // ════════════════════════════════════════════════════
        public void RegisterDonorUI(string name, int age, string phone, string bloodType)
        {
            var d = new Donor
            {
                DonorId = _nextDonorId++,
                Name = name,
                Age = age,
                PhoneNumber = phone,
                BloodType = bloodType,
                LastDonationDate = DateTime.Now
            };
            _donors.Add(d);
            SaveDonors();
        }

        // ════════════════════════════════════════════════════
        //  TRANSFER MANAGEMENT
        // ════════════════════════════════════════════════════
        public void AddTransfer(BloodTransfer transfer)
        {
            transfer.TransferId = _nextTransferId++;
            _transfers.Add(transfer);
            SaveTransfers();
        }

        // ════════════════════════════════════════════════════
        //  SAVE METHODS
        // ════════════════════════════════════════════════════
        private void SaveBloodData()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_bloodStocks, Formatting.Indented);
                File.WriteAllText(bloodFilePath, json);
            }
            catch (Exception ex) { Console.WriteLine("Error saving blood data: " + ex.Message); }
        }

        private void SaveDonors()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_donors, Formatting.Indented);
                File.WriteAllText(donorsFilePath, json);
            }
            catch (Exception ex) { Console.WriteLine("Error saving donors: " + ex.Message); }
        }

        private void SaveTransfers()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_transfers, Formatting.Indented);
                File.WriteAllText(transfersFilePath, json);
            }
            catch (Exception ex) { Console.WriteLine("Error saving transfers: " + ex.Message); }
        }

        // ════════════════════════════════════════════════════
        //  GETTERS & DASHBOARD
        // ════════════════════════════════════════════════════
        public Dictionary<string, int> GetStocks() => new Dictionary<string, int>(_bloodStocks);
        public List<Donor> GetDonors() => new List<Donor>(_donors);
        public List<BloodTransfer> GetTransfers() => new List<BloodTransfer>(_transfers);

        public string GetDashboardInfo()
        {
            int totalBags = 0;
            foreach (var t in _transfers) totalBags += t.Bags;

            return $"Total Donors     : {_donors.Count}\n"
                 + $"Total Transfers  : {_transfers.Count}\n"
                 + $"Total Bags Moved : {totalBags}\n";
        }
    }
}