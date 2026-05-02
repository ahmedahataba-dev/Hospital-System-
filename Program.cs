using System;
using System.Windows.Forms;

namespace Hospital_System
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                LoginResult result;

                using (var login = new LoginForm())
                {
                    if (login.ShowDialog() != DialogResult.OK) return;
                    result = login.Result;
                }

                if (result.IsEmployee)
                {
                    using (var main = new MainForm(result))
                    {
                        main.ShowDialog();
                    }
                    // after closing MainForm, loop back to login
                }
                else if (result.IsPatient)
                {
                    using (var portal = new PatientPortalForm(result.PatientAccount))
                    {
                        portal.ShowDialog();
                    }
                    // after closing PatientPortalForm, loop back to login
                }
                else
                {
                    return; // unknown role, exit
                }
            }
        }
    }
}
