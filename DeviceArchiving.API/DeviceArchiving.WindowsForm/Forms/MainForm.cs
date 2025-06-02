using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Dto.Users;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using DeviceArchiving.WindowsForm.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using DeviceArchiving.WindowsForm.Dtos;
namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class MainForm : Form
    {
        private readonly IDeviceService _deviceService;
        private readonly IOperationService _operationService;
        private readonly IOperationTypeService _operationTypeService;
        private readonly IAccountService _accountService;
        private readonly AuthenticationResponse _user;
        private List<GetAllDevicesDto> _devices = new List<GetAllDevicesDto>();
        private List<GetAllDevicesDto> _filteredDevices = new List<GetAllDevicesDto>();
        private GetAllDevicesDto _selectedDevice;
        private string _globalSearchQuery = "";
        private string _laptopNameFilter = "";
        private string _serialNumberFilter = "";
        private string _typeFilter = "";
        private List<ExcelDevice> _excelData = new List<ExcelDevice>();
        private bool _loading = false;

        public MainForm(IAccountService accountService, AuthenticationResponse user)
        {
            _accountService = accountService;
            _user = user;
            _deviceService = Program.Services.GetService<IDeviceService>();
            _operationService = Program.Services.GetService<IOperationService>();
            _operationTypeService = Program.Services.GetService<IOperationTypeService>();
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
            LoadDevices();
        }

        private void SetupUI()
        {
            this.Text = "≈œ«—… «·√ÃÂ“…";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Header
            Label lblHeader = new Label { Text = $"„—Õ»«° {_user.UserName}", Location = new Point(20, 20), AutoSize = true, Font = new Font("Arial", 14, FontStyle.Bold) };

            // Search Panel
            Panel searchPanel = new Panel { Location = new Point(20, 60), Size = new Size(this.ClientSize.Width - 40, 100), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            Label lblGlobalSearch = new Label { Text = "«·»ÕÀ «·⁄«„:", Location = new Point(20, 20), AutoSize = true };
            TextBox txtGlobalSearch = new TextBox { Location = new Point(100, 20), Width = 400, Name = "txtGlobalSearch" };
            Label lblLaptopName = new Label { Text = "«”„ «··«»  Ê»:", Location = new Point(20, 50), AutoSize = true };
            TextBox txtLaptopName = new TextBox { Location = new Point(100, 50), Width = 200, Name = "txtLaptopName" };
            Label lblSerialNumber = new Label { Text = "«·—ﬁ„ «· ”·”·Ì:", Location = new Point(320, 50), AutoSize = true };
            TextBox txtSerialNumber = new TextBox { Location = new Point(400, 50), Width = 200, Name = "txtSerialNumber" };
            Label lblType = new Label { Text = "«·‰Ê⁄:", Location = new Point(620, 50), AutoSize = true };
            ComboBox cmbType = new ComboBox { Location = new Point(660, 50), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Name = "cmbType" };
            Button btnClear = new Button { Text = "„”Õ", Location = new Point(860, 50), Width = 80 };
            txtGlobalSearch.TextChanged += (s, e) => { _globalSearchQuery = txtGlobalSearch.Text; ApplyFilter(); };
            txtLaptopName.TextChanged += (s, e) => { _laptopNameFilter = txtLaptopName.Text; ApplyFilter(); };
            txtSerialNumber.TextChanged += (s, e) => { _serialNumberFilter = txtSerialNumber.Text; ApplyFilter(); };
            cmbType.SelectedIndexChanged += (s, e) => { _typeFilter = cmbType.SelectedItem?.ToString() ?? ""; ApplyFilter(); };
            btnClear.Click += BtnClear_Click;
            searchPanel.Controls.AddRange(new Control[] { lblGlobalSearch, txtGlobalSearch, lblLaptopName, txtLaptopName, lblSerialNumber, txtSerialNumber, lblType, cmbType, btnClear });

            // Buttons Panel
            Panel buttonsPanel = new Panel { Location = new Point(20, 170), Size = new Size(this.ClientSize.Width - 40, 50), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            //  ⁄œÌ· „Ê«ﬁ⁄ «·√“—«— ·  ”⁄ œ«Œ· «··ÊÕ…
            Button btnAddDevice = new Button { Text = "≈÷«›… ÃÂ«“", Location = new Point(20, 10), Width = 100 };
            Button btnEditDevice = new Button { Text = " ⁄œÌ· ÃÂ«“", Location = new Point(130, 10), Width = 100 };
            Button btnDeleteDevice = new Button { Text = "Õ–› ÃÂ«“", Location = new Point(240, 10), Width = 100 };
            Button btnExportExcel = new Button { Text = " ’œÌ—", Location = new Point(350, 10), Width = 100 };
            Button btnImportExcel = new Button { Text = "«Œ — „·› Excel", Location = new Point(460, 10), Width = 100 };
            Button btnAddOperation = new Button { Text = "≈÷«›… ⁄„·Ì…", Location = new Point(570, 10), Width = 100 };
            Button btnShowOperations = new Button { Text = "⁄—÷ «·⁄„·Ì« ", Location = new Point(680, 10), Width = 100 };
            Button btnRefresh = new Button { Text = " ÕœÌÀ", Location = new Point(790, 10), Width = 100 };
            Button btnShowOperationTypes = new Button { Text = "⁄—÷ √‰Ê«⁄ «·⁄„·Ì« ", Location = new Point(900, 10), Width = 100 };

            btnAddDevice.Click += BtnAddDevice_Click;
            btnEditDevice.Click += BtnEditDevice_Click;
            btnDeleteDevice.Click += BtnDeleteDevice_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnImportExcel.Click += BtnImportExcel_Click;
            btnAddOperation.Click += BtnAddOperation_Click;
            btnShowOperations.Click += BtnShowOperations_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnShowOperationTypes.Click += BtnShowOperationTypes_Click;
            buttonsPanel.Controls.AddRange(new Control[] { btnAddDevice, btnEditDevice, btnDeleteDevice, btnExportExcel, btnImportExcel, btnAddOperation, btnShowOperations, btnRefresh , btnShowOperationTypes });

            // Devices Table
            DataGridView dataGridViewDevices = new DataGridView
            {
                Location = new Point(20, 230),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Name = "dataGridViewDevices",
                RightToLeft = RightToLeft.Yes
            };
            dataGridViewDevices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewDevices.MultiSelect = false;
            dataGridViewDevices.RowHeadersVisible = false;
            dataGridViewDevices.SelectionChanged += DataGridViewDevices_SelectionChanged;
            dataGridViewDevices.DoubleClick += (s, e) => BtnShowOperations_Click(s, e);

            this.Controls.AddRange(new Control[] { lblHeader, searchPanel, buttonsPanel, dataGridViewDevices });
        }

        private async void LoadDevices()
        {
            try
            {
                _loading = true;
                _devices = (await _deviceService.GetAllDevicesAsync()).ToList();
                _filteredDevices = _devices.ToList();
                UpdateDataGridView();
                UpdateDeviceTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Œÿ√ √À‰«¡  Õ„Ì· «·√ÃÂ“…: {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading = false;
            }
        }

        private void BtnShowOperationTypes_Click(object sender, EventArgs e)
        {
            using (var form = new OperationTypeListForm(_operationTypeService))
            {
                form.ShowDialog();
            }
        }
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }

        private void UpdateDataGridView()
        {
            var dataGridView = this.Controls["dataGridViewDevices"] as DataGridView;
            dataGridView.DataSource = null;
            dataGridView.DataSource = _filteredDevices;
            if (dataGridView.Columns["Source"] != null) dataGridView.Columns["Source"].HeaderText = "«·ÃÂ…";
            if (dataGridView.Columns["BrotherName"] != null) dataGridView.Columns["BrotherName"].HeaderText = "«”„ «·√Œ";
            if (dataGridView.Columns["LaptopName"] != null) dataGridView.Columns["LaptopName"].HeaderText = "«”„ «··«»  Ê»";
            if (dataGridView.Columns["SystemPassword"] != null) dataGridView.Columns["SystemPassword"].HeaderText = "ﬂ·„… „—Ê— «·‰Ÿ«„";
            if (dataGridView.Columns["WindowsPassword"] != null) dataGridView.Columns["WindowsPassword"].HeaderText = "ﬂ·„… „—Ê— ÊÌ‰œÊ“";
            if (dataGridView.Columns["HardDrivePassword"] != null) dataGridView.Columns["HardDrivePassword"].HeaderText = "ﬂ·„… «· ‘›Ì—";
            if (dataGridView.Columns["FreezePassword"] != null) dataGridView.Columns["FreezePassword"].HeaderText = "ﬂ·„… «· Ã„Ìœ";
            if (dataGridView.Columns["Code"] != null) dataGridView.Columns["Code"].HeaderText = "«·ﬂÊœ";
            if (dataGridView.Columns["Type"] != null) dataGridView.Columns["Type"].HeaderText = "«·‰Ê⁄";
            if (dataGridView.Columns["SerialNumber"] != null) dataGridView.Columns["SerialNumber"].HeaderText = "«·—ﬁ„ «· ”·”·Ì";
            if (dataGridView.Columns["Card"] != null) dataGridView.Columns["Card"].HeaderText = "«·ﬂ— ";
            if (dataGridView.Columns["Comment"] != null) dataGridView.Columns["Comment"].HeaderText = "„·«ÕŸ« ";
            if (dataGridView.Columns["ContactNumber"] != null) dataGridView.Columns["ContactNumber"].HeaderText = "—ﬁ„ «· Ê«’·";
            if (dataGridView.Columns["UserName"] != null) dataGridView.Columns["UserName"].HeaderText = " „ »Ê«”ÿ…";
            if (dataGridView.Columns["CreatedAt"] != null) dataGridView.Columns["CreatedAt"].HeaderText = " «—ÌŒ «·≈‰‘«¡";
        }

        private void UpdateDeviceTypes()
        {
            var cmbType = this.Controls.OfType<Panel>().First(p => p.Controls.ContainsKey("cmbType")).Controls["cmbType"] as ComboBox;
            var types = _devices.Select(d => d.Type).Distinct().ToList();
            cmbType.Items.Clear();
            cmbType.Items.Add("");
            cmbType.Items.AddRange(types.ToArray());
            cmbType.SelectedIndex = 0;
        }

        private void ApplyFilter()
        {
            _filteredDevices = _devices.Where(d =>
                (string.IsNullOrEmpty(_globalSearchQuery) ||
                 d.Source?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.BrotherName?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.LaptopName?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.SerialNumber?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.Type?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.SystemPassword?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.WindowsPassword?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.HardDrivePassword?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.FreezePassword?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.Code?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.Card?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.Comment?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.ContactNumber?.ToLower().Contains(_globalSearchQuery.ToLower()) == true ||
                 d.UserName?.ToLower().Contains(_globalSearchQuery.ToLower()) == true) &&
                (string.IsNullOrEmpty(_laptopNameFilter) || d.LaptopName?.ToLower().Contains(_laptopNameFilter.ToLower()) == true) &&
                (string.IsNullOrEmpty(_serialNumberFilter) || d.SerialNumber?.ToLower().Contains(_serialNumberFilter.ToLower()) == true) &&
                (string.IsNullOrEmpty(_typeFilter) || d.Type == _typeFilter)
            ).ToList();
            UpdateDataGridView();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            _globalSearchQuery = _laptopNameFilter = _serialNumberFilter = _typeFilter = "";
            var searchPanel = this.Controls.OfType<Panel>().First(p => p.Controls.ContainsKey("txtGlobalSearch"));
            (searchPanel.Controls["txtGlobalSearch"] as TextBox).Text = "";
            (searchPanel.Controls["txtLaptopName"] as TextBox).Text = "";
            (searchPanel.Controls["txtSerialNumber"] as TextBox).Text = "";
            (searchPanel.Controls["cmbType"] as ComboBox).SelectedIndex = 0;
            ApplyFilter();
        }

        private void BtnAddDevice_Click(object sender, EventArgs e)
        {
            DeviceForm deviceForm = new DeviceForm(_deviceService);
            if (deviceForm.ShowDialog() == DialogResult.OK)
            {
                LoadDevices();
            }
        }

        private void BtnEditDevice_Click(object sender, EventArgs e)
        {
            if (_selectedDevice == null)
            {
                MessageBox.Show("Ì—ÃÏ  ÕœÌœ ÃÂ«“ ·· ⁄œÌ·.", " Õ–Ì—", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DeviceForm deviceForm = new DeviceForm(_deviceService, _selectedDevice);
            if (deviceForm.ShowDialog() == DialogResult.OK)
            {
                LoadDevices();
            }
        }

        private async void BtnDeleteDevice_Click(object sender, EventArgs e)
        {
            if (_selectedDevice == null)
            {
                MessageBox.Show("Ì—ÃÏ  ÕœÌœ ÃÂ«“ ··Õ–›.", " Õ–Ì—", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"Â· √‰  „ √ﬂœ „‰ Õ–› «·ÃÂ«“ '{_selectedDevice.LaptopName}'ø", " √ﬂÌœ «·Õ–›", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _deviceService.DeleteDeviceAsync(_selectedDevice.Id);
                    _selectedDevice = null;
                    LoadDevices();
                    MessageBox.Show(" „ Õ–› «·ÃÂ«“ »‰Ã«Õ.", "‰Ã«Õ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Œÿ√ √À‰«¡ Õ–› «·ÃÂ«“: {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("«·√ÃÂ“…");
                    var headers = new[] { "«·ÃÂ…", "«”„ «·√Œ", "«”„ «··«»  Ê»", "ﬂ·„… „—Ê— «·‰Ÿ«„", "ﬂ·„… „—Ê— ÊÌ‰œÊ“", "ﬂ·„… «· ‘›Ì—", "ﬂ·„… «· Ã„Ìœ", "«·ﬂÊœ", "«·‰Ê⁄", "«·—ﬁ„ «· ”·”·Ì", "«·ﬂ— ", "„·«ÕŸ…", "—ﬁ„ «· Ê«’·", " „ »Ê«”ÿ…", " «—ÌŒ «·≈‰‘«¡" };
                    for (int i = 0; i < headers.Length; i++)
                        worksheet.Cell(1, i + 1).Value = headers[i];

                    for (int i = 0; i < _filteredDevices.Count; i++)
                    {
                        var device = _filteredDevices[i];
                        worksheet.Cell(i + 2, 1).Value = device.Source;
                        worksheet.Cell(i + 2, 2).Value = device.BrotherName;
                        worksheet.Cell(i + 2, 3).Value = device.LaptopName;
                        worksheet.Cell(i + 2, 4).Value = device.SystemPassword;
                        worksheet.Cell(i + 2, 5).Value = device.WindowsPassword;
                        worksheet.Cell(i + 2, 6).Value = device.HardDrivePassword;
                        worksheet.Cell(i + 2, 7).Value = device.FreezePassword;
                        worksheet.Cell(i + 2, 8).Value = device.Code;
                        worksheet.Cell(i + 2, 9).Value = device.Type;
                        worksheet.Cell(i + 2, 10).Value = device.SerialNumber;
                        worksheet.Cell(i + 2, 11).Value = device.Card;
                        worksheet.Cell(i + 2, 12).Value = device.Comment;
                        worksheet.Cell(i + 2, 13).Value = device.ContactNumber;
                        worksheet.Cell(i + 2, 14).Value = device.UserName;
                        worksheet.Cell(i + 2, 15).Value = device.CreatedAt.ToString("g");
                    }

                    using (var saveFileDialog = new SaveFileDialog { Filter = "Excel files (*.xlsx)|*.xlsx", FileName = "devices.xlsx" })
                    {
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            workbook.SaveAs(saveFileDialog.FileName);
                            MessageBox.Show(" „  ’œÌ— «·»Ì«‰«  »‰Ã«Õ.", "‰Ã«Õ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Œÿ√ √À‰«¡  ’œÌ— «·»Ì«‰« : {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImportExcel_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.CanUploadExcel)
            {
                MessageBox.Show(" „ —›⁄ „·› Excel „”»ﬁ«. ·« Ì„ﬂ‰ —›⁄ „·› ¬Œ—.", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (var openFileDialog = new OpenFileDialog { Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _loading = true;
                        _excelData = ReadExcelFile(openFileDialog.FileName);
                        if (_excelData.Count == 0) return;

                        if (!CheckDuplicatesInFile(_excelData))
                        {
                            _excelData.Clear();
                            return;
                        }

                        CheckDuplicatesInDatabase(_excelData);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Œÿ√ √À‰«¡ ﬁ—«¡… «·„·›: {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _loading = false;
                    }
                }
            }
        }

        private List<ExcelDevice> ReadExcelFile(string filePath)
        {
            var devices = new List<ExcelDevice>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var headers = new[] { "«·—ﬁ„", "«·ÃÂ…", "«”„ «·√Œ", "«”„ «··«» Ê»", "ﬂ·„… ”— «·‰Ÿ«„", "ﬂ·„… ”— «·ÊÌ‰œÊ“", "ﬂ·„… ”— «·Â«—œ", "ﬂ·„… «· Ã„Ìœ", "«·ﬂÊœ", "«·‰Ê⁄", "—ﬁ„ «·”Ì—Ì«·", "«·ﬂ— ", "„·«ÕŸ« ", "«· «—ÌŒ", "—ﬁ„ «· Ê«’·" };
                for (int i = 0; i < headers.Length; i++)
                {
                    if (worksheet.Cell(1, i + 1).GetString() != headers[i])
                    {
                        MessageBox.Show($"—√” «·ÃœÊ· €Ì— ’ÕÌÕ. Ì—ÃÏ «” Œœ«„ «·ﬁ«·» «·’ÕÌÕ: {string.Join(", ", headers)}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return devices;
                    }
                }

                var rows = worksheet.RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    devices.Add(new ExcelDevice
                    {
                        Source = row.Cell(2).GetString().Trim(),
                        BrotherName = row.Cell(3).GetString().Trim(),
                        LaptopName = row.Cell(4).GetString().Trim(),
                        SystemPassword = row.Cell(5).GetString().Trim(),
                        WindowsPassword = row.Cell(6).GetString().Trim(),
                        HardDrivePassword = row.Cell(7).GetString().Trim(),
                        FreezePassword = row.Cell(8).GetString().Trim(),
                        Code = row.Cell(9).GetString().Trim(),
                        Type = row.Cell(10).GetString().Trim(),
                        SerialNumber = row.Cell(11).GetString().Trim(),
                        Card = row.Cell(12).GetString().Trim(),
                        Comment = row.Cell(13).GetString().Trim() != "" ? row.Cell(13).GetString().Trim() : null,
                        ContactNumber = row.Cell(15).GetString().Trim(),
                        IsSelected = true,
                        IsDuplicateSerial = false,
                        IsDuplicateLaptopName = false
                    });
                }
            }

            if (devices.Count == 0)
                MessageBox.Show("·„ Ì „ «·⁄ÀÊ— ⁄·Ï »Ì«‰«  ’«·Õ… ›Ì «·„·›.", " Õ–Ì—", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return devices;
        }

        private bool CheckDuplicatesInFile(List<ExcelDevice> devices)
        {
            var serialNumbers = new HashSet<string>();
            var laptopNames = new HashSet<string>();
            var duplicateSerials = new HashSet<string>();
            var duplicateLaptopNames = new HashSet<string>();
            var columnErrors = new List<(int Row, List<string> Fields)>();

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var rowErrors = new List<string>();
                var rowNumber = i + 2; // Header row + 1-based indexing

                var requiredFields = new (string Field, string Label)[]
                {
                    (device.Source, "«·ÃÂ…"),
                    (device.BrotherName, "«”„ «·√Œ"),
                    (device.LaptopName, "«”„ «··«» Ê»"),
                    (device.SystemPassword, "ﬂ·„… ”— «·‰Ÿ«„"),
                    (device.WindowsPassword, "ﬂ·„… ”— «·ÊÌ‰œÊ“"),
                    (device.HardDrivePassword, "ﬂ·„… ”— «·Â«—œ"),
                    (device.FreezePassword, "ﬂ·„… «· Ã„Ìœ"),
                    (device.Code, "«·ﬂÊœ"),
                    (device.Type, "«·‰Ê⁄"),
                    (device.SerialNumber, "—ﬁ„ «·”Ì—Ì«·"),
                    (device.Card, "«·ﬂ— ")
                };

                foreach (var (field, label) in requiredFields)
                {
                    if (string.IsNullOrWhiteSpace(field))
                        rowErrors.Add(label);
                }

                if (!string.IsNullOrEmpty(device.SerialNumber))
                {
                    if (serialNumbers.Contains(device.SerialNumber))
                        duplicateSerials.Add(device.SerialNumber);
                    else
                        serialNumbers.Add(device.SerialNumber);
                }

                if (!string.IsNullOrEmpty(device.LaptopName))
                {
                    if (laptopNames.Contains(device.LaptopName))
                        duplicateLaptopNames.Add(device.LaptopName);
                    else
                        laptopNames.Add(device.LaptopName);
                }

                if (rowErrors.Count > 0)
                    columnErrors.Add((rowNumber, rowErrors));
            }

            var errorMessages = new List<string>();
            if (columnErrors.Any())
                errorMessages.Add($"ÕﬁÊ· „ÿ·Ê»… ›«—€…: {string.Join("∫ ", columnErrors.Select(e => $"«·’› {e.Row}: {string.Join("° ", e.Fields)}"))}");
            if (duplicateSerials.Any())
                errorMessages.Add($"√—ﬁ«„ ”Ì—Ì«· „ﬂ——…: {string.Join(", ", duplicateSerials)}");
            if (duplicateLaptopNames.Any())
                errorMessages.Add($"√”„«¡ ·«» Ê» „ﬂ——…: {string.Join(", ", duplicateLaptopNames)}");

            if (errorMessages.Any())
            {
                MessageBox.Show($"«·„·› ÌÕ ÊÌ ⁄·Ï √Œÿ«¡: {string.Join("° ", errorMessages)}\n √ﬂœ „‰ √‰ Ã„Ì⁄ «·ÕﬁÊ· «·„ÿ·Ê»… „„·Ê¡… Ê√‰ √—ﬁ«„ «·”Ì—Ì«· Ê√”„«¡ «··«» Ê» ›—Ìœ….", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private async void CheckDuplicatesInDatabase(List<ExcelDevice> devices)
        {
            try
            {
                var checkItems = devices.Select(d => new CheckDuplicateDto
                {
                    SerialNumber = d.SerialNumber,
                    LaptopName = d.LaptopName
                }).ToList();

                var response = await _deviceService.CheckDuplicatesAsync(checkItems);
                foreach (var device in devices)
                {
                    if (response.Data.DuplicateLaptopNames.Contains(device.SerialNumber))
                    {
                        device.IsDuplicateSerial = true;
                    }
                    if (response.Data.DuplicateLaptopNames.Contains(device.LaptopName))
                    {
                        device.IsDuplicateLaptopName = true;
                    }
                }

                _excelData = devices.OrderByDescending(d => d.IsDuplicateSerial)
                                   .ThenByDescending(d => d.IsDuplicateLaptopName)
                                   .ToList();

                ShowExcelPreviewDialog();
                LoadDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Œÿ√ √À‰«¡ «· Õﬁﬁ „‰ «· ﬂ—«—« : {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _excelData.Clear();
            }
        }

        private void ShowExcelPreviewDialog()
        {
            var previewForm = new ExcelPreviewForm(_excelData, UploadSelectedDevices);
            previewForm.ShowDialog();
        }

        private async void UploadSelectedDevices(List<ExcelDevice> selectedDevices)
        {
            if (!selectedDevices.Any())
            {
                MessageBox.Show("·„ Ì „  ÕœÌœ √Ì √ÃÂ“… ··—›⁄.", " Õ–Ì—", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _loading = true;
                var uploadDtos = selectedDevices.Select(d => new DeviceUploadDto
                {
                    Source = d.Source,
                    BrotherName = d.BrotherName,
                    LaptopName = d.LaptopName,
                    SystemPassword = d.SystemPassword,
                    WindowsPassword = d.WindowsPassword,
                    HardDrivePassword = d.HardDrivePassword,
                    FreezePassword = d.FreezePassword,
                    Code = d.Code,
                    Type = d.Type,
                    SerialNumber = d.SerialNumber,
                    Card = d.Card,
                    Comment = d.Comment,
                    ContactNumber = d.ContactNumber,
                    IsUpdate = d.IsDuplicateSerial || d.IsDuplicateLaptopName
                }).ToList();

                var count = await _deviceService.ProcessDevicesAsync(uploadDtos);
                MessageBox.Show($" „ „⁄«·Ã… {count} ÃÂ«“ »‰Ã«Õ.", "‰Ã«Õ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _excelData.Clear();
                LoadDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Œÿ√ √À‰«¡ „⁄«·Ã… «·√ÃÂ“…: {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading = false;
            }
        }

        private void DataGridViewDevices_SelectionChanged(object sender, EventArgs e)
        {
            var dataGridView = sender as DataGridView;
            if (dataGridView.SelectedRows.Count > 0)
                _selectedDevice = dataGridView.SelectedRows[0].DataBoundItem as GetAllDevicesDto;
            else
                _selectedDevice = null;
        }

        private void BtnAddOperation_Click(object sender, EventArgs e)
        {
            if (_selectedDevice == null)
            {
                MessageBox.Show("Ì—ÃÏ  ÕœÌœ ÃÂ«“ ·≈÷«›… ⁄„·Ì….", " Õ–Ì—", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddOperationDialog addOperationDialog = new AddOperationDialog(_operationTypeService,_operationService , _selectedDevice.Id, _selectedDevice.LaptopName);
            if (addOperationDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(" „ ≈÷«›… «·⁄„·Ì… »‰Ã«Õ.", "‰Ã«Õ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnShowOperations_Click(object sender, EventArgs e)
        {
            if (_selectedDevice == null)
            {
                MessageBox.Show("Ì—ÃÏ  ÕœÌœ ÃÂ«“ ·⁄—÷ «·⁄„·Ì« .", " Õ–Ì—", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var operations = await _operationService.GetAllOperations(_selectedDevice.Id);
                using (var operationListForm = new OperationListForm(_operationService, _selectedDevice.Id))
                {
                    operationListForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Œÿ√ √À‰«¡ Ã·» «·⁄„·Ì« : {ex.Message}", "Œÿ√", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }



}

