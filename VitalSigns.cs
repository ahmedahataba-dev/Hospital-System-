using System;

namespace Hospital_System
{
    // Made by Ahmed Essam
    public class VitalSigns
    {
        public int HeartRate { get; set; }
        public int OxygenLevel { get; set; }
        public double Temperature { get; set; }
        public int SystolicBP { get; set; }

        // Status properties (used in UI)
        public string HeartRateStatus =>
       (HeartRate >= 60 && HeartRate <= 100) ? "Normal" :
       (HeartRate < 60 && HeartRate > 0) ? "Low" :
       (HeartRate > 100) ? "High" : "Invalid";

        public string OxygenStatus =>
            (OxygenLevel >= 95 && OxygenLevel <= 100) ? "Normal" :
            (OxygenLevel >= 90 && OxygenLevel < 95) ? "Low (Caution)" :
            (OxygenLevel > 0) ? "Critical!" : "Invalid";

        public string TemperatureStatus =>
            (Temperature >= 36.5 && Temperature <= 37.5) ? "Normal" :
            (Temperature > 37.5 && Temperature <= 38.5) ? "Mild Fever" :
            (Temperature > 38.5) ? "High Fever!" : "Invalid";

        public string BPStatus =>
            (SystolicBP >= 90 && SystolicBP <= 120) ? "Normal" :
            (SystolicBP > 120) ? "High" :
            (SystolicBP >= 0) ? "Low" : "Invalid";
        public void CheckAllVitals()
        {
            Console.WriteLine("\n=== Vital Signs Report ===");
            Console.WriteLine($"Heart Rate: {HeartRate} bpm - {HeartRateStatus}");
            Console.WriteLine($"Oxygen Level: {OxygenLevel}% - {OxygenStatus}");
            Console.WriteLine($"Temperature: {Temperature}°C - {TemperatureStatus}");
            Console.WriteLine($"Blood Pressure: {SystolicBP} mmHg - {BPStatus}");
            Console.WriteLine("----------------------------------");
        }
    }
}
