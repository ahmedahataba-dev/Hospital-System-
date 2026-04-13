using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Floors
    {
        public int FloorNumber { get; set; }
        public string description;
        public bool hasElevatorAccess;
        public List<Departments> DepartmentsOnFloor { get; set; } = new List<Departments>();
        public Floors(int floorNumber,string description,bool hasElevatorAccess)
        {
            FloorNumber = floorNumber;
            this.description = description;
            this.hasElevatorAccess = hasElevatorAccess;
        }
    }
}
