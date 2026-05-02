using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hospital_System
{
    public class PatientPortalForm : Form
    {
        private readonly OnlineRegistration _patient;
        private readonly HospitalService    _svc     = new HospitalService();
        private readonly ClinicService      _clinics = new ClinicService();

        private Panel  sideNav;
        private Panel  contentArea;
        //private Button activeBtn;

        // ── Colours ───────────────────────────────────────
        static readonly Color NavBg      = Color.FromArgb(10, 25, 47);
        static readonly Color NavActive  = Color.FromArgb(0, 188, 212);
        static readonly Color PageBg     = Color.FromArgb(236, 240, 245);
        static readonly Color CardBg     = Color.White;
        static readonly Color AccentBlue = Color.FromArgb(21, 101, 192);
        static readonly Color AccentGrn  = Color.FromArgb(27, 152, 79);
        static readonly Color AccentRed  = Color.FromArgb(198, 40, 40);
        static readonly Color AccentOrg  = Color.FromArgb(230, 126, 34);
        static readonly Color TextDark   = Color.FromArgb(20, 30, 48);
        static readonly Color TextGrey   = Color.FromArgb(96, 108, 129);

        // ── Fonts ─────────────────────────────────────────
        static readonly Font FTitle = new Font("Segoe UI", 18, FontStyle.Bold);
        static readonly Font FSub   = new Font("Segoe UI", 10, FontStyle.Regular);
        static readonly Font FBold  = new Font("Segoe UI",  9, FontStyle.Bold);
        static readonly Font FNorm  = new Font("Segoe UI",  9, FontStyle.Regular);
        static readonly Font FNav   = new Font("Segoe UI", 10, FontStyle.Regular);
        static readonly Font FNavB  = new Font("Segoe UI", 10, FontStyle.Bold);

        public PatientPortalForm(OnlineRegistration patient)
        {
            _patient = patient;
            _svc.LoadData(); // Load persisted patients & bookings from JSON
            BuildShell();
            ClickNav("My Bookings");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // SaveAll is safe here because LoadData() was called in constructor
            _svc.SaveAll();
            base.OnFormClosed(e);
        }
        

        // ═════════════════════════════════════════════════
        //  SHELL
        // ═════════════════════════════════════════════════
        void BuildShell()
        {
            Text          = "NeurAi — Patient Portal";
            Size          = new Size(1200, 800);
            MinimumSize   = new Size(1000, 660);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor     = PageBg;
            Controls.Clear();

            // ── Sidebar ───────────────────────────────────
            sideNav = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = NavBg };
            Controls.Add(sideNav);

            // Logo panel
            var logo = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = NavBg };
            logo.Controls.Add(new Label
            {
                Text = "🏥  NeurAi",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = NavActive, AutoSize = true, Location = new Point(16, 14)
            });
            logo.Controls.Add(new Label
            {
                Text = "Welcome, " + _patient.NameEnglish,
                Font = FNorm, ForeColor = Color.FromArgb(160, 180, 210),
                AutoSize = true, Location = new Point(16, 52)
            });
            logo.Controls.Add(new Label
            {
                Text = "Patient Portal",
                Font = FNavB, ForeColor = NavActive,
                AutoSize = true, Location = new Point(16, 76)
            });
            sideNav.Controls.Add(logo);

            // Nav buttons
            var navItems = new (string icon, string label)[]
            {
                ("📋", "My Bookings"),
                ("🗓️", "Book Appointment"),
                ("👤", "My Profile"),
                ("🔑", "Change Password"),
            };

            int top = 114;
            foreach (var item in navItems)
            {
                var btn = MakeNavBtn(item.icon + "   " + item.label, item.label);
                btn.Top = top;
                sideNav.Controls.Add(btn);
                top += 50;
            }

            // Divider
            var div = new Panel
            {
                Location  = new Point(14, top + 6),
                Size      = new Size(212, 1),
                BackColor = Color.FromArgb(40, 60, 90)
            };
            sideNav.Controls.Add(div);

            // Switch to Employee portal
            var switchBtn = new Button
            {
                Text      = "👨‍💼  Employee Portal",
                Size      = new Size(240, 44),
                Location  = new Point(0, top + 14),
                FlatStyle = FlatStyle.Flat,
                Font      = FNav,
                ForeColor = Color.FromArgb(0, 188, 212),
                BackColor = Color.FromArgb(20, 40, 70)
            };
            switchBtn.FlatAppearance.BorderSize = 0;
            switchBtn.Click += (s, e) =>
            {
                var login = new LoginForm();
                login.Show();
                Close();
            };
            sideNav.Controls.Add(switchBtn);

            // Logout
            var logoutBtn = new Button
            {
                Text      = "🚪  Logout",
                Size      = new Size(240, 46),
                FlatStyle = FlatStyle.Flat,
                Font      = FNav,
                ForeColor = Color.FromArgb(180, 180, 200),
                BackColor = Color.FromArgb(15, 28, 50),
                Anchor    = AnchorStyles.Bottom | AnchorStyles.Left
            };
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Click += (s, e) =>
            {
                var login = new LoginForm();
                login.Show();
                Close();
            };
            sideNav.Controls.Add(logoutBtn);
            sideNav.Resize += (s, e) => logoutBtn.Location = new Point(0, sideNav.Height - 50);

            // ── Content area ──────────────────────────────
            contentArea = new Panel
            {
                BackColor = PageBg,
                Location  = new Point(240, 0),
                Size      = new Size(ClientSize.Width - 240, ClientSize.Height),
                Anchor    = AnchorStyles.Top | AnchorStyles.Bottom |
                            AnchorStyles.Left | AnchorStyles.Right
            };
            Resize += (s, e) =>
            {
                contentArea.Location = new Point(240, 0);
                contentArea.Size     = new Size(ClientSize.Width - 240, ClientSize.Height);
            };
            Controls.Add(contentArea);
            Controls.SetChildIndex(sideNav,     0);
            Controls.SetChildIndex(contentArea, 1);
        }

        // ── Navigation helpers ────────────────────────────
        Button MakeNavBtn(string text, string key)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(240, 48),
                FlatStyle = FlatStyle.Flat,
                Font      = FNav,
                ForeColor = Color.FromArgb(180, 200, 220),
                BackColor = NavBg,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(18, 0, 0, 0),
                Tag       = key
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(22, 40, 70);
            b.Click += (s, e) => ClickNav(key);
            return b;
        }

        void ClickNav(string key)
        {
            // Highlight active button
            foreach (Control c in sideNav.Controls)
            {
                if (c is Button nb && nb.Tag is string t)
                {
                    bool isActive = t == key;
                    nb.BackColor = isActive ? Color.FromArgb(0, 60, 90) : NavBg;
                    nb.ForeColor = isActive ? NavActive : Color.FromArgb(180, 200, 220);
                    nb.Font      = isActive ? FNavB : FNav;
                }
            }
            NavigateTo(key);
        }

        void NavigateTo(string key)
        {
            Panel page;
            switch (key)
            {
                case "My Bookings":      page = PageMyBookings();   break;
                case "Book Appointment": page = PageBook();          break;
                case "My Profile":       page = PageProfile();       break;
                case "Change Password":  page = PageChangePassword(); break;
                default:                 page = PageMyBookings();    break;
            }
            contentArea.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentArea.Controls.Add(page);
        }

        // ═════════════════════════════════════════════════
        //  HELPERS
        // ═════════════════════════════════════════════════
        Panel ScrollPage(string title, string sub = "")
        {
            var page = new Panel
            {
                BackColor = PageBg, Dock = DockStyle.Fill,
                Padding   = new Padding(24, 0, 24, 0)
            };

            var hdr = new Panel { Height = 72, Dock = DockStyle.Top, BackColor = PageBg };
            hdr.Controls.Add(new Label
            {
                Text = title, Font = FTitle, ForeColor = TextDark,
                AutoSize = true, Location = new Point(0, 6)
            });
            if (!string.IsNullOrEmpty(sub))
                hdr.Controls.Add(new Label
                {
                    Text = sub, Font = FSub, ForeColor = TextGrey,
                    AutoSize = true, Location = new Point(2, 46)
                });
            page.Controls.Add(hdr);

            var scroll = new Panel
            {
                Dock = DockStyle.Fill, AutoScroll = true,
                BackColor = PageBg, Padding = new Padding(0, 8, 0, 8)
            };
            page.Controls.Add(scroll);
            page.Tag = scroll;
            return page;
        }

        Panel GetScroll(Panel page) => (Panel)page.Tag;

        Panel Card(int x, int y, int w, int h, string title = "")
        {
            var c = new Panel
            {
                Location = new Point(x, y), Size = new Size(w, h),
                BackColor = CardBg
            };
            c.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(Color.FromArgb(220, 224, 230)))
                    e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            };
            if (!string.IsNullOrEmpty(title))
                c.Controls.Add(new Label
                {
                    Text = title, Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = TextDark, AutoSize = true, Location = new Point(16, 14)
                });
            return c;
        }

        DataGridView MakeGrid(int x, int y, int w, int h)
        {
            var g = new DataGridView
            {
                Location = new Point(x, y), Size = new Size(w, h),
                AllowUserToAddRows = false, ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = CardBg, BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 234, 240),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = FNorm, ColumnHeadersHeight = 38,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            g.ColumnHeadersDefaultCellStyle.BackColor = NavBg;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font      = FBold;
            g.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(207, 235, 252);
            g.DefaultCellStyle.SelectionForeColor     = TextDark;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            g.RowTemplate.Height = 34;
            return g;
        }

        Button MakeBtn(string text, Color bg, int x, int y, int w = 160, int h = 36)
        {
            var b = new Button
            {
                Text = text, Size = new Size(w, h), Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat, BackColor = bg,
                ForeColor = Color.White, Font = FBold, Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(bg, 0.08f);
            return b;
        }

        TextBox MakeTB(int x, int y, int w, bool password = false)
        {
            return new TextBox
            {
                Location = new Point(x, y), Size = new Size(w, 28),
                Font = FNorm, BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = password
            };
        }

        Label MakeLbl(string text, int x, int y, bool bold = false)
        {
            return new Label
            {
                Text = text, Font = bold ? FBold : FNorm,
                ForeColor = bold ? TextGrey : TextDark,
                AutoSize = true, Location = new Point(x, y)
            };
        }

        // ═════════════════════════════════════════════════
        //  PAGE: MY BOOKINGS
        // ═════════════════════════════════════════════════
        Panel PageMyBookings()
        {
            var page   = ScrollPage("📋  My Bookings", "All your upcoming and past appointments");
            var scroll = GetScroll(page);

            var bookings = _svc.GetPatientBookings(_patient.NationalID).ToList();

            if (bookings.Count == 0)
            {
                var emptyCard = Card(0, 0, 700, 120);
                emptyCard.Controls.Add(new Label
                {
                    Text = "You have no bookings yet.",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = TextGrey, AutoSize = true, Location = new Point(20, 20)
                });
                emptyCard.Controls.Add(new Label
                {
                    Text = "Click  'Book Appointment'  in the sidebar to get started.",
                    Font = FNorm, ForeColor = TextGrey, AutoSize = true,
                    Location = new Point(20, 52)
                });
                scroll.Controls.Add(emptyCard);
                return page;
            }

            var gcard = Card(0, 0, 860, 420, "Booking History  (" + bookings.Count + " records)");
            var grid  = MakeGrid(14, 42, 828, 364);
            grid.Columns.Add("Code",   "Booking Code");
            grid.Columns.Add("Clinic", "Clinic");
            grid.Columns.Add("Day",    "Day");
            grid.Columns.Add("When",   "Booked At");
            grid.Columns.Add("Paid",   "Status");

            foreach (var bk in bookings)
                grid.Rows.Add(bk.Code, bk.ClinicName, bk.Day,
                              bk.BookingDateTime, bk.IsPaid ? "✅ Paid" : "⏳ Pending");

            gcard.Controls.Add(grid);
            scroll.Controls.Add(gcard);
            return page;
        }

        // ═════════════════════════════════════════════════
        //  PAGE: BOOK APPOINTMENT
        // ═════════════════════════════════════════════════
        Panel PageBook()
        {
            var page   = ScrollPage("🗓️  Book Appointment", "Choose a clinic and your preferred day");
            var scroll = GetScroll(page);

            var card = Card(0, 16, 580, 380, "New Appointment");

            int lx = 20, ly = 52;

            // Clinic label + combo
            card.Controls.Add(MakeLbl("Select Clinic", lx, ly, true));
            ly += 24;
            var cbClinic = new ComboBox
            {
                Font = FNorm, Location = new Point(lx, ly),
                Size = new Size(360, 28), DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var c in _clinics.Clinics.Values)
                cbClinic.Items.Add(c.NameEnglish);
            card.Controls.Add(cbClinic);
            ly += 52;

            // Day label + combo
            card.Controls.Add(MakeLbl("Available Day", lx, ly, true));
            ly += 24;
            var cbDay = new ComboBox
            {
                Font = FNorm, Location = new Point(lx, ly),
                Size = new Size(360, 28), DropDownStyle = ComboBoxStyle.DropDownList
            };
            card.Controls.Add(cbDay);
            ly += 52;

            cbClinic.SelectedIndexChanged += (s, e) =>
            {
                cbDay.Items.Clear();
                int idx = cbClinic.SelectedIndex + 1;
                if (_clinics.Clinics.ContainsKey(idx))
                    foreach (var d in _clinics.Clinics[idx].AvailableDays)
                        cbDay.Items.Add(d);
                if (cbDay.Items.Count > 0) cbDay.SelectedIndex = 0;
            };

            var msgLbl = new Label
            {
                Text = "", Font = FBold, AutoSize = true,
                Location = new Point(lx, ly), MaximumSize = new Size(520, 0)
            };
            card.Controls.Add(msgLbl);
            ly += 34;

            var btnBook = MakeBtn("✅  Confirm Booking", AccentGrn, lx, ly, 210, 42);
            card.Controls.Add(btnBook);

            btnBook.Click += (s, e) =>
            {
                if (cbClinic.SelectedIndex < 0)
                { msgLbl.Text = "❌  Please select a clinic."; msgLbl.ForeColor = AccentRed; return; }
                if (cbDay.SelectedIndex < 0)
                { msgLbl.Text = "❌  Please select a day."; msgLbl.ForeColor = AccentRed; return; }

                var clinic = _clinics.Clinics[cbClinic.SelectedIndex + 1];
                var bk     = _svc.CreateBooking(_patient.NationalID, clinic, cbDay.SelectedItem.ToString());

                msgLbl.Text      = "✅  Booked!  Code: " + bk.Code + "   |   " + clinic.NameEnglish + "   |   " + bk.Day;
                msgLbl.ForeColor = AccentGrn;
            };

            card.Height = ly + 90;
            scroll.Controls.Add(card);
            return page;
        }

        // ═════════════════════════════════════════════════
        //  PAGE: MY PROFILE
        // ═════════════════════════════════════════════════
        Panel PageProfile()
        {
            var page   = ScrollPage("👤  My Profile", "Your personal account information");
            var scroll = GetScroll(page);

            var card = Card(0, 0, 540, 280, "Personal Information");

            int y = 46;
            void Row(string label, string value)
            {
                card.Controls.Add(new Label
                {
                    Text = label, Font = FBold, ForeColor = TextGrey,
                    Location = new Point(20, y), Size = new Size(140, 22)
                });
                card.Controls.Add(new Label
                {
                    Text = value, Font = FNorm, ForeColor = TextDark,
                    Location = new Point(168, y), AutoSize = true
                });
                y += 38;
            }

            Row("Full Name:",   _patient.NameEnglish);
            Row("National ID:", _patient.NationalID);
            Row("Phone:",       _patient.Phone);
            Row("Address:",     _patient.Address);
            Row("Status:",      _patient.IsRegistered ? "✅  Active" : "⏳  Pending");

            // Divider
            card.Controls.Add(new Panel
            {
                Location = new Point(20, y), Size = new Size(500, 1),
                BackColor = Color.FromArgb(220, 224, 230)
            });
            y += 10;

            card.Controls.Add(new Label
            {
                Text = "To update your password, use Change Password in the sidebar.",
                Font = FNorm, ForeColor = TextGrey, AutoSize = true,
                Location = new Point(20, y)
            });

            card.Height = y + 40;
            scroll.Controls.Add(card);
            return page;
        }

        // ═════════════════════════════════════════════════
        //  PAGE: CHANGE PASSWORD
        // ═════════════════════════════════════════════════
        Panel PageChangePassword()
        {
            var page   = ScrollPage("🔑  Change Password", "Update your account password");
            var scroll = GetScroll(page);

            var card = Card(0, 20, 480, 320, "Set New Password");

            int lx = 20, ly = 52;

            card.Controls.Add(MakeLbl("Current Password", lx, ly, true));
            var tbCurrent = MakeTB(lx, ly + 22, 360, true); card.Controls.Add(tbCurrent);
            ly += 70;

            card.Controls.Add(MakeLbl("New Password  (min 8 chars, include a number)", lx, ly, true));
            var tbNew = MakeTB(lx, ly + 22, 360, true); card.Controls.Add(tbNew);
            ly += 70;

            card.Controls.Add(MakeLbl("Confirm New Password", lx, ly, true));
            var tbConf = MakeTB(lx, ly + 22, 360, true); card.Controls.Add(tbConf);
            ly += 60;

            var msgLbl = new Label { Text = "", Font = FBold, AutoSize = true, Location = new Point(lx, ly) };
            card.Controls.Add(msgLbl);
            ly += 28;

            var btnSave = MakeBtn("💾  Save Password", AccentBlue, lx, ly, 190, 38);
            card.Controls.Add(btnSave);

            btnSave.Click += (s, e) =>
            {
                // Verify current password
                if (tbCurrent.Text != _patient.Password)
                {
                    msgLbl.Text = "❌  Current password is incorrect.";
                    msgLbl.ForeColor = AccentRed;
                    return;
                }

                string newPass = tbNew.Text;
                string confPass = tbConf.Text;

                if (newPass.Length < 8)
                { msgLbl.Text = "❌  Password must be at least 8 characters."; msgLbl.ForeColor = AccentRed; return; }

                bool hasDigit = false;
                foreach (char ch in newPass) if (char.IsDigit(ch)) { hasDigit = true; break; }
                if (!hasDigit)
                { msgLbl.Text = "❌  Password must include at least one number."; msgLbl.ForeColor = AccentRed; return; }

                if (newPass != confPass)
                { msgLbl.Text = "❌  Passwords do not match."; msgLbl.ForeColor = AccentRed; return; }

                // Update in memory and save to JSON
                _patient.Password = newPass;
                _svc.UpdatePatient(_patient);

                msgLbl.Text = "✅  Password updated and saved successfully!";
                msgLbl.ForeColor = AccentGrn;
                tbCurrent.Clear(); tbNew.Clear(); tbConf.Clear();
            };

            card.Height = ly + 80;
            scroll.Controls.Add(card);
            return page;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PatientPortalForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "PatientPortalForm";
            this.Icon = AppIcon.Get();
            this.Load += new System.EventHandler(this.PatientPortalForm_Load);
            this.ResumeLayout(false);

        }

        private void PatientPortalForm_Load(object sender, EventArgs e)
        {

        }
    }
}
