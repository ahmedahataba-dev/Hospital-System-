using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
   
namespace Hospital_System
    {
        internal class ExternalDepartments
        {
            public bool HasGarden { get; set; } = true;
            public int AmbulanceCount { get; set; }
            public int BloodBankCapacity { get; set; }
            
            public Pharmacy HospitalPharmacy { get; set; }
            public ExternalDepartments(int ambulanceCount, int bloodBankCapacity)
            {
                AmbulanceCount = ambulanceCount;
                BloodBankCapacity = bloodBankCapacity;
                HospitalPharmacy=new Pharmacy();
            }

            public void ShowExternalFacilities()
            {
                Console.WriteLine("\n--- External Facilities ---");
                Console.WriteLine($"Garden Available: {(HasGarden ? "Yes" : "No")}");
                Console.WriteLine($"Ambulances on Standby: {AmbulanceCount}");
                Console.WriteLine($"Blood Bank Capacity: {BloodBankCapacity} units");
                Console.WriteLine($"Pharmacy Categories Loaded: {HospitalPharmacy.categories.Count}");
            }
        }
    }
}

