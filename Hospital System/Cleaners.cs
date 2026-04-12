using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Cleaners : Employee
    {
        string assignedArea;//location of work in the hospital
        bool hasHazmatTraining;//to check if he has the ability to deal with dangerous materials or not
        DateTime lastSanitationCheck;//DateTime is a Function that gives us the current date and time
        TimeSpan shiftStartTime;//TimeSpan is a Function that gives us the current time
        string equipmentId;//to identify the cleaning equipment used by the cleaner
        string currentStatus;//to track the cleaner's current status (e.g., available, on break, assigned to a task)
        public enum TrainingLevel { None, Basic, Advanced }
        public string AssignedArea
        {
            get { return assignedArea; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Assigned area cannot be empty.");
                }
                assignedArea = value;
            }
        }
        public bool HasHazmatTraining
        {
            get { return hasHazmatTraining; }
            set { hasHazmatTraining = value; }
        }
        public DateTime LastSanitationCheck
        {
            get { return lastSanitationCheck; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Last sanitation check cannot be in the future.");
                }
                lastSanitationCheck = value;
            }
        }
        public TimeSpan ShiftStartTime
        {
            get { return shiftStartTime; }
            set
            {
                if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
                {
                    throw new ArgumentException("Shift start time must be between 00:00 and 23:59.");
                }
                shiftStartTime = value;
            }
        }
        public string EquipmentId
        {
            get { return equipmentId; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Equipment ID cannot be empty.");
                }
                equipmentId = value;
            }
        }
        public string CurrentStatus
        {
            get { return currentStatus; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Current status cannot be empty.");
                }
                currentStatus = value;
            }
        }
        public TrainingLevel CleanerTrainingLevel { get; set; }
        public Cleaners(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email
            , string address, decimal salary, double arrivaltime, double departuretime, double experienceyears,
            string assignedArea, bool hasHazmatTraining, DateTime lastSanitationCheck, TimeSpan shiftStartTime,
            string equipmentId, string currentStatus, TrainingLevel cleanerTrainingLevel)
            : base(name, age, gender, nationalId, phoneNumber, email, address, salary, arrivaltime, departuretime, experienceyears)
        {
            AssignedArea = assignedArea;
            HasHazmatTraining = hasHazmatTraining;
            LastSanitationCheck = lastSanitationCheck;
            ShiftStartTime = shiftStartTime;
            EquipmentId = equipmentId;
            CurrentStatus = currentStatus;
            CleanerTrainingLevel = cleanerTrainingLevel;
        }
    }
}

    