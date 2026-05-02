using System;
using System.Linq;

namespace Hospital_System
{
    // InputHelper was a console-only utility class.
    // All user input in this application is collected through Windows Forms controls.
    internal class InputHelper
    {
        public static int ReadInt(string prompt)
        {
            throw new NotSupportedException("InputHelper is not used in the Windows Forms application.");
        }

        public static double ReadDouble(string prompt)
        {
            throw new NotSupportedException("InputHelper is not used in the Windows Forms application.");
        }

        public static string ReadString(string prompt)
        {
            throw new NotSupportedException("InputHelper is not used in the Windows Forms application.");
        }

        public static string ReadLettersOnly(string prompt)
        {
            throw new NotSupportedException("InputHelper is not used in the Windows Forms application.");
        }
    }
}
