using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Hospital_System
    {
        /// <summary>
        /// Single source of truth for all JSON data file paths.
        /// Every file is stored in the same folder as the running .exe
        /// so relative paths never drift regardless of working directory.
        /// </summary>
        internal static class DataStore
        {
            private static readonly string _base =
                AppDomain.CurrentDomain.BaseDirectory;

            public static string Path(string fileName) =>
                System.IO.Path.Combine(_base, fileName);

            // ── File paths ──────────────────────────────────────────
            public static readonly string Patients = Path("patients.json");
            public static readonly string Bookings = Path("bookings.json");
            public static readonly string PatRec = Path("patients_reception.json");
            public static readonly string BookRec = Path("bookings_reception.json");
            public static readonly string Employees = Path("employees.json");
            public static readonly string Doctors = Path("doctors.json");
            public static readonly string Nurses = Path("nurses.json");
            public static readonly string Pharmacists = Path("pharmacists.json");
            public static readonly string SecurityStaff = Path("security.json");
            public static readonly string PatientsEngine = Path("patients_data.json");
            public static readonly string Operations = Path("operations_data.json");
            public static readonly string Rooms = Path("rooms_data.json");
            public static readonly string BloodInventory = Path("blood_inventory.json");
            public static readonly string Donors = Path("donors_data.json");
            public static readonly string Transfers = Path("transfers_data.json");
            public static readonly string RoomCleaning = Path("room_cleaning.json");
            public static readonly string Inventory = Path("inventory.json");
            public static readonly string LabRecords = Path("lab_records.json");
    }
    }


