using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class OperationTypeForm : Form
    {
        private readonly IOperationTypeService _operationTypeService;
        private readonly OperationType _operationType;
        private List<OperationType> _operationTypes = new List<OperationType>();

        public OperationTypeForm(IOperationTypeService operationTypeService, OperationType operationType = null)
        {
            _operationTypeService = operationTypeService;
            _operationType = operationType;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
            LoadOperationTypes();
        }

        private void SetupUI()
        {
            this.Text = _operationType == null ? "إضافة نوع عملية" : "تعديل نوع عملية";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ComboBox لاختيار اسم نوع العملية
            Label lblName = new Label { Text = "اسم نوع العملية:", Location = new Point(50, 50), AutoSize = true };
            ComboBox cmbName = new ComboBox
            {
                Location = new Point(150, 50),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cmbName"
            };

            // TextBox للوصف
            Label lblDescription = new Label { Text = "الوصف:", Location = new Point(50, 100), AutoSize = true };
            TextBox txtDescription = new TextBox
            {
                Location = new Point(150, 100),
                Width = 200,
                Height = 60,
                Multiline = true,
                Name = "txtDescription"
            };

            // أزرار الحفظ والإلغاء
            Button btnSave = new Button { Text = "حفظ", Location = new Point(100, 180), Width = 100 };
            Button btnCancel = new Button { Text = "إلغاء", Location = new Point(210, 180), Width = 100 };

            // تعيين القيم الافتراضية إذا كان تعديل
            if (_operationType != null)
            {
                txtDescription.Text = _operationType.Description ?? "";
                // سيتم تعيين القيمة في ComboBox بعد تحميل الأنواع
            }

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { lblName, cmbName, lblDescription, txtDescription, btnSave, btnCancel });
        }

        private void LoadOperationTypes()
        {
            try
            {
                _operationTypes = _operationTypeService.GetAllOperationsTypes(null).ToList(); // جلب جميع الأنواع بدون فلترة
                var cmbName = this.Controls["cmbName"] as ComboBox;
                if (cmbName != null)
                {
                    cmbName.Items.Clear();
                    cmbName.Items.AddRange(_operationTypes.Select(t => t.Name).ToArray());
                    if (_operationTypes.Any() && _operationType != null)
                    {
                        cmbName.SelectedItem = _operationType.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل أنواع العمليات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var cmbName = this.Controls["cmbName"] as ComboBox;
            var txtDescription = this.Controls["txtDescription"] as TextBox;

            if (cmbName.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار اسم نوع العملية", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_operationType == null)
                {
                    var operationType = new OperationType
                    {
                        Name = cmbName.SelectedItem.ToString(),
                        Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text
                    };
                    _operationTypeService.AddOperationType(operationType);
                    MessageBox.Show("تم إضافة نوع العملية بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    _operationType.Name = cmbName.SelectedItem.ToString();
                    _operationType.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text;
                    _operationTypeService.UpdateOperationType(_operationType);
                    MessageBox.Show("تم تعديل نوع العملية بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء حفظ نوع العملية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}