using System;
using System.Collections.Generic;
using System.Linq; 

namespace Hospital_System
{
    internal class Department
    {
        public string DeptName { get; set; }
        public List<Room> Rooms { get; private set; } = new List<Room>();
        public List<Doctor> Doctors { get; private set; } = new List<Doctor>();
        public List<Nurse> Nurses { get; private set; } = new List<Nurse>();

        //--- DEVICE MANAGEMENT --->ENABLE THIS PROPERTY WHEN YOU HAVE YOUR DEVICE CLASS READY  
        // public List<Device> Devices { get; private set; } = new List<Device>();
        public Department(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department must have a name.");

            DeptName = name;
        }
        
        public void AddRoom(int roomNumber, Room.RoomType type, int floorNumber)
        {
            Room newRoom = new Room(roomNumber, type, floorNumber);
            Rooms.Add(newRoom);
            Console.WriteLine($"[+] Room {roomNumber} ({type}) added to {DeptName}.");
        }

        public void AssignDoctor(Doctor doctor)
        {
            Doctors.Add(doctor);
            Console.WriteLine($"[+] Dr. {doctor.Name} assigned to {DeptName}.");
        }

        public void AssignNurse(Nurse nurse)
        {
            Nurses.Add(nurse);
            Console.WriteLine($"[+] Nurse {nurse.Name} assigned to {DeptName}.");
        }
        public void AssignDoctorToRoom(string doctorName, int roomNumber)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            var doctor = Doctors.FirstOrDefault(d => d.Name.Contains(doctorName));

            if (room != null && doctor != null)
            {
                room.AssignedDoctor = doctor.Name;
                Console.WriteLine($"[=>] Dr. {doctor.Name} is now managing Room {roomNumber}.");
            }
            else
            {
                Console.WriteLine("[!] Error: Either the room or the doctor was not found in this department.");
            }
        }

        // The pharmacy link
        public void ShowDepartmentMedicines(Pharmacy hospitalPharmacy)
        {
            Console.WriteLine($"\n--- {DeptName} Department Medicine Stock ---");
            if (hospitalPharmacy.categories.ContainsKey(this.DeptName))
            {
                foreach (var med in hospitalPharmacy.categories[this.DeptName])
                {
                    Console.WriteLine($"- {med.Name} (Qty: {med.Quantity})");
                }
            }
            else
            {
                Console.WriteLine($"No specific medicines found for {DeptName}. Check General pharmacy.");
            }
        }


        //------------------------------------------------------------------------------------------------------\\



        // --- DEVICE MANAGEMENT ---> enable this method when you have your Device class ready
        /*public void AddDevice(Device device)
        {
            Devices.Add(device);
            Console.WriteLine($"[+] Device '{device.Name}' added to {DeptName}.");
        }*/

        public void ShowDepartmentReport()
        {
            Console.WriteLine($"\n=== {DeptName} Department Report ===");
            Console.WriteLine($"Staff: {Doctors.Count} Doctors, {Nurses.Count} Nurses");
            // Console.WriteLine($"Facilities: {Rooms.Count} Rooms, {Devices.Count} Devices");==> enable this line when you have your Device class ready

            Console.WriteLine("\nRooms:");
            foreach (var room in Rooms)
            {
                Console.WriteLine($" - Room {room.RoomNumber} [{room.Type}] | Doctor: {room.AssignedDoctor}");
            }
        }
    }
}