using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Service;
using DeviceArchiving.WindowsForm.Dtos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class ExcelPreviewForm : Form
    {
        private readonly List<ExcelDevice> _devices;
        private readonly Action<List<ExcelDevice>> _uploadAction;
        private readonly IDeviceService _deviceService;
        private DataGridView _grid;
        private Button _btnUpload;

        public ExcelPreviewForm(List<ExcelDevice> devices, Action<List<ExcelDevice>> uploadAction)
        {
            _devices = devices;
            _uploadAction = uploadAction;
            _deviceService = Program.Services.GetService<IDeviceService>();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "معاينة بيانات Excel";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                RightToLeft = RightToLeft.Yes,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false
            };

            _grid.CellValueChanged += GridCellValueChanged;
            _grid.Columns.Clear();
            _grid.AutoGenerateColumns = false;

            _grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsSelected", HeaderText = "تحديد", DataPropertyName = "IsSelected" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Source", HeaderText = "الجهة", DataPropertyName = "Source" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BrotherName", HeaderText = "اسم الأخ", DataPropertyName = "BrotherName" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "LaptopName", HeaderText = "اسم اللاب توب", DataPropertyName = "LaptopName" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SystemPassword", HeaderText = "كلمة مرور النظام", DataPropertyName = "SystemPassword" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "WindowsPassword", HeaderText = "كلمة مرور ويندوز", DataPropertyName = "WindowsPassword" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "HardDrivePassword", HeaderText = "كلمة التشفير", DataPropertyName = "HardDrivePassword" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FreezePassword", HeaderText = "كلمة التجميد", DataPropertyName = "FreezePassword" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "الكود", DataPropertyName = "Code" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "النوع", DataPropertyName = "Type" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SerialNumber", HeaderText = "الرقم التسلسلي", DataPropertyName = "SerialNumber" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Card", HeaderText = "الكرت", DataPropertyName = "Card" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Comment", HeaderText = "ملاحظات", DataPropertyName = "Comment" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ContactNumber", HeaderText = "رقم التواصل", DataPropertyName = "ContactNumber" });

            // أخيرًا: ربط المصدر
            _grid.DataSource = _devices;


            var btnSelectAll = new Button { Text = "تحديد الكل", Dock = DockStyle.Left, Width = 100 };
            var btnDeselectAll = new Button { Text = "إلغاء تحديد الكل", Dock = DockStyle.Left, Width = 120 };
            var btnCancel = new Button { Text = "إلغاء", Dock = DockStyle.Right, Width = 100 };
            _btnUpload = new Button { Text = "رفع الأجهزة المحددة", Dock = DockStyle.Right, Width = 150 };

            btnSelectAll.Click += BtnSelectAll_Click;
            btnDeselectAll.Click += BtnDeselectAll_Click;
            btnCancel.Click += (s, e) => this.Close();
            _btnUpload.Click += BtnUpload_Click;

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            btnPanel.Controls.AddRange(new Control[] { _btnUpload, btnSelectAll, btnDeselectAll, btnCancel });

            this.Controls.AddRange(new Control[] { _grid, btnPanel });
        }

        private void GridCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                _devices[e.RowIndex].IsSelected = Convert.ToBoolean(_grid.Rows[e.RowIndex].Cells[0].Value);
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (var device in _devices)
            {
                device.IsSelected = true;
            }
            _grid.Refresh();
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (var device in _devices)
            {
                device.IsSelected = false;
            }
            _grid.Refresh();
        }

        private async void BtnUpload_Click(object sender, EventArgs e)
        {



            var selectedDevices = _devices
                .Where(d => d.IsSelected)
                .Select(device => new DeviceUploadDto
                {
                    Source = device.Source,
                    BrotherName = device.BrotherName,
                    LaptopName = device.LaptopName,
                    SystemPassword = device.SystemPassword,
                    WindowsPassword = device.WindowsPassword,
                    HardDrivePassword = device.HardDrivePassword,
                    FreezePassword = device.FreezePassword,
                    Code = device.Code,
                    Type = device.Type,
                    SerialNumber = device.SerialNumber,
                    Card = device.Card,
                    Comment = device.Comment,
                    ContactNumber = device.ContactNumber,
                    IsUpdate = device.IsDuplicateSerial || device.IsDuplicateLaptopName
                })
                .ToList();

            if (!selectedDevices.Any())
            {
                MessageBox.Show("لم يتم تحديد أي أجهزة للرفع", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _btnUpload.Enabled = false;

            try
            {
                var response = await _deviceService.ProcessDevicesAsync(selectedDevices);

                if (response.Success)
                {
                    MessageBox.Show(
                        response.Message ?? $"تم معالجة {response.Data} جهاز{(response.Data == 1 ? "" : "ات")} بنجاح",
                        "نجاح",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );


                    Properties.Settings.Default.CanUploadExcel = false;
                    Properties.Settings.Default.Save(); // Save the setting
                    _devices.Clear();
                    _uploadAction?.Invoke(new List<ExcelDevice>());
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Message ?? "فشل معالجة الأجهزة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message ?? "حدث خطأ أثناء معالجة الأجهزة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnUpload.Enabled = true;
            }
        }
    }
}
