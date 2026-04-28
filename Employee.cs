using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hospital_System
{
    internal class Employee : Person
    {
        public static int employeeid_counter = 1;
        private decimal salary;
        private DateTime checkouttime;
        private TimeSpan workedhours;
        private TimeSpan lateness;
        private TimeSpan shiftstart = new TimeSpan(9, 0, 0);
        private double experienceyears;
        private int employeeid;

        [JsonIgnore]
        public static List<Employee> employees = new List<Employee>();

        public Employee() : base() { }

        public int EmployeeId
        {
            get
            {
                return employeeid;
            }
            set
            {
                // Allow 0 silently during JSON deserialization.
                // AddEmployee() always assigns the counter value which is >= 1.
                employeeid = value;
            }
        }

        public decimal Salary
        {
            get
            {
                return salary;
            }
            set
            {
                // Allow 0 / negative values silently during JSON deserialization.
                // Business logic that creates employees via the constructor still
                // validates salary through the constructor parameter, so no real
                // employee will end up with an invalid salary at runtime.
                salary = value;
            }
        }

        public void SetSalaryValidated(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Invalid Salary.");
            salary = value;
        }

        public double ExperienceYears
        {
            get
            {
                return experienceyears;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Invalid Number.");
                }
                else
                {
                    experienceyears = value;
                }
            }
        }

        public bool IsCheckedIn { get; set; }
        public DateTime CheckInTime { get; set; }
        public decimal TotalDeductionAmmount { get; set; }

        public decimal NetSalary => Salary - TotalDeductionAmmount;

        public Employee(string name, int age, GenderType gender, string Nationalid, string phoneNumber, string email, string address, decimal salary, double experienceyears)
            : base(name, age, gender, Nationalid, phoneNumber, email, address)
        {
            Salary = salary;
            ExperienceYears = experienceyears;
            HospitalData.AddEmployee(this);
        }

        public void CheckIn()
        {
            if (!IsCheckedIn)
            {
                this.CheckInTime = DateTime.Now;
                IsCheckedIn = true;
                decimal dailydeductionammount = 0;

                lateness = (CheckInTime.TimeOfDay) - shiftstart;
                if (lateness > TimeSpan.Zero)
                {
                    dailydeductionammount = (salary / (30 * 8 * 60)) * (decimal)lateness.TotalMinutes;
                    TotalDeductionAmmount += dailydeductionammount;
                    Console.WriteLine($"You Are Late {lateness.TotalMinutes:f0} Minutes");
                }
                else
                {
                    Console.WriteLine("You Arrived On Time. Well Done!");
                }
                HospitalData.SaveEmployees();
            }
            else
            {
                Console.WriteLine("You Are Already Checked In.");
            }
        }

        public void CheckOut()
        {
            if (IsCheckedIn)
            {
                this.checkouttime = DateTime.Now;
                this.workedhours = checkouttime - CheckInTime;
                Console.WriteLine($"On {this.CheckInTime:dd-MM-yyyy}\n{this.Name} Checked In At :{this.CheckInTime:hh:mm:ss tt} | " +
                $"Checked Out At :{this.checkouttime:hh:mm:ss tt}\nGood Bye.");
                IsCheckedIn = false;
                HospitalData.SaveEmployees();
            }
            else
            {
                Console.WriteLine("Please Check In first.");
            }
        }

        public void CheckInandOut()
        {
            Console.WriteLine($"Hello {this.Name}\nChoose 1-Check IN | 2-Check Out");
            if (int.TryParse(Console.ReadLine(), out int chosenno))
            {
                switch (chosenno)
                {
                    case 1:
                    { 

                        CheckIn();
                        break; 

                    }
                    case 2:
                    {
                        CheckOut();
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Invalid choice.");
                        break; 
                    }
                }
            }
        }

        static public void ValidateId(int id)
        {
            var emp = employees.FirstOrDefault(e => e.EmployeeId == id);
            if (emp != null)
            {
                emp.CheckInandOut();
            }
            else
            {
                Console.WriteLine("You are not a registered employee.");
            }
        }
    }
}