using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;


namespace Hospital_System
{
    

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
        

        public const string filePath = "patients_data.json"; // الملف اللي هيتحفظ فيه ال Health Record
        public const string operationsFilePath = "operations_data.json";





        static void Main(string[] args)
        {

            HospitalEngine engine = new HospitalEngine();
            engine.LoadData();
            UIMission.Run(engine);
        }



       




















































    }
}



























































