using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Room
    {
        private int roomNumber;
        public int RoomNumber
        {
            get {  return roomNumber; } 
            set
            {
                if (value <= 0  &&   value>500)
                {
                    Console.WriteLine("Invalid Room Number");
                }
                roomNumber= value;
            }
        }
        public enum RoomType { General/*عام*/, ICU/*رعاية مركزة*/, OperatingTheater/*غرفة عمليات*/, Maternity/*غرفة ولادة*/, Incubator/*حضانة*/, Isolation/*غرف عزل*/ };
        public bool IsOccupied { get; set; } = false;
        public RoomType Type { get; set; }
        public string AssignedPatient { get; set; } = "None";
        public string AssignedDoctor { get; set; } = "None";
        public int FloorNumber { get; set; }
        public DateTime LastSanitizedDate { get; set; } = DateTime.Now;
        public Room(int roomNumber, RoomType type, int floorNumber)
        {
            RoomNumber = roomNumber;
            Type = type;
            FloorNumber = floorNumber;
        }
    }
}