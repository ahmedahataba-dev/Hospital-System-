                                using Newtonsoft.Json;
                                using Formatting = Newtonsoft.Json.Formatting;
                                using System;
                                using System.Collections.Generic;
                                using System.IO;
                                using System.Linq;
                                using System.Xml;


                                namespace Hospital_System
                                {
                                    // enum for managing the gender type ---> Ahmed AYman
                                    public enum GenderType { Male, Female }

                                    // enum for blood type
                                    public enum BloodGroup
                                    {

                                        A_Positive, A_Negative,
                                        B_Positive, B_Negative,
                                        AB_Positive, AB_Negative,
                                        O_Positive, O_Negative
                                    }

                                    public partial class program
                                    {
                                        static List<Patient> patients = new List<Patient>();
                                        static List<Operation> operationsList = new List<Operation>();

                                        const string filePath = "patients_data.json"; // الملف اللي هيتحفظ فيه ال Health Record
                                        const string operationsFilePath = "operations_data.json";

                                        //saving (inpatient / outpatient)
                                        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                                        {
                                            TypeNameHandling = TypeNameHandling.All, // عشان يحفظ النوعين inpatient/outpatient 
                                            Formatting = Formatting.Indented
                                        };

                                        static BloodBank myBank = new BloodBank();

                                        // دي مصفوفة بتمثل 5 غرف عمليات، لو false تبقى فاضية، لو true تبقى محجوزة
                                        static bool[] roomsStatus = new bool[9];

                                        static void Main(string[] args)
                                        {

            HospitalEngine engine = new HospitalEngine();
            engine.LoadData();
            UIMission.Run(engine);
        }



       




















































    }
}




























































