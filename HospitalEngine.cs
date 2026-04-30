using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Hospital_System
{

    internal class HospitalEngine
    {
        public static List<Patient>   patients       = new List<Patient>();
        public static List<Operation> operationsList = new List<Operation>();
        public static BloodBank       myBank         = new BloodBank();
        public static bool[]          roomsStatus    = new bool[9];

        private static readonly string filePath           = DataStore.PatientsEngine;
        private static readonly string operationsFilePath = DataStore.Operations;
        private static readonly string roomsFilePath      = DataStore.Rooms;

        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting       = Formatting.Indented
        };

        public void SaveData()
        {
            try
            {
                File.WriteAllText(filePath,           JsonConvert.SerializeObject(patients,       jsonSettings));
                File.WriteAllText(operationsFilePath, JsonConvert.SerializeObject(operationsList, jsonSettings));
                File.WriteAllText(roomsFilePath,      JsonConvert.SerializeObject(roomsStatus,    Formatting.Indented));
            }
            catch (Exception ex) { MessageBox.Show("Error saving data: " + ex.Message); }
        }

        public void LoadData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var loaded = JsonConvert.DeserializeObject<List<Patient>>(File.ReadAllText(filePath), jsonSettings);
                    if (loaded != null) patients = loaded;
                }
                if (File.Exists(operationsFilePath))
                {
                    var loaded = JsonConvert.DeserializeObject<List<Operation>>(File.ReadAllText(operationsFilePath), jsonSettings);
                    if (loaded != null) operationsList = loaded;
                }
                if (File.Exists(roomsFilePath))
                {
                    var loaded = JsonConvert.DeserializeObject<bool[]>(File.ReadAllText(roomsFilePath));
                    if (loaded != null) roomsStatus = loaded;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading data: " + ex.Message); }
        }
    }
}
