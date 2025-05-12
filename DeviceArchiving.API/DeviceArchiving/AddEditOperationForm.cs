using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Windows.Forms;

namespace DeviceArchiving
{
    public partial class AddEditOperationForm : Form
    {
        private readonly IDeviceService service;
        private readonly Operation? _operation;
        private readonly int _deviceId;

        public AddEditOperationForm(IDeviceService service, Operation? operation, int deviceId)
        {
            InitializeComponent();
            this.service = service;
            _operation = operation;
            _deviceId = deviceId;

            if (_operation != null)
            {
                txtName.Text = _operation.Name;
                txtDescription.Text = _operation.Description;
                dtpOperationDate.Value = _operation.OperationDate;
                Text = "Edit Operation";
            }
            else
            {
                Text = "Add Operation";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_operation == null)
            {
                var operation = new Operation
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    OperationDate = dtpOperationDate.Value,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Operations.Add(operation);
                _context.SaveChanges(); // Save to get OperationId
                _context.DeviceOperations.Add(new DeviceOperation
                {
                    DeviceId = _deviceId,
                    OperationId = operation.Id,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                _operation.Name = txtName.Text;
                _operation.Description = txtDescription.Text;
                _operation.OperationDate = dtpOperationDate.Value;
                _operation.UpdatedAt = DateTime.UtcNow;
            }

            try
            {
                _context.SaveChanges();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}