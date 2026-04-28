using System;
using System.Linq;

namespace Hospital_System
{
    // Made by Ahmed Essam
    internal class Emergency : Patient
    {
        public enum TriageLevel
        {
            Resuscitation, // Immediate resuscitation
            Emergent,      // Very urgent
            Urgent,        // Urgent
            LessUrgent,    // Less urgent
            Non_Urgent     // Non-urgent
        }

        public DateTime ArrivalTime { get; set; }
        public string ChiefComplaint { get; set; }
        public Patient CurrentPatient { get; set; }
        public VitalSigns EmergencyVitals { get; set; } = new VitalSigns();
        public DateTime? DoctorSeenTime { get; set; }

        // ── Triage ─────────────────────────────────────────────────────────
        public TriageLevel GetTriageLevel()
        {
            if (EmergencyVitals.OxygenLevel < 85 ||
                EmergencyVitals.HeartRate > 140 ||
                EmergencyVitals.HeartRate < 40)
                return TriageLevel.Resuscitation;

            if (EmergencyVitals.OxygenLevel < 92 ||
                EmergencyVitals.SystolicBP > 180 ||
                EmergencyVitals.Temperature > 40)
                return TriageLevel.Emergent;

            if (EmergencyVitals.Temperature > 38.5 ||
                EmergencyVitals.SystolicBP > 140)
                return TriageLevel.Urgent;

            if (EmergencyVitals.Temperature > 37.5 ||
                EmergencyVitals.HeartRate > 100)
                return TriageLevel.LessUrgent;

            return TriageLevel.Non_Urgent;
        }

        public string GetFormattedWaitTime()
        {
            var diff = (DoctorSeenTime ?? DateTime.Now) - ArrivalTime;
            return $"{diff.Hours}h {diff.Minutes}m";
        }

        // ── Ambulance request ───────────────────────────────────────────────
        public void RequestAmbulance(ExternalDepartments external, string patientAddress)
        {
            Console.WriteLine($"[System] Requesting ambulance to: {patientAddress}");
            external.DispatchAvailableAmbulance(patientAddress);
        }

        // ── Transfer to Inpatient ───────────────────────────────────────────
        public Inpatient TransferToDepartment(Department targetDept, int roomNumber, int bedNumber)
        {
            var room = targetDept.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room == null)
            {
                Console.WriteLine($"[!] Error: Room {roomNumber} not found in {targetDept.DeptName}.");
                return null;
            }

            var inpatient = new Inpatient
            {
                PatientId = CurrentPatient?.PatientId ?? PatientId,
                Name = CurrentPatient?.Name ?? Name,
                Age = CurrentPatient?.Age ?? Age,
                Gender = CurrentPatient?.Gender ?? Gender,
                WardName = targetDept.DeptName,
                RoomNumber = roomNumber,
                BedNumber = bedNumber,
                AttendingPhysician = room.AssignedDoctor ?? "Not Assigned",
                MedicalCase = ChiefComplaint,
                LastRecordedVitals = EmergencyVitals
            };

            Console.WriteLine($"[Success] {inpatient.Name} transferred to {targetDept.DeptName}.");
            return inpatient;
        }
    }
}
