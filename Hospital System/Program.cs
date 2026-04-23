using Hospital_System;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
// [STAThread] // You only need this if you are running a Windows Forms UI
// ==========================================
// Ahmed Hataba's Tests (Console Environment)
// ==========================================
Employee e1 = new Employee("Youssef", 19, GenderType.Male, "30704211200695", "01029611625", "youssef@example.com", "Mansoura", 15000, 5); e1.CheckInandOut(e1);

Console.WriteLine("\n-----------------------------------\n");

// ==========================================
// Youssef's Tests (Hospital & Rooms Environment)
// ==========================================

//Initialize the hospital
Hospital neurai = new Hospital("NeurAi Medical Center");
Doctor.HireInitialDoctors(neurai);
Hospital.RunMainMenu(neurai);

    //-------------------------------------------------------------------\\

