using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Receptionists: Employee
    {
        string assignedDesk=string.Empty;//location of work in the hospital
       // int accessLevel;//to determine the receptionist's access level to hospital systems and information
        int managedDoctors;//to track the number of doctors the receptionist is responsible for managing
        public string AssignedDesk
        {
            get { return assignedDesk; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Assigned desk cannot be empty.");
                }
                assignedDesk = value;
            }
        }
        public int ManagedDoctors
        {
            get { return managedDoctors; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Managed doctors cannot be negative.");
                }
                if(value > 20)
                {
                    throw new ArgumentException("One receptionist cannot manage more than 20 doctors.");
                }
                managedDoctors = value;
            }
        }
        public Receptionists(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email
            , string address, decimal salary, double arrivaltime, double departuretime, double experienceyears, string assignedDesk, int managedDoctors)
            : base(name, age, gender, nationalId, phoneNumber, email, address, salary, arrivaltime, departuretime, experienceyears)
        {
            AssignedDesk = assignedDesk;
            ManagedDoctors = managedDoctors;
        }
    }
}
