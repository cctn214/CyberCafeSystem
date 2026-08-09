using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCDomain.Model.UserModels;
using CCDomain.Model.UnitModels;
using CCDomain.Model.RentModels;
using CCWindowForm.Services;

namespace CCWindowForm
{
    public enum ActiveTab
    {
        User,
        Unit,
        Rent
    }

    public partial class Form1 : Form
    {
        // Service Layer
        private readonly UserApiService _userApiService;
        private readonly UnitApiService _unitApiService;
        private readonly RentApiService _rentApiService;

        // UI State
        private ActiveTab _activeTab = ActiveTab.User;
        private bool _editMode = false;
        private int? _selectedRecordId = null;

        // Layout Containers
        private Panel pnlTitle;
        private SplitContainer splitMain;
        private Panel pnlLeftContainer;
        private Panel pnlRightContainer;
        private Panel pnlTabButtons;
        private Panel pnlFormContainer;
        private Panel pnlLeftActions;

        // Tab Buttons
        private Button btnTabUser;
        private Button btnTabUnit;
        private Button btnTabRent;

        // Form Panels
        private Panel pnlUserForm;
        private Panel pnlUnitForm;
        private Panel pnlRentForm;

        // User Inputs
        private TextBox txtUserUsername;
        private TextBox txtUserPassword;
        private TextBox txtUserName;
        private NumericUpDown numUserBalance;

        // Unit Inputs
        private TextBox txtUnitType;
        private NumericUpDown numUnitRate;
        private CheckBox chkUnitActive;

        // Rent Inputs
        private ComboBox cboRentUser;
        private ComboBox cboRentUnit;
        private TextBox txtRentStartHour;
        private TextBox txtRentStartMin;
        private TextBox txtRentEndHour;
        private TextBox txtRentEndMin;

        // CRUD Buttons
        private Button btnSave;
        private Button btnChange;
        private Button btnClear;

        // Search & Row actions (Right Top)
        private TextBox txtSearchId;
        private Button btnSearch;
        private Button btnClearSearch;
        private Button btnEdit;
        private Button btnDelete;

        // Grid & Status (Right Center/Bottom)
        private DataGridView dgvData;
        private Label lblStatus;

        public Form1()
        {
            InitializeComponent();

            // Instantiate services
            _userApiService = new UserApiService();
            _unitApiService = new UnitApiService();
            _rentApiService = new RentApiService();

            // Initialize custom cyberpunk design layouts
            InitializeCustomLayout();

            // Load initial tab data (User)
            SwitchTab(ActiveTab.User);
        }

        private void InitializeCustomLayout()
        {
            // Styling Config
            Color bgColor = Color.FromArgb(10, 10, 14); // Dark Charcoal/Black with subtle neon tint
            Color panelColor = Color.FromArgb(20, 20, 26); // Dark Gray Panel
            Color accentPrimary = Color.FromArgb(0, 191, 255); // Electric Blue/Cyan
            Color accentSecondary = Color.FromArgb(186, 85, 211); // Purple/Magenta
            Color textColor = Color.FromArgb(240, 240, 240); // Off-White
            Font fontRegular = new Font("Segoe UI", 10F, FontStyle.Regular);
            Font fontBold = new Font("Segoe UI", 10F, FontStyle.Bold);
            Font fontIcon = new Font("Segoe UI", 12F, FontStyle.Bold);

            this.BackColor = bgColor;
            this.ForeColor = textColor;

            // 1. Title Panel
            pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(16, 16, 20),
                Padding = new Padding(10)
            };
            Label lblMainTitle = new Label
            {
                Text = "CYPHER CAFE SYSTEM",
                Font = new Font("Impact", 20F, FontStyle.Italic),
                ForeColor = accentPrimary,
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblMainTitle);

            // Subtle glowing bottom border for Title Panel
            Panel pnlGlowingBorder = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 3,
                BackColor = accentPrimary
            };
            pnlTitle.Controls.Add(pnlGlowingBorder);
            this.Controls.Add(pnlTitle);

            // 2. Main SplitContainer
            splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 5,
                BackColor = bgColor
            };
            this.Controls.Add(splitMain);

            // Crucial layout fix: splitMain must be index 0 so it docks last,
            // filling the remaining area below the top-docked title panel.
            splitMain.BringToFront();

            // Set initial 3/8 proportion and center title text
            splitMain.SplitterDistance = (int)(this.ClientSize.Width * 3.0 / 8.0);
            lblMainTitle.Location = new Point((pnlTitle.Width - lblMainTitle.PreferredWidth) / 2, 12);

            // Dynamically center header and maintain 3/8 proportion on resize
            this.Resize += (s, e) =>
            {
                if (pnlTitle.Width > 0 && lblMainTitle.Width > 0)
                {
                    lblMainTitle.Location = new Point((pnlTitle.Width - lblMainTitle.Width) / 2, 12);
                }
                if (this.ClientSize.Width > 0)
                {
                    splitMain.SplitterDistance = (int)(this.ClientSize.Width * 3.0 / 8.0);
                }
            };

            // Left & Right Panels
            pnlLeftContainer = splitMain.Panel1;
            pnlLeftContainer.BackColor = bgColor;
            pnlLeftContainer.Padding = new Padding(15);

            pnlRightContainer = splitMain.Panel2;
            pnlRightContainer.BackColor = bgColor;
            pnlRightContainer.Padding = new Padding(15);

            // ================== LEFT SIDE (CRUD AREA) ==================

            // 3. Tab Buttons Panel
            pnlTabButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(0, 0, 0, 5)
            };
            pnlLeftContainer.Controls.Add(pnlTabButtons);

            btnTabUser = CreateFlatButton("USER", panelColor, accentPrimary, fontBold);
            btnTabUser.Width = 120;
            btnTabUser.Dock = DockStyle.Left;
            btnTabUser.Click += (s, e) => SwitchTab(ActiveTab.User);

            btnTabUnit = CreateFlatButton("UNIT", panelColor, accentPrimary, fontBold);
            btnTabUnit.Width = 120;
            btnTabUnit.Dock = DockStyle.Left;
            btnTabUnit.Click += (s, e) => SwitchTab(ActiveTab.Unit);

            btnTabRent = CreateFlatButton("RENT", panelColor, accentPrimary, fontBold);
            btnTabRent.Width = 120;
            btnTabRent.Dock = DockStyle.Left;
            btnTabRent.Click += (s, e) => SwitchTab(ActiveTab.Rent);

            pnlTabButtons.Controls.Add(btnTabRent);
            pnlTabButtons.Controls.Add(btnTabUnit);
            pnlTabButtons.Controls.Add(btnTabUser);

            // 4. Form Container
            pnlFormContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = panelColor,
                Padding = new Padding(15)
            };
            pnlLeftContainer.Controls.Add(pnlFormContainer);

            // Left Actions panel (Save / Change / Clear)
            pnlLeftActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                Padding = new Padding(0, 10, 0, 0)
            };
            pnlLeftContainer.Controls.Add(pnlLeftActions);

            btnSave = CreateFlatButton("SAVE", Color.FromArgb(0, 120, 215), Color.White, fontBold);
            btnSave.Width = 120;
            btnSave.Location = new Point(0, 15);
            btnSave.Click += async (s, e) => await SaveRecordAsync();

            btnChange = CreateFlatButton("CHANGE", Color.FromArgb(120, 0, 120), Color.White, fontBold);
            btnChange.Width = 120;
            btnChange.Location = new Point(130, 15);
            btnChange.Enabled = false;
            btnChange.Click += async (s, e) => await ChangeRecordAsync();

            btnClear = CreateFlatButton("CLEAR", Color.FromArgb(50, 50, 50), Color.White, fontBold);
            btnClear.Width = 100;
            btnClear.Location = new Point(260, 15);
            btnClear.Click += (s, e) => ClearInputs();

            pnlLeftActions.Controls.Add(btnSave);
            pnlLeftActions.Controls.Add(btnChange);
            pnlLeftActions.Controls.Add(btnClear);

            // Make sure the Dock = Fill container panel is ordered properly
            pnlFormContainer.BringToFront();

            // Initialize Form Panels
            InitializeUserForm(panelColor, textColor, fontRegular);
            InitializeUnitForm(panelColor, textColor, fontRegular);
            InitializeRentForm(panelColor, textColor, fontRegular);

            pnlFormContainer.Controls.Add(pnlUserForm);
            pnlFormContainer.Controls.Add(pnlUnitForm);
            pnlFormContainer.Controls.Add(pnlRentForm);

            // ================== RIGHT SIDE (GRID & SEARCH AREA) ==================

            // Top Panel for Search and Edit/Delete Actions
            Panel pnlSearchAndActions = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 0, 0, 10)
            };
            pnlRightContainer.Controls.Add(pnlSearchAndActions);

            Label lblSearch = new Label
            {
                Text = "Search :",
                Font = fontBold,
                ForeColor = textColor,
                Location = new Point(15, 20),
                AutoSize = true
            };
            pnlSearchAndActions.Controls.Add(lblSearch);

            txtSearchId = new TextBox
            {
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                Location = new Point(95, 12),
                Width = 100
            };
            pnlSearchAndActions.Controls.Add(txtSearchId);

            // Square, logo-only search and reset buttons aligned vertically at Y = 12
            btnSearch = CreateFlatButton("🔍", Color.FromArgb(30, 30, 40), accentPrimary, fontIcon);
            btnSearch.Location = new Point(205, 12);
            btnSearch.Width = 35;
            btnSearch.Height = 35;
            btnSearch.Click += async (s, e) => await SearchByIdAsync();
            pnlSearchAndActions.Controls.Add(btnSearch);

            btnClearSearch = CreateFlatButton("↻", Color.FromArgb(30, 30, 40), Color.DarkGray, fontIcon);
            btnClearSearch.Location = new Point(245, 12);
            btnClearSearch.Width = 35;
            btnClearSearch.Height = 35;
            btnClearSearch.Click += async (s, e) => await ClearSearchAsync();
            pnlSearchAndActions.Controls.Add(btnClearSearch);

            // Right side buttons (Edit & Delete - anchored to right)
            btnDelete = CreateFlatButton("🗑️", Color.FromArgb(30, 30, 40), Color.Red, fontIcon);
            btnDelete.Location = new Point(pnlSearchAndActions.Width - 45, 12);
            btnDelete.Width = 35;
            btnDelete.Height = 35;
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Enabled = false;
            btnDelete.Click += async (s, e) => await DeleteSelectedRecordAsync();
            pnlSearchAndActions.Controls.Add(btnDelete);

            btnEdit = CreateFlatButton("✏️", Color.FromArgb(30, 30, 40), accentPrimary, fontIcon);
            btnEdit.Location = new Point(pnlSearchAndActions.Width - 90, 12);
            btnEdit.Width = 35;
            btnEdit.Height = 35;
            btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEdit.Enabled = false;
            btnEdit.Click += (s, e) => EnterEditModeFromSelection();
            pnlSearchAndActions.Controls.Add(btnEdit);

            // DataGridView Setup
            dgvData = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = panelColor,
                GridColor = Color.FromArgb(40, 40, 50),
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvData.DefaultCellStyle.BackColor = Color.FromArgb(24, 24, 30);
            dgvData.DefaultCellStyle.ForeColor = textColor;
            dgvData.DefaultCellStyle.SelectionBackColor = accentPrimary;
            dgvData.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvData.DefaultCellStyle.Font = fontRegular;

            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 40);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = accentPrimary;
            dgvData.ColumnHeadersDefaultCellStyle.Font = fontBold;
            dgvData.ColumnHeadersHeight = 35;

            dgvData.SelectionChanged += DgvData_SelectionChanged;

            pnlRightContainer.Controls.Add(dgvData);

            // Status Label
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 25,
                Text = "System initialized.",
                Font = fontRegular,
                ForeColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlRightContainer.Controls.Add(lblStatus);

            // Bring grid to front to make sure it fills correctly between search header and status bar
            dgvData.BringToFront();
        }

        private void InitializeUserForm(Color panelBg, Color textClr, Font font)
        {
            pnlUserForm = new Panel { Dock = DockStyle.Fill, BackColor = panelBg, Visible = true };

            int yOffset = 10;
            AddFormLabelAndControl(pnlUserForm, "Username:", txtUserUsername = CreateFormTextBox(font), ref yOffset);
            AddFormLabelAndControl(pnlUserForm, "Password:", txtUserPassword = CreateFormTextBox(font, true), ref yOffset);
            AddFormLabelAndControl(pnlUserForm, "Full Name:", txtUserName = CreateFormTextBox(font), ref yOffset);

            numUserBalance = new NumericUpDown
            {
                Font = font,
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = textClr,
                Maximum = 999999,
                DecimalPlaces = 2,
                Width = 200
            };
            AddFormLabelAndControl(pnlUserForm, "Balance ($):", numUserBalance, ref yOffset);
        }

        private void InitializeUnitForm(Color panelBg, Color textClr, Font font)
        {
            pnlUnitForm = new Panel { Dock = DockStyle.Fill, BackColor = panelBg, Visible = false };

            int yOffset = 10;
            AddFormLabelAndControl(pnlUnitForm, "Unit Type (e.g. PC, VIP):", txtUnitType = CreateFormTextBox(font), ref yOffset);

            numUnitRate = new NumericUpDown
            {
                Font = font,
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = textClr,
                Maximum = 1000,
                DecimalPlaces = 2,
                Width = 200
            };
            AddFormLabelAndControl(pnlUnitForm, "Hourly Rate ($/hr):", numUnitRate, ref yOffset);

            chkUnitActive = new CheckBox
            {
                Text = "Is Unit Active",
                Checked = true,
                Font = font,
                ForeColor = textClr,
                AutoSize = true
            };
            AddFormLabelAndControl(pnlUnitForm, "Status:", chkUnitActive, ref yOffset);
        }

        private void InitializeRentForm(Color panelBg, Color textClr, Font font)
        {
            pnlRentForm = new Panel { Dock = DockStyle.Fill, BackColor = panelBg, Visible = false };

            int yOffset = 10;

            cboRentUser = CreateFormComboBox(font);
            AddFormLabelAndControl(pnlRentForm, "Rentee (User):", cboRentUser, ref yOffset);

            cboRentUnit = CreateFormComboBox(font);
            AddFormLabelAndControl(pnlRentForm, "Select Unit:", cboRentUnit, ref yOffset);

            // Start Time: 2 small text boxes (Hour : Minute)
            FlowLayoutPanel pnlStartFlow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Width = 230,
                Height = 35,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            txtRentStartHour = CreateTimeTextBox(font);
            Label lblColonStart = new Label { Text = ":", ForeColor = textClr, Font = font, AutoSize = true, Margin = new Padding(3, 8, 3, 0) };
            txtRentStartMin = CreateTimeTextBox(font);
            pnlStartFlow.Controls.Add(txtRentStartHour);
            pnlStartFlow.Controls.Add(lblColonStart);
            pnlStartFlow.Controls.Add(txtRentStartMin);
            AddFormLabelAndControl(pnlRentForm, "Start Time (HH : MM):", pnlStartFlow, ref yOffset);

            // End Time: 2 small text boxes (Hour : Minute)
            FlowLayoutPanel pnlEndFlow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Width = 230,
                Height = 35,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            txtRentEndHour = CreateTimeTextBox(font);
            Label lblColonEnd = new Label { Text = ":", ForeColor = textClr, Font = font, AutoSize = true, Margin = new Padding(3, 8, 3, 0) };
            txtRentEndMin = CreateTimeTextBox(font);
            pnlEndFlow.Controls.Add(txtRentEndHour);
            pnlEndFlow.Controls.Add(lblColonEnd);
            pnlEndFlow.Controls.Add(txtRentEndMin);
            AddFormLabelAndControl(pnlRentForm, "End Time (HH : MM, optional):", pnlEndFlow, ref yOffset);

            // Register time validation logic
            RegisterTimeValidation(txtRentStartHour, txtRentStartMin, false);
            RegisterTimeValidation(txtRentEndHour, txtRentEndMin, true);
        }

        // Helper Layout methods
        private Button CreateFlatButton(string text, Color bg, Color accent, Font font)
        {
            Button btn = new Button
            {
                Text = text,
                BackColor = bg,
                ForeColor = accent,
                FlatStyle = FlatStyle.Flat,
                Font = font,
                Height = 35,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = accent;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(bg.R + 20, bg.G + 20, bg.B + 20);
            return btn;
        }

        private TextBox CreateFormTextBox(Font font, bool isPassword = false)
        {
            return new TextBox
            {
                Font = font,
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 230,
                UseSystemPasswordChar = isPassword
            };
        }

        private TextBox CreateTimeTextBox(Font font)
        {
            return new TextBox
            {
                Font = font,
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 45,
                Height = 30,
                TextAlign = HorizontalAlignment.Center,
                MaxLength = 2
            };
        }

        private ComboBox CreateFormComboBox(Font font)
        {
            return new ComboBox
            {
                Font = font,
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
        }

        private void AddFormLabelAndControl(Panel pnl, string labelText, Control ctrl, ref int yOffset)
        {
            Label lbl = new Label
            {
                Text = labelText,
                ForeColor = Color.FromArgb(170, 170, 180),
                Location = new Point(10, yOffset),
                AutoSize = true
            };
            ctrl.Location = new Point(10, yOffset + 22);

            pnl.Controls.Add(lbl);
            pnl.Controls.Add(ctrl);

            yOffset += 60;
        }

        // Time Validation Handler
        private void RegisterTimeValidation(TextBox txtHour, TextBox txtMin, bool isOptional)
        {
            txtHour.KeyPress += (s, e) =>
            {
                // Accept only digits and backspace/control chars
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };

            txtMin.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };

            txtHour.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtHour.Text))
                {
                    if (!isOptional) txtHour.Text = "00";
                    return;
                }
                if (int.TryParse(txtHour.Text, out int val))
                {
                    if (val > 23) txtHour.Text = "23";
                    else if (val < 0) txtHour.Text = "00";
                    else txtHour.Text = val.ToString("D2");
                }
                else
                {
                    txtHour.Text = isOptional ? "" : "00";
                }
            };

            txtMin.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtMin.Text))
                {
                    if (!isOptional) txtMin.Text = "00";
                    return;
                }
                if (int.TryParse(txtMin.Text, out int val))
                {
                    if (val > 59) txtMin.Text = "59";
                    else if (val < 0) txtMin.Text = "00";
                    else txtMin.Text = val.ToString("D2");
                }
                else
                {
                    txtMin.Text = isOptional ? "" : "00";
                }
            };
        }

        // ================== DATA HANDLING AND STATE ==================

        private async void SwitchTab(ActiveTab tab)
        {
            _activeTab = tab;
            ClearInputs();

            // Set Form Panels visibility
            pnlUserForm.Visible = (tab == ActiveTab.User);
            pnlUnitForm.Visible = (tab == ActiveTab.Unit);
            pnlRentForm.Visible = (tab == ActiveTab.Rent);

            // Style buttons
            Color activeBg = Color.FromArgb(0, 191, 255);
            Color activeText = Color.Black;
            Color inactiveBg = Color.FromArgb(20, 20, 26);
            Color inactiveText = Color.FromArgb(0, 191, 255);

            btnTabUser.BackColor = (tab == ActiveTab.User) ? activeBg : inactiveBg;
            btnTabUser.ForeColor = (tab == ActiveTab.User) ? activeText : inactiveText;

            btnTabUnit.BackColor = (tab == ActiveTab.Unit) ? activeBg : inactiveBg;
            btnTabUnit.ForeColor = (tab == ActiveTab.Unit) ? activeText : inactiveText;

            btnTabRent.BackColor = (tab == ActiveTab.Rent) ? activeBg : inactiveBg;
            btnTabRent.ForeColor = (tab == ActiveTab.Rent) ? activeText : inactiveText;

            // Load data
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            SetStatus("Loading data...");
            try
            {
                if (_activeTab == ActiveTab.User)
                {
                    var res = await _userApiService.GetAllUsersAsync();
                    if (res != null && res.IsSuccess)
                    {
                        dgvData.DataSource = res.Users;
                        FormatUserGrid();
                    }
                    else
                    {
                        SetStatus($"Failed: {res?.Message}");
                    }
                }
                else if (_activeTab == ActiveTab.Unit)
                {
                    var res = await _unitApiService.GetAllUnitsAsync();
                    if (res != null && res.IsSuccess)
                    {
                        dgvData.DataSource = res.Units;
                        FormatUnitGrid();
                    }
                    else
                    {
                        SetStatus($"Failed: {res?.Message}");
                    }
                }
                else if (_activeTab == ActiveTab.Rent)
                {
                    // Pre-fetch Users and Units for dropdowns
                    var usersTask = _userApiService.GetAllUsersAsync();
                    var unitsTask = _unitApiService.GetAllUnitsAsync();
                    var rentsTask = _rentApiService.GetAllRentsAsync();

                    await Task.WhenAll(usersTask, unitsTask, rentsTask);

                    var usersRes = usersTask.Result;
                    var unitsRes = unitsTask.Result;
                    var rentsRes = rentsTask.Result;

                    if (usersRes != null && usersRes.IsSuccess)
                    {
                        cboRentUser.DataSource = usersRes.Users.Select(u => new
                        {
                            UserId = u.UserId,
                            DisplayName = $"{u.Username} - {u.Name}"
                        }).ToList();
                        cboRentUser.ValueMember = "UserId";
                        cboRentUser.DisplayMember = "DisplayName";
                    }

                    if (unitsRes != null && unitsRes.IsSuccess)
                    {
                        cboRentUnit.DataSource = unitsRes.Units.Select(u => new
                        {
                            UnitId = u.UnitId,
                            DisplayName = $"Unit {u.UnitId} / {u.Type}"
                        }).ToList();
                        cboRentUnit.ValueMember = "UnitId";
                        cboRentUnit.DisplayMember = "DisplayName";
                    }

                    if (rentsRes != null && rentsRes.IsSuccess)
                    {
                        dgvData.DataSource = rentsRes.Rents;
                        FormatRentGrid();
                    }
                    else
                    {
                        SetStatus($"Failed: {rentsRes?.Message}");
                    }
                }
                SetStatus("Ready");
            }
            catch (Exception ex)
            {
                SetStatus("Unable to connect to the server.");
                MessageBox.Show($"Server connection error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Formatting Grid Columns
        private void FormatUserGrid()
        {
            if (dgvData.Columns["UserId"] != null) dgvData.Columns["UserId"].HeaderText = "User ID";
            if (dgvData.Columns["Username"] != null) dgvData.Columns["Username"].HeaderText = "Username";
            if (dgvData.Columns["Name"] != null) dgvData.Columns["Name"].HeaderText = "Full Name";
            if (dgvData.Columns["Balance"] != null)
            {
                dgvData.Columns["Balance"].HeaderText = "Balance ($)";
                dgvData.Columns["Balance"].DefaultCellStyle.Format = "C2";
            }
        }

        private void FormatUnitGrid()
        {
            if (dgvData.Columns["UnitId"] != null) dgvData.Columns["UnitId"].HeaderText = "Unit ID";
            if (dgvData.Columns["Type"] != null) dgvData.Columns["Type"].HeaderText = "Type";
            if (dgvData.Columns["Rate"] != null)
            {
                dgvData.Columns["Rate"].HeaderText = "Rate ($/hr)";
                dgvData.Columns["Rate"].DefaultCellStyle.Format = "C2";
            }
            if (dgvData.Columns["IsActive"] != null) dgvData.Columns["IsActive"].HeaderText = "Active";
        }

        private void FormatRentGrid()
        {
            if (dgvData.Columns["RentId"] != null) dgvData.Columns["RentId"].HeaderText = "Rent ID";
            if (dgvData.Columns["UserId"] != null) dgvData.Columns["UserId"].HeaderText = "User ID";
            if (dgvData.Columns["UnitId"] != null) dgvData.Columns["UnitId"].HeaderText = "Unit ID";
            if (dgvData.Columns["StartTime"] != null) dgvData.Columns["StartTime"].HeaderText = "Start Time";
            if (dgvData.Columns["EndTime"] != null) dgvData.Columns["EndTime"].HeaderText = "End Time";
            if (dgvData.Columns["Duration"] != null) dgvData.Columns["Duration"].HeaderText = "Duration (hrs)";
            if (dgvData.Columns["TotalCost"] != null)
            {
                dgvData.Columns["TotalCost"].HeaderText = "Total Cost ($)";
                dgvData.Columns["TotalCost"].DefaultCellStyle.Format = "C2";
            }
        }

        private void DgvData_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvData.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        // ================== CRUD OPERATIONS ==================

        private async Task SaveRecordAsync()
        {
            SetStatus("Saving...");
            if (_activeTab == ActiveTab.User)
            {
                if (string.IsNullOrWhiteSpace(txtUserUsername.Text) || string.IsNullOrWhiteSpace(txtUserPassword.Text) || string.IsNullOrWhiteSpace(txtUserName.Text))
                {
                    MessageBox.Show("Please fill in Username, Password, and Full Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStatus("Ready");
                    return;
                }

                var request = new UserCreateRequestModel
                {
                    Username = txtUserUsername.Text.Trim(),
                    Password = txtUserPassword.Text,
                    Name = txtUserName.Text.Trim(),
                    Balance = numUserBalance.Value
                };

                var res = await _userApiService.CreateUserAsync(request);
                if (res != null && res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "Failed to save user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (_activeTab == ActiveTab.Unit)
            {
                if (string.IsNullOrWhiteSpace(txtUnitType.Text))
                {
                    MessageBox.Show("Please specify the Unit Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStatus("Ready");
                    return;
                }

                var request = new UnitCreateRequestModel
                {
                    Type = txtUnitType.Text.Trim(),
                    Rate = numUnitRate.Value,
                    IsActive = chkUnitActive.Checked
                };

                var res = await _unitApiService.CreateUnitAsync(request);
                if (res != null && res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "Failed to save unit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (_activeTab == ActiveTab.Rent)
            {
                if (cboRentUser.SelectedValue == null || cboRentUnit.SelectedValue == null)
                {
                    MessageBox.Show("Please select a User and a Unit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStatus("Ready");
                    return;
                }

                // Parse Start Time
                if (!int.TryParse(txtRentStartHour.Text, out int startHour) || startHour < 0 || startHour > 23 ||
                    !int.TryParse(txtRentStartMin.Text, out int startMin) || startMin < 0 || startMin > 59)
                {
                    MessageBox.Show("Please enter a valid Start Time (Hour: 0-23, Minute: 0-59).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStatus("Ready");
                    return;
                }

                // Create DateTime combining current local date with fields and default 00 seconds
                DateTime startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, startHour, startMin, 0);

                // Parse End Time (optional)
                DateTime? endTime = null;
                bool hasEndHour = !string.IsNullOrWhiteSpace(txtRentEndHour.Text);
                bool hasEndMin = !string.IsNullOrWhiteSpace(txtRentEndMin.Text);
                if (hasEndHour || hasEndMin)
                {
                    if (!int.TryParse(txtRentEndHour.Text, out int endHour) || endHour < 0 || endHour > 23 ||
                        !int.TryParse(txtRentEndMin.Text, out int endMin) || endMin < 0 || endMin > 59)
                    {
                        MessageBox.Show("Please enter a valid End Time (Hour: 0-23, Minute: 0-59) or leave both fields empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetStatus("Ready");
                        return;
                    }
                    endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, endHour, endMin, 0);
                }

                var request = new RentCreateRequestModel
                {
                    UserId = (int)cboRentUser.SelectedValue,
                    UnitId = (int)cboRentUnit.SelectedValue,
                    StartTime = startTime,
                    EndTime = endTime.HasValue ? endTime.Value : DateTime.MinValue, // Backend expects DateTime value
                    Duration = null,  // Managed dynamically by backend RentService
                    TotalCost = null // Managed dynamically by backend RentService
                };

                var res = await _rentApiService.CreateRentAsync(request);
                if (res != null && res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "Failed to create rent.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            SetStatus("Ready");
        }

        private async Task ChangeRecordAsync()
        {
            if (!_editMode || !_selectedRecordId.HasValue) return;

            SetStatus("Updating...");
            if (_activeTab == ActiveTab.User)
            {
                var request = new UserPatchRequestModel
                {
                    Username = string.IsNullOrWhiteSpace(txtUserUsername.Text) ? null : txtUserUsername.Text.Trim(),
                    Password = string.IsNullOrEmpty(txtUserPassword.Text) ? null : txtUserPassword.Text,
                    Name = string.IsNullOrWhiteSpace(txtUserName.Text) ? null : txtUserName.Text.Trim(),
                    Balance = numUserBalance.Value
                };

                var res = await _userApiService.UpdateUserAsync(_selectedRecordId.Value, request);
                if (res != null && res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "Failed to update user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (_activeTab == ActiveTab.Unit)
            {
                var request = new UnitPatchRequestModel
                {
                    Type = string.IsNullOrWhiteSpace(txtUnitType.Text) ? null : txtUnitType.Text.Trim(),
                    Rate = numUnitRate.Value,
                    IsActive = chkUnitActive.Checked
                };

                var res = await _unitApiService.UpdateUnitAsync(_selectedRecordId.Value, request);
                if (res != null && res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "Failed to update unit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (_activeTab == ActiveTab.Rent)
            {
                // Parse Start Time
                if (!int.TryParse(txtRentStartHour.Text, out int startHour) || startHour < 0 || startHour > 23 ||
                    !int.TryParse(txtRentStartMin.Text, out int startMin) || startMin < 0 || startMin > 59)
                {
                    MessageBox.Show("Please enter a valid Start Time (Hour: 0-23, Minute: 0-59).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStatus("Ready");
                    return;
                }

                // Create DateTime combining current local date with fields and default 00 seconds
                DateTime startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, startHour, startMin, 0);

                // Parse End Time (optional)
                DateTime? endTime = null;
                bool hasEndHour = !string.IsNullOrWhiteSpace(txtRentEndHour.Text);
                bool hasEndMin = !string.IsNullOrWhiteSpace(txtRentEndMin.Text);
                if (hasEndHour || hasEndMin)
                {
                    if (!int.TryParse(txtRentEndHour.Text, out int endHour) || endHour < 0 || endHour > 23 ||
                        !int.TryParse(txtRentEndMin.Text, out int endMin) || endMin < 0 || endMin > 59)
                    {
                        MessageBox.Show("Please enter a valid End Time (Hour: 0-23, Minute: 0-59) or leave both fields empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetStatus("Ready");
                        return;
                    }
                    endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, endHour, endMin, 0);
                }

                var request = new RentPatchRequestModel
                {
                    UserId = (int?)cboRentUser.SelectedValue,
                    UnitId = (int?)cboRentUnit.SelectedValue,
                    StartTime = startTime,
                    EndTime = endTime,
                    Duration = null,  // Let backend recalculate
                    TotalCost = null // Let backend recalculate
                };

                var res = await _rentApiService.UpdateRentAsync(_selectedRecordId.Value, request);
                if (res != null && res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(res?.Message ?? "Failed to update rent.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            SetStatus("Ready");
        }

        private void EnterEditModeFromSelection()
        {
            if (dgvData.SelectedRows.Count == 0) return;

            var row = dgvData.SelectedRows[0];
            _editMode = true;

            btnSave.Enabled = false;
            btnChange.Enabled = true;

            if (_activeTab == ActiveTab.User)
            {
                _selectedRecordId = (int)row.Cells["UserId"].Value;
                txtUserUsername.Text = row.Cells["Username"].Value?.ToString() ?? "";
                txtUserName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                txtUserPassword.Text = ""; // Keep empty to not change unless typed
                numUserBalance.Value = Convert.ToDecimal(row.Cells["Balance"].Value ?? 0);
            }
            else if (_activeTab == ActiveTab.Unit)
            {
                _selectedRecordId = (int)row.Cells["UnitId"].Value;
                txtUnitType.Text = row.Cells["Type"].Value?.ToString() ?? "";
                numUnitRate.Value = Convert.ToDecimal(row.Cells["Rate"].Value ?? 0);
                chkUnitActive.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value ?? false);
            }
            else if (_activeTab == ActiveTab.Rent)
            {
                _selectedRecordId = (int)row.Cells["RentId"].Value;
                cboRentUser.SelectedValue = (int)row.Cells["UserId"].Value;
                cboRentUnit.SelectedValue = (int)row.Cells["UnitId"].Value;

                // Split StartTime into Hour and Minute
                DateTime startTimeVal = Convert.ToDateTime(row.Cells["StartTime"].Value);
                txtRentStartHour.Text = startTimeVal.Hour.ToString("D2");
                txtRentStartMin.Text = startTimeVal.Minute.ToString("D2");

                // Split EndTime into Hour and Minute
                var endVal = row.Cells["EndTime"].Value;
                if (endVal == null || endVal == DBNull.Value)
                {
                    txtRentEndHour.Text = "";
                    txtRentEndMin.Text = "";
                }
                else
                {
                    DateTime endTimeVal = Convert.ToDateTime(endVal);
                    txtRentEndHour.Text = endTimeVal.Hour.ToString("D2");
                    txtRentEndMin.Text = endTimeVal.Minute.ToString("D2");
                }
            }
            SetStatus($"Edit mode enabled for ID: {_selectedRecordId}");
        }

        private async Task DeleteSelectedRecordAsync()
        {
            if (dgvData.SelectedRows.Count == 0) return;

            var row = dgvData.SelectedRows[0];
            int id = 0;

            if (_activeTab == ActiveTab.User) id = (int)row.Cells["UserId"].Value;
            else if (_activeTab == ActiveTab.Unit) id = (int)row.Cells["UnitId"].Value;
            else if (_activeTab == ActiveTab.Rent) id = (int)row.Cells["RentId"].Value;

            var confirmResult = MessageBox.Show($"Are you sure you want to delete this {_activeTab} record (ID: {id})?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult != DialogResult.Yes) return;

            SetStatus("Deleting...");
            try
            {
                bool deleted = false;
                string msg = "";

                if (_activeTab == ActiveTab.User)
                {
                    var res = await _userApiService.DeleteUserAsync(id);
                    deleted = res?.IsSuccess ?? false;
                    msg = res?.Message ?? "Failed to delete user.";
                }
                else if (_activeTab == ActiveTab.Unit)
                {
                    var res = await _unitApiService.DeleteUnitAsync(id);
                    deleted = res?.IsSuccess ?? false;
                    msg = res?.Message ?? "Failed to delete unit.";
                }
                else if (_activeTab == ActiveTab.Rent)
                {
                    var res = await _rentApiService.DeleteRentAsync(id);
                    deleted = res?.IsSuccess ?? false;
                    msg = res?.Message ?? "Failed to delete rent.";
                }

                if (deleted)
                {
                    MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deletion failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetStatus("Ready");
        }

        private async Task SearchByIdAsync()
        {
            string searchInput = txtSearchId.Text.Trim();
            if (string.IsNullOrEmpty(searchInput))
            {
                MessageBox.Show("Please enter an ID to search.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(searchInput, out int id))
            {
                MessageBox.Show("Please enter a valid numeric ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetStatus($"Searching ID: {id}...");
            try
            {
                if (_activeTab == ActiveTab.User)
                {
                    var res = await _userApiService.GetUserByIdAsync(id);
                    if (res != null && res.IsSuccess)
                    {
                        dgvData.DataSource = new List<UserModel>
                        {
                            new UserModel
                            {
                                UserId = res.UserId,
                                Username = res.Username,
                                Name = res.Name,
                                Balance = res.Balance
                            }
                        };
                        FormatUserGrid();
                    }
                    else
                    {
                        MessageBox.Show("No record found with this ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvData.DataSource = new List<UserModel>();
                    }
                }
                else if (_activeTab == ActiveTab.Unit)
                {
                    var res = await _unitApiService.GetUnitByIdAsync(id);
                    if (res != null && res.IsSuccess)
                    {
                        dgvData.DataSource = new List<UnitModel>
                        {
                            new UnitModel
                            {
                                UnitId = res.UnitId,
                                Type = res.Type,
                                Rate = res.Rate,
                                IsActive = res.IsActive
                            }
                        };
                        FormatUnitGrid();
                    }
                    else
                    {
                        MessageBox.Show("No record found with this ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvData.DataSource = new List<UnitModel>();
                    }
                }
                else if (_activeTab == ActiveTab.Rent)
                {
                    var res = await _rentApiService.GetRentByIdAsync(id);
                    if (res != null && res.IsSuccess)
                    {
                        dgvData.DataSource = new List<RentModel>
                        {
                            new RentModel
                            {
                                RentId = res.RentId,
                                UserId = res.UserId,
                                UnitId = res.UnitId,
                                StartTime = res.StartTime,
                                EndTime = res.EndTime,
                                Duration = res.Duration,
                                TotalCost = res.TotalCost
                            }
                        };
                        FormatRentGrid();
                    }
                    else
                    {
                        MessageBox.Show("No record found with this ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvData.DataSource = new List<RentModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetStatus("Ready");
        }

        private async Task ClearSearchAsync()
        {
            txtSearchId.Text = "";
            await LoadDataAsync();
        }

        private void ClearInputs()
        {
            // Clear inputs
            txtUserUsername.Text = "";
            txtUserPassword.Text = "";
            txtUserName.Text = "";
            numUserBalance.Value = 0;

            txtUnitType.Text = "";
            numUnitRate.Value = 0;
            chkUnitActive.Checked = true;

            if (cboRentUser.Items.Count > 0) cboRentUser.SelectedIndex = 0;
            if (cboRentUnit.Items.Count > 0) cboRentUnit.SelectedIndex = 0;
            
            // Clear split DateTime fields to default 00:00 and empty optional fields
            txtRentStartHour.Text = "00";
            txtRentStartMin.Text = "00";
            txtRentEndHour.Text = "";
            txtRentEndMin.Text = "";

            // Reset edit state
            _editMode = false;
            _selectedRecordId = null;

            btnSave.Enabled = true;
            btnChange.Enabled = false;

            // Clear Grid selection
            if (dgvData.SelectedRows.Count > 0)
            {
                dgvData.ClearSelection();
            }

            SetStatus("Form cleared.");
        }

        private void SetStatus(string text)
        {
            lblStatus.Text = text;
        }
    }
}
