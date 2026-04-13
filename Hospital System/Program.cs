namespace Hospital_System
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            //// To customize application configuration such as set high DPI settings or default font,
            ////// see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1())
            //Person p1 = new Person("ahe", 50, GenderType.Male, "00701181200491", "01025920863");
            //System.Windows.Forms.MessageBox.Show($"phone no ={p1.PhoneNumber}");
            //System.Windows.Forms.MessageBox.Show($"id ={p1.NationalId}");
            //Employee e1 = new Employee("ahmed", 19, GenderType.Male, "30701181200491", "01025920863", 5000, 7, 16, 3);
            //Employee e2 = new Employee("sayed", 19, GenderType.Male, "30701181200491", "01025920863", 5000, 7, 16, 3);
            //Employee e3 = new Employee("yed", 19, GenderType.Male, "30701181200491", "01025920863", 5000, 7, 16.5, 3);

            //System.Windows.Forms.MessageBox.Show($"id ={e1.EmployeeId}");
            //System.Windows.Forms.MessageBox.Show($"id ={e2.EmployeeId}");
            //System.Windows.Forms.MessageBox.Show($"id ={e3.EmployeeId}");

            // Create a Department on Floor 2
            Department surgery = new Department("Surgery",2);

            // Create a Room that says it's on Floor 5 (Opps!)
            Room wrongRoom = new Room("501", "Operating Theater", 5);

            // This will trigger your "Error" message!
            surgery.AddRoom(wrongRoom);

        }
	}
}