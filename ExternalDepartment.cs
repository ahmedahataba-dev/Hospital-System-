using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital_System
{
    internal class ExternalDepartments
    {
        public bool HasGarden { get; set; } = true;
        public int BloodBankCapacity { get; set; }
        public Pharmacy HospitalPharmacy { get; set; }

        // Ahmed Essam: replaced simple AmbulanceCount int with a real Ambulance fleet
        public List<Ambulance> AmbulanceFleet { get; set; } = new List<Ambulance>();

        /// <summary>Backward-compatible property — returns total fleet size.</summary>
        public int AmbulanceCount => AmbulanceFleet.Count;
        List<Ambulance> ambulances = new List<Ambulance>();
        int ambulanceCounter = 1;

        public ExternalDepartments(int bloodBankCapacity)
        {
            BloodBankCapacity = bloodBankCapacity;
            HospitalPharmacy = new Pharmacy();
            InitializeInitialFleet();
        }

        // Legacy constructor kept for compatibility
        public ExternalDepartments(int ambulanceCount, int bloodBankCapacity)
            : this(bloodBankCapacity) { }

        private void InitializeInitialFleet()
        {
            for (int i = 0; i < 5; i++)
            {
                CreateAmbulance();
            }


            ambulanceCounter = AmbulanceFleet
                .Select(a => int.Parse(a.VehicleNumber.Split('-')[1]))
                .DefaultIfEmpty(0)
                .Max() + 1;
        }
        /// <summary>Auto-generates the next AMB-NNN id and adds the ambulance.</summary>
        public string AddAmbulanceUI(Ambulance amb, string model)
        {
            int nextId = AmbulanceFleet.Count + 1;
            string plateNumber = $"AMB-{nextId:D3}";
            AmbulanceFleet.Add(new Ambulance(plateNumber, model));
            // Console.WriteLine($"[+] Ambulance {plateNumber} added to fleet.");
            return plateNumber;
        }

        /// <summary>Dispatches first available ambulance; returns null if all busy.</summary>
        public Ambulance DispatchAvailableAmbulance(string destination)
        {
            var available = AmbulanceFleet.FirstOrDefault(a => a.IsAvailable);
            if (available != null)
            {
                available.Dispatch(destination);
                return available;
            }
            // Console.ForegroundColor = ConsoleColor.Red;
            // Console.WriteLine("[!] ALERT: All ambulances are currently on missions!");
            // Console.ResetColor();
            return null;
        }
        public void CreateAmbulance()
        {
            string id = $"AMB-{ambulanceCounter:D3}";

            Ambulance amb = new Ambulance(id, "Standard");
            ambulances.Add(amb);

            AddAmbulanceUI(amb, "Standard");

            ambulanceCounter++;
        }

        public void ShowExternalFacilities()
        {
            // Console.WriteLine("\n--- External Facilities ---");
            // Console.WriteLine($"Garden Available: {(HasGarden ? "Yes" : "No")}");
            // Console.WriteLine($"Blood Bank Capacity: {BloodBankCapacity} units");
            // Console.WriteLine($"Pharmacy Categories Loaded: {HospitalPharmacy.categories.Count}");
            int ready = AmbulanceFleet.Count(a => a.IsAvailable);
            // Console.WriteLine($"Ambulances: {AmbulanceFleet.Count} total | {ready} available | {AmbulanceFleet.Count - ready} busy");
        }
    }
}
