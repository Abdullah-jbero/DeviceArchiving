using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 100),
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
            CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical, // إضافة حدود عمودية
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeight = 40,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(26, 115, 232),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter // محاذاة النص في المنتصف
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9),
                SelectionBackColor = Color.FromArgb(220, 230, 250),
                SelectionForeColor = Color.Black
            }
        };
        this.Controls.AddRange(new Control[] { lblHeader, dataGridViewDeletedDevices });

        // Resize event handler
        this.Resize += (s, e) =>
        {
            dataGridViewDeletedDevices.Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 100);
            dataGridViewDeletedDevices.Location = new Point(20, 60);
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