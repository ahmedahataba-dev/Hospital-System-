using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Department
    {
        public string Deptname { get; set; } = String.Empty;
        public List<Doctor> StaffDoctor { get; set; } = [];//doctors working in the department
        public List<Nurse> StaffNurse { get; set; } = [];//nurses working in the department 
        public List<Room> Rooms { get; set; } = [];
        public Department(string name)
        {
            Deptname = name;
        }
       
    }
}
