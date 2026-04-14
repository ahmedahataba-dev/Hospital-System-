using System;
using System.Collections.Generic;
using System.Text;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
// By Ahmed Hataba

namespace Hospital_System
{
	internal class Employee : Person
	{
		static int employeeid_counter = 1;
		private decimal salary;
		private DateTime checkintime;
		private DateTime checkouttime;
		private TimeSpan workedhours;
		private double experienceyears;
		private int employeeid;
		private bool ischeckedin;

		//      public double ArrivalTime {
		//	get { return arrivaltime; }
		//	set
		//	{
		//		if (value <= 0)
		//		{
		//			throw new ArgumentException("Invalid Time .");
		//		}
		//              else{ 

		//		arrivaltime = value;
		//		}
		//	}
		//}

		//public double DepartureTime
		//{
		//	get { return departuretime; }
		//	set
		//	{
		//		if (value <= 0)
		//		{
		//			throw new ArgumentException("Invalid Time .");
		//		}
		//		else if (departuretime>arrivaltime)
		//		{

		//			departuretime = value;
		//		}

		//	}
		//}

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

		public Employee(string name, int age, GenderType gender, string Nationalid, string phoneNumber, string email, string address, decimal salary, double experienceyears/*, int employeeid*/)
			: base(name, age, gender, Nationalid, phoneNumber, email, address)
		{
			Salary = salary;
			ExperienceYears = experienceyears;
			EmployeeId = employeeid_counter;
			employeeid_counter++;
		}

		public void CheckIn()
		{
			if (!ischeckedin)
			{
				this.checkintime = DateTime.Now;
				ischeckedin = true;
			}
			else Console.WriteLine("You Are Already Checked In .");
		}

		public void CheckOut()
		{
			if (ischeckedin)
			{
				this.checkouttime = DateTime.Now;
				this.workedhours = checkouttime - checkintime;
				Console.WriteLine($"On {this.checkintime:dd-MM-yyyy}\n{this.Name} Checked In At :{this.checkintime:hh:mm:ss tt}	|	" +
				$"Checked Out At :{this.checkouttime:hh:mm:ss tt}\n Good Bye .");
				ischeckedin = false;
			}
			else Console.WriteLine("Please Check In first .");
		}

		public void CheckInandOut(Employee emp)
		{
			Console.Write($"Please Enter Your ID {this.Name} : ");
			Console.WriteLine($"Hello {this.Name} Do You Want To Check In Or out \nChoose 1-Check IN				2-Check Out  ");

			if (int.TryParse(Console.ReadLine(), out int chosenno)) //try parse to avoid wrong input crash 
			{
				switch (chosenno)
				{
					case 1:
						CheckIn();
						Console.WriteLine("Checked In Successfully");
						break;
					case 2:
						CheckOut();
						Console.WriteLine($"Total Time Worked: {this.workedhours.Hours}h {this.workedhours.Minutes}m");
						break;
					default:
						Console.WriteLine("Invalid Number Choosen");
						break;
				}
			}

			/*this.checkouttime = DateTime.Now;
			this.workedhours = checkouttime - checkintime;
			Console.WriteLine($"On {this.checkintime:dd-MM-yyyy}\n{this.Name} Checked In At :{this.checkintime:hh:mm:ss}	|	" +
			$"Checked Out At :{this.checkouttime:hh:mm:ss}");
*/
		}
	}
}