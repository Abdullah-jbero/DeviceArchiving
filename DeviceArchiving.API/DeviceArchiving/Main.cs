using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DeviceArchiving
{
    public partial class Main : Form
    {
        private readonly IDeviceService service;
        private string _deviceSearchText = "";
        private string _operationSearchText = "";

        public Main(IDeviceService service)
        {
            InitializeComponent();
            this.service = service;
            LoadDevices();
        }

        private void LoadDevices()
        {
            var devices = service.GetAllDevices().ToList();
            dataGridViewDevices.DataSource = devices;
            var column = dataGridViewDevices.Columns["DeviceOperations"];
            if (column != null)
            {
                column.Visible = false;
            }
            dataGridViewOperations.DataSource = null;
        }

        private void txtSearchDevices_TextChanged(object sender, EventArgs e)
        {
            _deviceSearchText = txtSearchDevices.Text.Trim();
            LoadDevices();
        }

        private void txtSearchOperations_TextChanged(object sender, EventArgs e)
        {
            _operationSearchText = txtSearchOperations.Text.Trim();
            dataGridViewDevices_SelectionChanged(sender, e);
        }

        private void dataGridViewDevices_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewDevices.SelectedRows.Count > 0)
            {
                var device = (Device)dataGridViewDevices.SelectedRows[0].DataBoundItem;
                var operations = service.GetDeviceOperations(device.Id, _operationSearchText);
                dataGridViewOperations.DataSource = operations;
                var column = dataGridViewOperations.Columns["DeviceOperations"];
                if (column != null)
                {
                    column.Visible = false;
                }
            }
            else
            {
                dataGridViewOperations.DataSource = null;
            }
        }

        private void btnAddDevice_Click(object sender, EventArgs e)
        {
            using var form = new AddEditDeviceForm(service, null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDevices();
            }
        }

        private void btnEditDevice_Click(object sender, EventArgs e)
        {
            if (dataGridViewDevices.SelectedRows.Count > 0)
            {
                var device = (Device)dataGridViewDevices.SelectedRows[0].DataBoundItem;
                using var form = new AddEditDeviceForm(service, device);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadDevices();
                }
            }
            else
            {
                MessageBox.Show("Please select a device to edit.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteDevice_Click(object sender, EventArgs e)
        {
            if (dataGridViewDevices.SelectedRows.Count > 0)
            {
                var device = (Device)dataGridViewDevices.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show($"Are you sure you want to delete device {device.LaptopName}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        service.DeleteDevice(device.Id);
                        LoadDevices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting device: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a device to delete.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAddOperation_Click(object sender, EventArgs e)
        {
            if (dataGridViewDevices.SelectedRows.Count > 0)
            {
                var device = (Device)dataGridViewDevices.SelectedRows[0].DataBoundItem;
                using var form = new AddEditOperationForm(service, null, device.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewDevices_SelectionChanged(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Please select a device first.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditOperation_Click(object sender, EventArgs e)
        {
            if (dataGridViewOperations.SelectedRows.Count > 0 && dataGridViewDevices.SelectedRows.Count > 0)
            {
                var operation = (Operation)dataGridViewOperations.SelectedRows[0].DataBoundItem;
                var device = (Device)dataGridViewDevices.SelectedRows[0].DataBoundItem;
                using var form = new AddEditOperationForm(service, operation, device.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewDevices_SelectionChanged(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Please select an operation and a device.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteOperation_Click(object sender, EventArgs e)
        {
            if (dataGridViewOperations.SelectedRows.Count > 0)
            {
                var operation = (Operation)dataGridViewOperations.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show($"Are you sure you want to delete operation {operation.Name}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        service.DeleteOperation(operation);
                        dataGridViewDevices_SelectionChanged(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an operation to delete.", "No Operation Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}