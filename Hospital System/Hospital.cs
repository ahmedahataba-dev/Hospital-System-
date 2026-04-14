using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    internal class Hospital
    {
        public string Name { get; set; }= "NeurAi";
        public List<Floors> Floor { get; set; }= new List<Floors>();
        public void AddFloor(Floors floor)
        {
            Floor.Add(floor);
        }
    }
}
