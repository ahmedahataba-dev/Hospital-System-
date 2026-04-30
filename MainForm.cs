using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hospital_System
{
    public class MainForm : Form
    {
        // ── backend ──────────────────────────────────────────
        private Hospital neurai;
        private HospitalEngine _engine = new HospitalEngine();

        // ── layout panels ────────────────────────────────────
        private Panel sideNav;
        private Panel contentArea;


        // ── colours ──────────────────────────────────────────
        static readonly Color NavBg = Color.FromArgb(10, 25, 47);
        static readonly Color NavActive = Color.FromArgb(0, 188, 212);
        static readonly Color PageBg = Color.FromArgb(236, 240, 245);
        static readonly Color CardBg = Color.White;
        static readonly Color AccentBlue = Color.FromArgb(21, 101, 192);
        static readonly Color AccentGrn = Color.FromArgb(27, 152, 79);
        static readonly Color AccentRed = Color.FromArgb(198, 40, 40);
        static readonly Color AccentOrg = Color.FromArgb(230, 126, 34);
        static readonly Color TextDark = Color.FromArgb(20, 30, 48);
        static readonly Color TextGrey = Color.FromArgb(96, 108, 129);

        // ── fonts ────────────────────────────────────────────
        static readonly Font FTitle = new Font("Segoe UI", 20, FontStyle.Bold);
        static readonly Font FSub = new Font("Segoe UI", 10, FontStyle.Regular);
        static readonly Font FBold = new Font("Segoe UI", 9, FontStyle.Bold);
        static readonly Font FNorm = new Font("Segoe UI", 9, FontStyle.Regular);
        static readonly Font FNav = new Font("Segoe UI", 10, FontStyle.Regular);
        static readonly Font FNavBig = new Font("Segoe UI", 10, FontStyle.Bold);
        static readonly Font FMono = new Font("Consolas", 9, FontStyle.Regular);

        private Button activeNavBtn;
        private List<string> ambulanceStatus = new List<string>();
        private HospitalService hospitalService = new HospitalService();
        private ClinicService clinicService = new ClinicService();
        private OnlineRegistration loggedInPatient = null;
        private ReceptionService receptionService = new ReceptionService();
        private LabService labService = new LabService();
        private string receptionChosenDay = "Saturday";
        private EmployeeAccountService empAccountSvc = new EmployeeAccountService();
        private LoginResult _loginResult;
        // ─────────────────────────────────────────────────────

        public void MainForm_Load(object sender, EventArgs e)
        {

        }



        public MainForm() : this(null) { }

        public MainForm(LoginResult loginResult)
        {
            _loginResult = loginResult;
            // init backend exactly like your Program.cs
            HospitalData.ExtractEmployees();
            HospitalData.ExtractDoctors();
            HospitalData.ExtractNurses();
            HospitalData.ExtractPharmacists();
            HospitalData.ExtractSecurity();

            neurai = new Hospital("NeurAi Medical Center");
            Doctor.HireInitialDoctors(neurai);
            foreach (var doc in Doctor.doctors)
            {
                var dept = neurai.ActiveDepartments
                    .FirstOrDefault(d => d.DeptName.Equals(doc.Department, StringComparison.OrdinalIgnoreCase));
                if (dept != null && !dept.Doctors.Any(d => d.MedicalLicenseNumber == doc.MedicalLicenseNumber))
                    dept.Doctors.Add(doc);
            }
            // ── Sync persisted security guards into the runtime Guards list ──
            foreach (var sec in Security.securities)
            {
                if (!neurai.CampusSecurity.Guards.Any(g => g.BadgeNumber == sec.BadgeNumber))
                    neurai.CampusSecurity.Guards.Add(sec);
            }

            _engine.LoadData();
            hospitalService.LoadData();    // online patients & bookings
            receptionService.LoadData();   // reception patients & bookings
            labService.LoadData();         // analysis lab records

            RoomCleaningTracker.LoadData(); // room cleaning history & statuses
            BuildShell();
            NavigateTo("Dashboard");
        }

        // ═════════════════════════════════════════════════════
        //  SHELL
        // ═════════════════════════════════════════════════════

        private List<Ambulance> ambulances = new List<Ambulance>();
        private int ambulanceCounter = 1;

        // ── Emergency page state (persists across reloads) ──
        private int erCaseId = 1;

        private void LoadEmergencyPage()
        {
            contentArea.Controls.Clear();

            // ── outer scroll container ──
            Panel container = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(236, 240, 245)
            };

            // ── page title ──
            Label title = new Label
            {
                Text = "🚨  Emergency Room",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = TextDark,
                Location = new Point(20, 18),
                AutoSize = true
            };

            Label subtitle = new Label
            {
                Text = "Smart ER: Triage + Ambulance Dispatch",
                Font = FSub,
                ForeColor = TextGrey,
                Location = new Point(22, 55),
                AutoSize = true
            };

            // ══════════════════════════════════════════════
            //  LEFT CARD  – vitals input + cases table
            // ══════════════════════════════════════════════
            Panel leftCard = new Panel
            {
                Location = new Point(20, 82),
                Size = new Size(680, 500),
                BackColor = CardBg
            };
            leftCard.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(220, 224, 230)), 0, 0, leftCard.Width - 1, leftCard.Height - 1);
            };

            // ── vital boxes ──
            string[] vLabels = { "HR", "O₂%", "Temp °C", "BP mmHg" };
            string[] vPlaceholders = { "e.g. 75", "e.g. 98", "e.g. 37", "e.g. 110" };
            List<TextBox> boxes = new List<TextBox>();

            for (int i = 0; i < 4; i++)
            {
                Label lbl = new Label
                {
                    Text = vLabels[i],
                    Font = FBold,
                    ForeColor = TextGrey,
                    Location = new Point(16 + i * 160, 14),
                    AutoSize = true
                };

                TextBox tb = new TextBox
                {
                    Font = FNorm,
                    Location = new Point(14 + i * 160, 36),
                    Size = new Size(130, 26),
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = vPlaceholders[i],
                    ForeColor = Color.Gray
                };

                boxes.Add(tb);
                leftCard.Controls.Add(lbl);
                leftCard.Controls.Add(tb);
            }

            // ── triage result label ──
            Label triageLabel = new Label
            {
                Text = "Triage: —",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextDark,
                Location = new Point(16, 76),
                AutoSize = true
            };
            leftCard.Controls.Add(triageLabel);

            // ── Register button ──
            Button btnRegister = new Button
            {
                Text = "Register",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = AccentRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(14, 108),
                Size = new Size(130, 34)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            leftCard.Controls.Add(btnRegister);

            // ── Cases label ──
            Label casesLabel = new Label
            {
                Text = "Cases",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = TextDark,
                Location = new Point(14, 158),
                AutoSize = true
            };
            leftCard.Controls.Add(casesLabel);

            // ── Cases grid ──
            DataGridView grid = new DataGridView
            {
                Location = new Point(14, 182),
                Size = new Size(648, 300),
                ColumnCount = 3,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackgroundColor = CardBg,
                GridColor = Color.FromArgb(220, 224, 230),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = FNorm
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 25, 45);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = FBold;
            grid.EnableHeadersVisualStyles = false;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 255);
            grid.DefaultCellStyle.SelectionForeColor = TextDark;

            grid.Columns[0].Name = "ID";
            grid.Columns[1].Name = "Name";
            grid.Columns[2].Name = "Triage";

            leftCard.Controls.Add(grid);

            // ══════════════════════════════════════════════
            //  RIGHT CARD  – ambulance panel
            // ══════════════════════════════════════════════
            Panel rightCard = new Panel
            {
                Location = new Point(716, 82),
                Size = new Size(360, 500),
                BackColor = CardBg,
                AutoScroll = true
            };
            rightCard.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(220, 224, 230)), 0, 0, rightCard.Width - 1, rightCard.Height - 1);
            };

            int ambY = 12;

            // ── add one ambulance row to right card ──
            void AddAmbulanceRow(Ambulance amb)
            {
                Panel row = new Panel
                {
                    Location = new Point(10, ambY),
                    Size = new Size(330, 44),
                    BackColor = CardBg
                };

                // status dot + label
                Label nameLbl = new Label
                {
                    Text = amb.VehicleNumber + "  ●  " + (amb.IsAvailable ? "Available" : "Busy"),
                    Font = FBold,
                    ForeColor = amb.IsAvailable ? Color.FromArgb(27, 152, 79) : AccentRed,
                    Location = new Point(8, 12),
                    AutoSize = true
                };

                Button btnD = new Button
                {
                    Text = "Dispatch",
                    Font = FBold,
                    BackColor = AccentRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(78, 28),
                    Location = new Point(178, 8)
                };
                btnD.FlatAppearance.BorderSize = 0;

                Button btnR = new Button
                {
                    Text = "Return",
                    Font = FBold,
                    BackColor = Color.FromArgb(27, 152, 79),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(68, 28),
                    Location = new Point(258, 8)
                };
                btnR.FlatAppearance.BorderSize = 0;

                btnD.Click += (s, e) =>
                {
                    amb.Dispatch("Emergency");
                    nameLbl.Text = amb.VehicleNumber + "  ●  Busy";
                    nameLbl.ForeColor = AccentRed;
                };

                btnR.Click += (s, e) =>
                {
                    amb.ReturnToBase();
                    nameLbl.Text = amb.VehicleNumber + "  ●  Available";
                    nameLbl.ForeColor = Color.FromArgb(27, 152, 79);
                };

                row.Controls.Add(nameLbl);
                row.Controls.Add(btnD);
                row.Controls.Add(btnR);
                rightCard.Controls.Add(row);

                // separator line
                Panel sep = new Panel
                {
                    Location = new Point(10, ambY + 44),
                    Size = new Size(330, 1),
                    BackColor = Color.FromArgb(220, 224, 230)
                };
                rightCard.Controls.Add(sep);

                ambY += 50;
            }

            // ── create & store a new ambulance ──
            void CreateAmbulance()
            {
                string id = string.Format("AMB-{0:D3}", ambulanceCounter);
                Ambulance amb = new Ambulance(id, "Standard");
                ambulances.Add(amb);
                AddAmbulanceRow(amb);
                ambulanceCounter++;
            }

            // If first time, seed 5 ambulances; else restore existing ones
            if (ambulances.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                    CreateAmbulance();
            }
            else
            {
                foreach (var existing in ambulances)
                    AddAmbulanceRow(existing);
            }

            // ── "+ Add Ambulance" box ──
            Panel addBox = new Panel
            {
                Location = new Point(10, ambY + 8),
                Size = new Size(330, 110),
                BackColor = CardBg
            };

            Label addTitle = new Label
            {
                Text = "+ Add Ambulance",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextDark,
                Location = new Point(0, 0),
                AutoSize = true
            };

            TextBox tbAmbId = new TextBox
            {
                Font = FNorm,
                Location = new Point(0, 26),
                Size = new Size(200, 26),
                BorderStyle = BorderStyle.FixedSingle
            };
            tbAmbId.Text = string.Format("AMB-{0:D3}", ambulanceCounter);

            TextBox tbAmbModel = new TextBox
            {
                Font = FNorm,
                Text = "Model",
                ForeColor = Color.Gray,
                Location = new Point(0, 58),
                Size = new Size(200, 26),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnAddAmb = new Button
            {
                Text = "Add",
                Font = FBold,
                BackColor = AccentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(64, 58),
                Location = new Point(210, 26)
            };
            btnAddAmb.FlatAppearance.BorderSize = 0;

            btnAddAmb.Click += (s, e) =>
            {
                string newId = tbAmbId.Text.Trim();
                if (string.IsNullOrEmpty(newId)) newId = string.Format("AMB-{0:D3}", ambulanceCounter);

                Ambulance newAmb = new Ambulance(newId, tbAmbModel.Text.Trim());
                ambulances.Add(newAmb);

                // move addBox down, then insert row above it
                addBox.Top = ambY + 50 + 8;
                AddAmbulanceRow(newAmb);

                ambulanceCounter++;
                tbAmbId.Text = string.Format("AMB-{0:D3}", ambulanceCounter);
                tbAmbModel.Clear();

                rightCard.AutoScrollMinSize = new Size(0, addBox.Bottom + 20);
            };

            addBox.Controls.Add(addTitle);
            addBox.Controls.Add(tbAmbId);
            addBox.Controls.Add(tbAmbModel);
            addBox.Controls.Add(btnAddAmb);
            rightCard.Controls.Add(addBox);
            rightCard.AutoScrollMinSize = new Size(0, addBox.Bottom + 20);

            // ══════════════════════════════════════════════
            //  Register click – triage logic
            // ══════════════════════════════════════════════
            btnRegister.Click += (s, e) =>
            {
                int hr, o2, bp;
                double temp;

                if (!int.TryParse(boxes[0].Text, out hr) ||
                    !int.TryParse(boxes[1].Text, out o2) ||
                    !double.TryParse(boxes[2].Text, out temp) ||
                    !int.TryParse(boxes[3].Text, out bp))
                {
                    MessageBox.Show("Please enter valid numbers in all vital fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                VitalSigns v = new VitalSigns
                {
                    HeartRate = hr,
                    OxygenLevel = o2,
                    Temperature = temp,
                    SystolicBP = bp
                };

                string triage;
                Color triageColor;

                if (v.OxygenLevel < 85 || v.HeartRate > 140 || v.HeartRate < 40)
                {
                    triage = "Resuscitation (Red)";
                    triageColor = AccentRed;
                }
                else if (v.OxygenLevel < 92 || v.SystolicBP > 180 || v.Temperature > 40)
                {
                    triage = "Emergent (Orange)";
                    triageColor = Color.OrangeRed;
                }
                else if (v.Temperature > 38.5 || v.SystolicBP > 140)
                {
                    triage = "Urgent (Yellow)";
                    triageColor = Color.DarkGoldenrod;
                }
                else if (v.Temperature > 37.5 || v.HeartRate > 100)
                {
                    triage = "Less Urgent (Green)";
                    triageColor = AccentGrn;
                }
                else
                {
                    triage = "Non-Urgent (Blue)";
                    triageColor = AccentBlue;
                }

                triageLabel.Text = "Triage: " + triage;
                triageLabel.ForeColor = triageColor;

                int rowIdx = grid.Rows.Add(erCaseId++, "Patient", triage);
                grid.Rows[rowIdx].DefaultCellStyle.ForeColor = triageColor;

                // auto-dispatch first available ambulance
                foreach (var amb in ambulances)
                {
                    if (amb.IsAvailable)
                    {
                        amb.Dispatch("Emergency");
                        break;
                    }
                }
            };

            // ── assemble ──
            container.Controls.Add(title);
            container.Controls.Add(subtitle);
            container.Controls.Add(leftCard);
            container.Controls.Add(rightCard);

            contentArea.Controls.Add(container);
        }

        private void btnEmergency_Click(object sender, EventArgs e)
        {
            LoadEmergencyPage();
        }


        void BuildShell()
        {
            Text = "NeurAi Medical Center — Hospital Management System";
            Size = new Size(1350, 860);
            MinimumSize = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = PageBg;
            Controls.Clear();

            // ── Sidebar  ──
            sideNav = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = NavBg,
                AutoScroll = true
            };
            Controls.Add(sideNav);

            // logo
            var logo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(5, 15, 35)
            };

            logo.Cursor = Cursors.Hand;
            logo.Click += (s, e) => NavigateTo("Dashboard");
            PictureBox Logo = new PictureBox
            {
                Image = Properties.Resources.NeurAI,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(220, 120),
                Location = new Point(0, 0)
            };

            sideNav.Controls.Add(logo);
            logo.Controls.Add(Logo);
            sideNav.Controls.Add(logo);

            // nav items
            var items = new (string, string)[]
            {
        ("📊", "Dashboard"),
        ("🏢", "Hospital Overview"),
        ("👨‍⚕️", "Doctors"),
        ("🧑‍🤝‍🧑", "Patients"),
        ("💊", "Pharmacy"),
        ("🏦", "Billing"),
        ("🚑", "Emergency"),
        ("💉", "Pharmacists"),
        ("🔒", "Security"),
        ("👥", "Staff"),
        ("📦", "Inventory"),
        ("🧹", "Room Cleaning"),
        ("🩸", "Blood Bank"),
        ("🏥", "Operations"),
        ("🔬", "Analysis Lab"),
        ("🎫", "Reception Booking"),
        ("🔑", "Accounts"),
        ("💊", "Prescription"),
            };

            int top = 82;
            foreach (var item in items)
            {
                var btn = NavBtn(item.Item1 + "   " + item.Item2, item.Item2);
                btn.Top = top;
                sideNav.Controls.Add(btn);
                top += 50;
            }

            // Save & Exit
            var saveExit = new Button
            {
                Text = "💾  Save & Exit",
                Size = new Size(230, 46),
                FlatStyle = FlatStyle.Flat,
                Font = FNav,
                ForeColor = Color.FromArgb(180, 180, 200),
                BackColor = Color.FromArgb(20, 35, 60),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            saveExit.FlatAppearance.BorderSize = 0;
            saveExit.Click += (s, e) => SaveAndExit();
            sideNav.Controls.Add(saveExit);

            sideNav.Resize += (s, e) =>
            {
                saveExit.Location = new Point(0, sideNav.Height - 50);
            };

            // ── Content area SECOND ──
            contentArea = new Panel
            {
                BackColor = PageBg,
                Location = new Point(230, 0), // 👈 force it to start AFTER sidebar
                Size = new Size(ClientSize.Width - 230, ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(24, 20, 24, 10)
            };
            this.Resize += (s, e) =>
            {
                contentArea.Location = new Point(230, 0);
                contentArea.Size = new Size(ClientSize.Width - 230, ClientSize.Height);
            };

            Controls.Add(contentArea);

            Controls.SetChildIndex(sideNav, 0);
            Controls.SetChildIndex(contentArea, 1);
        }

        Button NavBtn(string text, string pageKey)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(230, 48),
                FlatStyle = FlatStyle.Flat,
                Font = FNav,
                ForeColor = Color.FromArgb(180, 200, 220),
                BackColor = NavBg,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0),
                Tag = pageKey
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(22, 40, 70);
            b.Click += NavBtn_Click;
            return b;
        }

        void SetActive(Button btn)
        {
            if (activeNavBtn != null) { activeNavBtn.BackColor = NavBg; activeNavBtn.ForeColor = Color.FromArgb(180, 200, 220); activeNavBtn.Font = FNav; }
            activeNavBtn = btn;
            btn.BackColor = Color.FromArgb(0, 60, 90);
            btn.ForeColor = NavActive;
            btn.Font = FNavBig;
        }

        void NavBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            SetActive(btn);
            NavigateTo(btn.Tag?.ToString() ?? "Dashboard");
        }

        void NavigateTo(string key)
        {
            Panel page = null;

            if (key == "Dashboard") page = PageDashboard();
            else if (key == "Hospital Overview") page = PageHospitalOverview();
            else if (key == "Doctors") page = PageDoctors();
            else if (key == "Patients") page = PagePatients();
            else if (key == "Pharmacy") page = PagePharmacy();
            else if (key == "Pharmacists") page = PagePharmacists();
            else if (key == "Billing") page = PageBilling();
            else if (key == "Emergency")
            {
                page = new Panel(); // dummy container
                LoadEmergencyPage();
                return;
            }
            else if (key == "Security") page = PageSecurity();
            else if (key == "Staff") page = PageStaff();
            else if (key == "Inventory") page = PageInventory();
            else if (key == "Room Cleaning") page = PageRoomCleaning();
            else if (key == "Blood Bank") page = PageBloodBank();
            else if (key == "Operations") page = PageOperations();
            else if (key == "Analysis Lab") page = PageLabAnalysis();
            else if (key == "Reception Booking") page = PageReceptionBooking();
            else if (key == "Accounts") page = PageAccounts();
            else if (key == "Prescription") page = PagePrescription();
            else page = PageDashboard();

            contentArea.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentArea.Controls.Add(page);
        }

        // ═════════════════════════════════════════════════════
        //  HELPERS
        // ═════════════════════════════════════════════════════
        Panel ScrollPage(string title, string sub = "")
        {
            var page = new Panel
            {
                BackColor = PageBg,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 0, 0, 0) // 👈 THIS is the fix
            };

            var hdr = new Panel
            {
                Height = 68,
                Dock = DockStyle.Top,
                BackColor = PageBg
            };

            hdr.Controls.Add(new Label
            {
                Text = title,
                Font = FTitle,
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(0, 4)
            });

            if (!string.IsNullOrEmpty(sub))
            {
                hdr.Controls.Add(new Label
                {
                    Text = sub,
                    Font = FSub,
                    ForeColor = TextGrey,
                    AutoSize = true,
                    Location = new Point(2, 42)
                });
            }

            page.Controls.Add(hdr);

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = PageBg,
                Padding = new Padding(10)
            };

            page.Controls.Add(scroll);
            page.Tag = scroll;

            return page;
        }
        Panel GetScroll(Panel page) => (Panel)page.Tag;

        Panel Card(int x, int y, int w, int h, string title = "")
        {
            var c = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = CardBg };
            c.Paint += (s, e) =>
            {
                Pen p = new Pen(Color.FromArgb(18, 0, 0, 0));
                e.Graphics.DrawRectangle(p, 0, 0, c.Width - 1, c.Height - 1);
                p.Dispose();
            };
            if (!string.IsNullOrEmpty(title))
                c.Controls.Add(Lbl(title, FBold, TextGrey, 14, 10));
            return c;
        }

        Label Lbl(string text, Font f, Color col, int x, int y, int w = 0)
        {
            var l = new Label { Text = text, Font = f, ForeColor = col, AutoSize = w == 0, Location = new Point(x, y) };
            if (w > 0) l.Width = w;
            return l;
        }

        Button Btn(string text, Color bg, int x, int y, int w = 130, int h = 34)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(w, h),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Font = FBold,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(bg, 0.08f);
            return b;
        }

        TextBox TB(int x, int y, int w = 190, string ph = "")
        {
            var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 26), Font = FNorm, BorderStyle = BorderStyle.FixedSingle };
            if (!string.IsNullOrEmpty(ph))
            {
                t.Text = ph; t.ForeColor = TextGrey;
                t.GotFocus += (s, e) => { if (t.Text == ph) { t.Text = ""; t.ForeColor = TextDark; } };
                t.LostFocus += (s, e) => { if (t.Text == "") { t.Text = ph; t.ForeColor = TextGrey; } };
            }
            return t;
        }

        ComboBox CB(int x, int y, int w, string[] items)
        {
            var c = new ComboBox { Location = new Point(x, y), Size = new Size(w, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = FNorm };
            c.Items.AddRange(items);
            if (items.Length > 0) c.SelectedIndex = 0;
            return c;
        }

        DataGridView Grid(int x, int y, int w, int h)
        {
            var g = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = CardBg,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(220, 225, 235),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = FNorm,
                ColumnHeadersHeight = 36,
                EnableHeadersVisualStyles = false
            };
            g.ColumnHeadersDefaultCellStyle.BackColor = NavBg;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = FBold;
            g.DefaultCellStyle.BackColor = CardBg;
            g.DefaultCellStyle.ForeColor = TextDark;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 235, 252);
            g.DefaultCellStyle.SelectionForeColor = TextDark;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            g.RowTemplate.Height = 32;
            return g;
        }

        Panel StatCard(string icon, string label, string val, Color accent, int x, int y)
        {
            var c = new Panel { Location = new Point(x, y), Size = new Size(210, 100), BackColor = CardBg };
            c.Paint += (s, e) =>
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(22, accent));
                e.Graphics.FillRectangle(brush, 0, 0, 6, c.Height);
                brush.Dispose();
                Pen pen = new Pen(Color.FromArgb(14, 0, 0, 0));
                e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
                pen.Dispose();
            };
            c.Controls.Add(Lbl(icon, new Font("Segoe UI", 24), accent, 10, 12));
            c.Controls.Add(Lbl(val, new Font("Segoe UI", 20, FontStyle.Bold), TextDark, 56, 10));
            c.Controls.Add(Lbl(label, FNorm, TextGrey, 57, 52));
            return c;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: DASHBOARD
        // ═════════════════════════════════════════════════════
        Panel PageDashboard()
        {
            var page = ScrollPage("Dashboard", "Welcome to NeurAi Medical Center");
            var s = GetScroll(page);

            int docs = neurai.ActiveDepartments.Sum(d => d.Doctors.Count);
            int rooms = neurai.ActiveDepartments.Sum(d => d.Rooms.Count);
            int emps = Employee.employees.Count;
            int ambs = neurai.CampusFacilities.AmbulanceCount;
            bool locked = neurai.CampusSecurity.IsLockdownActive;

            s.Controls.Add(StatCard("👨‍⚕️", "Doctors", docs.ToString(), AccentBlue, 0, 0));
            s.Controls.Add(StatCard("🛏", "Rooms", rooms.ToString(), AccentGrn, 218, 0));
            s.Controls.Add(StatCard("👥", "Employees", emps.ToString(), AccentOrg, 436, 0));
            s.Controls.Add(StatCard("🚑", "Ambulances", ambs.ToString(), Color.OrangeRed, 654, 0));
            s.Controls.Add(StatCard("🔒", "Security", locked ? "LOCKDOWN" : "SECURE",
                                    locked ? AccentRed : AccentGrn, 872, 0));

            // Department grid
            var dcard = Card(0, 114, 700, 320, "Departments Overview");
            var dg = Grid(14, 34, 672, 275);
            dg.Columns.Add("dept", "Department");
            dg.Columns.Add("docs", "Doctors");
            dg.Columns.Add("rooms", "Rooms");
            foreach (var d in neurai.ActiveDepartments)
                dg.Rows.Add(d.DeptName, d.Doctors.Count, d.Rooms.Count);
            dcard.Controls.Add(dg);
            s.Controls.Add(dcard);

            // Facilities card
            var fcard = Card(718, 114, 360, 320, "Campus Facilities");
            int fy = 38;
            void FRow(string lbl, string v, Color col)
            {
                fcard.Controls.Add(Lbl(lbl, FBold, TextGrey, 14, fy));
                fcard.Controls.Add(Lbl(v, FBold, col, 190, fy));
                fy += 38;
            }
            FRow("Ambulances", ambs.ToString() + " on standby", AccentBlue);
            FRow("Blood Bank", neurai.CampusFacilities.BloodBankCapacity + " units", AccentRed);
            FRow("Garden", neurai.CampusFacilities.HasGarden ? "Available" : "No", AccentGrn);
            FRow("Pharmacy Depts", neurai.CampusFacilities.HospitalPharmacy.categories.Count.ToString(), AccentOrg);
            FRow("Floors", "5 active floors", TextGrey);
            FRow("Security", locked ? "🔴 LOCKDOWN" : "🟢 SECURE", locked ? AccentRed : AccentGrn);
            s.Controls.Add(fcard);

            // Quick-launch buttons
            var qcard = Card(0, 448, 1080, 80, "Quick Actions");
            int qx = 14;
            void QBtn(string txt, Color col, string nav)
            {
                var b = Btn(txt, col, qx, 28, 160, 36);
                b.Click += (s2, e2) => NavigateTo(nav);
                qcard.Controls.Add(b);
                qx += 170;
            }
            QBtn("View Doctors", AccentBlue, "Doctors");
            QBtn("View Pharmacy", AccentGrn, "Pharmacy");
            QBtn("Security Panel", AccentRed, "Security");
            QBtn("Billing", AccentOrg, "Billing");
            QBtn("Staff Roster", Color.FromArgb(90, 90, 160), "Staff");
            s.Controls.Add(qcard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: HOSPITAL OVERVIEW  (mirrors option 0 + floors 1-5)
        // ═════════════════════════════════════════════════════
        Panel PageHospitalOverview()
        {
            var page = ScrollPage("Hospital Overview", "All departments, floors and external facilities");
            var s = GetScroll(page);

            // Floor selector
            var floorCard = Card(0, 14, 1080, 80, "Browse by Floor");
            int fx = 14;
            for (int f = 1; f <= 5; f++)
            {
                int fn = f;
                var fb = Btn($"Floor {f}", AccentBlue, fx, 26, 110, 36);
                fb.Click += (s2, e2) =>
                {
                    string info = $"=== Floor {fn} ===\n";
                    info += $"Description: {neurai.Floors[fn - 1].Description}\n";
                    info += $"Elevator: {(neurai.Floors[fn - 1].hasElevatorAccess ? "Yes" : "No")}\n\n";
                    info += "Departments on this floor:\n";
                    bool found = false;
                    foreach (var dept in neurai.ActiveDepartments)
                        foreach (var room in dept.Rooms)
                            if (room.FloorNumber == fn) { info += $"  - {dept.DeptName}\n"; found = true; break; }
                    if (!found) info += "  No active departments.\n";
                    MessageBox.Show(info, $"Floor {fn} Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                floorCard.Controls.Add(fb);
                fx += 118;
            }
            s.Controls.Add(floorCard);

            // All departments table
            var dcard = Card(0, 94, 1080, 420, "All Departments");
            var dg = Grid(14, 34, 1052, 374);
            dg.Columns.Add("dept", "Department");
            dg.Columns.Add("docs", "Doctors");
            dg.Columns.Add("rooms", "Rooms");
            dg.Columns.Add("floor", "Primary Floor");
            foreach (var d in neurai.ActiveDepartments)
            {
                int pf = d.Rooms.Count > 0 ? d.Rooms[0].FloorNumber : 0;
                dg.Rows.Add(d.DeptName, d.Doctors.Count, d.Rooms.Count, pf > 0 ? $"Floor {pf}" : "—");
            }
            dcard.Controls.Add(dg);
            s.Controls.Add(dcard);

            // External facilities
            var ecard = Card(0, 528, 1080, 130, "External Facilities");
            ecard.Controls.Add(Lbl($"🌿 Garden: {(neurai.CampusFacilities.HasGarden ? "Available" : "N/A")}", FBold, AccentGrn, 14, 36));
            ecard.Controls.Add(Lbl($"🚑 Ambulances: {neurai.CampusFacilities.AmbulanceCount} on standby", FBold, AccentBlue, 14, 64));
            ecard.Controls.Add(Lbl($"🩸 Blood Bank: {neurai.CampusFacilities.BloodBankCapacity} units", FBold, AccentRed, 14, 92));
            ecard.Controls.Add(Lbl($"💊 Pharmacy Categories: {neurai.CampusFacilities.HospitalPharmacy.categories.Count}", FBold, AccentOrg, 380, 36));
            s.Controls.Add(ecard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: DOCTORS  
        // ═════════════════════════════════════════════════════
        Panel PageDoctors()
        {
            var page = ScrollPage("Doctor Management", "View, hire and assign doctors");
            var s = GetScroll(page);

            // ───────────── GRID ─────────────
            var gcard = Card(0, 14, 1080, 380, "All Doctors");
            var dg = Grid(14, 34, 1052, 334);

            dg.Columns.Add("name", "Name");
            dg.Columns.Add("rank", "Rank");
            dg.Columns.Add("dept", "Department");
            dg.Columns.Add("room", "Room");
            dg.Columns.Add("fee", "Consult Fee");
            dg.Columns.Add("blood", "Blood");
            dg.Columns.Add("exp", "Exp (yrs)");

            void RefreshGrid()
            {
                dg.Rows.Clear();
                foreach (var dept in neurai.ActiveDepartments)
                {
                    foreach (var doc in dept.Doctors)
                    {
                        dg.Rows.Add(
                            doc.Name,
                            doc.Rank,
                            doc.Department,
                           string.IsNullOrEmpty(doc.AssignedRoom)
                                ? "—"
                               : doc.AssignedRoom.StartsWith("Room")
                                  ? doc.AssignedRoom
                                 : $"Room {doc.AssignedRoom}",
                            $"{doc.ConsultationFee:C}",
                            doc.BloodType,
                            doc.ExperienceYears
                        );
                    }
                }
            }

            RefreshGrid();
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            // ───────────── HIRE DOCTOR ─────────────
            var hcard = Card(0, 394, 700, 310, "➕ Hire New Doctor");

            int lx = 14, ly = 34, gap = 38;

            hcard.Controls.Add(Lbl("Name:", FBold, TextGrey, lx, ly));
            var tbName = TB(lx + 80, ly, 200); hcard.Controls.Add(tbName);

            hcard.Controls.Add(Lbl("Blood:", FBold, TextGrey, lx + 300, ly));
            var cbBlood = CB(lx + 350, ly, 90, new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
            hcard.Controls.Add(cbBlood);

            ly += gap;

            hcard.Controls.Add(Lbl("Dept:", FBold, TextGrey, lx, ly));
            var cbDept = CB(lx + 80, ly, 220, neurai.ActiveDepartments.Select(d => d.DeptName).ToArray());
            hcard.Controls.Add(cbDept);

            ly += gap;

            hcard.Controls.Add(Lbl("Rank:", FBold, TextGrey, lx, ly));
            var cbRank = CB(lx + 80, ly, 140, new[] { "Trainee", "Junior", "Senior", "Consultant" });
            hcard.Controls.Add(cbRank);

            ly += gap;

            hcard.Controls.Add(Lbl("Salary:", FBold, TextGrey, lx, ly));
            var tbSal = TB(lx + 80, ly, 90, "15000"); hcard.Controls.Add(tbSal);

            hcard.Controls.Add(Lbl("Fee:", FBold, TextGrey, lx + 200, ly));
            var tbFee = TB(lx + 240, ly, 90, "300"); hcard.Controls.Add(tbFee);

            ly += gap + 6;

            var hireBtn = Btn("✅ Hire Doctor", AccentGrn, lx, ly, 150, 36);

            hireBtn.Click += (s2, e2) =>
            {
                try
                {
                    string nm = tbName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(nm))
                    {
                        MessageBox.Show("Enter a name.");
                        return;
                    }

                    string deptName = cbDept.SelectedItem?.ToString() ?? "";
                    string blood = cbBlood.SelectedItem?.ToString() ?? "O+";
                    var rank = (Doctor.DoctorRank)cbRank.SelectedIndex;

                    decimal sal = decimal.TryParse(tbSal.Text, out decimal sv) ? sv : 15000;
                    decimal fee = decimal.TryParse(tbFee.Text, out decimal fv) ? fv : 300;

                    bool dup = neurai.ActiveDepartments.Any(d =>
                        d.Doctors.Exists(doc => doc.Name.Equals(nm, StringComparison.OrdinalIgnoreCase)));

                    if (dup)
                    {
                        MessageBox.Show($"Dr. {nm} already exists.");
                        return;
                    }

                    var newDoc = new Doctor(
                        nm, 30, GenderType.Male,
                        "00000000000000", "00000000000",
                        "new@neurai.com", "Unknown",
                        sal, 1.0,
                        deptName, blood,
                        $"MED-NEW-{DateTime.Now.Ticks % 99999}",
                        fee, 20, "",
                        rank
                    );

                    neurai.ActiveDepartments
                        .Find(d => d.DeptName.Equals(deptName, StringComparison.OrdinalIgnoreCase))
                        ?.Doctors.Add(newDoc);

                    HospitalData.SaveDoctors();
                    RefreshGrid();

                    MessageBox.Show($"Dr. {nm} hired!", "Success ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            };

            hcard.Controls.Add(hireBtn);
            s.Controls.Add(hcard);

            // ───────────── SMART ASSIGN ─────────────
            var acard = Card(720, 394, 360, 310, "🏠 Assign Doctor to Room");

            int ay = 34;

            acard.Controls.Add(Lbl("Doctor:", FBold, TextGrey, 14, ay));

            var cbDoctor = new ComboBox
            {
                Location = new Point(14, ay + 22),
                Size = new Size(320, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = FNorm
            };
            acard.Controls.Add(cbDoctor);

            ay += 62;

            acard.Controls.Add(Lbl("Available Rooms:", FBold, TextGrey, 14, ay));

            var cbRooms = new ComboBox
            {
                Location = new Point(14, ay + 22),
                Size = new Size(200, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = FNorm
            };
            acard.Controls.Add(cbRooms);

            ay += 62;

            var doctorMap = new Dictionary<string, (Doctor doc, Department dept)>();
            void LoadAvailableRooms()
            {
                cbRooms.Items.Clear();

                if (cbDoctor.SelectedItem == null) return;

                var selected = doctorMap[cbDoctor.SelectedItem.ToString()];
                var dept = selected.dept;

                foreach (var room in dept.Rooms)
                {
                    bool occupied = dept.Doctors.Any(d => d.AssignedRoom == room.RoomNumber.ToString());

                    if (!occupied)
                        cbRooms.Items.Add(room.RoomNumber);
                }

                if (cbRooms.Items.Count > 0)
                    cbRooms.SelectedIndex = 0;
            }
            cbDoctor.SelectedIndexChanged += (s2, e2) =>
            {
                LoadAvailableRooms();
            };

            foreach (var dept in neurai.ActiveDepartments)
            {
                foreach (var doc in dept.Doctors)
                {
                    string key = $"{doc.Name} ({dept.DeptName})";
                    doctorMap[key] = (doc, dept);
                    cbDoctor.Items.Add(key);
                }
            }

            if (cbDoctor.Items.Count > 0)
                cbDoctor.SelectedIndex = 0;

            cbDoctor.SelectedIndexChanged += (s2, e2) =>
            {
                cbRooms.Items.Clear();

                if (cbDoctor.SelectedItem == null) return;

                var selected = doctorMap[cbDoctor.SelectedItem.ToString()];
                var dept = selected.dept;

                foreach (var room in dept.Rooms)
                {
                    bool occupied = dept.Doctors.Any(d => d.AssignedRoom == room.RoomNumber.ToString());
                    if (!occupied)
                        cbRooms.Items.Add(room.RoomNumber);
                }

                if (cbRooms.Items.Count > 0)
                    cbRooms.SelectedIndex = 0;
            };

            var aBtn = Btn("Assign Room", AccentBlue, 14, ay, 140, 36);

            aBtn.Click += (s2, e2) =>
            {
                if (cbDoctor.SelectedItem == null || cbRooms.SelectedItem == null)
                {
                    MessageBox.Show("Select doctor and room.");
                    return;
                }

                var selected = doctorMap[cbDoctor.SelectedItem.ToString()];
                var doc = selected.doc;
                var dept = selected.dept;

                int roomNumber = (int)cbRooms.SelectedItem;

                bool occupied = dept.Doctors.Any(d => d.AssignedRoom == roomNumber.ToString());
                if (occupied)
                {
                    MessageBox.Show("Room already occupied!");
                    return;
                }

                doc.AssignedRoom = roomNumber.ToString();
                HospitalData.SaveDoctors();

                RefreshGrid();

                MessageBox.Show($"Dr. {doc.Name} assigned to Room {roomNumber}.", "Assigned ✅");

                LoadAvailableRooms();
            };

            acard.Controls.Add(aBtn);
            s.Controls.Add(acard);

            return page;
        }
        // ═════════════════════════════════════════════════════
        //  PAGE: PATIENTS  (HospitalEngine + JSON persistence)
        // ═════════════════════════════════════════════════════
        Panel PagePatients()
        {
            var page = ScrollPage("Patient Records", "Manage inpatient and outpatient records — saved to patients_data.json");
            var s = GetScroll(page);

            // ── Grid ──
            var gcard = Card(0, 14, 1080, 320, "Patient List");
            var dg = Grid(14, 34, 1052, 275);
            dg.Columns.Add("id", "ID");
            dg.Columns.Add("name", "Full Name");
            dg.Columns.Add("age", "Age");
            dg.Columns.Add("gender", "Gender");
            dg.Columns.Add("type", "Type");
            dg.Columns.Add("case", "Medical Case");
            dg.Columns.Add("blood", "Blood Type");
            dg.Columns.Add("risk", "Risk");
            dg.Columns.Add("pay", "Payment");
            dg.Columns.Add("phone", "Phone");

            void RefreshGrid()
            {
                dg.Rows.Clear();
                foreach (var p in HospitalEngine.patients)
                {
                    int row = dg.Rows.Add(
                        p.PatientId, p.Name, p.Age, p.Gender,
                        p.PatientType ?? "—", p.MedicalCase ?? "—",
                        p.BloodTypeStr ?? "—", p.Risk ?? "—",
                        p.PaymentMethods ?? "—", p.PhoneNumber ?? "—");
                    if (p.PatientType == "Inpatient")
                        dg.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 255);
                }
            }
            RefreshGrid();
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            // ── Add Patient form ──
            var acard = Card(0, 334, 1080, 310, "➕ Add New Patient");
            int lx = 14, ly = 34, gap = 38;

            acard.Controls.Add(Lbl("Name:", FBold, TextGrey, lx, ly));
            var tbNm = TB(lx + 60, ly, 180); acard.Controls.Add(tbNm);
            acard.Controls.Add(Lbl("Age:", FBold, TextGrey, lx + 260, ly));
            var tbAge = TB(lx + 295, ly, 55); acard.Controls.Add(tbAge);
            acard.Controls.Add(Lbl("Gender:", FBold, TextGrey, lx + 368, ly));
            var cbGen = CB(lx + 428, ly, 110, new[] { "Male", "Female" }); acard.Controls.Add(cbGen);
            acard.Controls.Add(Lbl("Type:", FBold, TextGrey, lx + 556, ly));
            var cbType = CB(lx + 596, ly, 120, new[] { "Outpatient", "Inpatient" }); acard.Controls.Add(cbType);

            ly += gap;
            acard.Controls.Add(Lbl("Phone:", FBold, TextGrey, lx, ly));
            var tbPhone = TB(lx + 60, ly, 140); acard.Controls.Add(tbPhone);
            acard.Controls.Add(Lbl("National ID:", FBold, TextGrey, lx + 218, ly));
            var tbNid = TB(lx + 308, ly, 155); acard.Controls.Add(tbNid);
            acard.Controls.Add(Lbl("Blood:", FBold, TextGrey, lx + 480, ly));
            var cbBlood = CB(lx + 522, ly, 80, new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" }); acard.Controls.Add(cbBlood);
            acard.Controls.Add(Lbl("Risk:", FBold, TextGrey, lx + 618, ly));
            var cbRisk = CB(lx + 652, ly, 100, new[] { "Low", "Medium", "High", "Critical" }); acard.Controls.Add(cbRisk);

            ly += gap;
            acard.Controls.Add(Lbl("Medical Case:", FBold, TextGrey, lx, ly));
            var tbCase = TB(lx + 100, ly, 210); acard.Controls.Add(tbCase);
            acard.Controls.Add(Lbl("Payment:", FBold, TextGrey, lx + 326, ly));
            var cbPay = CB(lx + 390, ly, 150, new[] { "Cash", "Insurance", "Special Case" }); acard.Controls.Add(cbPay);

            ly += gap;
            acard.Controls.Add(Lbl("Allergies:", FBold, TextGrey, lx, ly));
            var tbAlrg = TB(lx + 74, ly, 200, "None"); acard.Controls.Add(tbAlrg);
            acard.Controls.Add(Lbl("Med. History:", FBold, TextGrey, lx + 290, ly));
            var tbHist = TB(lx + 384, ly, 290); acard.Controls.Add(tbHist);

            // Inpatient-only fields
            ly += gap + 4;
            var inpPanel = new Panel
            {
                Location = new Point(lx, ly),
                Size = new Size(1050, 38),
                BackColor = Color.FromArgb(232, 245, 255),
                Visible = false
            };
            inpPanel.Controls.Add(Lbl("Ward:", FBold, TextGrey, 0, 9));
            var tbWard = TB(46, 5, 120, "Ward"); inpPanel.Controls.Add(tbWard);
            inpPanel.Controls.Add(Lbl("Room#:", FBold, TextGrey, 178, 9));
            var tbRoom = TB(228, 5, 55, "1"); inpPanel.Controls.Add(tbRoom);
            inpPanel.Controls.Add(Lbl("Bed#:", FBold, TextGrey, 295, 9));
            var tbBed = TB(338, 5, 55, "1"); inpPanel.Controls.Add(tbBed);
            inpPanel.Controls.Add(Lbl("Physician:", FBold, TextGrey, 406, 9));
            var tbPhys = TB(478, 5, 155, "Dr. name"); inpPanel.Controls.Add(tbPhys);
            inpPanel.Controls.Add(Lbl("Has Op?", FBold, TextGrey, 646, 9));
            var chkOp = new CheckBox { Location = new Point(704, 7), Size = new Size(20, 20) }; inpPanel.Controls.Add(chkOp);
            inpPanel.Controls.Add(Lbl("Op Type:", FBold, TextGrey, 732, 9));
            var tbOpT = TB(796, 5, 170); inpPanel.Controls.Add(tbOpT);
            acard.Controls.Add(inpPanel);
            cbType.SelectedIndexChanged += (s2, e2) => inpPanel.Visible = cbType.SelectedItem?.ToString() == "Inpatient";

            ly += 44;
            var addBtn = Btn("➕ Add Patient", AccentBlue, lx, ly, 150, 34);
            addBtn.Click += (s2, e2) =>
            {
                string nm = tbNm.Text.Trim();
                if (string.IsNullOrWhiteSpace(nm)) { MessageBox.Show("Enter a name."); return; }

                int nextId = HospitalEngine.patients.Any() ? HospitalEngine.patients.Max(p => p.PatientId) + 1 : 1001;
                int age = int.TryParse(tbAge.Text, out int av) ? av : 0;
                bool isInp = cbType.SelectedItem?.ToString() == "Inpatient";

                Patient newP;
                if (isInp)
                {
                    var inp = new Inpatient();
                    inp.WardName = tbWard.Text.Trim();
                    inp.RoomNumber = int.TryParse(tbRoom.Text, out int rn) ? rn : 0;
                    inp.BedNumber = int.TryParse(tbBed.Text, out int bn) ? bn : 0;
                    inp.AttendingPhysician = tbPhys.Text.Trim();
                    inp.HasOperations = chkOp.Checked;
                    inp.OperationType = tbOpT.Text.Trim();
                    newP = inp;
                }
                else newP = new Outpatient();

                newP.PatientId = nextId;
                newP.Name = nm;
                newP.Age = age;
                newP.Gender = cbGen.SelectedIndex == 1 ? GenderType.Female : GenderType.Male;
                newP.PhoneNumber = tbPhone.Text.Trim();
                newP.NationalId = tbNid.Text.Trim();
                newP.BloodTypeStr = cbBlood.SelectedItem?.ToString() ?? "O+";
                newP.Risk = cbRisk.SelectedItem?.ToString() ?? "Low";
                newP.MedicalCase = tbCase.Text.Trim();
                newP.PaymentMethods = cbPay.SelectedItem?.ToString() ?? "Cash";
                newP.Allergies = tbAlrg.Text.Trim();
                newP.MedicalHistory = tbHist.Text.Trim();
                newP.PatientType = isInp ? "Inpatient" : "Outpatient";

                HospitalEngine.patients.Add(newP);
                _engine.SaveData();
                RefreshGrid();
                MessageBox.Show($"Patient '{nm}' added (ID {nextId}) — saved.", "Added ✅");
                tbNm.Clear(); tbAge.Clear(); tbPhone.Clear(); tbNid.Clear(); tbCase.Clear();
            };
            acard.Controls.Add(addBtn);

            var delBtn = Btn("🗑 Delete Selected", AccentRed, lx + 160, ly, 160, 34);
            delBtn.Click += (s2, e2) =>
            {
                if (dg.SelectedRows.Count == 0) { MessageBox.Show("Select a row first."); return; }
                int selId = Convert.ToInt32(dg.SelectedRows[0].Cells["id"].Value);
                var pat = HospitalEngine.patients.FirstOrDefault(p => p.PatientId == selId);
                if (pat == null) return;
                if (MessageBox.Show($"Delete patient '{pat.Name}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    HospitalEngine.patients.Remove(pat);
                    _engine.SaveData();
                    RefreshGrid();
                }
            };
            acard.Controls.Add(delBtn);
            s.Controls.Add(acard);

            // ── Search ──
            var scard = Card(0, 660, 1080, 68);
            scard.Controls.Add(Lbl("Search:", FBold, TextGrey, 14, 22));
            var tbSrch = TB(74, 18, 260, "Name or National ID..."); scard.Controls.Add(tbSrch);
            var srchBtn = Btn("🔍 Search", AccentBlue, 344, 16, 110, 34); scard.Controls.Add(srchBtn);
            var clrBtn = Btn("Clear", TextGrey, 462, 16, 80, 34); scard.Controls.Add(clrBtn);
            srchBtn.Click += (s2, e2) =>
            {
                string q = tbSrch.Text.Trim().ToLower();
                dg.Rows.Clear();
                foreach (var p in HospitalEngine.patients.Where(p =>
                    (p.Name ?? "").ToLower().Contains(q) || (p.NationalId ?? "").Contains(q)))
                    dg.Rows.Add(p.PatientId, p.Name, p.Age, p.Gender, p.PatientType ?? "—",
                        p.MedicalCase ?? "—", p.BloodTypeStr ?? "—", p.Risk ?? "—",
                        p.PaymentMethods ?? "—", p.PhoneNumber ?? "—");
            };
            clrBtn.Click += (s2, e2) => { tbSrch.Text = "Name or National ID..."; tbSrch.ForeColor = TextGrey; RefreshGrid(); };
            s.Controls.Add(scard);

            // ── Online Registered Patients (from Patient Portal) ──
            var ocard = Card(0, 742, 1080, 300, "🌐  Online Registered Patients (Patient Portal)");
            var odg = Grid(14, 42, 1052, 244);
            odg.Columns.Add("nid", "National ID");
            odg.Columns.Add("name", "Full Name");
            odg.Columns.Add("phone", "Phone");
            odg.Columns.Add("address", "Address");
            odg.Columns.Add("status", "Status");

            void RefreshOnlineGrid()
            {
                odg.Rows.Clear();
                foreach (var op in hospitalService.AllPatients)
                {
                    int r = odg.Rows.Add(
                        op.NationalID,
                        op.NameEnglish,
                        op.Phone ?? "—",
                        op.Address ?? "—",
                        op.IsRegistered ? "✅ Registered" : "⏳ Pending");
                    odg.Rows[r].DefaultCellStyle.BackColor = Color.FromArgb(232, 250, 238);
                }
                if (odg.Rows.Count == 0)
                {
                    int r = odg.Rows.Add("—", "No online registrations yet", "—", "—", "—");
                    odg.Rows[r].DefaultCellStyle.ForeColor = TextGrey;
                }
            }
            RefreshOnlineGrid();
            ocard.Controls.Add(odg);
            s.Controls.Add(ocard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: PHARMACY  (mirrors RunPharmacyMenu)
        // ═════════════════════════════════════════════════════
        Panel PagePharmacy()
        {
            var page = ScrollPage("Pharmacy", "Medicine inventory — mirrors console Pharmacy Terminal");
            var s = GetScroll(page);
            var pharmacy = neurai.CampusFacilities.HospitalPharmacy;

            // Search + filter row
            var fcard = Card(0, 14, 1080, 72);
            fcard.Controls.Add(Lbl("Department:", FBold, TextGrey, 14, 22));
            var cbDept = CB(100, 18, 210, new[] { "— All —" }.Concat(pharmacy.categories.Keys).ToArray()); fcard.Controls.Add(cbDept);
            fcard.Controls.Add(Lbl("Search:", FBold, TextGrey, 330, 22));
            var tbSearch = TB(390, 18, 240, "Medicine name..."); fcard.Controls.Add(tbSearch);
            var srchBtn = Btn("🔍 Search", AccentBlue, 640, 18, 110, 34); fcard.Controls.Add(srchBtn);
            var clrBtn = Btn("Clear", TextGrey, 760, 18, 80, 34); fcard.Controls.Add(clrBtn);
            s.Controls.Add(fcard);

            // Grid
            var gcard = Card(0, 86, 1080, 420, "Medicine Stock");
            var dg = Grid(14, 34, 1052, 374);
            dg.Columns.Add("id", "ID"); dg.Columns.Add("name", "Medicine"); dg.Columns.Add("dept", "Department");
            dg.Columns.Add("price", "Price"); dg.Columns.Add("qty", "Qty"); dg.Columns.Add("strips", "Strips");
            dg.Columns.Add("tabs", "Tabs/Strip"); dg.Columns.Add("exp", "Expiry"); dg.Columns.Add("status", "Status");

            void FillGrid(string filter, string q)
            {
                dg.Rows.Clear();
                foreach (var kv in pharmacy.categories)
                {
                    if (filter != "— All —" && !kv.Key.Equals(filter, StringComparison.OrdinalIgnoreCase)) continue;
                    foreach (var m in kv.Value)
                    {
                        if (!string.IsNullOrWhiteSpace(q) && q != "Medicine name..." && m.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0) continue;
                        string st = m.IsExpired() ? "❌ Expired" : m.Quantity < 10 ? "⚠️ Low" : "✅ OK";
                        int row = dg.Rows.Add(m.MedicineId, m.Name, kv.Key, $"{m.Price:F2}",
                                              m.Quantity, m.NumberOfStrips, m.NumberOfTabletsPerStrip,
                                              $"{m.ExpiryMonth:D2}/{m.ExpiryYear}", st);
                        if (m.IsExpired()) dg.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                        else if (m.Quantity < 10) dg.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(255, 253, 220);
                    }
                }
            }
            FillGrid("— All —", "");
            cbDept.SelectedIndexChanged += (s2, e2) => FillGrid(cbDept.SelectedItem?.ToString(), tbSearch.Text);
            srchBtn.Click += (s2, e2) => FillGrid(cbDept.SelectedItem?.ToString(), tbSearch.Text);
            clrBtn.Click += (s2, e2) => { tbSearch.Text = "Medicine name..."; tbSearch.ForeColor = TextGrey; cbDept.SelectedIndex = 0; FillGrid("— All —", ""); };
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: BILLING
        // ═════════════════════════════════════════════════════
        Panel PageBilling()
        {
            var page = ScrollPage("Billing & Financial", "Generate invoices and apply discounts");
            var s = GetScroll(page);

            var pharmacy = neurai.CampusFacilities.HospitalPharmacy;
            var services = new System.Collections.Generic.List<(string name, decimal price)>();
            var prescriptionItems = new System.Collections.Generic.List<(Medicine med, int qty)>();

            var fcard = Card(0, 14, 860, 660, "Create Invoice");
            int lx = 14, ly = 14, gap = 38;

            // ── Patient Info Section ──
            var patientHeader = new Panel
            {
                Location = new Point(lx, ly),
                Size = new Size(820, 44),
                BackColor = Color.FromArgb(21, 101, 192)
            };
            patientHeader.Controls.Add(new Label
            {
                Text = "  👤  Patient Information",
                Font = FBold,
                ForeColor = Color.White,
                Location = new Point(8, 12),
                AutoSize = true
            });
            fcard.Controls.Add(patientHeader);

            ly += 52;
            fcard.Controls.Add(Lbl("Patient ID:", FBold, TextGrey, lx, ly));
            var tbPid = TB(lx + 90, ly, 150); fcard.Controls.Add(tbPid);

            var btnLookup = Btn("🔍 Lookup", AccentBlue, lx + 254, ly - 2, 90, 28);
            fcard.Controls.Add(btnLookup);

            fcard.Controls.Add(Lbl("Patient Name:", FBold, TextGrey, lx + 360, ly));
            var tbPnm = TB(lx + 460, ly, 200); fcard.Controls.Add(tbPnm);

            ly += gap + 4;

            // ── Separator ──
            var sep = new Panel
            {
                Location = new Point(lx, ly),
                Size = new Size(820, 1),
                BackColor = Color.FromArgb(200, 210, 230)
            };
            fcard.Controls.Add(sep);
            ly += 10;

            // ── Patient ID with auto-fill ──

            btnLookup.Click += (s2, e2) =>
            {
                string pid = tbPid.Text.Trim();
                if (string.IsNullOrWhiteSpace(pid))
                { MessageBox.Show("Enter a Patient ID first."); return; }

                var rp = receptionService.AllPatients.Find(p =>
                    p.NationalID.Equals(pid, System.StringComparison.OrdinalIgnoreCase));
                if (rp != null) { tbPnm.Text = rp.NameEnglish; return; }

                var hp = hospitalService.AllPatients.Find(p =>
                    p.NationalID.Equals(pid, System.StringComparison.OrdinalIgnoreCase));
                if (hp != null) { tbPnm.Text = hp.NameEnglish; return; }

                MessageBox.Show("Patient ID not found. You can type the name manually.",
                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // ── Services ──
            ly += gap;
            fcard.Controls.Add(Lbl("Services:", FBold, TextGrey, lx, ly));
            ly += 22;
            var lb = new ListBox { Location = new Point(lx, ly), Size = new Size(500, 80), Font = FNorm };
            fcard.Controls.Add(lb);
            ly += 88;
            var tbSvc = TB(lx, ly, 210, "Service name"); fcard.Controls.Add(tbSvc);
            var tbPrc = TB(lx + 220, ly, 80, "Price"); fcard.Controls.Add(tbPrc);
            var addSvc = Btn("+ Add", AccentGrn, lx + 310, ly - 2, 80, 30);
            fcard.Controls.Add(addSvc);
            addSvc.Click += (s2, e2) =>
            {
                string sn = tbSvc.Text.Trim();
                if (sn == "" || sn == "Service name") return;
                decimal.TryParse(tbPrc.Text, out decimal pr);
                services.Add((sn, pr));
                lb.Items.Add($"{sn}  —  {pr:C}");
                tbSvc.Clear(); tbPrc.Clear();
            };

            // ── Prescription ──
            ly += gap + 4;
            fcard.Controls.Add(Lbl("Prescription:", FBold, TextGrey, lx, ly));

            var allMeds = new System.Collections.Generic.List<Medicine>();
            foreach (var kv in pharmacy.categories)
                allMeds.AddRange(kv.Value);

            var deptNames = new[] { "— All —" }.Concat(pharmacy.categories.Keys).ToArray();
            var cbDept = CB(lx + 110, ly, 180, deptNames); fcard.Controls.Add(cbDept);

            fcard.Controls.Add(Lbl("Search:", FBold, TextGrey, lx + 302, ly));
            var tbMedSearch = TB(lx + 360, ly, 160, "Medicine name"); fcard.Controls.Add(tbMedSearch);

            ly += 30;
            var lbMeds = new ListBox
            {
                Location = new Point(lx, ly),
                Size = new Size(360, 90),
                Font = FNorm
            };
            fcard.Controls.Add(lbMeds);

            fcard.Controls.Add(Lbl("Qty:", FBold, TextGrey, lx + 374, ly));
            var tbQty = TB(lx + 410, ly, 60, "1"); fcard.Controls.Add(tbQty);
            var btnAddMed = Btn("+ Add Med", AccentOrg, lx + 480, ly, 110, 30);
            fcard.Controls.Add(btnAddMed);

            ly += 96;
            var lbPrescription = new ListBox
            {
                Location = new Point(lx, ly),
                Size = new Size(500, 70),
                Font = FNorm
            };
            fcard.Controls.Add(lbPrescription);

            void RefreshMedList()
            {
                lbMeds.Items.Clear();
                string dept = cbDept.SelectedItem?.ToString() ?? "— All —";
                string search = tbMedSearch.Text.Trim().ToLower();
                foreach (var m in allMeds)
                {
                    bool inDept = dept == "— All —" || pharmacy.categories.Any(kv =>
                        kv.Key == dept && kv.Value.Contains(m));
                    bool matchSearch = string.IsNullOrEmpty(search) ||
                                       search == "medicine name" ||
                                       m.Name.ToLower().Contains(search);
                    if (inDept && matchSearch && !m.IsExpired() && m.Quantity > 0)
                        lbMeds.Items.Add($"{m.Name}  —  {m.Price:C}  (Stock: {m.Quantity})");
                }
            }
            RefreshMedList();
            cbDept.SelectedIndexChanged += (s2, e2) => RefreshMedList();
            tbMedSearch.TextChanged += (s2, e2) => RefreshMedList();

            btnAddMed.Click += (s2, e2) =>
            {
                if (lbMeds.SelectedIndex < 0)
                { MessageBox.Show("Select a medicine first."); return; }
                if (!int.TryParse(tbQty.Text, out int qty) || qty <= 0)
                { MessageBox.Show("Enter a valid quantity."); return; }

                string selName = lbMeds.SelectedItem.ToString()
                    .Split(new[] { "  —  " }, System.StringSplitOptions.None)[0];
                Medicine selMed = allMeds.Find(m => m.Name == selName);
                if (selMed == null) return;

                if (qty > selMed.Quantity)
                { MessageBox.Show($"Only {selMed.Quantity} units in stock."); return; }

                selMed.Quantity -= qty;

                var existing = prescriptionItems.FindIndex(x => x.med.Name == selMed.Name);
                if (existing >= 0)
                {
                    var old = prescriptionItems[existing];
                    prescriptionItems[existing] = (old.med, old.qty + qty);
                }
                else
                {
                    prescriptionItems.Add((selMed, qty));
                }

                decimal medPrice = (decimal)(selMed.Price * qty);
                services.Add(($"💊 {selMed.Name} x{qty}", medPrice));
                lb.Items.Add($"💊 {selMed.Name} x{qty}  —  {medPrice:C}");

                lbPrescription.Items.Clear();
                foreach (var pi in prescriptionItems)
                    lbPrescription.Items.Add(
                        $"💊 {pi.med.Name}  x{pi.qty}  —  {pi.med.Price * pi.qty:C}");

                RefreshMedList();
                tbQty.Text = "1";
            };

            // ── Discount ──
            ly += 78;
            fcard.Controls.Add(Lbl("Discount:", FBold, TextGrey, lx, ly));
            var cbDisc = CB(lx + 80, ly, 220,
                new[] { "None (0%)", "Insurance (30% off)", "Special Case (15% off)" });
            fcard.Controls.Add(cbDisc);

            // ── Receipt ──
            ly += gap + 6;
            var receipt = new RichTextBox
            {
                Location = new Point(lx, ly),
                Size = new Size(820, 0),
                Font = FMono,
                BackColor = Color.FromArgb(246, 249, 252),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            fcard.Controls.Add(receipt);

            ly += 6;
            var genBtn = Btn("🧾 Generate Invoice", AccentBlue, lx, ly, 190, 36);
            fcard.Controls.Add(genBtn);

            genBtn.Click += (s2, e2) =>
            {
                if (!int.TryParse(tbPid.Text, out int pid))
                { MessageBox.Show("Enter valid Patient ID."); return; }
                string pnm = tbPnm.Text.Trim();
                if (string.IsNullOrWhiteSpace(pnm) || pnm == "Type patient name here")
                { MessageBox.Show("Enter patient name."); return; }
                if (services.Count == 0)
                { MessageBox.Show("Add at least one service or medicine."); return; }

                decimal sub = services.Sum(sv => sv.price);
                decimal disc = cbDisc.SelectedIndex == 1 ? sub * 0.30m
                              : cbDisc.SelectedIndex == 2 ? sub * 0.15m : 0;
                decimal total = sub - disc;

                string eq = new string('=', 44);
                string dl = new string('-', 44);
                string txt = eq + "\n   INVOICE — " +
                             System.DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\n" + eq + "\n";
                txt += "Patient ID : " + pid + "\nName       : " + pnm + "\n" + dl + "\n";

                txt += "Services:\n";
                foreach (var sv in services)
                    if (!sv.name.StartsWith("💊"))
                        txt += "  " + sv.name.PadRight(28) + sv.price.ToString("C").PadLeft(9) + "\n";

                if (prescriptionItems.Count > 0)
                {
                    txt += dl + "\nPrescription:\n";
                    foreach (var pi in prescriptionItems)
                        txt += $"  💊 {pi.med.Name,-24} x{pi.qty,3}" +
                               $"   {(pi.med.Price * pi.qty),8:C}\n";
                }

                txt += dl + "\nSubtotal   : " + sub.ToString("C").PadLeft(10)
                     + "\nDiscount   : -" + disc.ToString("C").PadLeft(9) + "\n"
                     + eq + "\nTOTAL DUE  : " + total.ToString("C").PadLeft(10) + "\n" + eq;

                receipt.Text = txt;
                receipt.Height = 260;
                genBtn.Location = new Point(lx, receipt.Bottom + 10);
                fcard.Height = genBtn.Bottom + 20;
            };

            s.Controls.Add(fcard);

            // ── Invoice History ──
            var hcard = Card(880, 0, 480, 340, "Invoice History");
            var hg = Grid(14, 34, 452, 290);
            hg.Columns.Add("id", "Invoice ID");
            hg.Columns.Add("date", "Date");
            hg.Columns.Add("pt", "Patient");
            hg.Columns.Add("total", "Total");
            hcard.Controls.Add(hg);
            genBtn.Click += (s2, e2) =>
            {
                if (receipt.Text.Length > 10 && int.TryParse(tbPid.Text, out int pid2))
                    hg.Rows.Add(new System.Random().Next(1000, 9999),
                                System.DateTime.Now.ToString("yyyy-MM-dd"),
                                tbPnm.Text,
                                $"{services.Sum(sv => sv.price):C}");
            };
            s.Controls.Add(hcard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: AMBULANCE
        // ═════════════════════════════════════════════════════
        Panel PageAmbulance()
        {
            var page = ScrollPage("Ambulance Fleet", "Real-time dispatch — use Emergency button to open the emergency room");
            var s = GetScroll(page);

            // ── Emergency Room quick-access button ────────────
            var emergBtn = new Button
            {
                Text = "🚨  Open Emergency Room",
                Size = new Size(260, 44),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(180, 20, 20),
                Cursor = Cursors.Hand
            };
            emergBtn.FlatAppearance.BorderSize = 0;
            emergBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 40, 40);
            emergBtn.Click += (s2, e2) => NavigateTo("Emergency");
            s.Controls.Add(emergBtn);

            var fleet = neurai.CampusFacilities.AmbulanceFleet;
            int cardH = fleet.Count * 62 + 90;
            var fcard = Card(0, 58, 700, cardH, "🚑 Fleet Status");

            // Summary row
            int ready = fleet.Count(a => a.IsAvailable);
            fcard.Controls.Add(Lbl($"{ready}/{fleet.Count} ambulances available", FBold, ready > 0 ? AccentGrn : AccentRed, 14, 36));

            for (int i = 0; i < fleet.Count; i++)
            {
                int ai = i;
                var amb = fleet[ai];
                bool avail = amb.IsAvailable;

                var lbl = Lbl(
                    $"🚑  {amb.VehicleNumber}  ({amb.Model})  —  {(avail ? "🟢 Available" : "🔴 On Mission")}",
                    FBold, avail ? AccentGrn : AccentRed, 14, 62 + ai * 60);

                var db = Btn("Dispatch", AccentRed, 460, 58 + ai * 60, 100, 32);
                var rb = Btn("Return", AccentGrn, 568, 58 + ai * 60, 90, 32);
                db.Enabled = avail;
                rb.Enabled = !avail;

                db.Click += (s2, e2) =>
                {
                    amb.Dispatch("Emergency Location");
                    lbl.Text = $"🚑  {amb.VehicleNumber}  ({amb.Model})  —  🔴 On Mission";
                    lbl.ForeColor = AccentRed;
                    db.Enabled = false; rb.Enabled = true;
                };

                rb.Click += (s2, e2) =>
                {
                    amb.ReturnToBase();
                    lbl.Text = $"🚑  {amb.VehicleNumber}  ({amb.Model})  —  🟢 Available";
                    lbl.ForeColor = AccentGrn;
                    db.Enabled = true; rb.Enabled = false;
                };

                fcard.Controls.Add(lbl);
                fcard.Controls.Add(db);
                fcard.Controls.Add(rb);
            }
            s.Controls.Add(fcard);

            // ── Add ambulance panel — ID is auto-generated ────
            var acard = Card(720, 58, 360, 160, "➕ Add Ambulance");
            // ID is auto-generated — show a preview label
            int nextNum = fleet.Count + 1;
            var idPreview = Lbl($"Auto ID: AMB-{nextNum:D3}", FBold, AccentBlue, 14, 38, 320);
            acard.Controls.Add(idPreview);
            acard.Controls.Add(Lbl("Model:", FBold, TextGrey, 14, 68));
            var tbModel = TB(14, 40, 150, "Model");
            var addBtn = Btn("Add to Fleet", AccentBlue, 14, 102, 140, 34);
            addBtn.Click += (s2, e2) =>
            {
                string model = tbModel.Text.Trim();
                if (string.IsNullOrWhiteSpace(model) || model == "e.g. Ford Transit")
                { MessageBox.Show("Enter the vehicle model."); return; }
                addBtn.Click += (sender, ev) =>
                {
                    neurai.CampusFacilities.CreateAmbulance();
                    NavigateTo("Emergency");
                };
                NavigateTo("Ambulance"); // refresh page
            };
            acard.Controls.Add(addBtn);
            s.Controls.Add(acard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: EMERGENCY  — VitalSigns + Ambulance + Transfer
        // ═════════════════════════════════════════════════════
        Panel PageEmergency()
        {
            var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // ===== TITLE =====
            var title = new Label
            {
                Text = "Emergency Room",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Top = 20,
                Left = 20,
                AutoSize = true
            };

            // ===== TRIAGE INPUT =====
            var tbHR = new TextBox { Left = 20, Top = 70, Width = 60, Text = "75" };
            var tbO2 = new TextBox { Left = 90, Top = 70, Width = 60, Text = "98" };
            var tbTemp = new TextBox { Left = 160, Top = 70, Width = 60, Text = "37" };
            var tbBP = new TextBox { Left = 230, Top = 70, Width = 60, Text = "110" };

            var lblTriage = new Label
            {
                Text = "Triage: —",
                Top = 110,
                Left = 20,
                AutoSize = true
            };

            var btnRegister = new Button
            {
                Text = "Register",
                BackColor = Color.Red,
                ForeColor = Color.White,
                Left = 20,
                Top = 140,
                Width = 150,
                Height = 40
            };

            // ===== CASES GRID =====
            var grid = new DataGridView
            {
                Left = 20,
                Top = 220,
                Width = 600,
                Height = 250,
                ColumnCount = 3
            };

            grid.Columns[0].Name = "ID";
            grid.Columns[1].Name = "Name";
            grid.Columns[2].Name = "Triage";

            // ===== AMBULANCE PANEL =====
            var ambPanel = new Panel
            {
                Left = 650,
                Top = 70,
                Width = 250,
                Height = 400
            };

            var ambLabel = new Label
            {
                Text = "Ambulances",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Top = 10,
                Left = 10
            };

            ambPanel.Controls.Add(ambLabel);

            // sample ambulance buttons
            for (int i = 0; i < 4; i++)
            {
                var btn = new Button
                {
                    Text = $"AMB-00{i + 2} Dispatch",
                    BackColor = Color.Red,
                    ForeColor = Color.White,
                    Width = 200,
                    Height = 35,
                    Left = 10,
                    Top = 50 + (i * 50)
                };

                ambPanel.Controls.Add(btn);
            }

            // ===== REGISTER LOGIC =====
            int caseId = 1;

            btnRegister.Click += (s, e) =>
            {
                int hr = int.Parse(tbHR.Text);
                int o2 = int.Parse(tbO2.Text);
                double temp = double.Parse(tbTemp.Text);
                int bp = int.Parse(tbBP.Text);

                string triage;

                if (hr > 120 || o2 < 90 || temp > 39)
                    triage = "CRITICAL";
                else if (hr > 100 || temp > 37.5)
                    triage = "URGENT";
                else
                    triage = "STABLE";

                lblTriage.Text = "Triage: " + triage;

                grid.Rows.Add(caseId++, "Unknown", triage);
            };

            // ===== ADD EVERYTHING =====
            p.Controls.Add(title);
            p.Controls.Add(tbHR);
            p.Controls.Add(tbO2);
            p.Controls.Add(tbTemp);
            p.Controls.Add(tbBP);
            p.Controls.Add(lblTriage);
            p.Controls.Add(btnRegister);
            p.Controls.Add(grid);
            p.Controls.Add(ambPanel);

            return p;
        }
        // ═════════════════════════════════════════════════════
        //  PAGE: INVENTORY 
        // ═════════════════════════════════════════════════════
        Panel PageInventory()
        {
            var page = ScrollPage("Hospital Inventory", "Storage management ");
            var s = GetScroll(page);

            var inventory = new InventoryManager();

            // ── Stock grid ────────────────────────────────────
            var gcard = Card(0, 14, 1080, 420, "Current Stock");
            var dg = Grid(14, 34, 1052, 374);
            dg.Columns.Add("name", "Item");
            dg.Columns.Add("cat", "Category");
            dg.Columns.Add("qty", "Quantity");
            dg.Columns.Add("stat", "Status");

            void RefreshGrid()
            {
                dg.Rows.Clear();
                foreach (var item in inventory.Items)
                {
                    string status = item.Quantity < 5 ? "⚠️ LOW" : "✅ OK";
                    int row = dg.Rows.Add(item.Name, item.Category.ToString(), item.Quantity, status);
                    if (item.Quantity < 5)
                        dg.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                }
            }
            RefreshGrid();
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            // ── Search ────────────────────────────────────────
            var scard = Card(0, 434, 500, 140, "🔍 Search Item");
            scard.Controls.Add(Lbl("Item Name:", FBold, TextGrey, 14, 38));
            var tbSearch = TB(100, 34, 200); scard.Controls.Add(tbSearch);
            var srchBtn = Btn("Search", AccentBlue, 310, 32, 100, 32);
            var srchRes = Lbl("", FNorm, TextDark, 14, 78, 460); scard.Controls.Add(srchRes);
            srchBtn.Click += (s2, e2) =>
            {
                var found = inventory.SearchItem(tbSearch.Text.Trim());
                srchRes.Text = found != null
                    ? $"Found: {found.Name} | Category: {found.Category} | Qty: {found.Quantity}"
                    : "Item not found.";
                srchRes.ForeColor = found != null ? AccentGrn : AccentRed;
            };
            scard.Controls.Add(srchBtn);
            s.Controls.Add(scard);

            // ── Add/Restock ───────────────────────────────────
            var acard = Card(520, 434, 560, 140, "➕ Restock Item");
            acard.Controls.Add(Lbl("Name:", FBold, TextGrey, 14, 38));
            var tbItem = TB(70, 34, 150, "Item name"); acard.Controls.Add(tbItem);
            acard.Controls.Add(Lbl("Category:", FBold, TextGrey, 240, 38));
            var cbCat = CB(320, 34, 120, new[] { "Furniture", "Medical", "Cleaning", "Food" }); acard.Controls.Add(cbCat);
            acard.Controls.Add(Lbl("Qty:", FBold, TextGrey, 14, 78));
            var tbQty = TB(50, 74, 70); acard.Controls.Add(tbQty);
            var addBtn = Btn("Add / Restock", AccentGrn, 140, 72, 140, 34);
            addBtn.Click += (s2, e2) =>
            {
                string nm = tbItem.Text.Trim();
                if (string.IsNullOrWhiteSpace(nm) || nm == "Item name") { MessageBox.Show("Enter an item name."); return; }
                int qty = int.TryParse(tbQty.Text, out int qv) ? qv : 0;
                if (qty <= 0) { MessageBox.Show("Enter a valid quantity."); return; }

                var existing = inventory.SearchItem(nm);
                if (existing != null)
                    existing.Quantity += qty;
                else
                {
                    var cat = (ItemCategory)cbCat.SelectedIndex;
                    inventory.Items.Add(new StorageItem(nm, cat, qty));
                }
                RefreshGrid();
                inventory.Save();
                MessageBox.Show($"Stock updated for '{nm}'.", "Inventory Updated ✅");
                tbItem.Clear(); tbQty.Clear();
            };
            acard.Controls.Add(addBtn);
            s.Controls.Add(acard);

            return page;
        }
        // ═════════════════════════════════════════════════════
        //  PAGE: SECURITY  (mirrors RunSecurityMenu)
        // ═════════════════════════════════════════════════════
        Panel PageSecurity()
        {
            var page = ScrollPage("Security Department", "Patrol, lockdown and guard management");
            var s = GetScroll(page);

            bool locked = neurai.CampusSecurity.IsLockdownActive;

            // Status banner
            var banner = new Panel { Location = new Point(0, 0), Size = new Size(1080, 72), BackColor = locked ? AccentRed : AccentGrn };
            var statusLbl = new Label
            {
                Text = locked ? "🔴  HOSPITAL LOCKDOWN ACTIVE" : "🟢  HOSPITAL STATUS: SECURE",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(680, 50),
                Location = new Point(14, 12),
                TextAlign = ContentAlignment.MiddleLeft
            };
            banner.Controls.Add(statusLbl);

            var toggleBtn = Btn(locked ? "🔓 Lift Lockdown" : "🔒 Initiate Lockdown",
                                locked ? AccentGrn : AccentRed, 780, 18, 200, 38);
            toggleBtn.Click += (s2, e2) =>
            {
                neurai.CampusSecurity.ToggleLockdown();
                bool nl = neurai.CampusSecurity.IsLockdownActive;
                banner.BackColor = nl ? AccentRed : AccentGrn;
                statusLbl.Text = nl ? "🔴  HOSPITAL LOCKDOWN ACTIVE" : "🟢  HOSPITAL STATUS: SECURE";
                toggleBtn.Text = nl ? "🔓 Lift Lockdown" : "🔒 Initiate Lockdown";
                toggleBtn.BackColor = nl ? AccentGrn : AccentRed;
                MessageBox.Show(nl ? "LOCKDOWN INITIATED.\nAll exits sealed." : "LOCKDOWN LIFTED.\nNormal operations resumed.",
                    "Security Alert", MessageBoxButtons.OK, nl ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            };
            banner.Controls.Add(toggleBtn);
            s.Controls.Add(banner);

            // Guards grid
            var gcard = Card(0, 86, 660, 320, "Active Guard Roster");
            var dg = Grid(14, 34, 632, 272);
            dg.Columns.Add("badge", "Badge #"); dg.Columns.Add("name", "Officer Name"); dg.Columns.Add("shift", "Shift");
            void RefreshGuards()
            {
                dg.Rows.Clear();
                foreach (var g in Security.securities)
                    dg.Rows.Add($"SEC-{g.BadgeNumber:D3}", g.Name, g.Shift);
            }
            RefreshGuards();
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            // Patrol floor
            var pcard = Card(680, 86, 400, 180, "Dispatch Patrol to Floor");
            pcard.Controls.Add(Lbl("Floor (1–5):", FBold, TextGrey, 14, 44));
            var tbFloor = TB(110, 40, 60); pcard.Controls.Add(tbFloor);
            var patBtn = Btn("🚶 Patrol", AccentBlue, 14, 82, 130, 36);
            var patResult = Lbl("", FBold, TextGrey, 14, 130, 370); pcard.Controls.Add(patResult);
            patBtn.Click += (s2, e2) =>
            {
                if (int.TryParse(tbFloor.Text, out int fn) && fn >= 1 && fn <= 5)
                {
                    neurai.CampusSecurity.PatrolFloor(fn, neurai.Floors);
                    bool op = neurai.Floors[fn - 1].IsOperating;
                    patResult.Text = $"Floor {fn}: {(op ? "✅ Clear and Safe" : "🚫 Restricted")}";
                    patResult.ForeColor = op ? AccentGrn : AccentRed;
                }
                else patResult.Text = "Enter floor 1–5.";
            };
            pcard.Controls.Add(patBtn);
            s.Controls.Add(pcard);

            // Hire substitute guard
            var hcard = Card(0, 420, 660, 180, "➕ Hire Substitute Guard");
            hcard.Controls.Add(Lbl("Name:", FBold, TextGrey, 14, 42));
            var tbGnm = TB(70, 38, 210); hcard.Controls.Add(tbGnm);
            hcard.Controls.Add(Lbl("Shift:", FBold, TextGrey, 298, 42));
            var cbShift = CB(348, 38, 120, new[] { "Day", "Evening", "Night" }); hcard.Controls.Add(cbShift);
            var hgBtn = Btn("Hire Guard", AccentGrn, 14, 90, 130, 36);
            // AFTER
            hgBtn.Click += (s2, e2) =>
            {
                string nm = tbGnm.Text.Trim();
                string sh = cbShift.SelectedItem?.ToString() ?? "Day";
                if (string.IsNullOrWhiteSpace(nm)) { MessageBox.Show("Enter a name."); return; }

                int nextBadge = (Security.securities.Any() ? Security.securities.Max(g => g.BadgeNumber) : 0) + 1;
                string badgeStr = $"SEC-{nextBadge:D3}";

                var newGuard = new Security(nm, badgeStr, sh);
                newGuard.NationalId = $"SUB-{nextBadge:D3}";

                // REMOVED: neurai.CampusSecurity.Guards.Add(newGuard);  ← no longer needed
                HospitalData.AddSecurity(newGuard); // adds to Security.securities + saves JSON

                RefreshGuards(); // now correctly shows the updated Security.securities
                MessageBox.Show($"Guard '{nm}' hired for {sh} shift.", "Hired ✅");
                tbGnm.Clear();
            };
            hcard.Controls.Add(hgBtn);
            s.Controls.Add(hcard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: STAFF  (Employees / Nurses / Pharmacists)
        // ═════════════════════════════════════════════════════
        Panel PageStaff()
        {
            var page = ScrollPage("Staff Management", "Employees, nurses, pharmacists");
            var s = GetScroll(page);

            // Tab buttons
            var tabBar = new Panel { Location = new Point(0, 0), Size = new Size(1080, 44), BackColor = PageBg };
            Panel activeTab = null;

            Panel EmpTab()
            {
                var t = new Panel { BackColor = PageBg, Size = new Size(1080, 400) };
                var c = Card(0, 14, 1080, 380, $"Employees  ({Employee.employees.Count})");
                var g = Grid(14, 34, 1052, 330);
                g.Columns.Add("id", "ID"); g.Columns.Add("name", "Name"); g.Columns.Add("age", "Age");
                g.Columns.Add("email", "Email"); g.Columns.Add("phone", "Phone"); g.Columns.Add("sal", "Salary"); g.Columns.Add("exp", "Exp");
                foreach (var e in Employee.employees)
                    g.Rows.Add(e.EmployeeId, e.Name, e.Age, e.Email, e.PhoneNumber, $"{e.Salary:C}", $"{e.ExperienceYears}y");
                c.Controls.Add(g); t.Controls.Add(c); return t;
            }

            Panel NurseTab()
            {
                var t = new Panel { BackColor = PageBg, Size = new Size(1080, 620) };

                // ── Display grid (unchanged) ──
                var c = Card(0, 14, 1080, 380, $"Nurses  ({Nurse.nurses.Count})");
                var g = Grid(14, 34, 1052, 330);
                g.Columns.Add("name", "Name"); g.Columns.Add("lic", "License"); g.Columns.Add("spec", "Speciality");
                g.Columns.Add("ward", "Ward"); g.Columns.Add("call", "On Call"); g.Columns.Add("load", "Patient Load");
                void RefreshNurses()
                {
                    g.Rows.Clear();
                    foreach (var n in Nurse.nurses)
                        g.Rows.Add(n.Name, n.LicenseNumber, n.Speciality, n.AssignedWard, n.IsOnCall ? "✅" : "–", n.CurrentPatientLoad);
                }
                RefreshNurses();
                c.Controls.Add(g); t.Controls.Add(c);

                // ── Hire Nurse form ──
                var hc = Card(0, 394, 1080, 210, "➕ Hire New Nurse");
                int lx = 14, ly = 36, gap = 38;

                hc.Controls.Add(Lbl("Name:", FBold, TextGrey, lx, ly));
                var tbNName = TB(lx + 60, ly, 200); hc.Controls.Add(tbNName);

                hc.Controls.Add(Lbl("License:", FBold, TextGrey, lx + 280, ly));
                var tbNLic = TB(lx + 350, ly, 150, $"NUR-{DateTime.Now.Ticks % 99999}"); hc.Controls.Add(tbNLic);

                ly += gap;
                hc.Controls.Add(Lbl("Speciality:", FBold, TextGrey, lx, ly));
                var cbNSpec = CB(lx + 80, ly, 180, new[] { "General", "Pediatric", "Surgical", "ICU", "Emergency" }); hc.Controls.Add(cbNSpec);

                hc.Controls.Add(Lbl("Ward:", FBold, TextGrey, lx + 280, ly));
                var cbNWard = CB(lx + 330, ly, 160, new[] { "Ward A", "Ward B", "ICU", "ER", "Pediatrics", "Surgery" }); hc.Controls.Add(cbNWard);

                ly += gap;
                hc.Controls.Add(Lbl("Salary:", FBold, TextGrey, lx, ly));
                var tbNSal = TB(lx + 60, ly, 90, "8000"); hc.Controls.Add(tbNSal);

                hc.Controls.Add(Lbl("Exp (yrs):", FBold, TextGrey, lx + 168, ly));
                var tbNExp = TB(lx + 255, ly, 60, "1"); hc.Controls.Add(tbNExp);

                var cbNCall = new CheckBox { Text = "On Call", Font = FBold, ForeColor = TextGrey, Location = new Point(lx + 340, ly), AutoSize = true };
                hc.Controls.Add(cbNCall);

                ly += gap + 4;
                var hireNBtn = Btn("✅ Hire Nurse", AccentGrn, lx, ly, 140, 34);
                hireNBtn.Click += (s2, e2) =>
                {
                    try
                    {
                        string nm = tbNName.Text.Trim();
                        if (string.IsNullOrWhiteSpace(nm)) { MessageBox.Show("Enter a name."); return; }
                        string lic = tbNLic.Text.Trim();
                        if (Nurse.nurses.Any(n => n.LicenseNumber == lic)) { MessageBox.Show("License number already exists."); return; }

                        decimal sal = decimal.TryParse(tbNSal.Text, out decimal sv) ? sv : 8000;
                        double exp = double.TryParse(tbNExp.Text, out double ev) ? ev : 1;

                        // Nurse constructor auto-calls HospitalData.AddNurse(this) + SaveNurses()
                        var _ = new Nurse(
                            nm, 25, GenderType.Female, $"NID-{DateTime.Now.Ticks % 99999}",
                            "00000000000", "nurse@neurai.com", "Unknown",
                            sal, exp,
                            lic,
                            cbNSpec.SelectedItem?.ToString() ?? "General",
                            "BSN",
                            cbNWard.SelectedItem?.ToString() ?? "Ward A",
                            cbNCall.Checked, 0
                        );

                        RefreshNurses();
                        MessageBox.Show($"Nurse '{nm}' hired!", "Success ✅");
                        tbNName.Clear();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                };
                hc.Controls.Add(hireNBtn);
                t.Controls.Add(hc);

                return t;
            }
            Panel PharmTab()
            {
                var t = new Panel { BackColor = PageBg, Size = new Size(1080, 580) };

                // ── Display grid (unchanged) ──
                var c = Card(0, 14, 1080, 380, $"Pharmacists  ({PharmacyStaff.pharmacists.Count})");
                var g = Grid(14, 34, 1052, 330);
                g.Columns.Add("id", "ID"); g.Columns.Add("name", "Name"); g.Columns.Add("role", "Role"); g.Columns.Add("sal", "Salary");
                void RefreshPharm()
                {
                    g.Rows.Clear();
                    foreach (var p in PharmacyStaff.pharmacists)
                        g.Rows.Add(p.PharmacyStaffId, p.Name, p.Role, p.Salary.ToString("C"));
                }
                RefreshPharm();
                c.Controls.Add(g); t.Controls.Add(c);

                // ── Hire Pharmacist form ──
                var hc = Card(0, 394, 1080, 170, "➕ Hire New Pharmacist");
                int lx = 14, ly = 36, gap = 38;

                hc.Controls.Add(Lbl("Name:", FBold, TextGrey, lx, ly));
                var tbPName = TB(lx + 60, ly, 200); hc.Controls.Add(tbPName);

                hc.Controls.Add(Lbl("Role:", FBold, TextGrey, lx + 280, ly));
                var cbPRole = CB(lx + 330, ly, 180, new[] { "Pharmacist", "Senior Pharmacist", "Pharmacy Technician", "Intern" });
                hc.Controls.Add(cbPRole);

                ly += gap;
                hc.Controls.Add(Lbl("Salary:", FBold, TextGrey, lx, ly));
                var tbPSal = TB(lx + 60, ly, 90, "9000"); hc.Controls.Add(tbPSal);

                hc.Controls.Add(Lbl("Exp (yrs):", FBold, TextGrey, lx + 168, ly));
                var tbPExp = TB(lx + 255, ly, 60, "1"); hc.Controls.Add(tbPExp);

                ly += gap + 4;
                var hirePhBtn = Btn("✅ Hire Pharmacist", AccentGrn, lx, ly, 160, 34);
                hirePhBtn.Click += (s2, e2) =>
                {
                    try
                    {
                        string nm = tbPName.Text.Trim();
                        if (string.IsNullOrWhiteSpace(nm)) { MessageBox.Show("Enter a name."); return; }

                        decimal sal = decimal.TryParse(tbPSal.Text, out decimal sv) ? sv : 9000;
                        double exp = double.TryParse(tbPExp.Text, out double ev) ? ev : 1;
                        string nid = $"PH-{DateTime.Now.Ticks % 99999}";

                        var newPharm = new PharmacyStaff(
                            nm, 28, GenderType.Male, nid,
                            "00000000000", "pharm@neurai.com", "Unknown",
                            sal, exp,
                            cbPRole.SelectedItem?.ToString() ?? "Pharmacist"
                        );

                        // PharmacyStaff constructor does NOT auto-save — call AddPharmacists explicitly
                        HospitalData.AddPharmacists(newPharm);

                        RefreshPharm();
                        MessageBox.Show($"Pharmacist '{nm}' hired!", "Success ✅");
                        tbPName.Clear();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                };
                hc.Controls.Add(hirePhBtn);
                t.Controls.Add(hc);

                return t;
            }

            void ShowTab(Panel tab)
            {
                if (activeTab != null) s.Controls.Remove(activeTab);
                tab.Location = new Point(0, 50); s.Controls.Add(tab); activeTab = tab;
            }

            string[] tabs = { "Employees", "Nurses", "Pharmacists" };
            int tx = 0;
            foreach (var tab in tabs)
            {
                string t2 = tab;
                var tb = Btn(t2, AccentBlue, tx, 4, 140, 36);
                tb.Click += (s2, e2) => ShowTab(t2 == "Employees" ? EmpTab() : t2 == "Nurses" ? NurseTab() : PharmTab());
                tabBar.Controls.Add(tb); tx += 148;
            }
            s.Controls.Add(tabBar);
            ShowTab(EmpTab());

            // Check-in/out
            var ccard = Card(0, 460, 560, 180, "👤 Employee Check-In / Check-Out");
            ccard.Controls.Add(Lbl("Employee ID:", FBold, TextGrey, 14, 42));
            var tbEid = TB(110, 38, 100); ccard.Controls.Add(tbEid);
            var ciBtn = Btn("Check IN", AccentGrn, 14, 84, 120, 36);
            var coBtn = Btn("Check OUT", AccentRed, 142, 84, 120, 36);
            var ciLbl = Lbl("", FNorm, TextGrey, 14, 132, 520); ccard.Controls.Add(ciLbl);
            ciBtn.Click += (s2, e2) =>
            {
                if (!int.TryParse(tbEid.Text, out int eid)) return;
                var emp = Employee.employees.FirstOrDefault(e => e.EmployeeId == eid);
                if (emp == null) { ciLbl.Text = "Employee not found."; ciLbl.ForeColor = AccentRed; return; }
                emp.CheckIn(); ciLbl.Text = $"✅ {emp.Name} checked in at {emp.CheckInTime:HH:mm:ss}"; ciLbl.ForeColor = AccentGrn;
            };
            coBtn.Click += (s2, e2) =>
            {
                if (!int.TryParse(tbEid.Text, out int eid)) return;
                var emp = Employee.employees.FirstOrDefault(e => e.EmployeeId == eid);
                if (emp == null) { ciLbl.Text = "Employee not found."; ciLbl.ForeColor = AccentRed; return; }
                emp.CheckOut(); ciLbl.Text = $"🚪 {emp.Name} checked out."; ciLbl.ForeColor = TextGrey;
            };
            ccard.Controls.Add(ciBtn); ccard.Controls.Add(coBtn);
            s.Controls.Add(ccard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: BLOOD BANK  (BloodBank + JSON persistence)
        // ═════════════════════════════════════════════════════
        Panel PageBloodBank()
        {
            var page = ScrollPage("Blood Bank", "Donors Management ");
            var s = GetScroll(page);

            // ── Inventory ──
            var invCard = Card(0, 14, 500, 320, "🩸 Blood Inventory");
            var invGrid = Grid(14, 34, 472, 274);
            invGrid.Columns.Add("type", "Blood Type");
            invGrid.Columns.Add("qty", "Bags");
            invGrid.Columns.Add("stat", "Status");
            void RefreshInventory()
            {
                invGrid.Rows.Clear();
                foreach (var kv in HospitalEngine.myBank.GetStocks())
                {
                    string st = kv.Value < 5 ? "⚠️ CRITICAL" : "✅ STABLE";
                    int row = invGrid.Rows.Add(kv.Key, kv.Value, st);
                    if (kv.Value < 5) invGrid.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                }
            }
            RefreshInventory();
            invCard.Controls.Add(invGrid);
            s.Controls.Add(invCard);



            // ── Donors ──
            var donorCard = Card(0, 334, 1080, 320, "👤 Donors");
            var donorGrid = Grid(14, 34, 700, 274);
            donorGrid.Columns.Add("id", "ID");
            donorGrid.Columns.Add("name", "Name");
            donorGrid.Columns.Add("age", "Age");
            donorGrid.Columns.Add("blood", "Blood Type");
            donorGrid.Columns.Add("phone", "Phone");
            donorGrid.Columns.Add("last", "Last Donation");
            donorGrid.Columns.Add("count", "Total Donations");
            void RefreshDonors()
            {
                donorGrid.Rows.Clear();
                foreach (var d in HospitalEngine.myBank.GetDonors())
                    donorGrid.Rows.Add(d.DonorId, d.Name, d.Age, d.BloodType, d.PhoneNumber,
                        d.LastDonationDate.ToShortDateString(), d.DonationHistory.Count);
            }
            RefreshDonors();
            donorCard.Controls.Add(donorGrid);

            // Register Donor (right side of donor card)
            int dx = 730, dy = 34;
            donorCard.Controls.Add(Lbl("Register New Donor", FBold, TextDark, dx, dy)); dy += 28;
            donorCard.Controls.Add(Lbl("Name:", FBold, TextGrey, dx, dy));
            var tbDName = TB(dx + 55, dy, 190); donorCard.Controls.Add(tbDName); dy += 36;
            donorCard.Controls.Add(Lbl("Age:", FBold, TextGrey, dx, dy));
            var tbDAge = TB(dx + 40, dy, 60); donorCard.Controls.Add(tbDAge);
            donorCard.Controls.Add(Lbl("Phone:", FBold, TextGrey, dx + 118, dy));
            var tbDPhone = TB(dx + 168, dy, 120); donorCard.Controls.Add(tbDPhone); dy += 36;
            donorCard.Controls.Add(Lbl("Blood:", FBold, TextGrey, dx, dy));
            var cbDBld = CB(dx + 50, dy, 90, new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" }); donorCard.Controls.Add(cbDBld); dy += 42;
            var regDonorBtn = Btn("Register Donor", AccentGrn, dx, dy, 150, 34);
            regDonorBtn.Click += (s2, e2) =>
            {
                string nm = tbDName.Text.Trim();
                if (string.IsNullOrWhiteSpace(nm)) { MessageBox.Show("Enter donor name."); return; }
                HospitalEngine.myBank.RegisterDonorUI(nm,
                    int.TryParse(tbDAge.Text, out int a) ? a : 0,
                    tbDPhone.Text.Trim(),
                    cbDBld.SelectedItem?.ToString() ?? "O+");
                RefreshDonors();
                MessageBox.Show($"Donor '{nm}' registered!", "Success ✅");
                tbDName.Clear(); tbDAge.Clear(); tbDPhone.Clear();
            };
            donorCard.Controls.Add(regDonorBtn);
            s.Controls.Add(donorCard);

            // ── Withdraw / Donate ──
            var wdCard = Card(0, 668, 540, 160, "💉 Withdraw / Donate Blood");
            int wx = 14, wy = 36;
            wdCard.Controls.Add(Lbl("Blood Type:", FBold, TextGrey, wx, wy));
            var cbWBld = CB(wx + 90, wy, 90, new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" }); wdCard.Controls.Add(cbWBld);
            wdCard.Controls.Add(Lbl("Bags:", FBold, TextGrey, wx + 200, wy));
            var tbWBags = TB(wx + 245, wy, 60, "1"); wdCard.Controls.Add(tbWBags); wy += 40;
            var donateBtn = Btn("➕ Donate", AccentGrn, wx, wy, 120, 34);
            var withdrawBtn = Btn("➖ Withdraw", AccentRed, wx + 130, wy, 120, 34);
            var wResult = Lbl("", FBold, TextGrey, wx, wy + 44, 510); wdCard.Controls.Add(wResult);
            donateBtn.Click += (s2, e2) =>
            {
                int bags = int.TryParse(tbWBags.Text, out int b) ? b : 0;
                if (bags <= 0) { MessageBox.Show("Enter valid bag count."); return; }
                string type = cbWBld.SelectedItem?.ToString() ?? "O+";
                HospitalEngine.myBank.DonateBlood(type, bags);
                RefreshInventory();
                wResult.Text = $"✅ Added {bags} bags of {type}."; wResult.ForeColor = AccentGrn;
            };
            withdrawBtn.Click += (s2, e2) =>
            {
                int bags = int.TryParse(tbWBags.Text, out int b) ? b : 0;
                if (bags <= 0) { MessageBox.Show("Enter valid bag count."); return; }
                string type = cbWBld.SelectedItem?.ToString() ?? "O+";
                bool ok = HospitalEngine.myBank.WithdrawBlood(type, bags);
                RefreshInventory();
                wResult.Text = ok ? $"✅ Issued {bags} bags of {type}." : $"❌ Insufficient {type} stock.";
                wResult.ForeColor = ok ? AccentGrn : AccentRed;
            };
            wdCard.Controls.Add(donateBtn); wdCard.Controls.Add(withdrawBtn);
            s.Controls.Add(wdCard);

            // ── Transfers ──
            var trCard = Card(560, 668, 520, 500, "🔄 Transfer Records");
            var trGrid = Grid(14, 34, 492, 274);
            trGrid.Columns.Add("id", "ID");
            trGrid.Columns.Add("donor", "Donor");
            trGrid.Columns.Add("pt", "Patient");
            trGrid.Columns.Add("blood", "Blood");
            trGrid.Columns.Add("bags", "Bags");
            trGrid.Columns.Add("date", "Date");
            trGrid.Columns.Add("stat", "Status");
            void RefreshTransfers()
            {
                trGrid.Rows.Clear();
                foreach (var t in HospitalEngine.myBank.GetTransfers())
                    trGrid.Rows.Add(t.TransferId, t.DonorName, t.PatientName,
                        t.BloodType, t.Bags, t.Date.ToShortDateString(), t.Status);
            }
            RefreshTransfers();
            trCard.Controls.Add(trGrid);

            // ── New Transfer Form ──
            int tx = 14, ty = 318;
            trCard.Controls.Add(Lbl("── New Transfer ──", FBold, TextDark, tx, ty)); ty += 26;
            trCard.Controls.Add(Lbl("Donor ID:", FBold, TextGrey, tx, ty));
            var tbTrDonorId = TB(tx + 75, ty, 80, "ID"); trCard.Controls.Add(tbTrDonorId);
            trCard.Controls.Add(Lbl("Bags:", FBold, TextGrey, tx + 180, ty));
            var tbTrBags = TB(tx + 225, ty, 60, "1"); trCard.Controls.Add(tbTrBags); ty += 36;
            trCard.Controls.Add(Lbl("Patient:", FBold, TextGrey, tx, ty));
            var tbTrPatient = TB(tx + 65, ty, 200, "Patient name"); trCard.Controls.Add(tbTrPatient); ty += 36;
            var trResult = Lbl("", FBold, TextGrey, tx, ty + 40, 490); trCard.Controls.Add(trResult);
            var btnDoTransfer = Btn("🔄 Process Transfer", AccentBlue, tx, ty, 180, 34);
            trCard.Controls.Add(btnDoTransfer);

            btnDoTransfer.Click += (s2, e2) =>
            {
                if (!int.TryParse(tbTrDonorId.Text, out int donorId))
                { MessageBox.Show("Enter valid Donor ID."); return; }
                string patient = tbTrPatient.Text.Trim();
                if (string.IsNullOrWhiteSpace(patient) || patient == "Patient name")
                { MessageBox.Show("Enter patient name."); return; }
                if (!int.TryParse(tbTrBags.Text, out int bags) || bags <= 0)
                { MessageBox.Show("Enter valid bag count."); return; }

                var donors = HospitalEngine.myBank.GetDonors();
                var donor = donors.Find(d => d.DonorId == donorId);
                if (donor == null) { MessageBox.Show("Donor not found!"); return; }

                var stocks = HospitalEngine.myBank.GetStocks();
                if (stocks[donor.BloodType] < bags)
                {
                    trResult.Text = $"❌ Only {stocks[donor.BloodType]} bags of {donor.BloodType} available.";
                    trResult.ForeColor = AccentRed; return;
                }

                HospitalEngine.myBank.WithdrawBlood(donor.BloodType, bags);

                var newTransfer = new BloodTransfer
                {
                    DonorName = donor.Name,
                    PatientName = patient,
                    BloodType = donor.BloodType,
                    Bags = bags,
                    Date = DateTime.Now,
                    Status = "Completed"
                };
                HospitalEngine.myBank.AddTransfer(newTransfer);

                RefreshTransfers(); RefreshInventory();
                trResult.Text = $"✅ Transfer done: {bags} bags of {donor.BloodType} → {patient}.";
                trResult.ForeColor = AccentGrn;
                tbTrDonorId.Clear();
                tbTrPatient.Text = "Patient name"; tbTrPatient.ForeColor = TextGrey;
                tbTrBags.Text = "1"; tbTrBags.ForeColor = TextGrey;
            };

            s.Controls.Add(trCard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: OPERATIONS  (HospitalEngine + JSON persistence)
        // ═════════════════════════════════════════════════════
        Panel PageOperations()
        {
            var page = ScrollPage("Operating Rooms", "Schedule operations and manage room availability");
            var s = GetScroll(page);

            // ── Room status ──
            var roomCard = Card(0, 14, 1080, 68, "🏥 Operating Rooms");
            for (int i = 0; i < 9; i++)
            {
                int idx = i;
                bool occ = HospitalEngine.roomsStatus[idx];
                var lbl = new Label
                {
                    Text = $"Room {idx + 1}  {(occ ? "🔴" : "🟢")}",
                    Location = new Point(14 + idx * 118, 18),
                    Size = new Size(112, 36),
                    Font = FBold,
                    ForeColor = occ ? AccentRed : AccentGrn,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle
                };
                roomCard.Controls.Add(lbl);
            }
            s.Controls.Add(roomCard);

            // ── Schedule grid ──
            var gcard = Card(0, 82, 1080, 280, "📋 Surgery Schedule");
            var dg = Grid(14, 34, 1052, 234);
            dg.Columns.Add("pt", "Patient");
            dg.Columns.Add("doc", "Surgeon");
            dg.Columns.Add("type", "Surgery Type");
            dg.Columns.Add("date", "Date");
            dg.Columns.Add("time", "Time");
            dg.Columns.Add("room", "Room");
            dg.Columns.Add("emr", "Emergency");
            void RefreshOps()
            {
                dg.Rows.Clear();
                foreach (var op in HospitalEngine.operationsList)
                {
                    int row = dg.Rows.Add(op.PatientName, op.DoctorName, op.OperationType,
                        op.SurgeryDate.ToShortDateString(), op.Time.ToShortTimeString(),
                        op.RoomNumber, op.IsEmergency ? "🚨 YES" : "No");
                    if (op.IsEmergency) dg.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                }
            }
            RefreshOps();
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            // ── Schedule normal op ──
            var scard = Card(0, 376, 700, 260, "📅 Schedule New Operation");
            int lx = 14, ly = 34, gap = 38;
            scard.Controls.Add(Lbl("Patient Name:", FBold, TextGrey, lx, ly));
            var tbPat = TB(lx + 110, ly, 200, "Name"); scard.Controls.Add(tbPat);
            scard.Controls.Add(Lbl("Surgeon:", FBold, TextGrey, lx + 330, ly));
            var tbDoc = TB(lx + 400, ly, 180, "Dr. name"); scard.Controls.Add(tbDoc);
            ly += gap;
            scard.Controls.Add(Lbl("Surgery Type:", FBold, TextGrey, lx, ly));
            var tbSType = TB(lx + 100, ly, 220); scard.Controls.Add(tbSType);
            ly += gap;
            scard.Controls.Add(Lbl("Date (yyyy-MM-dd):", FBold, TextGrey, lx, ly));
            var tbDate = TB(lx + 150, ly, 130, DateTime.Now.ToString("yyyy-MM-dd")); scard.Controls.Add(tbDate);
            scard.Controls.Add(Lbl("Time (HH:mm):", FBold, TextGrey, lx + 300, ly));
            var tbTime = TB(lx + 400, ly, 80, "08:00"); scard.Controls.Add(tbTime);
            ly += gap;
            scard.Controls.Add(Lbl("Room (1-9):", FBold, TextGrey, lx, ly));
            var cbRoom = CB(lx + 82, ly, 70, new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" }); scard.Controls.Add(cbRoom);
            ly += gap + 4;
            var schedBtn = Btn("📅 Schedule", AccentBlue, lx, ly, 140, 34);
            schedBtn.Click += (s2, e2) =>
            {
                string pat = tbPat.Text.Trim();
                if (string.IsNullOrWhiteSpace(pat)) { MessageBox.Show("Enter patient name."); return; }
                if (!DateTime.TryParse(tbDate.Text, out DateTime dt)) { MessageBox.Show("Invalid date."); return; }
                if (!DateTime.TryParse(tbTime.Text, out DateTime tm)) { MessageBox.Show("Invalid time."); return; }
                int rn = int.Parse(cbRoom.SelectedItem?.ToString() ?? "1");
                HospitalEngine.roomsStatus[rn - 1] = true;
                HospitalEngine.operationsList.Add(new Operation
                {
                    PatientName = pat,
                    DoctorName = tbDoc.Text.Trim(),
                    OperationType = tbSType.Text.Trim(),
                    SurgeryDate = dt,
                    Time = tm,
                    RoomNumber = rn.ToString(),
                    IsEmergency = false
                });
                _engine.SaveData();
                RefreshOps();
                MessageBox.Show($"Operation scheduled for '{pat}' — saved!", "Scheduled ✅");
                NavigateTo("Operations");
            };
            scard.Controls.Add(schedBtn);
            s.Controls.Add(scard);

            // ── Emergency ──
            var ecard = Card(720, 376, 360, 200, "🚨 Emergency Operation");
            ecard.BackColor = Color.FromArgb(255, 245, 245);
            ecard.Controls.Add(Lbl("Patient:", FBold, AccentRed, 14, 40));
            var tbEmPat = TB(80, 36, 250, "Unknown"); ecard.Controls.Add(tbEmPat);
            var emergBtn = new Button
            {
                Text = "🚨 EMERGENCY NOW",
                Size = new Size(330, 44),
                Location = new Point(14, 84),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            emergBtn.FlatAppearance.BorderSize = 0;
            var emergResult = Lbl("", FBold, AccentRed, 14, 138, 330); ecard.Controls.Add(emergResult);
            emergBtn.Click += (s2, e2) =>
            {
                string pat = tbEmPat.Text.Trim();
                if (string.IsNullOrWhiteSpace(pat)) pat = "Unknown";
                int freeRoom = Array.IndexOf(HospitalEngine.roomsStatus, false);
                string roomStr = freeRoom != -1 ? (HospitalEngine.roomsStatus[freeRoom] = true, (freeRoom + 1).ToString()).Item2 : "WAITING";
                HospitalEngine.operationsList.Add(new Operation
                {
                    PatientName = pat,
                    DoctorName = "On-Call Surgeon",
                    OperationType = "Urgent Surgery",
                    SurgeryDate = DateTime.Now,
                    Time = DateTime.Now,
                    RoomNumber = roomStr,
                    IsEmergency = true
                });
                _engine.SaveData();
                RefreshOps();
                emergResult.Text = roomStr == "WAITING" ? "⚠️ All rooms full — added to queue!" : $"✅ Room {roomStr} reserved!";
                NavigateTo("Operations");
            };
            ecard.Controls.Add(emergBtn);
            s.Controls.Add(ecard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  SAVE & EXIT
        // ═════════════════════════════════════════════════════
        void SaveAndExit()
        {
            HospitalData.SaveEmployees();
            HospitalData.SaveDoctors();
            HospitalData.SaveNurses();
            HospitalData.SavePharmacists();
            HospitalData.SaveSecurity();
            _engine.SaveData();
            try
            {
                hospitalService.SaveAll();
                receptionService.SaveAll();
                RoomCleaningTracker.SaveData();
            }
            catch { }
            MessageBox.Show("All data saved successfully!", "Saved ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Staff / personnel
            HospitalData.SaveEmployees();
            HospitalData.SaveDoctors();
            HospitalData.SaveNurses();
            HospitalData.SavePharmacists();
            HospitalData.SaveSecurity();

            // Patients, operations, rooms
            _engine.SaveData();

            // Online patients & bookings  (also auto-saved on every mutation)
            hospitalService.SaveAll();

            // Reception patients & bookings (also auto-saved on every mutation)
            receptionService.SaveAll();

            // Room cleaning history & statuses (also auto-saved on every mutation)
            RoomCleaningTracker.SaveData();

            base.OnFormClosed(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hospital Management System";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        // ═══════════════════════════════════════════════════════
        //  PAGE: ROOM CLEANING
        // ═══════════════════════════════════════════════════════
        Panel PageRoomCleaning()
        {
            var page = ScrollPage("🧹  Room Cleaning Tracker", "Track and update room cleaning status");
            var scroll = GetScroll(page);

            // Status overview grid
            var statusCard = Card(0, 14, 860, 280, "Current Room Status");
            var statusGrid = Grid(14, 42, 828, 224);
            statusGrid.Columns.Add("Room", "Room #");
            statusGrid.Columns.Add("Status", "Status");
            statusGrid.Columns.Add("Last", "Last Cleaned By");
            statusGrid.Columns.Add("When", "Last Cleaned At");

            void RefreshStatusGrid()
            {
                statusGrid.Rows.Clear();
                var statuses = RoomCleaningTracker.GetAllStatuses();
                if (statuses.Count == 0)
                {
                    statusGrid.Rows.Add("—", "No rooms tracked yet", "—", "—");
                    return;
                }
                foreach (var kv in statuses)
                {
                    var last = RoomCleaningTracker.GetLastCleaning(kv.Key);
                    string icon;
                    Color rowColor;
                    switch (kv.Value)
                    {
                        case CleaningStatus.Clean: icon = "✅  Clean"; rowColor = Color.FromArgb(232, 250, 238); break;
                        case CleaningStatus.NeedsCleaning: icon = "⚠️  Needs Cleaning"; rowColor = Color.FromArgb(255, 248, 220); break;
                        case CleaningStatus.InProgress: icon = "🔄  In Progress"; rowColor = Color.FromArgb(232, 244, 255); break;
                        case CleaningStatus.Sanitized: icon = "🛡️  Sanitized"; rowColor = Color.FromArgb(240, 232, 255); break;
                        default: icon = "—"; rowColor = CardBg; break;
                    }
                    string lastBy = last != null ? last.CleanedBy : "—";
                    string lastWhen = last != null ? last.CleanedAt.ToString("dd/MM/yyyy HH:mm") : "—";
                    int rowIdx = statusGrid.Rows.Add(kv.Key, icon, lastBy, lastWhen);
                    statusGrid.Rows[rowIdx].DefaultCellStyle.BackColor = rowColor;
                }
            }
            RefreshStatusGrid();
            statusCard.Controls.Add(statusGrid);
            scroll.Controls.Add(statusCard);

            // Update status card
            var updateCard = Card(0, 294, 580, 240, "Update Room Status");
            int lx = 16, ly = 44;

            updateCard.Controls.Add(Lbl("Room Number", FBold, TextDark, lx, ly));
            var tbRoom = TB(lx, ly + 20, 100); updateCard.Controls.Add(tbRoom);

            updateCard.Controls.Add(Lbl("Staff Name", FBold, TextDark, lx + 120, ly));
            var tbStaff = TB(lx + 120, ly + 20, 180); updateCard.Controls.Add(tbStaff);
            ly += 68;

            updateCard.Controls.Add(Lbl("New Status", FBold, TextDark, lx, ly));
            var cbStatus = CB(lx, ly + 20, 200, new[] { "Clean", "Needs Cleaning", "In Progress", "Sanitized" });
            updateCard.Controls.Add(cbStatus);
            ly += 68;

            updateCard.Controls.Add(Lbl("Notes (optional)", FBold, TextDark, lx, ly));
            var tbNotes = TB(lx, ly + 20, 360); updateCard.Controls.Add(tbNotes);
            ly += 68;

            var updMsg = new Label { Text = "", Font = FBold, AutoSize = true, Location = new Point(lx, ly) };
            updateCard.Controls.Add(updMsg);

            var btnUpd = Btn("Update Status", AccentBlue, lx + 380, ly - 36, 160, 36);
            updateCard.Controls.Add(btnUpd);

            btnUpd.Click += (s, e) =>
            {
                int roomNum;
                if (!int.TryParse(tbRoom.Text.Trim(), out roomNum) || roomNum < 1)
                { updMsg.Text = "Enter a valid room number."; updMsg.ForeColor = AccentRed; return; }
                if (string.IsNullOrWhiteSpace(tbStaff.Text))
                { updMsg.Text = "Enter staff name."; updMsg.ForeColor = AccentRed; return; }

                string staff = tbStaff.Text.Trim();
                string notes = tbNotes.Text.Trim();

                switch (cbStatus.SelectedIndex)
                {
                    case 0: RoomCleaningTracker.MarkCleaned(roomNum, staff, notes); break;
                    case 1: RoomCleaningTracker.MarkNeedsCleaning(roomNum, notes); break;
                    case 2: RoomCleaningTracker.MarkInProgress(roomNum, staff); break;
                    case 3: RoomCleaningTracker.MarkSanitized(roomNum, staff, notes); break;
                }

                updMsg.Text = "✅  Room " + roomNum + " updated to: " + cbStatus.SelectedItem;
                updMsg.ForeColor = AccentGrn;
                tbRoom.Clear(); tbStaff.Clear(); tbNotes.Clear();
                RefreshStatusGrid();
            };

            updateCard.Height = ly + 60;
            scroll.Controls.Add(updateCard);

            // Alert card
            var alertCard = Card(596, 294, 264, 180, "⚠️  Alert: Needs Cleaning");
            var alertList = new ListBox
            {
                Location = new Point(12, 42),
                Size = new Size(238, 124),
                Font = FNorm,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(255, 248, 220)
            };
            var dirty = RoomCleaningTracker.GetRoomsNeedingCleaning();
            if (dirty.Count == 0)
                alertList.Items.Add("All rooms are clean ✅");
            else
                foreach (var r in dirty)
                    alertList.Items.Add("Room " + r);
            alertCard.Controls.Add(alertList);
            scroll.Controls.Add(alertCard);

            return page;
        }

        // ═══════════════════════════════════════════════════════
        //  PAGE: ONLINE BOOKING
        // ═══════════════════════════════════════════════════════
        Panel PageOnlineBooking()
        {

            var page = ScrollPage("🗓️  Online Booking", "Register, login, and book clinic appointments");

            // ── tab strip ──
            Panel tabStrip = new Panel
            {
                Location = new Point(0, 70),
                Size = new Size(900, 38),
                BackColor = Color.FromArgb(220, 228, 240)
            };

            string[] tabNames = { "Register", "Login / Book", "My Bookings", "Search Booking" };
            Panel[] tabPages = new Panel[tabNames.Length];
            Button[] tabBtns = new Button[tabNames.Length];

            // build blank tab pages
            for (int i = 0; i < tabPages.Length; i++)
            {
                tabPages[i] = new Panel
                {
                    Location = new Point(0, 108),
                    Size = new Size(900, 600),
                    BackColor = CardBg,
                    Visible = false
                };
            }

            void ShowTab(int idx)
            {
                for (int k = 0; k < tabBtns.Length; k++)
                {
                    tabPages[k].Visible = (k == idx);
                    tabBtns[k].BackColor = (k == idx) ? AccentBlue : Color.FromArgb(200, 210, 230);
                    tabBtns[k].ForeColor = (k == idx) ? Color.White : TextDark;
                }
            }

            for (int i = 0; i < tabNames.Length; i++)
            {
                int idx = i;
                tabBtns[i] = new Button
                {
                    Text = tabNames[i],
                    Font = FBold,
                    Size = new Size(160, 36),
                    Location = new Point(4 + i * 164, 1),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(200, 210, 230),
                    ForeColor = TextDark
                };
                tabBtns[i].FlatAppearance.BorderSize = 0;
                tabBtns[i].Click += (s, e) => ShowTab(idx);
                tabStrip.Controls.Add(tabBtns[i]);
            }

            // ─────────────────────────────────────────────
            //  TAB 0 – Register
            // ─────────────────────────────────────────────
            Panel regTab = tabPages[0];

            Label regTitle = new Label { Text = "New Patient Registration", Font = FTitle, ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true };
            regTab.Controls.Add(regTitle);

            string[] regFields = { "Full Name", "National ID (14 digits)", "Phone (11 digits)", "Address" };
            TextBox[] regBoxes = new TextBox[regFields.Length];

            for (int i = 0; i < regFields.Length; i++)
            {
                regTab.Controls.Add(new Label { Text = regFields[i], Font = FBold, ForeColor = TextGrey, Location = new Point(20, 70 + i * 56), AutoSize = true });
                regBoxes[i] = new TextBox { Font = FNorm, Location = new Point(20, 90 + i * 56), Size = new Size(340, 26), BorderStyle = BorderStyle.FixedSingle };
                regTab.Controls.Add(regBoxes[i]);
            }

            regTab.Controls.Add(new Label { Text = "Password", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 70 + regFields.Length * 56), AutoSize = true });
            TextBox tbRegPass = new TextBox { Font = FNorm, UseSystemPasswordChar = true, Location = new Point(20, 90 + regFields.Length * 56), Size = new Size(340, 26), BorderStyle = BorderStyle.FixedSingle };
            regTab.Controls.Add(tbRegPass);

            regTab.Controls.Add(new Label { Text = "Confirm Password", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 146 + regFields.Length * 56), AutoSize = true });
            TextBox tbRegConf = new TextBox { Font = FNorm, UseSystemPasswordChar = true, Location = new Point(20, 166 + regFields.Length * 56), Size = new Size(340, 26), BorderStyle = BorderStyle.FixedSingle };
            regTab.Controls.Add(tbRegConf);

            Label regMsg = new Label { Text = "", Font = FBold, AutoSize = true, Location = new Point(20, 222 + regFields.Length * 56) };
            regTab.Controls.Add(regMsg);

            Button btnReg = new Button { Text = "Create Account", Font = FBold, BackColor = AccentBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 248 + regFields.Length * 56), Size = new Size(160, 34) };
            btnReg.FlatAppearance.BorderSize = 0;
            regTab.Controls.Add(btnReg);

            btnReg.Click += (s, e) =>
            {
                string err = hospitalService.CheckSignUp(regBoxes[0].Text, regBoxes[1].Text, regBoxes[2].Text, regBoxes[3].Text);
                if (err != "")
                {
                    regMsg.Text = "❌ " + err;
                    regMsg.ForeColor = AccentRed;
                    return;
                }
                OnlineRegistration newP = new OnlineRegistration
                {
                    NameEnglish = regBoxes[0].Text.Trim(),
                    NationalID = regBoxes[1].Text.Trim(),
                    Phone = regBoxes[2].Text.Trim(),
                    Address = regBoxes[3].Text.Trim()
                };
                string perr = hospitalService.SetPassword(newP, tbRegPass.Text, tbRegConf.Text);
                if (perr != "")
                {
                    regMsg.Text = "❌ " + perr;
                    regMsg.ForeColor = AccentRed;
                    return;
                }
                regMsg.Text = "✅ Account created! You can now login.";
                regMsg.ForeColor = AccentGrn;
                foreach (var rb in regBoxes) rb.Clear();
                tbRegPass.Clear(); tbRegConf.Clear();
            };

            // ─────────────────────────────────────────────
            //  TAB 1 – Login / Book
            // ─────────────────────────────────────────────
            Panel loginTab = tabPages[1];

            // login section
            Label loginTitle = new Label { Text = "Login", Font = FTitle, ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true };
            loginTab.Controls.Add(loginTitle);

            loginTab.Controls.Add(new Label { Text = "National ID", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 70), AutoSize = true });
            TextBox tbLId = new TextBox { Font = FNorm, Location = new Point(20, 92), Size = new Size(280, 26), BorderStyle = BorderStyle.FixedSingle };
            loginTab.Controls.Add(tbLId);

            loginTab.Controls.Add(new Label { Text = "Password", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 128), AutoSize = true });
            TextBox tbLPass = new TextBox { Font = FNorm, UseSystemPasswordChar = true, Location = new Point(20, 150), Size = new Size(280, 26), BorderStyle = BorderStyle.FixedSingle };
            loginTab.Controls.Add(tbLPass);

            Label loginMsg = new Label { Text = "", Font = FBold, AutoSize = true, Location = new Point(20, 186) };
            loginTab.Controls.Add(loginMsg);

            Button btnLogin = new Button { Text = "Login", Font = FBold, BackColor = AccentBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 210), Size = new Size(120, 34) };
            btnLogin.FlatAppearance.BorderSize = 0;
            loginTab.Controls.Add(btnLogin);

            // booking section (hidden until login)
            Panel bookSection = new Panel { Location = new Point(0, 260), Size = new Size(860, 330), BackColor = CardBg, Visible = false };
            loginTab.Controls.Add(bookSection);

            Label bookTitle = new Label { Text = "Book an Appointment", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 10), AutoSize = true };
            bookSection.Controls.Add(bookTitle);

            Label lblWelcome = new Label { Text = "", Font = FBold, ForeColor = AccentGrn, Location = new Point(20, 40), AutoSize = true };
            bookSection.Controls.Add(lblWelcome);

            // clinic picker
            bookSection.Controls.Add(new Label { Text = "Select Clinic", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 72), AutoSize = true });
            ComboBox cbClinic = new ComboBox { Font = FNorm, Location = new Point(20, 92), Size = new Size(280, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in clinicService.Clinics.Values)
                cbClinic.Items.Add(c.NameEnglish);
            bookSection.Controls.Add(cbClinic);

            // day picker
            bookSection.Controls.Add(new Label { Text = "Available Day", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 128), AutoSize = true });
            ComboBox cbDay = new ComboBox { Font = FNorm, Location = new Point(20, 148), Size = new Size(280, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            bookSection.Controls.Add(cbDay);

            cbClinic.SelectedIndexChanged += (s, e) =>
            {
                cbDay.Items.Clear();
                int selIdx = cbClinic.SelectedIndex + 1;
                if (clinicService.Clinics.ContainsKey(selIdx))
                    foreach (var d in clinicService.Clinics[selIdx].AvailableDays)
                        cbDay.Items.Add(d);
            };

            Label bookMsg = new Label { Text = "", Font = FBold, AutoSize = true, Location = new Point(20, 190) };
            bookSection.Controls.Add(bookMsg);

            Button btnBook = new Button { Text = "Confirm Booking", Font = FBold, BackColor = AccentGrn, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 215), Size = new Size(170, 34) };
            btnBook.FlatAppearance.BorderSize = 0;
            bookSection.Controls.Add(btnBook);

            btnLogin.Click += (s, e) =>
            {
                var p = hospitalService.FindPatient(tbLId.Text.Trim(), tbLPass.Text);
                if (p == null)
                {
                    loginMsg.Text = "❌ Invalid ID or password.";
                    loginMsg.ForeColor = AccentRed;
                    return;
                }
                loggedInPatient = p;
                loginMsg.Text = "✅ Login successful!";
                loginMsg.ForeColor = AccentGrn;
                lblWelcome.Text = "Welcome, " + p.NameEnglish + "  (ID: " + p.NationalID + ")";
                bookSection.Visible = true;
            };

            btnBook.Click += (s, e) =>
            {
                if (loggedInPatient == null) { bookMsg.Text = "Please login first."; bookMsg.ForeColor = AccentRed; return; }
                if (cbClinic.SelectedIndex < 0) { bookMsg.Text = "Select a clinic."; bookMsg.ForeColor = AccentRed; return; }
                if (cbDay.SelectedIndex < 0) { bookMsg.Text = "Select a day."; bookMsg.ForeColor = AccentRed; return; }

                int cIdx = cbClinic.SelectedIndex + 1;
                Clinic clinic = clinicService.Clinics[cIdx];
                string day = cbDay.SelectedItem.ToString();

                Booking b = hospitalService.CreateBooking(loggedInPatient.NationalID, clinic, day);
                bookMsg.Text = "✅ Booked! Code: " + b.Code + " | " + clinic.NameEnglish + " | " + day;
                bookMsg.ForeColor = AccentGrn;
            };

            // ─────────────────────────────────────────────
            //  TAB 2 – My Bookings
            // ─────────────────────────────────────────────
            Panel myBookTab = tabPages[2];

            Label myTitle = new Label { Text = "My Bookings", Font = FTitle, ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true };
            myBookTab.Controls.Add(myTitle);

            myBookTab.Controls.Add(new Label { Text = "National ID", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 74), AutoSize = true });
            TextBox tbMyId = new TextBox { Font = FNorm, Location = new Point(20, 94), Size = new Size(240, 26), BorderStyle = BorderStyle.FixedSingle };
            myBookTab.Controls.Add(tbMyId);

            Button btnMyLoad = new Button { Text = "Load", Font = FBold, BackColor = AccentBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(272, 92), Size = new Size(90, 30) };
            btnMyLoad.FlatAppearance.BorderSize = 0;
            myBookTab.Controls.Add(btnMyLoad);

            DataGridView myGrid = new DataGridView
            {
                Location = new Point(20, 140),
                Size = new Size(820, 380),
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = CardBg,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(220, 224, 230),
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = FNorm,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            myGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 25, 45);
            myGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            myGrid.ColumnHeadersDefaultCellStyle.Font = FBold;
            myGrid.EnableHeadersVisualStyles = false;
            myGrid.Columns.Add("Code", "Code");
            myGrid.Columns.Add("Clinic", "Clinic");
            myGrid.Columns.Add("Day", "Day");
            myGrid.Columns.Add("DateTime", "Booked At");
            myGrid.Columns.Add("Paid", "Paid");
            myBookTab.Controls.Add(myGrid);

            btnMyLoad.Click += (s, e) =>
            {
                myGrid.Rows.Clear();
                foreach (var bk in hospitalService.GetPatientBookings(tbMyId.Text.Trim()))
                    myGrid.Rows.Add(bk.Code, bk.ClinicName, bk.Day, bk.BookingDateTime, bk.IsPaid ? "Yes" : "No");
                if (myGrid.Rows.Count == 0)
                    MessageBox.Show("No bookings found for this ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // ─────────────────────────────────────────────
            //  TAB 3 – Search Booking
            // ─────────────────────────────────────────────
            Panel searchTab = tabPages[3];

            Label searchTitle = new Label { Text = "Search Booking", Font = FTitle, ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true };
            searchTab.Controls.Add(searchTitle);

            searchTab.Controls.Add(new Label { Text = "Enter Booking Code or Patient ID", Font = FBold, ForeColor = TextGrey, Location = new Point(20, 74), AutoSize = true });
            TextBox tbSearch = new TextBox { Font = FNorm, Location = new Point(20, 94), Size = new Size(300, 26), BorderStyle = BorderStyle.FixedSingle };
            searchTab.Controls.Add(tbSearch);

            Button btnSearch = new Button { Text = "Search", Font = FBold, BackColor = AccentBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(332, 92), Size = new Size(100, 30) };
            btnSearch.FlatAppearance.BorderSize = 0;
            searchTab.Controls.Add(btnSearch);

            DataGridView searchGrid = new DataGridView
            {
                Location = new Point(20, 140),
                Size = new Size(820, 360),
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = CardBg,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(220, 224, 230),
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = FNorm,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            searchGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 25, 45);
            searchGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            searchGrid.ColumnHeadersDefaultCellStyle.Font = FBold;
            searchGrid.EnableHeadersVisualStyles = false;
            searchGrid.Columns.Add("PatID", "Patient ID");
            searchGrid.Columns.Add("Code", "Code");
            searchGrid.Columns.Add("Clinic", "Clinic");
            searchGrid.Columns.Add("Day", "Day");
            searchGrid.Columns.Add("DateTime", "Booked At");
            searchTab.Controls.Add(searchGrid);

            btnSearch.Click += (s, e) =>
            {
                searchGrid.Rows.Clear();
                foreach (var bk in hospitalService.SearchBookings(tbSearch.Text.Trim()))
                    searchGrid.Rows.Add(bk.PatientID, bk.Code, bk.ClinicName, bk.Day, bk.BookingDateTime);
                if (searchGrid.Rows.Count == 0)
                    MessageBox.Show("No bookings matched your query.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // ── assemble into page ──
            var scrollInner = GetScroll(page);
            scrollInner.Controls.Add(tabStrip);
            foreach (var tp in tabPages)
                scrollInner.Controls.Add(tp);

            ShowTab(0);
            return page;
        }


        // ═══════════════════════════════════════════════════════════════════
        // INSTRUCTIONS: Paste this entire block inside your MainForm class,
        // replacing the existing PageLabAnalysis() method.
        // ═══════════════════════════════════════════════════════════════════

        Panel PageLabAnalysis()
        {
            var page = ScrollPage("🔬  Analysis Lab", "Enter test results and generate a colour-coded report");
            var scroll = GetScroll(page);

            // ── Tab strip: Enter Results | Search Patient | Today's Tests ──
            var tabStrip = new Panel { Location = new Point(0, 0), Size = new Size(900, 38), BackColor = Color.FromArgb(220, 228, 240) };
            string[] tabNames = { "Enter Results", "Search Patient", "Today's Tests" };
            Panel[] tabPages = new Panel[tabNames.Length];
            Button[] tabBtns = new Button[tabNames.Length];
            for (int ti = 0; ti < tabPages.Length; ti++)
                tabPages[ti] = new Panel { Location = new Point(0, 42), Size = new Size(1060, 1000), BackColor = Color.FromArgb(236, 240, 245), Visible = false };

            void ShowLabTab(int idx)
            {
                for (int k = 0; k < tabBtns.Length; k++)
                {
                    tabPages[k].Visible = (k == idx);
                    tabBtns[k].BackColor = (k == idx) ? AccentBlue : Color.FromArgb(200, 210, 230);
                    tabBtns[k].ForeColor = (k == idx) ? Color.White : TextDark;
                }
            }
            for (int ti = 0; ti < tabNames.Length; ti++)
            {
                int capturedIdx = ti;
                tabBtns[ti] = new Button
                {
                    Text = tabNames[ti],
                    Font = FBold,
                    Size = new Size(200, 36),
                    Location = new Point(4 + ti * 204, 1),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(200, 210, 230),
                    ForeColor = TextDark
                };
                tabBtns[ti].FlatAppearance.BorderSize = 0;
                tabBtns[ti].Click += (s, e) => ShowLabTab(capturedIdx);
                tabStrip.Controls.Add(tabBtns[ti]);
            }
            scroll.Controls.Add(tabStrip);
            foreach (var tp in tabPages) scroll.Controls.Add(tp);

            // ════════════════════════════════════════════════════
            //  TAB 0 – Enter Results  (original logic, unchanged)
            // ════════════════════════════════════════════════════
            Panel enterTab = tabPages[0];

            // ── Patient identity fields (new) ──
            enterTab.Controls.Add(Lbl("Patient Name", FBold, TextGrey, 0, 10));
            var tbPatName = TB(0, 30, 280); enterTab.Controls.Add(tbPatName);

            enterTab.Controls.Add(Lbl("Patient ID", FBold, TextGrey, 300, 10));
            var tbPatID = TB(300, 30, 200); enterTab.Controls.Add(tbPatID);

            var menuCard = Card(0, 76, 1040, 420, "Test Menu — enter values in the Result column");
            menuCard.Paint += (s2, e2) => e2.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(220, 224, 230)), 0, 0, menuCard.Width - 1, menuCard.Height - 1);
            enterTab.Controls.Add(menuCard);

            // header row
            var hdr = new Panel { Location = new Point(0, 26), Size = new Size(1040, 34), BackColor = NavBg };
            void AddHdr(string t, int x, int w) { hdr.Controls.Add(new Label { Text = t, Font = FBold, ForeColor = Color.White, Location = new Point(x, 7), Size = new Size(w, 20) }); }
            AddHdr("Test", 10, 160); AddHdr("Min", 178, 60); AddHdr("Max", 248, 60);
            AddHdr("Unit", 316, 90); AddHdr("Your Result", 414, 120); AddHdr("Status", 544, 130);
            menuCard.Controls.Add(hdr);

            // ── ORIGINAL test list (unchanged) ──
            var labMenu = new System.Collections.Generic.List<(string name, double min, double max, string unit)>
            {
                ("Glucose",       70,   100,  "mg/dL"),   ("HGB",           13.5, 17.5, "g/dL"),
                ("WBC",           4.0,  11.0, "10^9/L"),  ("RBC",           4.5,  5.9,  "10^12/L"),
                ("TSH",           0.4,  6.1,  "uIU/mL"),  ("Uric Acid",     3.4,  7.0,  "mg/dL"),
                ("Calcium",       8.5,  10.2, "mg/dL"),   ("Platelets",     150,  450,  "10^9/L"),
                ("Creatinine",    0.6,  1.3,  "mg/dL"),   ("Cholesterol",   125,  200,  "mg/dL"),
                ("Triglycerides", 0,    150,  "mg/dL"),   ("ALT (SGPT)",    7,    56,   "U/L"),
                ("AST (SGOT)",    5,    40,   "U/L"),     ("Bilirubin",     0.1,  1.2,  "mg/dL"),
                ("Vitamin D",     20,   50,   "ng/mL"),   ("Iron",          60,   170,  "ug/dL")
            };

            var resultBoxes = new System.Collections.Generic.List<(string name, double min, double max, string unit, TextBox tb, Label stLbl)>();

            Panel listPanel = new Panel { Location = new Point(0, 60), Size = new Size(1040, 356), AutoScroll = true, BackColor = CardBg };
            menuCard.Controls.Add(listPanel);

            for (int i = 0; i < labMenu.Count; i++)
            {
                var test = labMenu[i];
                Color rowBg = (i % 2 == 0) ? CardBg : Color.FromArgb(248, 250, 253);
                Panel row = new Panel { Location = new Point(0, i * 34), Size = new Size(1020, 33), BackColor = rowBg };
                row.Controls.Add(new Label { Text = test.name, Font = FNorm, ForeColor = TextDark, Location = new Point(10, 7), Size = new Size(155, 20) });
                row.Controls.Add(new Label { Text = test.min.ToString(), Font = FNorm, ForeColor = TextGrey, Location = new Point(178, 7), Size = new Size(55, 20) });
                row.Controls.Add(new Label { Text = test.max.ToString(), Font = FNorm, ForeColor = TextGrey, Location = new Point(248, 7), Size = new Size(55, 20) });
                row.Controls.Add(new Label { Text = test.unit, Font = FNorm, ForeColor = TextGrey, Location = new Point(316, 7), Size = new Size(86, 20) });
                TextBox tb = new TextBox { Font = FNorm, Location = new Point(414, 5), Size = new Size(110, 22), BorderStyle = BorderStyle.FixedSingle };
                Label stLbl = new Label { Text = "—", Font = FBold, Location = new Point(544, 7), Size = new Size(200, 20), ForeColor = TextGrey };
                row.Controls.Add(tb); row.Controls.Add(stLbl);
                listPanel.Controls.Add(row);
                resultBoxes.Add((test.name, test.min, test.max, test.unit, tb, stLbl));
            }

            var btnReport = Btn("Generate Report", AccentBlue, 0, 508, 180, 36);
            enterTab.Controls.Add(btnReport);


            var btnShowToday = Btn("👥 Today's Patients", AccentGrn, 190, 508, 200, 36);
            enterTab.Controls.Add(btnShowToday);

            var todayPopup = new Panel
            {
                Location = new Point(0, 950),
                Size = new Size(1040, 300),
                BackColor = Color.White,
                Visible = false
            };
            enterTab.Controls.Add(todayPopup);

            btnShowToday.Click += (s, e) =>
            {
                todayPopup.Controls.Clear();
                todayPopup.Visible = true;
                var recs = labService.GetTodaysRecords();

                if (recs.Count == 0)
                {
                    todayPopup.Controls.Add(new Label
                    {
                        Text = "There is no patient today till now",
                        Font = FBold,
                        ForeColor = TextGrey,
                        Location = new Point(10, 10),
                        Size = new Size(500, 28)
                    });
                    return;
                }

                int y = 8;
                todayPopup.Controls.Add(new Label
                {
                    Text = $"👥  Today's patients — {recs.Count} status",
                    Font = FBold,
                    ForeColor = AccentBlue,
                    Location = new Point(10, y),
                    Size = new Size(600, 24)
                });
                y += 30;

                foreach (var rec in recs)
                {
                    int abnormal = rec.Results.Count(tr => tr.Status != "NORMAL");
                    var lbl = new Label
                    {
                        Text = $"🔹 {rec.PatientName}  |  ID: {rec.PatientID}  |  {rec.TestTime}  |  test: {rec.Results.Count}  |  غير طبيعي: {abnormal}",
                        Font = FNorm,
                        ForeColor = abnormal > 0 ? AccentRed : AccentGrn,
                        Location = new Point(10, y),
                        Size = new Size(1000, 22)
                    };
                    todayPopup.Controls.Add(lbl);
                    y += 26;
                }
            };

            // ── Report grid (original columns preserved) ──
            var reportGrid = Grid(0, 558, 1040, 320);
            reportGrid.Columns.Add("Test", "Test Name");
            reportGrid.Columns.Add("Result", "Result");
            reportGrid.Columns.Add("Unit", "Unit");
            reportGrid.Columns.Add("Range", "Ref. Range");
            reportGrid.Columns.Add("Status", "Status");
            reportGrid.Visible = false;
            enterTab.Controls.Add(reportGrid);

            // Patient info header shown above report
            var reportHeaderLbl = Lbl("", new Font("Segoe UI", 10, FontStyle.Bold), TextDark, 0, 892);
            enterTab.Controls.Add(reportHeaderLbl);

            var sumLbl = Lbl("", FBold, TextGrey, 0, 916);
            enterTab.Controls.Add(sumLbl);

            var savedLbl = Lbl("", FBold, AccentGrn, 0, 940);
            enterTab.Controls.Add(savedLbl);

            // ── Generate Report button handler (original logic + save) ──
            btnReport.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tbPatName.Text))
                { MessageBox.Show("Enter patient name.", "Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (string.IsNullOrWhiteSpace(tbPatID.Text))
                { MessageBox.Show("Enter patient ID.", "Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                reportGrid.Rows.Clear();
                int abnormal = 0; bool any = false;

                // Build the persisted record
                var record = new LabPatientRecord
                {
                    PatientName = tbPatName.Text.Trim(),
                    PatientID = tbPatID.Text.Trim(),
                    TestDate = System.DateTime.Today.ToString("yyyy-MM-dd"),
                    TestTime = System.DateTime.Now.ToString("HH:mm:ss")
                };

                foreach (var r in resultBoxes)
                {
                    if (string.IsNullOrWhiteSpace(r.tb.Text)) continue;
                    double val;
                    if (!double.TryParse(r.tb.Text, out val))
                    { r.stLbl.Text = "Invalid"; r.stLbl.ForeColor = Color.OrangeRed; continue; }

                    any = true;
                    string status; Color col;
                    // ── ORIGINAL High/Low logic (unchanged) ──
                    if (val < r.min) { status = "LOW  (L)"; col = AccentRed; }
                    else if (val > r.max) { status = "HIGH (H)"; col = AccentRed; }
                    else { status = "NORMAL"; col = AccentGrn; }

                    r.stLbl.Text = status; r.stLbl.ForeColor = col;
                    if (status != "NORMAL") abnormal++;

                    int idx = reportGrid.Rows.Add(r.name, val, r.unit, r.min + " \u2013 " + r.max, status);
                    if (status != "NORMAL")
                    { reportGrid.Rows[idx].DefaultCellStyle.ForeColor = AccentRed; reportGrid.Rows[idx].DefaultCellStyle.Font = FBold; }

                    // Add to persistent record
                    record.Results.Add(new LabTestResult
                    {
                        TestName = r.name,
                        Value = val,
                        Unit = r.unit,
                        RefRange = r.min + " – " + r.max,
                        Status = status
                    });
                }

                if (!any) { MessageBox.Show("Enter at least one result.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                // ── Show patient header in report ──
                reportHeaderLbl.Text = $"Patient: {record.PatientName}   |   ID: {record.PatientID}   |   Date: {record.TestDate}  {record.TestTime}";

                reportGrid.Visible = true;
                sumLbl.Text = abnormal == 0 ? "All results within normal ranges." : abnormal + " abnormal result(s) — consult a doctor.";
                sumLbl.ForeColor = abnormal == 0 ? AccentGrn : AccentRed;

                // ── Save to JSON ──
                labService.SaveRecord(record);
                savedLbl.Text = "✔  Report saved to lab_records.json";
                savedLbl.ForeColor = AccentGrn;

                // ── Reset fields only (report stays visible) ──
                tbPatName.Clear();
                tbPatID.Clear();
                foreach (var r in resultBoxes)
                {
                    r.tb.Text = "";
                    r.stLbl.Text = "—";
                    r.stLbl.ForeColor = TextGrey;
                }

            };


            // ════════════════════════════════════════════════════
            //  TAB 1 – Search Patient by ID
            // ════════════════════════════════════════════════════
            Panel searchTab = tabPages[1];

            searchTab.Controls.Add(Lbl("Search by Patient ID", FBold, TextGrey, 0, 10));
            var tbSearchID = TB(0, 30, 260); searchTab.Controls.Add(tbSearchID);
            var btnSearch = Btn("Search", AccentBlue, 272, 28, 110, 34); searchTab.Controls.Add(btnSearch);
            var searchMsg = Lbl("", FBold, TextGrey, 0, 72); searchTab.Controls.Add(searchMsg);

            var searchResultPanel = new Panel { Location = new Point(0, 96), Size = new Size(1040, 800), AutoScroll = true, BackColor = Color.FromArgb(236, 240, 245) };
            searchTab.Controls.Add(searchResultPanel);

            btnSearch.Click += (s, e) =>
            {
                searchResultPanel.Controls.Clear();
                string searchId = tbSearchID.Text.Trim();
                if (string.IsNullOrWhiteSpace(searchId))
                { searchMsg.Text = "Enter a Patient ID to search."; searchMsg.ForeColor = AccentRed; return; }

                var found = labService.SearchByID(searchId);
                if (found.Count == 0)
                { searchMsg.Text = "No records found for ID: " + searchId; searchMsg.ForeColor = AccentRed; return; }

                searchMsg.Text = $"Found {found.Count} record(s) for ID: {searchId}";
                searchMsg.ForeColor = AccentGrn;

                int yOff = 0;
                foreach (var rec in found)
                {
                    // Record header card
                    var recCard = new Panel
                    {
                        Location = new Point(0, yOff),
                        Size = new Size(1020, 36 + rec.Results.Count * 28 + 16),
                        BackColor = Color.White
                    };
                    recCard.Paint += (sc, ec) => ec.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(200, 210, 230)), 0, 0, recCard.Width - 1, recCard.Height - 1);

                    recCard.Controls.Add(new Label
                    {
                        Text = $"  {rec.PatientName}   |   ID: {rec.PatientID}   |   {rec.TestDate}  {rec.TestTime}",
                        Font = FBold,
                        ForeColor = AccentBlue,
                        Location = new Point(6, 8),
                        Size = new Size(980, 22)
                    });

                    int rowY = 34;
                    foreach (var tr in rec.Results)
                    {
                        Color fgCol = (tr.Status == "NORMAL") ? AccentGrn : AccentRed;
                        var rowLbl = new Label
                        {
                            Text = $"    {tr.TestName,-18}  {tr.Value,8}  {tr.Unit,-10}  Ref: {tr.RefRange,-14}  →  {tr.Status}",
                            Font = (tr.Status == "NORMAL") ? FNorm : FBold,
                            ForeColor = fgCol,
                            Location = new Point(6, rowY),
                            Size = new Size(980, 22)
                        };
                        recCard.Controls.Add(rowLbl);
                        rowY += 26;
                    }

                    searchResultPanel.Controls.Add(recCard);
                    yOff += recCard.Height + 10;
                }
            };

            // ════════════════════════════════════════════════════
            //  TAB 2 – Today's Tests
            // ════════════════════════════════════════════════════
            Panel todayTab = tabPages[2];

            var btnRefreshToday = Btn("Refresh", AccentBlue, 0, 10, 110, 34); todayTab.Controls.Add(btnRefreshToday);
            var todayMsg = Lbl("", FBold, TextGrey, 124, 18); todayTab.Controls.Add(todayMsg);

            var todayGrid = Grid(0, 56, 1040, 200);
            todayGrid.Columns.Add("tpname", "Patient Name");
            todayGrid.Columns.Add("tpid", "Patient ID");
            todayGrid.Columns.Add("ttime", "Time");
            todayGrid.Columns.Add("ttests", "Tests Performed");
            todayGrid.Columns.Add("tabnorm", "Abnormal Results");
            todayTab.Controls.Add(todayGrid);

            var todayDetailPanel = new Panel { Location = new Point(0, 268), Size = new Size(1040, 700), AutoScroll = true, BackColor = Color.FromArgb(236, 240, 245) };
            todayTab.Controls.Add(todayDetailPanel);

            void LoadTodayRecords()
            {
                todayGrid.Rows.Clear();
                todayDetailPanel.Controls.Clear();

                var todayRecs = labService.GetTodaysRecords();
                todayMsg.Text = $"Today ({System.DateTime.Today:yyyy-MM-dd}): {todayRecs.Count} test session(s)";
                todayMsg.ForeColor = todayRecs.Count > 0 ? AccentBlue : TextGrey;

                int yOff = 0;
                foreach (var rec in todayRecs)
                {
                    int abnCount = rec.Results.Count(tr => tr.Status != "NORMAL");
                    todayGrid.Rows.Add(rec.PatientName, rec.PatientID, rec.TestTime,
                                       rec.Results.Count, abnCount == 0 ? "None" : abnCount.ToString());

                    // Detail card
                    var recCard = new Panel
                    {
                        Location = new Point(0, yOff),
                        Size = new Size(1020, 36 + rec.Results.Count * 26 + 16),
                        BackColor = Color.White
                    };
                    recCard.Paint += (sc, ec) => ec.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(200, 210, 230)), 0, 0, recCard.Width - 1, recCard.Height - 1);
                    recCard.Controls.Add(new Label
                    {
                        Text = $"  {rec.PatientName}   |   ID: {rec.PatientID}   |   {rec.TestTime}",
                        Font = FBold,
                        ForeColor = AccentBlue,
                        Location = new Point(6, 8),
                        Size = new Size(980, 22)
                    });
                    int rowY = 34;
                    foreach (var tr in rec.Results)
                    {
                        Color fgCol = (tr.Status == "NORMAL") ? AccentGrn : AccentRed;
                        var rowLbl = new Label
                        {
                            Text = $"    {tr.TestName,-18}  {tr.Value,8}  {tr.Unit,-10}  Ref: {tr.RefRange,-14}  →  {tr.Status}",
                            Font = (tr.Status == "NORMAL") ? FNorm : FBold,
                            ForeColor = fgCol,
                            Location = new Point(6, rowY),
                            Size = new Size(980, 22)
                        };
                        recCard.Controls.Add(rowLbl);
                        rowY += 26;
                    }
                    todayDetailPanel.Controls.Add(recCard);
                    yOff += recCard.Height + 10;
                }
            }

            btnRefreshToday.Click += (s, e) => LoadTodayRecords();
            LoadTodayRecords();   // auto-load on page open

            btnRefreshToday.Click += (s, e) => LoadTodayRecords();
            LoadTodayRecords();   // auto-load on page open

            // ── Show first tab by default ──
            ShowLabTab(0);

            return page;
        }









        // ═══════════════════════════════════════════════════════
        //  PAGE: RECEPTION BOOKING
        // ═══════════════════════════════════════════════════════
        Panel PageReceptionBooking()
        {
            var page = ScrollPage("🎫  Reception Booking", "Walk-in bookings, daily schedule — saved to JSON");
            var scroll = GetScroll(page);

            string[] days = { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            string[] clinics = receptionService.Clinics;

            // Tab strip
            var tabStrip = new Panel { Location = new Point(0, 0), Size = new Size(900, 38), BackColor = Color.FromArgb(220, 228, 240) };
            string[] tabNames = { "New Booking", "Today's Schedule", "All Bookings" };
            Panel[] tabPages = new Panel[tabNames.Length];
            Button[] tabBtns = new Button[tabNames.Length];
            for (int i = 0; i < tabPages.Length; i++)
                tabPages[i] = new Panel { Location = new Point(0, 42), Size = new Size(900, 600), BackColor = CardBg, Visible = false };

            void ShowTab(int idx)
            {
                for (int k = 0; k < tabBtns.Length; k++)
                {
                    tabPages[k].Visible = (k == idx);
                    tabBtns[k].BackColor = (k == idx) ? AccentBlue : Color.FromArgb(200, 210, 230);
                    tabBtns[k].ForeColor = (k == idx) ? Color.White : TextDark;
                }
            }

            for (int i = 0; i < tabNames.Length; i++)
            {
                int idx = i;
                tabBtns[i] = new Button
                {
                    Text = tabNames[i],
                    Font = FBold,
                    Size = new Size(180, 36),
                    Location = new Point(4 + i * 184, 1),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(200, 210, 230),
                    ForeColor = TextDark
                };
                tabBtns[i].FlatAppearance.BorderSize = 0;
                tabBtns[i].Click += (s, e) => ShowTab(idx);
                tabStrip.Controls.Add(tabBtns[i]);
            }

            // TAB 0 – New Booking
            Panel newTab = tabPages[0];
            newTab.Controls.Add(Lbl("Patient Name", FBold, TextGrey, 20, 20)); var tbPatName = TB(20, 40, 280); newTab.Controls.Add(tbPatName);
            newTab.Controls.Add(Lbl("National ID", FBold, TextGrey, 20, 76)); var tbPatId = TB(20, 96, 280); newTab.Controls.Add(tbPatId);
            newTab.Controls.Add(Lbl("Clinic", FBold, TextGrey, 20, 132));
            var cbClin = CB(20, 152, 280, clinics); newTab.Controls.Add(cbClin);
            newTab.Controls.Add(Lbl("Day", FBold, TextGrey, 20, 188));
            var cbDay = CB(20, 208, 280, days); newTab.Controls.Add(cbDay);
            cbDay.SelectedItem = receptionChosenDay;

            var bookMsg = Lbl("", FBold, TextGrey, 20, 250); newTab.Controls.Add(bookMsg);
            var btnBk = Btn("✅ Confirm Booking", AccentGrn, 20, 276, 170, 34); newTab.Controls.Add(btnBk);

            // ── Bookings list section (shown below the form after first booking) ──
            var separatorLbl = Lbl("", FBold, TextGrey, 20, 322); newTab.Controls.Add(separatorLbl);

            // Section header
            var bookingListHeader = new Panel
            {
                Location = new Point(20, 330),
                Size = new Size(840, 32),
                BackColor = Color.FromArgb(30, 39, 58),
                Visible = false
            };
            var hdrLbl = new Label { Text = "📋  Bookings This Session", Font = FBold, ForeColor = Color.White, AutoSize = true, Location = new Point(10, 7) };
            bookingListHeader.Controls.Add(hdrLbl);
            newTab.Controls.Add(bookingListHeader);

            // Grid showing all bookings made
            var bookingGrid = Grid(20, 364, 840, 200);
            bookingGrid.Visible = false;
            bookingGrid.Columns.Add("bname", "Patient Name");
            bookingGrid.Columns.Add("bid", "National ID");
            bookingGrid.Columns.Add("bclinic", "Clinic");
            bookingGrid.Columns.Add("bday", "Day");
            bookingGrid.Columns.Add("bcode", "Booking Code");
            bookingGrid.Columns.Add("btime", "Booked At");
            bookingGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 39, 58);
            bookingGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            bookingGrid.ColumnHeadersDefaultCellStyle.Font = FBold;
            bookingGrid.EnableHeadersVisualStyles = false;
            bookingGrid.ColumnHeadersHeight = 34;
            newTab.Controls.Add(bookingGrid);

            // Total count label
            var totalBookingsLbl = Lbl("", FBold, AccentBlue, 20, 572); newTab.Controls.Add(totalBookingsLbl);

            // Resize newTab to fit everything
            newTab.Size = new Size(900, 610);

            // ── Load existing bookings from JSON on page open ──
            void LoadAllBookingsToGrid()
            {
                bookingGrid.Rows.Clear();
                foreach (var b in receptionService.AllBookings)
                {
                    var p2 = receptionService.FindPatient(b.PatientID);
                    string pname = p2 != null ? p2.NameEnglish : b.PatientID;
                    bookingGrid.Rows.Add(pname, b.PatientID, b.ClinicName, b.Day, b.Code, b.BookingDateTime);
                }
                if (bookingGrid.Rows.Count > 0)
                {
                    bookingListHeader.Visible = true;
                    bookingGrid.Visible = true;
                    totalBookingsLbl.Text = $"Total bookings: {bookingGrid.Rows.Count}";
                }
            }
            LoadAllBookingsToGrid();

            btnBk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tbPatName.Text) || string.IsNullOrWhiteSpace(tbPatId.Text))
                { bookMsg.Text = "⚠ Name and ID required."; bookMsg.ForeColor = AccentRed; return; }
                if (cbClin.SelectedIndex < 0) { bookMsg.Text = "⚠ Select a clinic."; bookMsg.ForeColor = AccentRed; return; }

                var pat = new HospitalBooking { NameEnglish = tbPatName.Text.Trim(), NationalID = tbPatId.Text.Trim() };
                receptionService.AddPatientIfNew(pat);
                receptionChosenDay = cbDay.SelectedItem.ToString();
                var bk = receptionService.CreateBooking(pat.NationalID, cbClin.SelectedItem.ToString(), receptionChosenDay);

                bookMsg.Text = "✔ Booked!  Code: " + bk.Code + "   " + bk.ClinicName + "   " + receptionChosenDay;
                bookMsg.ForeColor = AccentGrn;

                // Reload grid (includes new booking just saved to JSON)
                LoadAllBookingsToGrid();

                // Highlight the newest row
                if (bookingGrid.Rows.Count > 0)
                {
                    bookingGrid.Rows[bookingGrid.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 230);
                    bookingGrid.Rows[bookingGrid.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.FromArgb(20, 100, 40);
                    bookingGrid.FirstDisplayedScrollingRowIndex = bookingGrid.Rows.Count - 1;
                }

                tbPatName.Clear(); tbPatId.Clear();
            };

            // TAB 1 – Today's Schedule
            Panel schedTab = tabPages[1];
            schedTab.Controls.Add(Lbl("Day", FBold, TextGrey, 20, 20));
            var cbSchedDay = CB(20, 40, 200, days); cbSchedDay.SelectedItem = receptionChosenDay; schedTab.Controls.Add(cbSchedDay);
            var btnLoad = Btn("Load", AccentBlue, 232, 38, 90, 30); schedTab.Controls.Add(btnLoad);

            var schedGrid = Grid(20, 84, 840, 440);
            schedGrid.Columns.Add("Name", "Patient Name"); schedGrid.Columns.Add("ID", "National ID");
            schedGrid.Columns.Add("Clinic", "Clinic"); schedGrid.Columns.Add("Code", "Booking Code");
            schedTab.Controls.Add(schedGrid);

            btnLoad.Click += (s, e) =>
            {
                schedGrid.Rows.Clear();
                string day = cbSchedDay.SelectedItem != null ? cbSchedDay.SelectedItem.ToString() : receptionChosenDay;
                foreach (var bk in receptionService.GetBookingsByDay(day))
                {
                    var pat = receptionService.FindPatient(bk.PatientID);
                    schedGrid.Rows.Add(pat != null ? pat.NameEnglish : "Unknown", bk.PatientID, bk.ClinicName, bk.Code);
                }
                if (schedGrid.Rows.Count == 0)
                    MessageBox.Show("No bookings for " + day + ".", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // TAB 2 – All Bookings
            Panel allTab = tabPages[2];
            var btnRef = Btn("Refresh", AccentBlue, 20, 20, 120, 32); allTab.Controls.Add(btnRef);
            var allGrid = Grid(20, 66, 840, 460);
            allGrid.Columns.Add("Name", "Patient Name"); allGrid.Columns.Add("ID", "National ID");
            allGrid.Columns.Add("Clinic", "Clinic"); allGrid.Columns.Add("Day", "Day"); allGrid.Columns.Add("Code", "Code");
            allTab.Controls.Add(allGrid);

            btnRef.Click += (s, e) =>
            {
                allGrid.Rows.Clear();
                foreach (var bk in receptionService.AllBookings)
                {
                    var pat = receptionService.FindPatient(bk.PatientID);
                    allGrid.Rows.Add(pat != null ? pat.NameEnglish : "Unknown", bk.PatientID, bk.ClinicName, bk.Day, bk.Code);
                }
            };

            scroll.Controls.Add(tabStrip);
            foreach (var tp in tabPages) scroll.Controls.Add(tp);
            ShowTab(0);
            return page;
        }

        // ═══════════════════════════════════════════════════════
        //  PAGE: ACCOUNTS  (employee account management)
        // ═══════════════════════════════════════════════════════
        Panel PageAccounts()
        {
            empAccountSvc.LoadData();
            var page = ScrollPage("🔑  Account Management", "Employee login accounts — saved to employee_accounts.json");
            var scroll = GetScroll(page);

            bool isAdmin = _loginResult != null && _loginResult.Role == "Admin";

            // Logged-in user info
            var infoCard = Card(0, 14, 500, 90, "Current Session");
            if (_loginResult != null)
            {
                infoCard.Controls.Add(Lbl("Logged in as: " + _loginResult.Username + "   (" + _loginResult.Role + ")", FBold, AccentGrn, 14, 36));
            }
            scroll.Controls.Add(infoCard);

            if (!isAdmin)
            {
                scroll.Controls.Add(Lbl("Only Admins can manage employee accounts.", FBold, AccentRed, 0, 104));
                return page;
            }

            // Add account card
            var addCard = Card(0, 104, 560, 220, "Add Employee Account");
            addCard.Controls.Add(Lbl("Username", FBold, TextGrey, 14, 38)); var tbUser = TB(14, 58, 240); addCard.Controls.Add(tbUser);
            addCard.Controls.Add(Lbl("Password", FBold, TextGrey, 14, 94)); var tbPass = TB(14, 114, 240, ""); tbPass.UseSystemPasswordChar = true; addCard.Controls.Add(tbPass);
            addCard.Controls.Add(Lbl("Role", FBold, TextGrey, 14, 150));
            var cbRole = CB(14, 170, 160, new[] { "Admin", "Doctor", "Nurse", "Staff" }); addCard.Controls.Add(cbRole);
            var addMsg = Lbl("", FBold, TextGrey, 14, 210); addCard.Controls.Add(addMsg);
            var addBtn = Btn("Add Account", AccentBlue, 300, 150, 140, 36); addCard.Controls.Add(addBtn);

            addBtn.Click += (s, e) =>
            {
                string u = tbUser.Text.Trim();
                string p = tbPass.Text;
                string r = cbRole.SelectedItem?.ToString() ?? "Staff";
                if (string.IsNullOrWhiteSpace(u) || p.Length < 6)
                { addMsg.Text = "Username and password (min 6 chars) required."; addMsg.ForeColor = AccentRed; return; }
                bool ok = empAccountSvc.AddAccount(u, p, r);
                addMsg.Text = ok ? "Account created for " + u + "." : "Username already exists.";
                addMsg.ForeColor = ok ? AccentGrn : AccentRed;
                if (ok) { tbUser.Clear(); tbPass.Clear(); }
            };
            scroll.Controls.Add(addCard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: PHARMACISTS  (mirrors PageDoctors pattern)
        // ═════════════════════════════════════════════════════
        Panel PagePharmacists()
        {
            var page = ScrollPage("Pharmacist Management", "View and hire pharmacy staff");
            var s = GetScroll(page);

            // ───────────── GRID ─────────────
            var gcard = Card(0, 14, 1080, 380, "All Pharmacists");
            var dg = Grid(14, 34, 1052, 334);

            dg.Columns.Add("id", "Staff ID");
            dg.Columns.Add("name", "Name");
            dg.Columns.Add("role", "Role");

            void RefreshGrid()
            {
                dg.Rows.Clear();
                foreach (var p in PharmacyStaff.pharmacists)
                    dg.Rows.Add(p.PharmacyStaffId, p.Name, p.Role);
            }

            RefreshGrid();
            gcard.Controls.Add(dg);
            s.Controls.Add(gcard);

            // ───────────── HIRE PHARMACIST ─────────────
            var hcard = Card(0, 394, 1080, 200, "➕ Hire New Pharmacist");
            int lx = 14, ly = 34, gap = 38;

            hcard.Controls.Add(Lbl("Name:", FBold, TextGrey, lx, ly));
            var tbName = TB(lx + 60, ly, 220); hcard.Controls.Add(tbName);

            hcard.Controls.Add(Lbl("Role:", FBold, TextGrey, lx + 300, ly));
            var cbRole = CB(lx + 348, ly, 200,
                new[] { "Pharmacist", "Senior Pharmacist", "Pharmacy Technician", "Intern" });
            hcard.Controls.Add(cbRole);

            ly += gap;

            hcard.Controls.Add(Lbl("Salary:", FBold, TextGrey, lx, ly));
            var tbSal = TB(lx + 60, ly, 100, "9000"); hcard.Controls.Add(tbSal);

            hcard.Controls.Add(Lbl("Exp (yrs):", FBold, TextGrey, lx + 180, ly));
            var tbExp = TB(lx + 268, ly, 70, "1"); hcard.Controls.Add(tbExp);

            ly += gap + 6;

            var hireBtn = Btn("✅ Hire Pharmacist", AccentGrn, lx, ly, 170, 36);
            hireBtn.Click += (s2, e2) =>
            {
                try
                {
                    string nm = tbName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(nm)) { MessageBox.Show("Enter a name."); return; }

                    bool dup = PharmacyStaff.pharmacists
                        .Any(p => p.Name.Equals(nm, StringComparison.OrdinalIgnoreCase));
                    if (dup) { MessageBox.Show($"Pharmacist '{nm}' already exists."); return; }

                    decimal sal = decimal.TryParse(tbSal.Text, out decimal sv) ? sv : 9000;
                    double exp = double.TryParse(tbExp.Text, out double ev) ? ev : 1;
                    string nid = $"PH-{DateTime.Now.Ticks % 99999}";

                    var newPharm = new PharmacyStaff(
                        nm, 28, GenderType.Male, nid,
                        "00000000000", "pharm@neurai.com", "Unknown",
                        sal, exp,
                        cbRole.SelectedItem?.ToString() ?? "Pharmacist"
                    );

                    // AddPharmacists adds to the list AND saves to pharmacists.json
                    HospitalData.AddPharmacists(newPharm);

                    RefreshGrid();
                    MessageBox.Show($"Pharmacist '{nm}' hired! (ID: {newPharm.PharmacyStaffId})", "Success ✅");
                    tbName.Clear();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            };

            hcard.Controls.Add(hireBtn);
            s.Controls.Add(hcard);

            return page;
        }

        // ═════════════════════════════════════════════════════
        //  PAGE: PRESCRIPTION MANAGEMENT
        // ═════════════════════════════════════════════════════
        Panel PagePrescription()
        {
            var page = ScrollPage("Prescription Management", "Add, remove and search medicines");
            var s = GetScroll(page);

            var pharmacy = neurai.CampusFacilities.HospitalPharmacy;
            var prescriptionItems = new System.Collections.Generic.List<PrescriptionItem>();

            // ─── Search Bar ───
            var searchBox = new TextBox
            {
                Location = new Point(0, 0),
                Size = new Size(680, 30),
                Font = FNorm,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Search result will appear here...",
                ForeColor = TextGrey
            };
            var searchResult = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(680, 30),
                BackColor = CardBg
            };
            searchResult.Controls.Add(searchBox);

            // Search card
            var searchCard = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1080, 50),
                BackColor = CardBg
            };
            searchCard.Paint += (sv, ev) =>
            {
                ev.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(18, 0, 0, 0)), 0, 0, searchCard.Width - 1, searchCard.Height - 1);
            };
            var tbMedSearch = TB(12, 12, 500, "Search medicines by name...");
            tbMedSearch.Font = FNorm;
            var searchDropdown = new Panel
            {
                BackColor = CardBg,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Location = new Point(12, 40),
                Size = new Size(500, 120),
                AutoScroll = true
            };
            searchCard.Controls.Add(tbMedSearch);
            searchCard.Controls.Add(searchDropdown);
            s.Controls.Add(searchCard);

            // ─── Two-column area ───
            var leftCard = Card(0, 60, 530, 200, "+ Add Medicine to Prescription");
            var rightCard = Card(545, 60, 535, 200, "🖹 Add / Update Medicine in Pharmacy");

            // LEFT CARD — Add to prescription
            int lx = 14, ly = 36;
            leftCard.Controls.Add(Lbl("Medicine Name:", FBold, TextGrey, lx, ly + 6));
            var tbMedName = TB(lx + 108, ly, 280, "Medicine name");
            leftCard.Controls.Add(tbMedName);

            ly += 40;
            leftCard.Controls.Add(Lbl("Qty:", FBold, TextGrey, lx, ly + 6));
            var tbQty = TB(lx + 36, ly, 60, "1"); leftCard.Controls.Add(tbQty);

            leftCard.Controls.Add(Lbl("Times/day:", FBold, TextGrey, lx + 110, ly + 6));
            var tbTimes = TB(lx + 198, ly, 60, "1"); leftCard.Controls.Add(tbTimes);

            leftCard.Controls.Add(Lbl("Days:", FBold, TextGrey, lx + 272, ly + 6));
            var tbDays = TB(lx + 310, ly, 60, "1"); leftCard.Controls.Add(tbDays);

            ly += 44;
            var btnAdd = Btn("+ Add", AccentGrn, lx, ly, 90, 34);
            var btnClear = Btn("Clear", TextGrey, lx + 104, ly, 80, 34);
            btnClear.BackColor = Color.FromArgb(108, 117, 125);
            leftCard.Controls.Add(btnAdd);
            leftCard.Controls.Add(btnClear);
            s.Controls.Add(leftCard);

            // RIGHT CARD — Add/Update in Pharmacy
            int rx = 14, ry = 36;
            rightCard.Controls.Add(Lbl("Medicine:", FBold, TextGrey, rx, ry + 6));
            var tbPharmMed = TB(rx + 78, ry, 380, "Medicine name"); rightCard.Controls.Add(tbPharmMed);
            ry += 44;
            var btnAddUpdate = Btn("Add / Update", AccentOrg, rx, ry, 130, 34);
            rightCard.Controls.Add(btnAddUpdate);
            s.Controls.Add(rightCard);

            // ─── Current Prescription Table ───
            var tableCard = new Panel
            {
                Location = new Point(0, 274),
                Size = new Size(1080, 380),
                BackColor = CardBg
            };
            tableCard.Paint += (sv, ev) =>
                ev.Graphics.DrawRectangle(new System.Drawing.Pen(Color.FromArgb(18, 0, 0, 0)), 0, 0, tableCard.Width - 1, tableCard.Height - 1);

            // Header row
            var headerPnl = new Panel { Location = new Point(0, 0), Size = new Size(1080, 28), BackColor = Color.Empty };
            headerPnl.Controls.Add(Lbl("Current Prescription", FBold, TextGrey, 14, 6));
            var btnClearAll = Btn("Clear All", AccentRed, 940, 0, 90, 28);
            headerPnl.Controls.Add(btnClearAll);
            tableCard.Controls.Add(headerPnl);

            var dg = Grid(0, 28, 1080, 310);
            dg.Columns.Add("med", "Medicine");
            dg.Columns.Add("cat", "Category");
            dg.Columns.Add("qty", "Qty");
            dg.Columns.Add("tpd", "Times/Day");
            dg.Columns.Add("days", "Days");
            dg.Columns.Add("price", "Total Price");

            // Header style — dark background
            dg.EnableHeadersVisualStyles = false;
            dg.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 39, 58);
            dg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dg.ColumnHeadersDefaultCellStyle.Font = FBold;
            dg.ColumnHeadersHeight = 36;

            tableCard.Controls.Add(dg);

            // Total label
            var lblTotal = Lbl("Total: 0.00 EGP", FBold, TextDark, 900, 344, 160);
            tableCard.Controls.Add(lblTotal);
            s.Controls.Add(tableCard);

            // ─── Save Prescription button ───
            var btnSave = Btn("💾  Save Prescription", AccentBlue, 0, 668, 220, 44);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            s.Controls.Add(btnSave);

            // ─── Helper: refresh grid ───
            void RefreshGrid()
            {
                dg.Rows.Clear();
                double total = 0;
                foreach (var item in prescriptionItems)
                {
                    string category = "";
                    foreach (var kv in pharmacy.categories)
                        if (kv.Value.Contains(item.Medicine)) { category = kv.Key; break; }
                    double tp = item.TotalPrice();
                    total += tp;
                    dg.Rows.Add(item.Medicine.Name, category, item.Quantity, item.TimesPerDay, item.NumberOfDays, $"{tp:F2} EGP");
                }
                lblTotal.Text = $"Total: {total:F2} EGP";
            }

            // ─── Search medicine live ───
            tbMedSearch.TextChanged += (sv, ev) =>
            {
                string q = tbMedSearch.Text.Trim();
                if (string.IsNullOrEmpty(q) || q == "Search medicines by name...") { searchDropdown.Visible = false; return; }
                searchDropdown.Controls.Clear();
                int dy = 2;
                foreach (var kv in pharmacy.categories)
                {
                    foreach (var m in kv.Value)
                    {
                        if (m.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var captured = m;
                            var lb = new Label
                            {
                                Text = $"{m.Name}  [{kv.Key}]  — {m.Price:F2} EGP",
                                Font = FNorm,
                                ForeColor = TextDark,
                                Location = new Point(4, dy),
                                AutoSize = true,
                                Cursor = Cursors.Hand
                            };
                            lb.Click += (s2, e2) =>
                            {
                                tbMedName.Text = captured.Name;
                                tbMedName.ForeColor = TextDark;
                                tbMedSearch.Text = "";
                                searchDropdown.Visible = false;
                            };
                            searchDropdown.Controls.Add(lb);
                            dy += 24;
                        }
                    }
                }
                searchDropdown.Visible = searchDropdown.Controls.Count > 0;
            };

            // ─── Add medicine to prescription ───
            btnAdd.Click += (sv, ev) =>
            {
                string name = tbMedName.Text.Trim();
                if (string.IsNullOrEmpty(name) || name == "Medicine name") { MessageBox.Show("Enter a medicine name."); return; }
                Medicine found = pharmacy.FindMedicine(name);
                if (found == null) { MessageBox.Show($"Medicine '{name}' not found in pharmacy."); return; }
                if (!int.TryParse(tbQty.Text, out int qty) || qty <= 0) { MessageBox.Show("Enter a valid quantity."); return; }
                if (!int.TryParse(tbTimes.Text, out int times) || times <= 0) { MessageBox.Show("Enter valid times/day."); return; }
                if (!int.TryParse(tbDays.Text, out int days) || days <= 0) { MessageBox.Show("Enter valid number of days."); return; }
                prescriptionItems.Add(new PrescriptionItem(found, qty, times, days));
                RefreshGrid();
                // reset fields
                tbMedName.Text = "Medicine name"; tbMedName.ForeColor = TextGrey;
                tbQty.Text = "1"; tbQty.ForeColor = TextGrey;
                tbTimes.Text = "1"; tbTimes.ForeColor = TextGrey;
                tbDays.Text = "1"; tbDays.ForeColor = TextGrey;
            };

            // ─── Clear fields ───
            btnClear.Click += (sv, ev) =>
            {
                tbMedName.Text = "Medicine name"; tbMedName.ForeColor = TextGrey;
                tbQty.Text = "1"; tbQty.ForeColor = TextGrey;
                tbTimes.Text = "1"; tbTimes.ForeColor = TextGrey;
                tbDays.Text = "1"; tbDays.ForeColor = TextGrey;
            };

            // ─── Clear All prescription items ───
            btnClearAll.Click += (sv, ev) =>
            {
                if (prescriptionItems.Count == 0) return;
                if (MessageBox.Show("Clear all items?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    prescriptionItems.Clear();
                    RefreshGrid();
                }
            };

            // ─── Add/Update medicine in Pharmacy ───
            btnAddUpdate.Click += (sv, ev) =>
            {
                string nm = tbPharmMed.Text.Trim();
                if (string.IsNullOrEmpty(nm) || nm == "Medicine name") { MessageBox.Show("Enter a medicine name."); return; }
                Medicine existing = pharmacy.FindMedicine(nm);
                if (existing != null)
                {
                    existing.Quantity += 10;
                    MessageBox.Show($"Updated: {nm} — new qty: {existing.Quantity}", "Updated ✅");
                }
                else
                {
                    pharmacy.AddMedicineToCategory("General Medicine", nm, 50, 50, 2, 10, 2027, 12);
                    MessageBox.Show($"Added '{nm}' to General Medicine.", "Added ✅");
                }
                tbPharmMed.Text = "Medicine name"; tbPharmMed.ForeColor = TextGrey;
            };

            // ─── Save Prescription ───
            btnSave.Click += (sv, ev) =>
            {
                if (prescriptionItems.Count == 0) { MessageBox.Show("No medicines in prescription."); return; }
                double total = 0;
                foreach (var it in prescriptionItems) total += it.TotalPrice();
                MessageBox.Show($"Prescription saved!\n{prescriptionItems.Count} item(s) — Total: {total:F2} EGP", "Saved ✅");
                prescriptionItems.Clear();
                RefreshGrid();
            };

            return page;
        }

    }
}