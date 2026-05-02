using System;

namespace Hospital_System
{
    // Made by Ahmed Essam
    internal class Inpatient : Patient
    {
        public bool HasOperations { get; set; }
        public string OperationType { get; set; }
        public string WardName { get; set; }
        public int RoomNumber { get; set; }
        public int BedNumber { get; set; }
        public string Medication { get; set; }
        public string AttendingPhysician { get; set; }
        public string DietPlan { get; set; }

        //public override void Display()
        //{
        //    base.Display();
        //    Console.ForegroundColor = ConsoleColor.Cyan;
        //    Console.WriteLine("      Inpatient Specific Info      ");
        //    Console.ResetColor();
        //    Console.WriteLine($"Ward: {WardName} | Room: {RoomNumber} | Bed: {BedNumber}");
        //    Console.WriteLine($"Physician: {AttendingPhysician} | Diet: {DietPlan}");
        //    if (HasOperations) Console.WriteLine($"Operation: {OperationType}");
        //    Console.WriteLine("--------------------------------------------------");
        //}
    }
}
