using DeviceArchiving.Data.Dto;
using DeviceArchiving.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeviceArchiving.WindowsForm.Forms
{
    public partial class AddOperationDialog : Form
    {
        private readonly IOperationService _operationService;
        private readonly int _deviceId;
        private readonly string _deviceName;

        public AddOperationDialog(IOperationService operationService, int deviceId, string deviceName)
        {
            _operationService = operationService;
            _deviceId = deviceId;
            _deviceName = deviceName;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = $"إضافة عملية جديدة لـ {_deviceName}";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblOperationName = new Label { Text = "اسم العملية:", Location = new Point(20, 20), AutoSize = true };
            TextBox txtOperationName = new TextBox { Location = new Point(100, 20), Width = 250, Name = "txtOperationName" };
            Label lblOldValue = new Label { Text = "القيمة القديمة:", Location = new Point(20, 50), AutoSize = true };
            TextBox txtOldValue = new TextBox { Location = new Point(100, 50), Width = 250, Name = "txtOldValue" };
            Label lblNewValue = new Label { Text = "القيمة الجديدة:", Location = new Point(20, 80), AutoSize = true };
            TextBox txtNewValue = new TextBox { Location = new Point(100, 80), Width = 250, Name = "txtNewValue" };
            Label lblComment = new Label { Text = "التعليق:", Location = new Point(20, 110), AutoSize = true };
            TextBox txtComment = new TextBox { Location = new Point(100, 110), Width = 250, Height = 60, Multiline = true, Name = "txtComment" };

            Button btnSave = new Button { Text = "حفظ", Location = new Point(150, 190), Width = 100 };
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { lblOperationName, txtOperationName, lblOldValue, txtOldValue, lblNewValue, txtNewValue, lblComment, txtComment, btnSave });
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            var txtOperationName = this.Controls["txtOperationName"] as TextBox;
            var txtOldValue = this.Controls["txtOldValue"] as TextBox;
            var txtNewValue = this.Controls["txtNewValue"] as TextBox;
            var txtComment = this.Controls["txtComment"] as TextBox;

            if (string.IsNullOrWhiteSpace(txtOperationName.Text))
            {
                MessageBox.Show("اسم العملية مطلوب.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var operation = new CreateOperation
                {
                    DeviceId = _deviceId,
                    OperationName = txtOperationName.Text,
                    OldValue = string.IsNullOrWhiteSpace(txtOldValue.Text) ? null : txtOldValue.Text,
                    NewValue = string.IsNullOrWhiteSpace(txtNewValue.Text) ? null : txtNewValue.Text,
                    Comment = string.IsNullOrWhiteSpace(txtComment.Text) ? null : txtComment.Text
                };
                await _operationService.AddOperations(operation);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء إضافة العملية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


}
