using System;
using System.Collections.Generic;

namespace Hospital_System
{

    //made by Youssef Essam
    internal class Rooms
    {
        private static readonly List<Rooms> rooms = new List<Rooms>();
        private static readonly List<Rooms> AllRooms = rooms;



        private int roomNumber;
        bool isoccupied = false;
        public int RoomNumber
        {
            get { return roomNumber; }
            set
            {
                if (value <= 0 || value > 599)
                {
                    Console.WriteLine("Invalid Room Number");
                }
                roomNumber = value;
            }
        }

        public enum RoomType { General/*عام*/, ICU/*رعاية مركزة*/, OperatingTheater/*غرفة عمليات*/, Maternity/*غرفة ولادة*/, Incubator/*حضانة*/, Isolation/*غرف عزل*/ , Emergency };



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



        public Rooms(int roomNumber, RoomType type, int floorNumber)
        {

            int minRange = floorNumber * 100;
            int maxRange = minRange + 99;

            if (roomNumber < minRange || roomNumber > maxRange)
            {

                Console.WriteLine($"!!! WARNING: Room {roomNumber} cannot be on Floor {floorNumber}.");


                this.FloorNumber = roomNumber / 100;
                Console.WriteLine($"Correcting: Room {roomNumber} has been moved to Floor {this.FloorNumber}.");
            }
            else
            {
                this.FloorNumber = floorNumber;
            }


            this.RoomNumber = roomNumber;
            this.Type = type;
            this.isoccupied = false;

            AllRooms.Add(this);
        }



        //-----------------------------------------------------------
        //By Ahmed Hataba 
        public static void ShowAvailableRooms()
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