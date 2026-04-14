using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace Hospital_System
{
	
		//made by Youssef Essam
		internal class Room 
    {
		private static List<Room> AllRooms = new List<Room>();



		private int roomNumber;
        bool isoccupied = false;
        public int RoomNumber
        {
            get { return roomNumber; }
            set
            {
                if (value <= 0 || value > 500)
                {
                    Console.WriteLine("Invalid Room Number");
                }
                roomNumber = value;
            }
        }

        public enum RoomType { General/*عام*/, ICU/*رعاية مركزة*/, OperatingTheater/*غرفة عمليات*/, Maternity/*غرفة ولادة*/, Incubator/*حضانة*/, Isolation/*غرف عزل*/ };



        public enum Roomstate { Occupied, Available }
        public string IsOccupied
        {
            set
            {

            }

            get
            {
                return $"room number {roomNumber} is {(isoccupied ? "Occupied " : "Available")} ";

            }

        }



        public RoomType Type { get; set; }



        public string AssignedPatient { get; set; } = "None";



        public string AssignedDoctor { get; set; } = "None";



        public int FloorNumber { get; set; }



        public DateTime LastSanitizedDate { get; set; } = DateTime.Now;



        public Room(int roomNumber, RoomType type, int floorNumber)
        {
            RoomNumber = roomNumber;
            Type = type;
            FloorNumber = floorNumber;
            AllRooms.Add(this);
            isoccupied = false;
        }

        

        //-----------------------------------------------------------
        //By Ahmed Hataba 
        public static void  ShowAvailableRooms()
        {
            int availableroomscount = 0;
			System.Windows.Forms.MessageBox.Show(("Available Rooms Report ."));
            foreach (var room in AllRooms)
            {
                if (!room.isoccupied)
                {
                    System.Windows.Forms.MessageBox.Show(($"Room Number {room.roomNumber} {room.Type} on Floor {room.FloorNumber} Is Available ."));
                    availableroomscount++;
                }

            }
                if (availableroomscount == 0)
                {
                    System.Windows.Forms.MessageBox.Show(("No Available Rooms Now."));
                }
                else { System.Windows.Forms.MessageBox.Show(($"There Are {availableroomscount} Room Available .")); }


			
        }

        


    }
}