using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hospital_System
{
    public class HospitalService
    {
        public List<OnlineRegistration> AllPatients = new List<OnlineRegistration>();
        public List<Booking> AllBookings = new List<Booking>();

        private readonly string _patientsFile = "patients.txt";
        private readonly string _bookingsFile = "bookings.txt";

        //  File I/O 

        public void LoadData()
        {
            if (File.Exists(_patientsFile))
            {
                foreach (var line in File.ReadAllLines(_patientsFile))
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 5)
                        AllPatients.Add(new OnlineRegistration
                        {
                            NationalID = parts[0],
                            NameEnglish = parts[1],
                            Phone = parts[2],
                            Address = parts[3],
                            Password = parts[4],
                            IsRegistered = true
                        });
                }
            }

            if (File.Exists(_bookingsFile))
            {
                foreach (var line in File.ReadAllLines(_bookingsFile))
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 6)
                        AllBookings.Add(new Booking
                        {
                            PatientID = parts[0],
                            ClinicName = parts[1],
                            Day = parts[2],
                            Code = parts[3],
                            IsPaid = bool.Parse(parts[4]),
                            BookingDateTime = parts[5]
                        });
                }
            }
        }

        //  Registration 

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
            File.AppendAllText(_patientsFile, patient.ToFileLine() + Environment.NewLine);
            return "";
        }

        //  Login 

        public OnlineRegistration FindPatient(string id, string password) =>
            AllPatients.FirstOrDefault(p => p.NationalID == id && p.Password == password);

        //  Booking 

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
                BookingDateTime = currentTime
            };

            AllBookings.Add(booking);
            File.AppendAllText(_bookingsFile, booking.ToFileLine() + Environment.NewLine);
            return booking;
        }

        public IEnumerable<Booking> GetPatientBookings(string patientId) =>
            AllBookings.Where(b => b.PatientID == patientId);

        public IEnumerable<Booking> SearchBookings(string query) =>
            AllBookings.Where(b => b.Code == query || b.PatientID == query);
    }
}
