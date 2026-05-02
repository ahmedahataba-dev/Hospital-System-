using System;
using System.Collections.Generic;

namespace Hospital_System
{
    // Made by Ahmed Essam
    internal class Ambulance
    {
        public string VehicleNumber { get; set; }
        public string Model { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string CurrentLocation { get; set; } = "Hospital Base";
        public List<string> MedicalEquipment { get; private set; } = new List<string>();
        public string AssignedDriver { get; set; }
        public string AssignedParamedic { get; set; }

        public Ambulance(string vehicleNumber, string model)
        {
            VehicleNumber = vehicleNumber;
            Model = model;
            InitializeEquipment();
        }

        private void InitializeEquipment()
        {
            MedicalEquipment.AddRange(new[]
                { "Defibrillator", "Oxygen Tank", "Stretcher", "First Aid Kit" });
        }

        public void Dispatch(string destination)
        {
            if (IsAvailable)
            {
                IsAvailable = false;
                CurrentLocation = destination;
                // Console.WriteLine($"[EMERGENCY] Ambulance {VehicleNumber} dispatched to: {destination}");
            }
            else
            {
                // Console.WriteLine($"[ALERT] Ambulance {VehicleNumber} is currently busy.");
            }
        }

        public void ReturnToBase()
        {
            IsAvailable = true;
            CurrentLocation = "Hospital Base";
            // Console.WriteLine($"[STATUS] Ambulance {VehicleNumber} is back at base and ready.");
        }

        public void ShowStatus()
        {
            // Console.WriteLine($"\n--- Ambulance {VehicleNumber} Status ---");
            // Console.WriteLine($"Model: {Model}");
            // Console.WriteLine($"Status: {(IsAvailable ? "Available" : "On Mission")}");
            // Console.WriteLine($"Location: {CurrentLocation}");
            // Console.WriteLine($"Driver: {AssignedDriver ?? "N/A"} | Paramedic: {AssignedParamedic ?? "N/A"}");
            // Console.WriteLine("Equipment: " + string.Join(", ", MedicalEquipment));
        }
    }
}
