using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_System
{
    public partial class program
    {
        //  عشان نضمن إن اليوزر دخل رقم صحيح مش حروف
        static int ReadInt(string Int)
        {
            while (true)
            {
                Console.Write(Int);

                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;

                Console.WriteLine("Error! Please enter a valid number.");
            }
        }

        // عشان الارقام العشرية
        static double ReadDouble(string Double)
        {
            while (true)
            {
                Console.Write(Double);
                if (double.TryParse(Console.ReadLine(), out double result))
                    return result;
                Console.WriteLine("Error! Please enter a valid decimal number.");
            }
        }

        //  عشان نضمن إن الخانة متتسابش فاضية
        static string ReadString(string empty)
        {
            while (true)
            {
                Console.Write(empty);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("Error! This field cannot be empty.");
            }
        }

        // بتخلي اليوزر يدخل حروف ومسافات بس 
        static string ReadLettersOnly(string letters)
        {
            while (true)
            {
                Console.Write(letters);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return input;
                Console.WriteLine("Error! Only letters and spaces are allowed.");
            }
        }


        //static bool ReadBool(string prompt)
        //{
        //    while (true)
        //    {
        //        Console.Write(prompt + " (y/n): ");
        //        string input = (Console.ReadLine() ?? "").ToLower();

        //        if (input == "y")
        //            return true;
        //        if (input == "n")
        //            return false;

        //        Console.WriteLine("Invalid input! Please enter 'y' for Yes or 'n' for No.");
        //    }
        //}
    }
}