using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
	internal class Person
	{
		private string name;
		private string surname;
		private int age;
		private char gender;
		private string id;
		private string phoneNumber;
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
        public string Surname
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
        }
        public int Age {
			get { return age; }
			set
			{
                if (value < 0)
                {
                    throw new ArgumentException("Age cannot be negative.");
                }
                if (value > 150) { throw new ArgumentException("Age cannot be greater than 150."); }

					age = value;
               
            }

            }
		public char Gender
		{
			get { return gender; }
			set
			{
				char input = char.ToUpper(value);
				if (input == 'M' || input == 'F')
				{
					gender = input;
				}
				else
				{
					throw new ArgumentException("Gender must be 'M' or 'F'.");
				}
			}
		}
		public string Id
		{
			get => id;
			set
			{
				if (string.IsNullOrWhiteSpace(value)&&value.Length!=14)
				{
                    throw new ArgumentException("ID cannot be empty and must be 14 characters.");
                }
				id = value;
            }
        }
        public string PhoneNumber
		{
			get => phoneNumber;
			set
			{
							if (string.IsNullOrWhiteSpace(value)||value.Length!=11)
				{
					throw new ArgumentException("Phone number cannot be empty and must be 11 digits.");
				}
				
					phoneNumber = value;
                
            }
		}
        public Person(string name, string surname, int age, char gender, string id, string phoneNumber)
		{
			this.Name = name;
			this.Surname = surname;
			this.Age = age;
			this.Gender = gender;
			this.Id = id;
			this.PhoneNumber = phoneNumber;
		}
		public string GetFullname()
		{
			return $"{Name} {Surname}";
		}
    }
}
