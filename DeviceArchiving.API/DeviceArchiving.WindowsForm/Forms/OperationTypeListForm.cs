using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class OperationTypeListForm : Form
    {
        private readonly IOperationTypeService _operationTypeService;
        private List<OperationType> _operationTypes = new List<OperationType>();
        private string _searchTerm = "";
        private DataGridView dataGridViewOperationTypes; // Declare as a class-level field

        public OperationTypeListForm(IOperationTypeService operationTypeService)
        {
            _operationTypeService = operationTypeService;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
            LoadOperationTypes();
        }

        private void SetupUI()
        {
            this.Text = "قائمة أنواع العمليات";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Header
            Label lblHeader = new Label { Text = "قائمة أنواع العمليات", Location = new Point(20, 20), AutoSize = true, Font = new Font("Arial", 14, FontStyle.Bold) };

            // Search Panel
            Panel searchPanel = new Panel { Location = new Point(20, 60), Size = new Size(this.ClientSize.Width - 40, 50), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            Label lblSearch = new Label { Text = "البحث في أنواع العمليات:", Location = new Point(20, 15), AutoSize = true };
            TextBox txtSearch = new TextBox { Location = new Point(150, 15), Width = 300, Name = "txtSearch" };
            Button btnClear = new Button { Text = "مسح", Location = new Point(460, 15), Width = 80 };
            txtSearch.TextChanged += (s, e) => { _searchTerm = txtSearch.Text; LoadOperationTypes(); };
            btnClear.Click += (s, e) => { txtSearch.Text = ""; _searchTerm = ""; LoadOperationTypes(); };
            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnClear });

            // Buttons Panel
            Panel buttonsPanel = new Panel { Location = new Point(20, 120), Size = new Size(this.ClientSize.Width - 40, 50), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            Button btnAddOperationType = new Button { Text = "إضافة نوع عملية", Location = new Point(this.ClientSize.Width - 180, 10), Width = 120, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnAddOperationType.Click += BtnAddOperationType_Click;
            buttonsPanel.Controls.Add(btnAddOperationType);

            // DataGridView لعرض الأنواع
            dataGridViewOperationTypes = new DataGridView // Assign to the class-level field
            {
                Location = new Point(20, 180),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 250),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Name = "dataGridViewOperationTypes",
                RightToLeft = RightToLeft.Yes,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };
            dataGridViewOperationTypes.Columns.Add("Name", "الاسم");
            var actionColumn = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "الإجراءات - تعديل",
                Text = "تعديل",
                UseColumnTextForButtonValue = true
            };
            dataGridViewOperationTypes.Columns.Add(actionColumn);
            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "الإجراءات - حذف",
                Text = "حذف",
                UseColumnTextForButtonValue = true
            };
            dataGridViewOperationTypes.Columns.Add(deleteColumn);
            dataGridViewOperationTypes.CellClick += DataGridViewOperationTypes_CellClick;

            this.Controls.AddRange(new Control[] { lblHeader, searchPanel, buttonsPanel, dataGridViewOperationTypes });
        }

        private void LoadOperationTypes()
        {
            try
            {
                _operationTypes = _operationTypeService.GetAllOperationsTypes(_searchTerm).ToList();
                UpdateDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل أنواع العمليات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDataGridView()
        {
            dataGridViewOperationTypes.Rows.Clear();
            foreach (var type in _operationTypes)
            {
                dataGridViewOperationTypes.Rows.Add(type.Name);
            }
            if (_operationTypes.Count == 0)
            {
                dataGridViewOperationTypes.Rows.Add("لا توجد أنواع عمليات متاحة", "", "");
            }
        }

        private void BtnAddOperationType_Click(object sender, EventArgs e)
        {
            using (var form = new OperationTypeForm(_operationTypeService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadOperationTypes();
                }
            }
        }

        private void DataGridViewOperationTypes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _operationTypes.Count) return;

            var selectedType = _operationTypes[e.RowIndex];

            // تعديل
            if (e.ColumnIndex == dataGridViewOperationTypes.Columns["Edit"].Index)
            {
                using (var form = new OperationTypeForm(_operationTypeService, selectedType))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadOperationTypes();
                    }
                }
            }
            // حذف
            else if (e.ColumnIndex == dataGridViewOperationTypes.Columns["Delete"].Index)
            {
                if (MessageBox.Show("هل أنت متأكد من حذف نوع العملية؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _operationTypeService.DeleteOperationType(selectedType.Id);
                        MessageBox.Show("تم حذف نوع العملية بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOperationTypes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ أثناء الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}