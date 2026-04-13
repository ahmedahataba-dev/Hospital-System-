using System;

namespace Hospital_System
{
	internal class Outpatient : Patient
	{
		public Outpatient(string name, int age, GenderType gender, string nationalId, string phoneNumber)
			: base(name, age, gender, nationalId, phoneNumber) { }

		public string ClinicName { get; set; }
		public double ConsultationFee { get; set; }
		public string ChiefComplaint { get; set; }

		public override void Display()
		{
			base.Display();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("      Outpatient Specific Info      ");
			Console.ResetColor();
			Console.WriteLine($"Clinic: {ClinicName} | Fee: {ConsultationFee} EGP");
			Console.WriteLine($"Complaint: {ChiefComplaint}");
			Console.WriteLine("--------------------------------------------------");
		}
	}
}