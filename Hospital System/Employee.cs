using System;
using System.Collections.Generic;
using System.Text;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Hospital_System
{
	internal class Employee: Person
    {
		static int employeeid_counter = 1;
		private decimal salary;
        private double arrivaltime;
        private double departuretime;
		private double experienceyears;
        private int employeeid;



        public double ArrivalTime {
			get { return arrivaltime; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Invalid Time .");
				}
                else{ 

				arrivaltime = value;
				}
			}
		}


		public double DepartureTime
		{
			get { return departuretime; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Invalid Time .");
				}
				else if (departuretime>arrivaltime)
				{

					departuretime = value;
				}
				
				

		
			}
		}


		public double ExperienceYears
		{
			get { return experienceyears; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Invalid Number .");
				}
				else
				{

					experienceyears = value;
				}
			}
		}





		public int EmployeeId
		{
			get { return employeeid; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Invalid ID .");
				}
				else
				{

					employeeid = value;
				}
			}
		}



		public decimal Salary
        {
            get => salary;
            set
            {
                if (value <= 0) throw new ArgumentException("Invalid Salary .");
                salary = value;
			}
		}








		public Employee(string name,int age , GenderType gender, string Nationalid, string phoneNumber, decimal salary ,double arrivaltime,double departuretime, double experienceyears/*, int employeeid*/)
			: base(name,age,gender, Nationalid, phoneNumber)
		{
			Salary = salary;
			ArrivalTime = arrivaltime;
			DepartureTime = departuretime;
			ExperienceYears = experienceyears;
		   EmployeeId = employeeid_counter;
			employeeid_counter++;



		}
	}
}
