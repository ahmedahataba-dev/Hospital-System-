using System;
using System.Collections.Generic;

namespace Hospital_System
{
	internal class PharmacyStaff : Employee
	{
		public static List<PharmacyStaff> pharmacists = new List<PharmacyStaff>();
		public static int pharmacyStaffIdCounter = 1;
		private int pharmacyStaffId;

		public PharmacyStaff() : base() { }

		public int PharmacyStaffId
		{
			get { return pharmacyStaffId; }
			set { pharmacyStaffId = value; }
		}
		public string Role { get; set; }

		public PharmacyStaff(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email, string address, decimal salary, double experienceYears, string role)
			: base(name, age, gender, nationalId, phoneNumber, email, address, salary, experienceYears)
		{
			PharmacyStaffId = pharmacyStaffIdCounter++;
			Role = role;
		}
	}
}