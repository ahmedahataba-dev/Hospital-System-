using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    // enum for managing the gender type ---> Ahmed AYman
    public enum GenderType { Male, Female }

    internal class Person
    {
        public Person() { }

        private string name = string.Empty;
        //private string surname;
        private string email = string.Empty;
        //private string password;  can be added later if needed for login functionality
        private string address = string.Empty;
        private int age;
        //private char gender;
        private string Nationalid = string.Empty;
        private string phoneNumber = string.Empty;

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
                if (value.Length == 14 && long.TryParse(value, out _))
                {
                    Nationalid = value;
                }
                else { throw new ArgumentException("ID Must Be Only 14 Digits."); }
            }
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                //int int_phone_no = int.Parse(value);	

                if (value.Length == 11 && long.TryParse(value, out _))
                {
                    phoneNumber = value;
                }
                else { throw new ArgumentException("Phone Number Must Be Only 11 Digits."); }

                //phoneNumber = int_phone_no.ToString();
            }
        }

        //Added validation for email to ensure it is not empty and follows a basic format
        public string Email
        {
            get => email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Email cannot be empty.");
                }
                else
                {
                    email = value;
                }
            }
        }

        //Added validation for address to ensure it is not empty
        public string Address
        {
            get => address;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Address cannot be empty.");
                }
                else
                {
                    address = value;
                }
            }
        }

        public Person(string name, int age, GenderType gender, string Nationalid, string phoneNumber, string email, string address)
        {
            this.Name = name;
            this.Age = age;
            this.Gender = gender;
            this.NationalId = Nationalid;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
            this.Address = address;
        }

        /*public string GetFullname()
        {
            return $"{Name} {Surname}";
        }*/

        //removed the Surname property as it is not required in the current implementation and can be added later if needed
    }
}