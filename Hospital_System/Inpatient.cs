using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Hospital_System
{
    class Inpatient : Patient
    {
#nullable enable
        public bool HasOperations { get; set; }
        public string? OperationType { get; set; }
        public string? WardName { get; set; }
        public int RoomNumber { get; set; }
        public int BedNumber { get; set; }
        public string? Medication { get; set; }
        public string? AttendingPhysician { get; set; }
        public string? DietPlan { get; set; }


        public override void Display()
        {
            base.Display();
            Console.WriteLine("      Inpatient Specific Info      ");
            Console.WriteLine($"Ward: {WardName} \nRoom: {RoomNumber} \nBed: {BedNumber}");
            Console.WriteLine($"Physician: {AttendingPhysician} \nDiet: {DietPlan}");
            if (HasOperations) Console.WriteLine($"Operation: {OperationType}");
            Console.WriteLine("--------------------------------------------------");
        }




    }

}



