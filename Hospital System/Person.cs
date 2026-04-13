using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{

// enum for managing the gender type ---> Ahmed AYman
public enum GenderType{ Male,Female }






	internal class Person
	{

		private string name;
		//private string surname;
		private int age;
		//private char gender;
		private string Nationalid;
		private string  phoneNumber;


		//Patient name Property
		public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be empty.");
                }
                else
                {
                    name = value;
                }
            }
		}
		/*public string Surname
        {
            get => surname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Surname cannot be empty.");
                }
                else
                {
                    surname = value;
                }
            }
        }*/




		//Patient name Age

		public int Age
		{
			get { return age; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Age cannot be negative and must be greater than zero.");
				}
				if (value > 150) { throw new ArgumentException("Age cannot be greater than 150."); }

				age = value;

			}

		}


		//Gender Enum to chose only Male or Female 

		public GenderType Gender { set; get; }


		//Patient name Gender

		//public char Gender
		//{
		//	get { return gender; }
		//	set
		//	{
		//		char input = char.ToUpper(value);
		//		if (input == 'M' || input == 'F' )
		//		{
		//			gender = input;
		//		}
		//		else
		//		{
		//			throw new ArgumentException("Gender must be 'M' or 'F'.");
		//		}
		//	}
		//}
		public string NationalId
		{
			get => Nationalid;
			set
			{
				if (value.Length ==14 && value.All(char.IsDigit) && long.TryParse(value,out _))
				{
                     Nationalid = value;
				}
				else { throw new ArgumentException("ID Must Be 14 Digits only ."); }
				
            }
		}
		public string PhoneNumber
		{
			get => phoneNumber;
			set
			{
				//int int_phone_no = int.Parse(value);	

				if (value.Length == 11 &&value.All(char.IsDigit)&& long.TryParse(value, out _))
				{
					phoneNumber = value;
				}
				else { throw new ArgumentException("Phone Number Must be 11 Digits only."); }

				//phoneNumber = int_phone_no.ToString();

			}







		}
		public Person(string name,int age, GenderType gender, string Nationalid, string phoneNumber)
		{
			this.Name = name;
			//this.Surname = surname;
			this.Age = age;
			this.Gender = gender;
			this.NationalId = Nationalid;
			this.PhoneNumber = phoneNumber;
		}
		/*public string GetFullname()
		{
			return $"{Name} {Surname}";
		}*/
    }
}
