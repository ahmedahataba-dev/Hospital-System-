using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    //made by Youssef Essam
    internal class Security: Employee
    {
        string assignedArea=string.Empty;//location in hospital
       // bool isFireSafetyTrained=false;
        TimeSpan patrolTime; //time taken for each patrol round
        string shiftReport=string.Empty; //report of any incidents during the shift
        public string AssignedArea
        {
            get { return assignedArea; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Assigned area name mustn't have space , and it can't be empty ");
                }
                assignedArea = value;
            }
        }
        public bool IsFireSafetyTrained
        {
            get;
            set;
        }
        public TimeSpan PatrolTime
        {
            get { return patrolTime; }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Patrol time must be positive.");
                }
                patrolTime = value;
            }
        }
        public string ShiftReport
        {
            get { return shiftReport; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Shift report cannot be empty.");
                }
                shiftReport = value;
            }
        }
        public Security(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email
            , string address, decimal salary, double arrivaltime, double departuretime, double experienceyears, string assignedArea, bool isFireSafetyTrained, TimeSpan patrolTime, string shiftReport)
            : base(name, age, gender, nationalId, phoneNumber, email, address, salary, arrivaltime, departuretime, experienceyears)
        {
            AssignedArea = assignedArea;
            IsFireSafetyTrained = isFireSafetyTrained;
            PatrolTime = patrolTime;
            ShiftReport = shiftReport;
        }
    }
}
