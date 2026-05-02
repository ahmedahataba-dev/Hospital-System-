using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hospital_System
{
    public enum CleaningStatus
    {
        Clean,
        NeedsCleaning,
        InProgress,
        Sanitized
    }

    public class CleaningRecord
    {
        public int            RoomNumber { get; set; }
        public DateTime       CleanedAt  { get; set; }
        public string         CleanedBy  { get; set; }
        public CleaningStatus Status     { get; set; }
        public string         Notes      { get; set; }

        // Parameterless constructor required for JSON deserialization
        public CleaningRecord() { }

        public CleaningRecord(int roomNumber, string cleanedBy, CleaningStatus status, string notes = "")
        {
            RoomNumber = roomNumber;
            CleanedAt  = DateTime.Now;
            CleanedBy  = cleanedBy;
            Status     = status;
            Notes      = notes;
        }

        public override string ToString()
        {
            return "[" + CleanedAt.ToString("dd/MM/yyyy HH:mm") + "] Room " + RoomNumber +
                   " | Status: " + Status + " | Staff: " + CleanedBy +
                   (string.IsNullOrEmpty(Notes) ? "" : " | Notes: " + Notes);
        }
    }

    // Serialization helper DTO
    internal class RoomCleaningData
    {
        public List<CleaningRecord>              History       { get; set; } = new List<CleaningRecord>();
        public Dictionary<int, CleaningStatus>   CurrentStatus { get; set; } = new Dictionary<int, CleaningStatus>();
    }

    public class RoomCleaningTracker
    {
        private static List<CleaningRecord>              _history       = new List<CleaningRecord>();
        private static Dictionary<int, CleaningStatus>  _currentStatus = new Dictionary<int, CleaningStatus>();

        private static readonly string _filePath = DataStore.RoomCleaning;
        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters    = { new JsonStringEnumConverter() }
        };

        // ── Persistence ──────────────────────────────────────────

        public static void SaveData()
        {
            try
            {
                var data = new RoomCleaningData
                {
                    History       = _history,
                    CurrentStatus = _currentStatus
                };
                File.WriteAllText(_filePath, JsonSerializer.Serialize(data, _jsonOpts));
            }
            catch { /* non-critical */ }
        }

        public static void LoadData()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var data = JsonSerializer.Deserialize<RoomCleaningData>(File.ReadAllText(_filePath), _jsonOpts);
                if (data != null)
                {
                    _history       = data.History       ?? new List<CleaningRecord>();
                    _currentStatus = data.CurrentStatus ?? new Dictionary<int, CleaningStatus>();
                }
            }
            catch { /* start fresh if file is corrupt */ }
        }

        // ── Mutations (save after every change) ──────────────────

        public static void MarkCleaned(int roomNumber, string staffName, string notes = "")
        {
            _history.Add(new CleaningRecord(roomNumber, staffName, CleaningStatus.Clean, notes));
            _currentStatus[roomNumber] = CleaningStatus.Clean;
            SaveData();
        }

        public static void MarkNeedsCleaning(int roomNumber, string reason = "")
        {
            _currentStatus[roomNumber] = CleaningStatus.NeedsCleaning;
            SaveData();
        }

        public static void MarkInProgress(int roomNumber, string staffName)
        {
            _history.Add(new CleaningRecord(roomNumber, staffName, CleaningStatus.InProgress));
            _currentStatus[roomNumber] = CleaningStatus.InProgress;
            SaveData();
        }

        public static void MarkSanitized(int roomNumber, string staffName, string notes = "")
        {
            _history.Add(new CleaningRecord(roomNumber, staffName, CleaningStatus.Sanitized, notes));
            _currentStatus[roomNumber] = CleaningStatus.Sanitized;
            SaveData();
        }

        public static CleaningStatus GetStatus(int roomNumber)
        {
            return _currentStatus.TryGetValue(roomNumber, out var s) ? s : CleaningStatus.NeedsCleaning;
        }

        public static CleaningRecord GetLastCleaning(int roomNumber)
        {
            return _history
                .Where(r => r.RoomNumber == roomNumber)
                .OrderByDescending(r => r.CleanedAt)
                .FirstOrDefault();
        }

        public static List<int> GetRoomsNeedingCleaning()
        {
            return _currentStatus
                .Where(kv => kv.Value == CleaningStatus.NeedsCleaning)
                .Select(kv => kv.Key)
                .ToList();
        }

        public static Dictionary<int, CleaningStatus> GetAllStatuses()
        {
            return new Dictionary<int, CleaningStatus>(_currentStatus);
        }

        public static List<CleaningRecord> GetHistory(int roomNumber)
        {
            return _history
                .Where(r => r.RoomNumber == roomNumber)
                .OrderByDescending(r => r.CleanedAt)
                .ToList();
        }

        public static void OnPatientDischarged(int roomNumber)
        {
            MarkNeedsCleaning(roomNumber, "Patient discharged - routine cleaning required");
        }
    }
}
