using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Nurses : Employee
    {
        private string licenseNumber;// can be used for verification of the nurse's credentials
        private string speciality;// can be used to specify the nurse's area of expertise (e.g., pediatric nurse, surgical nurse, etc.)
        private string degree;// can be used to specify the nurse's educational background (e.g., Bachelor of Science in Nursing, Master of Science in Nursing, etc.)
        string assignedWard;// can be used to specify the ward or department the nurse is assigned to (e.g., emergency, intensive care unit, etc.)
        bool isOnCall;// can be used to indicate whether the nurse is currently on call or not
        int currentPatientLoad;// to track number of patients to manage workload
        bool canAdministerMedication;// can be used to indicate whether the nurse is authorized to administer medication to patients
        bool isHeadNurse;// can be used to indicate whether the nurse holds a leadership position

        public string LicenseNumber
        {
            get { return licenseNumber; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("License number cannot be empty.");
                }
                else
                {
                    licenseNumber = value;
                }
            }
        }
        public string Speciality
        {
            get { return speciality; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Speciality cannot be empty.");
                }
                else
                {
                    speciality = value;
                }
            }
        }
    

      public string Degree
        {
            get { return degree; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Degree cannot be empty.");
                }
                else
                {
                    degree = value;
                }
            }
        }
        public string AssignedWard
        {
            get { return assignedWard; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Assigned ward cannot be empty.");
                }
                else
                {
                    assignedWard = value;
                }
            }
        }
            public bool IsOnCall
            {get; set;}
        public int CurrentPatientLoad
        {
           get { return currentPatientLoad; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Current patient load cannot be negative.");
                }
                else
                {
                    currentPatientLoad = value;
                }
            }
        }
        public bool CanAdministerMedication
        {
            get; set;
        }
        public bool IsHeadNurse
        {
            get; set;
        }
        public Nurses(string name, int age, string email, string address, string nationalId, string phoneNumber,
            decimal salary, double arrivalTime, double departureTime, double experienceYears,
            string licenseNumber, string speciality, string degree, string assignedWard,
            bool isOnCall, int currentPatientLoad, bool canAdministerMedication, bool isHeadNurse):base(name,age,email,address,nationalId,phoneNumber,salary,arrivalTime,departureTime,experienceYears)
        {
            LicenseNumber = licenseNumber;
            Speciality = speciality;
            Degree = degree;
            AssignedWard = assignedWard;
            IsOnCall = isOnCall;
            this.currentPatientLoad = currentPatientLoad;
            CanAdministerMedication = canAdministerMedication;
            IsHeadNurse = isHeadNurse;
        }
    }
}
