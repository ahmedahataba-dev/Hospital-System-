using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Floors
    {
        int floornumber;
        public int FloorNumber
        {
            get { return floornumber; }
            set { floornumber = value; }
        }
        public string description;
        public bool hasElevatorAccess;
        public List<Department> DepartmentsOnFloor { get; set; } = new List<Department>();
        public Floors(int floorNumber,string description,bool hasElevatorAccess)
        {
            FloorNumber = floorNumber;
            this.description = description;
            this.hasElevatorAccess = hasElevatorAccess;
        }
    }
}
