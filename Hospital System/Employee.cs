using System;
using System.Collections.Generic;
using System.IO;//using static System.Windows.Forms.VisualStyles.VisualStyleElement;// By Ahmed Hatabanamespace Hospital_System
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
namespace Hospital_System
{
	internal class Employee : Person
	{
		public static int employeeid_counter = 1;
		private decimal salary;//basic salary if there is no deductions 
		private DateTime checkintime;
		private DateTime checkouttime;
		private TimeSpan workedhours;
		private TimeSpan lateness;
		private TimeSpan shiftstart = new TimeSpan(9, 0, 0);
		private double experienceyears;
		private int employeeid;
		private bool ischeckedin;
		//list includes All employees
		public static List<Employee> employees = new List<Employee>();


		//      public double ArrivalTime {
		//	get { return arrivaltime; }
		//	set
		//	{
		//		if (value <= 0)
		//		{
		//			throw new ArgumentException("Invalid Time .");
		//		}
		//              else{ 

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

		public bool IsCheckedIn { get; set; }
		public DateTime CheckInTime { get; set; }
		//public decimal NetSalary
		//{
		//	get => netsalary;
		//	set
		//	{
		//		if (value <= 0) throw new ArgumentException("Invalid Salary .");
		//		netsalary = value;
		//	}
		//}
		public decimal TotalDeductionAmmount { set; get; }

		public decimal NetSalary => Salary - TotalDeductionAmmount;


		public Employee(string name, int age, GenderType gender, string Nationalid, string phoneNumber, string email, string address, decimal salary, double experienceyears/*, int employeeid*/)
			: base(name, age, gender, Nationalid, phoneNumber, email, address)
		{
			Salary = salary;
			ExperienceYears = experienceyears;
			//employees.Add(this);
			HospitalData.AddEmployee(this);

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
				Console.WriteLine("u are not a registered employee .");
			}
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
					//netsalary = salary - totaldeductionammount;
					Console.WriteLine($"You Are Late {lateness.TotalMinutes:f0} Minutes");
				}
				else
				{
					Console.WriteLine("You Arrived On Time . Well Done!");
				}
			}
			else Console.WriteLine("You Are Already Checked In .");
			HospitalData.SaveEmployees();//saves the public attributes in the "employee.json" file
		}

		public void CheckOut()
		{
			if (IsCheckedIn)
			{
				this.checkouttime = DateTime.Now;
				this.workedhours = checkouttime - CheckInTime;
				Console.WriteLine($"On {this.CheckInTime:dd-MM-yyyy}\n{this.Name} Checked In At :{this.CheckInTime:hh:mm:ss tt}	|	" +
				$"Checked Out At :{this.checkouttime:hh:mm:ss tt}\nGood Bye .");
				IsCheckedIn = false;
			}
			else Console.WriteLine("Please Check In first .");
		}

		public void CheckInandOut()
		{
			//Console.Write($"Please Enter Your ID {this.Name} : ");
			Console.WriteLine($"Hello {this.Name} Do You Want To Check In Or out \nChoose 1-Check IN				2-Check Out  ");

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
