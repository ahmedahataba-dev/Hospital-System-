using System.Text.Json.Serialization;

namespace Hospital_System
{
    public class OnlineRegistration
    {
        // Properties (not fields) so System.Text.Json can serialize them
        public string NameEnglish  { get; set; } = "";
        public string NationalID   { get; set; } = "";
        public string Phone        { get; set; } = "";
        public string Address      { get; set; } = "";
        public string Password     { get; set; } = "";
        public bool   IsRegistered { get; set; } = false;
    }
}
