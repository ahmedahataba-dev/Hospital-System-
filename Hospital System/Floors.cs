using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Floors
    {
        int floornumber;
        public int FloorNumber
        {
            get { return floornumber; }
            set { floornumber = value; }
        }

        public string Description;
        public bool hasElevatorAccess;
        public int MaxRoomCapacity { get; set; } = 20;
        public bool IsOperating { get; set; } = true;

        public bool CanAddMoreRooms()
        {
            int currentRoomCount = 0;
            foreach (var dept in DepartmentsOnFloor)
            {
                currentRoomCount += dept.Rooms.Count;
            }
            return currentRoomCount < MaxRoomCapacity;
        }

        public List<Department> DepartmentsOnFloor { get; set; } = new List<Department>();

        public Floors(int floorNumber, string description, bool hasElevatorAccess)
        {
            FloorNumber = floorNumber;
            this.Description = description;
            this.hasElevatorAccess = hasElevatorAccess;
        }
    }
}