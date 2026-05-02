// ═══════════════════════════════════════════════════════════════════
// FILE: LabService.cs
// Add this as a NEW file in your Visual Studio project.
// Build Action: Compile  (default for .cs files)
// ═══════════════════════════════════════════════════════════════════

using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Hospital_System
{
    /// <summary>One individual test result within a lab report.</summary>

    public class LabTestResult
    {
        public string TestName { get; set; } = "";
        public double Value { get; set; }
        public string Unit { get; set; } = "";
        public string RefRange { get; set; } = "";
        public string Status { get; set; } = "";
    }

    /// <summary>A complete lab report for one patient visit.</summary>
    public class LabPatientRecord
    {
        public string PatientName { get; set; } = "";
        public string PatientID { get; set; } = "";
        public string TestDate { get; set; } = "";
        public string TestTime { get; set; } = "";
        public System.Collections.Generic.List<LabTestResult> Results { get; set; } = new System.Collections.Generic.List<LabTestResult>();
    }

    /// <summary>
    /// Handles load / save / search for Analysis Lab records.
    /// Follows the exact same pattern as ReceptionService.
    /// </summary>
    public class LabService
    {
        public System.Collections.Generic.List<LabPatientRecord> AllRecords = new System.Collections.Generic.List<LabPatientRecord>();
        private readonly string _file = DataStore.LabRecords;

        public void LoadData()
        {
            try
            {
                if (File.Exists(_file))
                {
                    string j = File.ReadAllText(_file);
                    AllRecords = JsonSerializer.Deserialize<System.Collections.Generic.List<LabPatientRecord>>(j)
                                 ?? new System.Collections.Generic.List<LabPatientRecord>();
                }
            }
            catch { AllRecords = new System.Collections.Generic.List<LabPatientRecord>(); }
        }

        public void SaveRecord(LabPatientRecord record)
        {
            AllRecords.Add(record);
            try
            {
                var opt = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_file, JsonSerializer.Serialize(AllRecords, opt));
            }
            catch { /* non-fatal */ }
        }

        public System.Collections.Generic.List<LabPatientRecord> SearchByID(string patientId) =>
            AllRecords.FindAll(r => r.PatientID.Equals(patientId.Trim(),
                                       System.StringComparison.OrdinalIgnoreCase));

        public System.Collections.Generic.List<LabPatientRecord> GetTodaysRecords()
        {
            string today = System.DateTime.Today.ToString("yyyy-MM-dd");
            return AllRecords.FindAll(r => r.TestDate == today);
        }
    }
}