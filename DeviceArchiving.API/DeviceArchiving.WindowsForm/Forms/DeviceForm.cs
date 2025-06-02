using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Service;
using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class DeviceForm : Form
    {
        private readonly IDeviceService _deviceService;
        private readonly GetAllDevicesDto _device;

        public DeviceForm(IDeviceService deviceService, GetAllDevicesDto device = null)
        {
            _deviceService = deviceService;
            _device = device;
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "إدارة الأجهزة";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // Header
            var lblHeader = new Guna2HtmlLabel
            {
                Text = $"مرحبًا، {_user.UserName}",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 115, 232)
            };

            // Search Panel
            var searchPanel = new Guna2Panel
            {
                Location = new Point(20, 60),
                Size = new Size(this.ClientSize.Width - 40, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderColor = Color.LightGray,
                BorderThickness = 1,
                FillColor = Color.White,
                Radius = 10,
                ShadowDecoration = { Enabled = true, Shadow = new Padding(5) }
            };

            var lblGlobalSearch = new Guna2HtmlLabel { Text = "البحث العام", Location = new Point(20, 20), AutoSize = true };
            var txtGlobalSearch = new Guna2TextBox
            {
                Location = new Point(100, 15),
                Width = 400,
                Name = "txtGlobalSearch",
                PlaceholderText = "ابحث هنا...",
                BorderRadius = 8
            };

            var lblLaptopName = new Guna2HtmlLabel { Text = "اسم اللاب توب", Location = new Point(20, 55), AutoSize = true };
            var txtLaptopName = new Guna2TextBox
            {
                Location = new Point(100, 50),
                Width = 200,
                Name = "txtLaptopName",
                PlaceholderText = "اسم اللاب توب",
                BorderRadius = 8
            };

            var lblSerialNumber = new Guna2HtmlLabel { Text = "الرقم التسلسلي", Location = new Point(320, 55), AutoSize = true };
            var txtSerialNumber = new Guna2TextBox
            {
                Location = new Point(400, 50),
                Width = 200,
                Name = "txtSerialNumber",
                PlaceholderText = "الرقم التسلسلي",
                BorderRadius = 8
            };

            var lblType = new Guna2HtmlLabel { Text = "النوع", Location = new Point(620, 55), AutoSize = true };
            var cmbType = new Guna2ComboBox
            {
                Location = new Point(660, 50),
                Width = 200,
                Name = "cmbType",
                BorderRadius = 8,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var btnClear = new Guna2Button
            {
                Text = "مسح",
                Location = new Point(880, 50),
                Width = 70,
                BorderRadius = 8,
                FillColor = Color.FromArgb(230, 57, 70),
                ForeColor = Color.White,
                HoverState = { FillColor = Color.FromArgb(200, 30, 50) }
            };

            // ربط الأحداث
            txtGlobalSearch.TextChanged += (s, e) => { _globalSearchQuery = txtGlobalSearch.Text; ApplyFilter(); };
            txtLaptopName.TextChanged += (s, e) => { _laptopNameFilter = txtLaptopName.Text; ApplyFilter(); };
            txtSerialNumber.TextChanged += (s, e) => { _serialNumberFilter = txtSerialNumber.Text; ApplyFilter(); };
            cmbType.SelectedIndexChanged += (s, e) => { _typeFilter = cmbType.SelectedItem?.ToString() ?? ""; ApplyFilter(); };
            btnClear.Click += BtnClear_Click;

            searchPanel.Controls.AddRange(new Control[] {
                lblGlobalSearch, txtGlobalSearch,
                lblLaptopName, txtLaptopName,
                lblSerialNumber, txtSerialNumber,
                lblType, cmbType,
                btnClear
            });

            // Buttons Panel
            var buttonsPanel = new Guna2Panel
            {
                Location = new Point(20, 180),
                Size = new Size(this.ClientSize.Width - 40, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderColor = Color.LightGray,
                BorderThickness = 1,
                FillColor = Color.White,
                Radius = 10,
                ShadowDecoration = { Enabled = true, Shadow = new Padding(5) }
            };

            var buttonColor = Color.FromArgb(26, 115, 232);

            var btnAddDevice = new Guna2Button { Text = "إضافة جهاز", Location = new Point(20, 10), Width = 110, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnEditDevice = new Guna2Button { Text = "تعديل جهاز", Location = new Point(140, 10), Width = 110, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnDeleteDevice = new Guna2Button { Text = "حذف جهاز", Location = new Point(260, 10), Width = 110, BorderRadius = 8, FillColor = Color.FromArgb(230, 57, 70), ForeColor = Color.White };
            var btnExportExcel = new Guna2Button { Text = "تصدير", Location = new Point(380, 10), Width = 110, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnImportExcel = new Guna2Button { Text = "اختر ملف Excel", Location = new Point(500, 10), Width = 130, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnAddOperation = new Guna2Button { Text = "إضافة عملية", Location = new Point(640, 10), Width = 110, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnShowOperations = new Guna2Button { Text = "عرض العمليات", Location = new Point(760, 10), Width = 110, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnRefresh = new Guna2Button { Text = "تحديث", Location = new Point(880, 10), Width = 80, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };
            var btnShowOperationTypes = new Guna2Button { Text = "عرض أنواع العمليات", Location = new Point(970, 10), Width = 130, BorderRadius = 8, FillColor = buttonColor, ForeColor = Color.White };

            btnAddDevice.Click += BtnAddDevice_Click;
            btnEditDevice.Click += BtnEditDevice_Click;
            btnDeleteDevice.Click += BtnDeleteDevice_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnImportExcel.Click += BtnImportExcel_Click;
            btnAddOperation.Click += BtnAddOperation_Click;
            btnShowOperations.Click += BtnShowOperations_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnShowOperationTypes.Click += BtnShowOperationTypes_Click;

            buttonsPanel.Controls.AddRange(new Control[] {
                btnAddDevice, btnEditDevice, btnDeleteDevice,
                btnExportExcel, btnImportExcel, btnAddOperation,
                btnShowOperations, btnRefresh, btnShowOperationTypes
            });

            // Devices Table
            var dataGridViewDevices = new Guna2DataGridView
            {
                Location = new Point(20, 240),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Name = "dataGridViewDevices",
                RightToLeft = RightToLeft.Yes,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(26, 115, 232),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(220, 230, 250),
                    SelectionForeColor = Color.Black
                }
            };

            dataGridViewDevices.SelectionChanged += DataGridViewDevices_SelectionChanged;
            dataGridViewDevices.DoubleClick += (s, e) => BtnShowOperations_Click(s, e);

            this.Controls.AddRange(new Control[] { lblHeader, searchPanel, buttonsPanel, dataGridViewDevices });
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // Get controls
            var txtLaptopName = this.Controls["txtLaptopName"] as Guna2TextBox;
            var txtSerialNumber = this.Controls["txtSerialNumber"] as Guna2TextBox;
            var txtSource = this.Controls["txtSource"] as Guna2TextBox;
            var txtBrotherName = this.Controls["txtBrotherName"] as Guna2TextBox;
            var txtSystemPassword = this.Controls["txtSystemPassword"] as Guna2TextBox;
            var txtWindowsPassword = this.Controls["txtWindowsPassword"] as Guna2TextBox;
            var txtHardDrivePassword = this.Controls["txtHardDrivePassword"] as Guna2TextBox;
            var txtFreezePassword = this.Controls["txtFreezePassword"] as Guna2TextBox;
            var txtCode = this.Controls["txtCode"] as Guna2TextBox;
            var txtType = this.Controls["txtType"] as Guna2TextBox;
            var txtComment = this.Controls["txtComment"] as Guna2TextBox;
            var txtContactNumber = this.Controls["txtContactNumber"] as Guna2TextBox;
            var txtCard = this.Controls["txtCard"] as Guna2TextBox;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtLaptopName.Text))
            {
                MessageBox.Show("اسم اللاب توب مطلوب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtSerialNumber.Text))
            {
                MessageBox.Show("رقم التسلسل مطلوب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_device == null)
                {
                    var dto = new CreateDeviceDto
                    {
                        Source = txtSource.Text,
                        BrotherName = txtBrotherName.Text,
                        LaptopName = txtLaptopName.Text,
                        SystemPassword = txtSystemPassword.Text,
                        WindowsPassword = txtWindowsPassword.Text,
                        HardDrivePassword = txtHardDrivePassword.Text,
                        FreezePassword = txtFreezePassword.Text,
                        Code = txtCode.Text,
                        Type = txtType.Text,
                        SerialNumber = txtSerialNumber.Text,
                        Comment = txtComment.Text,
                        ContactNumber = txtContactNumber.Text,
                        Card = txtCard.Text
                    };
                    var response = await _deviceService.AddDeviceAsync(dto);
                    if (response.Success)
                    {
                        MessageBox.Show("تم إضافة الجهاز بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(response.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var dto = new UpdateDeviceDto
                    {
                        Source = txtSource.Text,
                        BrotherName = txtBrotherName.Text,
                        LaptopName = txtLaptopName.Text,
                        SystemPassword = txtSystemPassword.Text,
                        WindowsPassword = txtWindowsPassword.Text,
                        HardDrivePassword = txtHardDrivePassword.Text,
                        FreezePassword = txtFreezePassword.Text,
                        Code = txtCode.Text,
                        Type = txtType.Text,
                        SerialNumber = txtSerialNumber.Text,
                        Comment = txtComment.Text,
                        ContactNumber = txtContactNumber.Text,
                        Card = txtCard.Text
                    };
                    var response = await _deviceService.UpdateDeviceAsync(_device.Id, dto);
                    if (response.Success)
                    {
                        MessageBox.Show("تم تعديل الجهاز بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(response.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء حفظ الجهاز: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
