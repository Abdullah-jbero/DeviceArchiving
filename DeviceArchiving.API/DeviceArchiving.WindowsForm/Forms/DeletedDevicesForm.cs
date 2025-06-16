using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Service.DeviceServices;
using Guna.UI2.WinForms;


namespace DeviceArchiving.WindowsForm.Forms;
public partial class DeletedDevicesForm : Form
{
    private readonly IDeviceService _deviceService;
    private List<GetAllDevicesDto> _deletedDevices = new List<GetAllDevicesDto>();

    public DeletedDevicesForm(IDeviceService deviceService)
    {
        _deviceService = deviceService;
        SetupUI();
        LoadDeletedDevices();
    }

    private void SetupUI()
    {
        this.Text = "الأجهزة المحذوفة";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.WhiteSmoke;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        // Header Label
        var lblHeader = new Guna2HtmlLabel
        {
            Text = "قائمة الأجهزة المحذوفة",
            Location = new Point(20, 20),
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(26, 115, 232),
            RightToLeft = RightToLeft.Yes
        };
        // Devices Table
        var dataGridViewDeletedDevices = new Guna2DataGridView
        {
            Location = new Point(20, 60),
            Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 150), 
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            Name = "dataGridViewDeletedDevices",
            RightToLeft = RightToLeft.Yes,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            ReadOnly = true,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeight = 40,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(26, 115, 232),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9),
                SelectionBackColor = Color.FromArgb(220, 230, 250),
                SelectionForeColor = Color.Black
            }
        };
        // Restore Button
        var btnRestore = new Guna2Button
        {
            Text = "استعادة الجهاز",
            Size = new Size(150, 40),
            Location = new Point(this.ClientSize.Width - 170, this.ClientSize.Height - 80),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            FillColor = Color.FromArgb(26, 115, 232),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            RightToLeft = RightToLeft.Yes,
            BorderRadius = 5,
            Enabled = false 
        };
        btnRestore.Click += async (s, e) => await RestoreDeviceAsync(dataGridViewDeletedDevices);
        // Enable/Disable Restore Button based on selection
        dataGridViewDeletedDevices.SelectionChanged += (s, e) =>
        {
            btnRestore.Enabled = dataGridViewDeletedDevices.SelectedRows.Count > 0;
        };
        this.Controls.AddRange(new Control[] { lblHeader, dataGridViewDeletedDevices, btnRestore });
        // Resize event handler
        this.Resize += (s, e) =>
        {
            dataGridViewDeletedDevices.Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 150);
            dataGridViewDeletedDevices.Location = new Point(20, 60);
            btnRestore.Location = new Point(this.ClientSize.Width - 170, this.ClientSize.Height - 80);
        };
    }

    private async void LoadDeletedDevices()
    {
        try
        {
            _deletedDevices = (await _deviceService.GetAllDevicesAsync()).Where(d => d.IsActive == false).ToList();
            UpdateDeletedDevicesGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء تحميل الأجهزة المحذوفة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task RestoreDeviceAsync(Guna2DataGridView dgv)
    {
        if (dgv.SelectedRows.Count == 0) return;

        var selectedDevice = dgv.SelectedRows[0].DataBoundItem as GetAllDevicesDto;
        if (selectedDevice == null) return;

        var confirmResult = MessageBox.Show(
            $"هل أنت متأكد من استعادة الجهاز '{selectedDevice.LaptopName}'؟",
            "تأكيد الاستعادة",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

        if (confirmResult != DialogResult.Yes) return;

        try
        {
            await _deviceService.RestoreDeviceAsync(selectedDevice.Id);
            _deletedDevices.Remove(selectedDevice);
            UpdateDeletedDevicesGrid();
            MessageBox.Show("تم استعادة الجهاز بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء استعادة الجهاز: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateDeletedDevicesGrid()
    {
        var dgv = Controls.Find("dataGridViewDeletedDevices", true).FirstOrDefault() as Guna2DataGridView;
        if (dgv == null) return;

        dgv.DataSource = null;
        dgv.DataSource = _deletedDevices;
        dgv.ScrollBars = ScrollBars.Both;
        var columnsRename = new Dictionary<string, string>
        {
            {"Id", "م"},
            {"Source", "الجهة"},
            {"BrotherName", "اسم الأخ"},
            {"LaptopName", "اسم اللاب توب"},
            {"SystemPassword", "كلمة مرور النظام"},
            {"WindowsPassword", "كلمة مرور ويندوز"},
            {"HardDrivePassword", "كلمة التشفير"},
            {"FreezePassword", "كلمة التجميد"},
            {"Code", "الكود"},
            {"Type", "النوع"},
            {"SerialNumber", "الرقم التسلسلي"},
            {"Card", "الكرت"},
            {"Comment", "ملاحظات"},
            {"ContactNumber", "رقم التواصل"},
            {"UserName", "تم بواسطة"},
            {"CreatedAt", "تاريخ الإنشاء"}
        };
        foreach (DataGridViewColumn column in dgv.Columns)
        {
            if (columnsRename.ContainsKey(column.Name))
            {
                column.HeaderText = columnsRename[column.Name];
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
            else
            {
                column.Visible = false;
            }
        }
    }
}