using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class OperationTypeForm : Form
    {
        private readonly IOperationTypeService _operationTypeService;
        private OperationType _operationType;
        private bool _isEditMode;

        public OperationTypeForm(IOperationTypeService operationTypeService, OperationType operationType = null)
        {
            _operationTypeService = operationTypeService;
            _operationType = operationType ?? new OperationType();
            _isEditMode = operationType != null;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = _isEditMode ? "تعديل نوع العملية" : "إضافة نوع العملية";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Label و TextBox للاسم
            Label lblName = new Label { Text = "الاسم: *", Location = new Point(50, 30), AutoSize = true };
            TextBox txtName = new TextBox
            {
                Location = new Point(150, 30),
                Width = 200,
                Name = "txtName",
                Text = _operationType.Name ?? ""
            };

            // أزرار الحفظ والإلغاء
            Button btnSave = new Button { Text = "حفظ", Location = new Point(150, 80), Width = 100 };
            Button btnCancel = new Button { Text = "إلغاء", Location = new Point(260, 80), Width = 100 };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { lblName, txtName, btnSave, btnCancel });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var txtName = this.Controls["txtName"] as TextBox;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("الاسم مطلوب.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _operationType.Name = txtName.Text;
                if (_isEditMode)
                {
                    _operationTypeService.UpdateOperationType(_operationType);
                    MessageBox.Show("تم تحديث نوع العملية بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _operationTypeService.AddOperationType(_operationType);
                    MessageBox.Show("تم إضافة نوع العملية بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء حفظ نوع العملية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}