using System;
using System.Collections.Generic;

namespace Hospital_System
{
	internal class Program
	{
		// [STAThread] // You only need this if you are running a Windows Forms UI
		//static void Main()
		//{
		//	HospitalData.ExtractEmployees();

		//	Employee e1 = new Employee("Khaled", 19, GenderType.Male, "30702181200491", "01025920864", "kh@example.com", "meetghamr", 15000, 4.5);
		//	Employee e2 = new Employee("Sayed", 19, GenderType.Male, "30701481251478", "01025920864", "sy@example.com", "meet mahmoud", 1000, 3);
		//	HospitalData.SaveEmployees();

		//	foreach (var emp in Employee.employees)
		//	{
		//		Console.WriteLine($"ID: {emp.EmployeeId} | Name: {emp.Name} | Salary: {emp.Salary}");
		//	}



		//	//while (true){

		//	//	Console.Write("Please Enter Your ID To Check In/Out :");
		//	//int.TryParse(Console.ReadLine().Trim(),out int enteredid);
		//	//	Employee.ValidateId(enteredid);
		//	//}



		//}

		static void Main()
		{
			HospitalData.ExtractEmployees();

			//if (Employee.employees.Count == 0)
			//{
			//	Console.WriteLine("System is empty. Seeding initial data...");
			//	new Employee("Khaled", 19, GenderType.Male, "30702181200491", "01025920864", "kh@example.com", "meetghamr", 15000, 4.5);
			//	new Employee("Sayed", 19, GenderType.Male, "30701481251478", "01025920864", "sy@example.com", "meet mahmoud", 1000, 3);

			//	HospitalData.SaveEmployees();
			//}
			//Employee e1=new Employee("ahmed", 19, GenderType.Male, "30701181200491", "01025920863", "ahmed@example.com","cairo", 30000, 4.5);
			HospitalData.SaveEmployees();


			Console.WriteLine("\n--- Current Registered Employees ---");
			foreach (var emp in Employee.employees)
			{
				Console.WriteLine($"ID: {emp.EmployeeId} | Name: {emp.Name} | Salary: {emp.Salary}");
			}
		}
	}
}