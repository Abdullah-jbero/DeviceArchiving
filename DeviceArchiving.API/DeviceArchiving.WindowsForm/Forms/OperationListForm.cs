using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeviceArchiving.Service;
using DeviceArchiving.Data.Dto.Devices;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class OperationListForm : Form
    {
        private readonly IOperationService _operationService;
        private List<OperationDto> _operations = new List<OperationDto>();
        private readonly int _deviceId;

        public OperationListForm(IOperationService operationService, int deviceId)
        {
            _operationService = operationService;
            _deviceId = deviceId;
            SetupUI();
            LoadOperations();
        }

        private void SetupUI()
        {
            this.Text = "العمليات";
            this.Size = new Size(800, 600); 
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // DataGridView لعرض العمليات
            DataGridView dataGridViewOperations = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                RightToLeft = RightToLeft.Yes,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };
            dataGridViewOperations.Columns.Add("OperationName", "اسم العملية");
            dataGridViewOperations.Columns.Add("OldValue", "القيمة القديمة");
            dataGridViewOperations.Columns.Add("NewValue", "القيمة الجديدة");
            dataGridViewOperations.Columns.Add("Comment", "ملاحظة");
            dataGridViewOperations.Columns.Add("UserName", "تم بواسطة");
            dataGridViewOperations.Columns.Add("CreatedAt", "تاريخ الإنشاء");

            // زر إغلاق
            Button btnClose = new Button
            {
                Text = "إغلاق",
                Location = new Point(this.ClientSize.Width - 120, this.ClientSize.Height - 60),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(dataGridViewOperations);
            this.Controls.Add(btnClose);
        }
        private async void LoadOperations()
        {
            try
            {
                var operations = await _operationService.GetAllOperations(_deviceId);

                // Check if devices is not null
                if (operations == null)
                {
                    MessageBox.Show("لا يوجد بيانات للعمليات.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _operations = operations.Select(d => new OperationDto
                {
                    Comment = d.Comment,
                    UserName = d.User?.UserName ?? "غير معروف", // Handle potential null User
                    NewValue = d.NewValue,
                    OldValue = d.OldValue,
                    CreatedAt = d.CreatedAt,
                    OperationName = d.OperationName,
                }).ToList();

                UpdateDataGridView();
            }
            catch (Exception ex)
            {
                // Log the exception details if needed
                MessageBox.Show($"خطأ أثناء جلب العمليات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDataGridView()
        {
            var dataGridView = this.Controls[0] as DataGridView;
            dataGridView.Rows.Clear();
            foreach (var operation in _operations)
            {
                dataGridView.Rows.Add(
                    operation.OperationName ?? "-",
                    operation.OldValue ?? "-",
                    operation.NewValue ?? "-",
                    operation.Comment ?? "-",
                    operation.UserName ?? "-",
                    operation.CreatedAt.ToString("g") ?? "-"
                );
            }
            if (_operations.Count == 0)
            {
                dataGridView.Rows.Add("لا توجد عمليات متاحة", "-", "-", "-", "-", "-");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}