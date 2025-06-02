using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class AddOperationDialog : Form
    {
        private readonly IOperationTypeService _operationTypeService;
        private readonly IOperationService _operationService;
        private readonly int _deviceId;
        private readonly string _laptopName;
        private Operation _operation = new Operation { OperationName = "", OldValue = null, NewValue = null, Comment = null };
        private List<OperationType> _operationTypes = new List<OperationType>();

        public AddOperationDialog(IOperationTypeService operationTypeService, IOperationService operationService, int deviceId, string laptopName)
        {
            _operationTypeService = operationTypeService;
            _operationService = operationService;
            _deviceId = deviceId;
            _laptopName = laptopName;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
            LoadOperationTypes();
        }

        private void SetupUI()
        {
            this.Text = $"إضافة عملية لجهاز: {_laptopName}";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblOperationType = new Label { Text = "نوع العملية:", Location = new Point(50, 30), AutoSize = true };
            ComboBox cmbOperationType = new ComboBox
            {
                Location = new Point(150, 30),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cmbOperationType"
            };

            Label lblOldValue = new Label { Text = "القيمة القديمة:", Location = new Point(50, 70), AutoSize = true };
            TextBox txtOldValue = new TextBox { Location = new Point(150, 70), Width = 200, Name = "txtOldValue" };

            Label lblNewValue = new Label { Text = "القيمة الجديدة:", Location = new Point(50, 110), AutoSize = true };
            TextBox txtNewValue = new TextBox { Location = new Point(150, 110), Width = 200, Name = "txtNewValue" };

            Label lblComment = new Label { Text = "التعليق:", Location = new Point(50, 150), AutoSize = true };
            TextBox txtComment = new TextBox
            {
                Location = new Point(150, 150),
                Width = 200,
                Height = 60,
                Multiline = true,
                Name = "txtComment"
            };

            Button btnSave = new Button { Text = "حفظ", Location = new Point(150, 230), Width = 100 };
            Button btnCancel = new Button { Text = "إلغاء", Location = new Point(260, 230), Width = 100 };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] { lblOperationType, cmbOperationType, lblOldValue, txtOldValue, lblNewValue, txtNewValue, lblComment, txtComment, btnSave, btnCancel });
        }

        private void LoadOperationTypes()
        {
            try
            {
                _operationTypes = _operationTypeService.GetAllOperationsTypes(null).ToList();
                var cmbOperationType = this.Controls["cmbOperationType"] as ComboBox;
                if (cmbOperationType != null)
                {
                    cmbOperationType.Items.Clear();
                    cmbOperationType.Items.AddRange(_operationTypes.Select(t => t.Name).ToArray());
                    if (_operationTypes.Any())
                    {
                        cmbOperationType.SelectedIndex = 0;
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
            var cmbOperationType = this.Controls["cmbOperationType"] as ComboBox;
            var txtOldValue = this.Controls["txtOldValue"] as TextBox;
            var txtNewValue = this.Controls["txtNewValue"] as TextBox;
            var txtComment = this.Controls["txtComment"] as TextBox;

            if (cmbOperationType.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار نوع العملية", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _operation.OperationName = cmbOperationType.SelectedItem.ToString();
            _operation.OldValue = string.IsNullOrWhiteSpace(txtOldValue.Text) ? null : txtOldValue.Text;
            _operation.NewValue = string.IsNullOrWhiteSpace(txtNewValue.Text) ? null : txtNewValue.Text;
            _operation.Comment = string.IsNullOrWhiteSpace(txtComment.Text) ? null : txtComment.Text;

            var createOperation = new CreateOperation()
            {
                DeviceId = _deviceId,
                NewValue = _operation.NewValue,
                OldValue = _operation.OldValue,
                OperationName = cmbOperationType.SelectedItem.ToString(),
                Comment = _operation.Comment,

            };
            _operationService.AddOperations(createOperation);

            MessageBox.Show("تم حفظ العملية بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // خاصية لاسترجاع العملية بعد الحفظ
        public Operation Operation => _operation;
    }
}