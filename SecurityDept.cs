using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital_System
{


    internal class SecurityDepartment : Department
    {
        // A specific list just for guards
        public List<Security> Guards { get; private set; } = new List<Security>();

        public bool IsLockdownActive { get; private set; } = false;

        public SecurityDepartment() : base("Security")
        {
        }

        public void HireGuard(Security guard)
        {
            Guards.Add(guard);
            Console.WriteLine($"[Security] Guard {guard.Name} (Badge: {guard.BadgeNumber}) hired for {guard.Shift} shift.");
        }


        public void PatrolFloor(int floorNumber, Floors[] hospitalFloors)
        {
            if (floorNumber >= 1 && floorNumber <= hospitalFloors.Length)
            {
                var targetFloor = hospitalFloors[floorNumber - 1];
                Console.WriteLine($"\n[Security] Patrolling Floor {floorNumber}...");
                Console.WriteLine($"Status: {(targetFloor.IsOperating ? "Clear and Safe" : "Restricted Area")}");
                Console.WriteLine($"Elevator Access: {(targetFloor.hasElevatorAccess ? "Monitored" : "Secured")}");
            }
            else
            {
                Console.WriteLine("[Security] Invalid floor number for patrol.");
            }
        }
        public void ToggleLockdown()
        {
            IsLockdownActive = !IsLockdownActive;
            if (IsLockdownActive)
            {
                Console.WriteLine("\n[!!!] ALARM: HOSPITAL LOCKDOWN INITIATED [!!!]");
                Console.WriteLine("All external doors locked. Elevators restricted to emergency personnel only.");
            }
            else
            {
                Console.WriteLine("\n[+] LOCKDOWN LIFTED. Hospital returning to normal operations.");
            }
        }
        public void HireSubstituteGuard(string name, string shiftTime)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("\n[!] Invalid name. The name cannot be empty.");
                return;
            }
            if (!name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                Console.WriteLine("\n[!] Invalid name. Names can only contain letters and spaces (no numbers!).");
                return;
            }
            // ------------------------------------------
            if (!string.IsNullOrWhiteSpace(shiftTime))
            {
                shiftTime = char.ToUpper(shiftTime[0]) + shiftTime.Substring(1).ToLower();
            }
            if (shiftTime != "Day" && shiftTime != "Evening" && shiftTime != "Night")
            {
                Console.WriteLine("\n[!] Invalid shift. You must enter 'Day', 'Evening', or 'Night'.");
                return;
            }
            int nextBadgeNumber = Guards.Count + 1;
            string autoBadgeString = $"SEC-{nextBadgeNumber:D3}";

            try
            {
                Security newGuard = new Security(name, autoBadgeString, shiftTime);
                Guards.Add(newGuard);

                Console.WriteLine($"\n[+] SUCCESS: Substitute Guard '{name}' hired!");
                Console.WriteLine($"    Assigned Badge: {autoBadgeString} | Shift: {shiftTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[!] Failed to hire guard: {ex.Message}");
            }
        }
        protected internal static void RunSecurityMenu(Hospital neurai)
        {
            bool securityMenu = true;
            while (securityMenu)
            {
                Console.WriteLine("\n=== SECURITY HEADQUARTERS ===");
                Console.WriteLine($"Current Status: {(neurai.CampusSecurity.IsLockdownActive ? "LOCKDOWN" : "SECURE")}");
                Console.WriteLine("1: Dispatch Guard to Patrol a Floor");
                Console.WriteLine("2: View Active Guard Roster");
                Console.WriteLine("3: Toggle Hospital Lockdown");
                Console.WriteLine("4: Hire Substitute Guard");
                Console.WriteLine("0: Return to Main Menu");
                Console.Write("Command: ");

                string secChoice = Console.ReadLine() ?? string.Empty;

                if (secChoice == "1")
                {
                    Console.Write("Enter Floor Number to Patrol (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int floorToPatrol))
                    {
                        neurai.CampusSecurity.PatrolFloor(floorToPatrol, neurai.Floors);
                    }
                }
                else if (secChoice == "2")
                {
                    Console.WriteLine("\n--- Active Guards ---");
                    foreach (var guard in neurai.CampusSecurity.Guards)
                    {
                        Console.WriteLine($"- Officer {guard.Name} | Badge: {guard.BadgeNumber} | Shift: {guard.Shift}");
                    }
                }
                else if (secChoice == "3")
                {
                    neurai.CampusSecurity.ToggleLockdown();
                }
                else if (secChoice == "4")
                {
                    Console.WriteLine("\n--- Hire Substitute Guard ---");
                    Console.Write("Enter Guard Name: ");
                    string newName = Console.ReadLine() ?? string.Empty;

                    Console.Write("Enter Shift Time (Day, Evening, Night) : ");
                    string newShift = Console.ReadLine() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(newName) && !string.IsNullOrWhiteSpace(newShift))
                    {
                        neurai.CampusSecurity.HireSubstituteGuard(newName, newShift);
                    }
                    else
                    {
                        Console.WriteLine("\n[!] Invalid input. Name and Shift cannot be empty.");
                    }
                }
                else if (secChoice == "0")
                {
                    securityMenu = false;
                    Console.WriteLine("\nLogging out of Security Terminal...");
                }
            }
        }
    }
}