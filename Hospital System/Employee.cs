using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
	internal class Employee: Person
    {
		private decimal salary;
        public decimal Salary
        {
            get => salary;
            set
            {
                if (value <= 0) throw new ArgumentException("Salary cannot be negative and must be greater than zero.");
                    salary = value;
            }
        }
        public Employee(string name, string surname, int age, char gender, long Nationalid, long phoneNumber, decimal salary)
            : base(name, surname, age, gender, Nationalid, phoneNumber)
        {
            this.Salary = salary;
        }
    }
}
