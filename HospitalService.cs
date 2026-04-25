
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // المكتبة المسؤولة عن تحويل البيانات لـ JSON

namespace Hospital_System
{
    public class HospitalService
    {
        // القوائم التي تخزن البيانات في الذاكرة أثناء تشغيل البرنامج
        public List<OnlineRegistration> AllPatients = new List<OnlineRegistration>();
        public List<Booking> AllBookings = new List<Booking>();

        // أسماء الملفات بامتداد .json
        private readonly string _patientsFile = "patients.json";
        private readonly string _bookingsFile = "bookings.json";

        // --- إدارة الملفات (JSON I/O) ---

        public void LoadData()
        {
            // تحميل بيانات المرضى
            if (File.Exists(_patientsFile))
            {
                string jsonString = File.ReadAllText(_patientsFile);
                // تحويل النص من JSON إلى القائمة AllPatients
                AllPatients = JsonSerializer.Deserialize<List<OnlineRegistration>>(jsonString) ?? new List<OnlineRegistration>();
            }

            // تحميل بيانات الحجوزات
            if (File.Exists(_bookingsFile))
            {
                string jsonString = File.ReadAllText(_bookingsFile);
                // تحويل النص من JSON إلى القائمة AllBookings
                AllBookings = JsonSerializer.Deserialize<List<Booking>>(jsonString) ?? new List<Booking>();
            }
        }

        private void SavePatients()
        {
            // تحويل القائمة كاملة إلى نص بتنسيق JSON وحفظها
            var options = new JsonSerializerOptions { WriteIndented = true }; // لجعل الملف سهل القراءة للعين
            string jsonString = JsonSerializer.Serialize(AllPatients, options);
            File.WriteAllText(_patientsFile, jsonString);
        }

        private void SaveBookings()
        {
            // تحويل قائمة الحجوزات كاملة إلى JSON وحفظها
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(AllBookings, options);
            File.WriteAllText(_bookingsFile, jsonString);
        }

        // --- التسجيل (Registration) ---

        public string CheckSignUp(string name, string id, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Name cannot be empty.";
            if (id.Length != 14 || !id.All(char.IsDigit)) return "National ID must be 14 digits.";
            if (phone.Length != 11 || !phone.All(char.IsDigit)) return "Phone must be 11 digits.";
            if (string.IsNullOrWhiteSpace(address)) return "Address cannot be empty.";
            if (AllPatients.Any(p => p.NationalID == id)) return "This ID is already registered.";
            return "";
        }

        public string SetPassword(OnlineRegistration patient, string pass, string confirm)
        {
            if (pass.Length < 8) return "Password must be at least 8 characters.";
            if (pass != confirm) return "Passwords do not match.";
            if (!pass.Any(char.IsDigit)) return "Password must include at least one number.";

            patient.Password = pass;
            patient.IsRegistered = true;

            AllPatients.Add(patient);
            SavePatients(); // استدعاء دالة الحفظ الجديدة
            return "";
        }

        // --- تسجيل الدخول (Login) ---

        public OnlineRegistration FindPatient(string id, string password) =>
            AllPatients.FirstOrDefault(p => p.NationalID == id && p.Password == password);

        // --- الحجز (Booking) ---

        public Booking CreateBooking(string patientId, Clinic clinic, string day)
        {
            string code = new Random().Next(100000, 999999).ToString();
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var booking = new Booking
            {
                PatientID = patientId,
                ClinicName = clinic.NameEnglish,
                Day = day,
                Code = code,
                BookingDateTime = currentTime,
                IsPaid = false
            };

            AllBookings.Add(booking);
            SaveBookings(); // حفظ قائمة الحجوزات المحدثة
            return booking;
        }

        public IEnumerable<Booking> GetPatientBookings(string patientId) =>
            AllBookings.Where(b => b.PatientID == patientId);

        public IEnumerable<Booking> SearchBookings(string query) =>
            AllBookings.Where(b => b.Code == query || b.PatientID == query);
    }
}
