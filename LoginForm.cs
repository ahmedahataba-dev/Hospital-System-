using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hospital_System
{
    // ── Result passed back to Program.cs after login ──────
    public class LoginResult
    {
        public bool IsEmployee { get; set; }
        public bool IsPatient { get; set; }
        public string Role { get; set; } = "";   // Admin / Doctor / Staff
        public string Username { get; set; } = "";
        public OnlineRegistration PatientAccount { get; set; }
    }

    public class LoginForm : Form
    {
        // ── Services ──────────────────────────────────────
        private readonly EmployeeAccountService _empSvc = new EmployeeAccountService();
        private readonly HospitalService _patSvc = new HospitalService();

        // ── Result ────────────────────────────────────────
        public LoginResult Result { get; private set; }

        // ── Colours / Fonts ───────────────────────────────
        static readonly Color NavBg = Color.FromArgb(10, 25, 47);
        static readonly Color AccentBlue = Color.FromArgb(21, 101, 192);
        static readonly Color AccentGrn = Color.FromArgb(27, 152, 79);
        static readonly Color AccentRed = Color.FromArgb(198, 40, 40);
        static readonly Color TextDark = Color.FromArgb(20, 30, 48);
        static readonly Color TextGrey = Color.FromArgb(96, 108, 129);
        static readonly Font FTitle = new Font("Segoe UI", 18, FontStyle.Bold);
        static readonly Font FBold = new Font("Segoe UI", 9, FontStyle.Bold);
        static readonly Font FNorm = new Font("Segoe UI", 9, FontStyle.Regular);

        // ── Controls we need to reference ────────────────
        private Panel _empPanel, _patPanel;
        private Button _btnEmpTab, _btnPatTab;

        public LoginForm()
        {
            _empSvc.LoadData();
            _patSvc.LoadData();
            Build();
        }

        private void Build()
        {
            Text = "NeurAi Medical Center — Login";
            Size = new Size(560, 680);
            MinimumSize = new Size(560, 680);
            MaximumSize = new Size(560, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(236, 240, 245);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Top banner ────────────────────────────────
            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = NavBg
            };

            // Logo image (replaces the 🏥 emoji)
            var logoPic = new PictureBox
            {
                Image = Properties.Resources.NeurAI,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(90, 70),
                Location = new Point(12, 10)
            };
            banner.Controls.Add(logoPic);

            banner.Controls.Add(new Label
            {
                Text = "NeurAi Medical Center",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 188, 212),
                AutoSize = true,
                Location = new Point(110, 14)
            });
            banner.Controls.Add(new Label
            {
                Text = "Hospital Management System",
                Font = FNorm,
                ForeColor = Color.FromArgb(160, 180, 210),
                AutoSize = true,
                Location = new Point(112, 54)
            });
            Controls.Add(banner);

            // ── Tab selector ─────────────────────────────
            var tabBar = new Panel
            {
                Location = new Point(0, 90),
                Size = new Size(560, 46),
                BackColor = Color.FromArgb(220, 228, 240)
            };
            Controls.Add(tabBar);

            _btnEmpTab = MakeTabBtn("👨‍💼  Employee Login", 0);
            _btnPatTab = MakeTabBtn("🧑‍🤝‍🧑  Patient Portal", 280);
            tabBar.Controls.Add(_btnEmpTab);
            tabBar.Controls.Add(_btnPatTab);

            // ── Employee panel ────────────────────────────
            _empPanel = BuildEmployeePanel();
            _empPanel.Location = new Point(0, 136);
            Controls.Add(_empPanel);

            // ── Patient panel ─────────────────────────────
            _patPanel = BuildPatientPanel();
            _patPanel.Location = new Point(0, 136);
            _patPanel.Visible = false;
            Controls.Add(_patPanel);

            _btnEmpTab.Click += (s, e) => SwitchTab(true);
            _btnPatTab.Click += (s, e) => SwitchTab(false);
            SwitchTab(true);
        }

        Button MakeTabBtn(string text, int x)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(280, 44),
                Location = new Point(x, 1),
                FlatStyle = FlatStyle.Flat,
                Font = FBold,
                BackColor = Color.FromArgb(200, 210, 230),
                ForeColor = TextDark
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        void SwitchTab(bool isEmployee)
        {
            _empPanel.Visible = isEmployee;
            _patPanel.Visible = !isEmployee;
            _btnEmpTab.BackColor = isEmployee ? AccentBlue : Color.FromArgb(200, 210, 230);
            _btnEmpTab.ForeColor = isEmployee ? Color.White : TextDark;
            _btnPatTab.BackColor = !isEmployee ? AccentBlue : Color.FromArgb(200, 210, 230);
            _btnPatTab.ForeColor = !isEmployee ? Color.White : TextDark;
        }

        // ─────────────────────────────────────────────────
        //  EMPLOYEE PANEL
        // ─────────────────────────────────────────────────
        Panel BuildEmployeePanel()
        {
            var p = new Panel
            {
                Size = new Size(560, 520),
                BackColor = Color.FromArgb(236, 240, 245)
            };

            var card = new Panel
            {
                Location = new Point(80, 20),
                Size = new Size(400, 360),
                BackColor = Color.White
            };
            card.Paint += (s, e) =>
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(Color.FromArgb(220, 224, 230)),
                    0, 0, card.Width - 1, card.Height - 1);

            card.Controls.Add(new Label
            {
                Text = "Employee Sign In",
                Font = FTitle,
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(20, 20)
            });

            // Default credentials hint (hidden — remove before production)
            var hint = new Label
            {
                Text = "Admin: youssef / 21042007Y\nStaff: ahmed | haneen | rahma | salma  /  12345",
                Font = new Font("Segoe UI", 8),
                ForeColor = TextGrey,
                Location = new Point(20, 52),
                Size = new Size(360, 34),
                Visible = false   // ← set to true during development only
            };
            card.Controls.Add(hint);

            card.Controls.Add(Lbl("Username", 20, 90));
            var tbUser = TB(20, 110, 360); card.Controls.Add(tbUser);

            card.Controls.Add(Lbl("Password", 20, 148));
            var tbPass = TB(20, 168, 360, true); card.Controls.Add(tbPass);

            var msgLbl = new Label
            {
                Text = "",
                Font = FBold,
                AutoSize = true,
                Location = new Point(20, 208)
            };
            card.Controls.Add(msgLbl);

            var btnLogin = MakeBtn("Sign In", AccentBlue, 20, 234, 160, 38);
            card.Controls.Add(btnLogin);

            btnLogin.Click += (s, e) =>
            {
                var acc = _empSvc.Login(tbUser.Text.Trim(), tbPass.Text);
                if (acc == null)
                {
                    msgLbl.Text = "❌  Invalid username or password.";
                    msgLbl.ForeColor = AccentRed;
                    return;
                }
                Result = new LoginResult
                {
                    IsEmployee = true,
                    Role = acc.Role,
                    Username = acc.Username
                };
                DialogResult = DialogResult.OK;
                Close();
            };

            // Allow Enter key
            tbPass.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnLogin.PerformClick();
            };

            p.Controls.Add(card);

            // ── Info box ──────────────────────────────────
            var infoBox = new Panel
            {
                Location = new Point(80, 392),
                Size = new Size(400, 80),
                BackColor = Color.FromArgb(232, 244, 255)
            };
            infoBox.Controls.Add(new Label
            {
                Text = "ℹ️  Employee accounts are managed by your system administrator.\n" +
                            "Contact admin to reset your password.",
                Font = FNorm,
                ForeColor = TextGrey,
                Location = new Point(12, 10),
                Size = new Size(376, 58)
            });
            p.Controls.Add(infoBox);

            return p;
        }

        // ─────────────────────────────────────────────────
        //  PATIENT PANEL  (register + login tabs)
        // ─────────────────────────────────────────────────
        Panel BuildPatientPanel()
        {
            var p = new Panel
            {
                Size = new Size(560, 520),
                BackColor = Color.FromArgb(236, 240, 245)
            };

            // inner sub-tabs
            var subBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(560, 38),
                BackColor = Color.FromArgb(210, 220, 235)
            };
            p.Controls.Add(subBar);

            Panel loginCard = BuildPatLoginCard();
            Panel registerCard = BuildPatRegCard();
            loginCard.Location = new Point(0, 42);
            registerCard.Location = new Point(0, 42);
            registerCard.Visible = false;
            p.Controls.Add(loginCard);
            p.Controls.Add(registerCard);

            Button bLogin = MakeTabBtn2("Login", 0);
            Button bReg = MakeTabBtn2("Register", 186);
            subBar.Controls.Add(bLogin);
            subBar.Controls.Add(bReg);

            bLogin.Click += (s, e) =>
            {
                loginCard.Visible = true;
                registerCard.Visible = false;
                bLogin.BackColor = AccentGrn;
                bLogin.ForeColor = Color.White;
                bReg.BackColor = Color.FromArgb(190, 205, 225);
                bReg.ForeColor = TextDark;
            };
            bReg.Click += (s, e) =>
            {
                loginCard.Visible = false;
                registerCard.Visible = true;
                bReg.BackColor = AccentGrn;
                bReg.ForeColor = Color.White;
                bLogin.BackColor = Color.FromArgb(190, 205, 225);
                bLogin.ForeColor = TextDark;
            };

            bLogin.BackColor = AccentGrn;
            bLogin.ForeColor = Color.White;

            return p;
        }

        Button MakeTabBtn2(string text, int x)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(184, 36),
                Location = new Point(x, 1),
                FlatStyle = FlatStyle.Flat,
                Font = FBold,
                BackColor = Color.FromArgb(190, 205, 225),
                ForeColor = TextDark
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        Panel BuildPatLoginCard()
        {
            var card = MakeCard(80, 0, 400, 360);

            card.Controls.Add(new Label
            {
                Text = "Patient Login",
                Font = FTitle,
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(20, 18)
            });

            card.Controls.Add(Lbl("National ID (14 digits)", 20, 68));
            var tbId = TB(20, 88, 360); card.Controls.Add(tbId);

            card.Controls.Add(Lbl("Password", 20, 126));
            var tbPass = TB(20, 146, 360, true); card.Controls.Add(tbPass);

            var msgLbl = new Label
            {
                Text = "",
                Font = FBold,
                AutoSize = true,
                Location = new Point(20, 184)
            };
            card.Controls.Add(msgLbl);

            var btn = MakeBtn("Login", AccentGrn, 20, 210, 150, 38);
            card.Controls.Add(btn);

            btn.Click += (s, e) =>
            {
                var pat = _patSvc.FindPatient(tbId.Text.Trim(), tbPass.Text);
                if (pat == null)
                {
                    msgLbl.Text = "❌  Invalid ID or password.";
                    msgLbl.ForeColor = AccentRed;
                    return;
                }
                Result = new LoginResult
                {
                    IsPatient = true,
                    PatientAccount = pat,
                    Username = pat.NameEnglish
                };
                DialogResult = DialogResult.OK;
                Close();
            };

            tbPass.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btn.PerformClick();
            };

            var p = new Panel { Size = new Size(560, 460), BackColor = Color.FromArgb(236, 240, 245) };
            p.Controls.Add(card);
            return p;
        }

        Panel BuildPatRegCard()
        {
            var outer = new Panel
            {
                Size = new Size(560, 460),
                BackColor = Color.FromArgb(236, 240, 245)
            };

            var card = MakeCard(40, 0, 480, 440);

            card.Controls.Add(new Label
            {
                Text = "Create Patient Account",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(16, 14)
            });

            string[] labels = { "Full Name", "National ID (14 digits)", "Phone (11 digits)", "Address" };
            TextBox[] boxes = new TextBox[labels.Length];
            for (int i = 0; i < labels.Length; i++)
            {
                card.Controls.Add(Lbl(labels[i], 16, 60 + i * 48));
                boxes[i] = TB(16, 78 + i * 48, 448);
                card.Controls.Add(boxes[i]);
            }

            int oy = 60 + labels.Length * 48;
            card.Controls.Add(Lbl("Password (min 8 chars, include number)", 16, oy));
            var tbPass = TB(16, oy + 20, 448, true); card.Controls.Add(tbPass);

            card.Controls.Add(Lbl("Confirm Password", 16, oy + 56));
            var tbConf = TB(16, oy + 76, 448, true); card.Controls.Add(tbConf);

            var msgLbl = new Label { Text = "", Font = FBold, AutoSize = true, Location = new Point(16, oy + 112) };
            card.Controls.Add(msgLbl);

            var btn = MakeBtn("Create Account", AccentBlue, 16, oy + 136, 180, 36);
            card.Controls.Add(btn);

            btn.Click += (s, e) =>
            {
                string err = _patSvc.CheckSignUp(boxes[0].Text, boxes[1].Text, boxes[2].Text, boxes[3].Text);
                if (err != "") { msgLbl.Text = "❌ " + err; msgLbl.ForeColor = AccentRed; return; }

                var newP = new OnlineRegistration
                {
                    NameEnglish = boxes[0].Text.Trim(),
                    NationalID = boxes[1].Text.Trim(),
                    Phone = boxes[2].Text.Trim(),
                    Address = boxes[3].Text.Trim()
                };
                string perr = _patSvc.SetPassword(newP, tbPass.Text, tbConf.Text);
                if (perr != "") { msgLbl.Text = "❌ " + perr; msgLbl.ForeColor = AccentRed; return; }

                msgLbl.Text = "✅ Account created! Switch to Login tab.";
                msgLbl.ForeColor = AccentGrn;
                foreach (var b2 in boxes) b2.Clear();
                tbPass.Clear(); tbConf.Clear();
            };

            outer.Controls.Add(card);
            return outer;
        }

        // ── Helpers ───────────────────────────────────────
        Panel MakeCard(int x, int y, int w, int h)
        {
            var c = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.White };
            c.Paint += (s, e) =>
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(Color.FromArgb(220, 224, 230)),
                    0, 0, c.Width - 1, c.Height - 1);
            return c;
        }

        Label Lbl(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = FBold,
                ForeColor = TextGrey,
                AutoSize = true,
                Location = new Point(x, y)
            };
        }

        TextBox TB(int x, int y, int w, bool password = false)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 26),
                Font = FNorm,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = password
            };
        }

        Button MakeBtn(string text, Color bg, int x, int y, int w, int h)
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
            return b;
        }
    }
}