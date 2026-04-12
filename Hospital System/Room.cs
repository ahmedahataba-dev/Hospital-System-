using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Room(string roomNumber, string type, bool isOccupied, string assignedPatient, string assignedDoctor, int floorNumber, DateTime lastSanitizationDate)
    {
        public string RoomNumber { get; set; } = roomNumber;
        public string Type { get; set; } = type;
        public bool IsOccupied { get; set; } = false;
        public string AssignedPatient { get; set; } = assignedPatient;
        public string AssignedDoctor { get; set; } = assignedDoctor;
        public int FloorNumber { get; set; } = floorNumber;
        public DateTime LastSanitizedDate { get; set; } = DateTime.Now;
    }
}
