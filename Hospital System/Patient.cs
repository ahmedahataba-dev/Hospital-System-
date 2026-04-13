using System;

namespace Hospital_System
{
	public enum BloodGroup
	{
		A_Positive, A_Negative, B_Positive, B_Negative,
		AB_Positive, AB_Negative, O_Positive, O_Negative
	}

	internal class Patient : Person
	{
		// Constructor calling the base (Person) constructor
		public Patient(string name, int age, GenderType gender, string nationalId, string phoneNumber)
			: base(name, age, gender, nationalId, phoneNumber) { }

		public int PatientId { get; set; }
		public double Height { get; set; }
		public double Weight { get; set; }
		public string Allergies { get; set; }
		public string MedicalCase { get; set; }
		public BloodGroup BloodType { get; set; }
		public string MedicalHistory { get; set; }
		public string FamilyHistory { get; set; }
		public string Risk { get; set; }
		public string PaymentMethods { get; set; }

		public virtual void Display()
		{
			Console.WriteLine($"\n[ID: {PatientId}] | Name: {Name} | Age: {Age} | Gender: {Gender}");
			Console.WriteLine($"National ID: {NationalId} | Phone: {PhoneNumber}");
			Console.WriteLine($"Case: {MedicalCase} | Risk: {Risk} | Blood: {BloodType}");
			Console.WriteLine($"Height: {Height}m | Weight: {Weight}kg");
			Console.WriteLine($"Allergies: {Allergies} | Payment: {PaymentMethods}");
		}
	}
}