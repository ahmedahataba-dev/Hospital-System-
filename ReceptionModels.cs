using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Hospital_System
{
    // ── Walk-in patient (reception desk) ──────────────────
    public class HospitalBooking
    {
        public string NameEnglish { get; set; } = "";
        public string NationalID { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    // ── Walk-in booking record ─────────────────────────────
    public class ReceptionBooking
    {
        public string PatientID { get; set; } = "";
        public string ClinicName { get; set; } = "";
        public string Day { get; set; } = "";
        public string Code { get; set; } = "";
    }

    // ── Reception service  ──
    public class ReceptionService
    {
        public List<HospitalBooking> AllPatients = new List<HospitalBooking>();
        public List<ReceptionBooking> AllBookings = new List<ReceptionBooking>();

        private readonly string _pFile = DataStore.PatRec;
        private readonly string _bFile = DataStore.BookRec;

        public readonly string[] Clinics =
        {
            "Cardiology", "Pulmonology", "General Medicine", "Orthopedics",
            "Internal Medicine", "Neurology", "Ophthalmology",
            "Physical Medicine", "ENT", "Dermatology", "General Surgery"
        };

        public void LoadData()
        {
            if (File.Exists(_pFile))
            {
                string j = File.ReadAllText(_pFile);
                AllPatients = JsonSerializer.Deserialize<List<HospitalBooking>>(j)
                              ?? new List<HospitalBooking>();
            }
            if (File.Exists(_bFile))
            {
                string j = File.ReadAllText(_bFile);
                AllBookings = JsonSerializer.Deserialize<List<ReceptionBooking>>(j)
                              ?? new List<ReceptionBooking>();
            }
        }

        private void SavePatients()
        {
            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_pFile, JsonSerializer.Serialize(AllPatients, opt));
        }

        private void SaveBookings()
        {
            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_bFile, JsonSerializer.Serialize(AllBookings, opt));
        }

        public void SaveAll()
        {
            SavePatients();
            SaveBookings();
        }

        public void AddPatientIfNew(HospitalBooking p)
        {
            if (AllPatients.Find(x => x.NationalID == p.NationalID) == null)
            {
                AllPatients.Add(p);
                SavePatients();
            }
        }

        public HospitalBooking FindPatient(string id) =>
            AllPatients.Find(p => p.NationalID == id);

        public ReceptionBooking CreateBooking(string patId, string clinic, string day)
        {
            string code = new System.Random().Next(100000, 999999).ToString();
            var bk = new ReceptionBooking
            {
                PatientID = patId,
                ClinicName = clinic,
                Day = day,
                Code = code
            };
            AllBookings.Add(bk);
            SaveBookings();
            return bk;
        }

        public List<ReceptionBooking> GetBookingsByDay(string day) =>
            AllBookings.FindAll(b =>
                b.Day.Equals(day, System.StringComparison.OrdinalIgnoreCase));
    }

    // ── Employee account (for staff login) ────────────────
    public class EmployeeAccount
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Staff"; // Admin, Doctor, Nurse, Staff
    }

    // ── Employee account service ──────────────────────────
    // Youssef,Ahmed, Haneen, Rahmah, Salma: Staff  password: 12345
    public class EmployeeAccountService
    {
        private static readonly string[] StaffUsernames =
            { "Youssef", "Ahmed", "Haneen", "Rahmah", "Salma" };

        private const string AdminUsername = "Youssif";
        private const string AdminHash = "ML2UJrRNxmwMz1+aVhpqXAK7N06ECRn3h/zBpF1m/uo=";
        private const string SharedPassword = "12345";
        private string FormatName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = input.Trim().ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        private string Hash(string input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return System.Convert.ToBase64String(bytes);
            }
        }

        public void LoadData() { }

        public EmployeeAccount Login(string username, string password)
        {
            string formatted = FormatName(username);


            if (formatted == AdminUsername && Hash(password) == AdminHash)
            {
                return new EmployeeAccount
                {
                    Username = formatted,
                    Role = "Admin"
                };
            }

            if (password == SharedPassword &&
                System.Array.Exists(StaffUsernames, u => u == formatted))
            {
                return new EmployeeAccount
                {
                    Username = formatted,
                    Password = SharedPassword,
                    Role = "Staff"
                };
            }

            return null;
        }

        public bool AddAccount(string username, string password, string role) => false;
    }
}