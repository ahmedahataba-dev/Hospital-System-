using System.Collections.Generic;

namespace Hospital_System
{
    public class ClinicService
    {
        public Dictionary<int, Clinic> Clinics = new Dictionary<int, Clinic>();

        public ClinicService()
        {
            Clinics[1] = new Clinic(1, "General Medicine", new[] { "Saturday", "Monday", "Wednesday" });
            Clinics[2] = new Clinic(2, "Internal Medicine", new[] { "Sunday", "Tuesday", "Thursday" });
            Clinics[3] = new Clinic(3, "Neurology", new[] { "Saturday", "Tuesday" });
            Clinics[4] = new Clinic(4, "Ophthalmology", new[] { "Sunday", "Wednesday" });
            Clinics[5] = new Clinic(5, "Physical Medicine", new[] { "Monday", "Thursday" });
            Clinics[6] = new Clinic(6, "ENT", new[] { "Saturday", "Wednesday" });
            Clinics[7] = new Clinic(7, "Cardiology", new[] { "Sunday", "Tuesday", "Thursday" });
            Clinics[8] = new Clinic(8, "Orthopedics", new[] { "Monday", "Wednesday" });
            Clinics[9] = new Clinic(9, "Pulmonology", new[] { "Saturday", "Thursday" });
            Clinics[10] = new Clinic(10, "Dermatology", new[] { "Sunday", "Monday" });
            Clinics[11] = new Clinic(11, "Emergency", new[] { "Daily" });
        }
    }
}