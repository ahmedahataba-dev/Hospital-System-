using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Linq;


//By Ahmed Hataba
namespace Hospital_System
{
	internal class HospitalData
	{
		private const string employeeFileName = "employees.json";

		public static void AddEmployee(Employee newemp) 
		{
			if (!Employee.employees.Any(e=>e.NationalId == newemp.NationalId))
			{
			Employee.employees.Add(newemp);
				newemp.EmployeeId = Employee.employeeid_counter;

				Employee.employeeid_counter++;
			}
			else
			{
				Console.WriteLine($"Warning: Employee with ID {newemp.NationalId} already exists.");
			}

		}

		public static void SaveEmployees()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string serializedemployeeslist=JsonSerializer.Serialize(Employee.employees,options);
			
			File.WriteAllText(employeeFileName,serializedemployeeslist);

		}
		public static void ExtractEmployees()
		{
			if (File.Exists(employeeFileName))
			{
			string deserializedemployeeslist= File.ReadAllText(employeeFileName);
				Employee.employees=JsonSerializer.Deserialize<List<Employee>>(deserializedemployeeslist);
				if (Employee.employees.Any())
				{
					int max_id = Employee.employees.Max(e => e.EmployeeId);
				
				Employee.employeeid_counter = max_id+1;
				}
			}
		}



	}
}
