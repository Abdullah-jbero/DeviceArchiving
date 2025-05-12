using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DeviceArchiving;
public partial class AddEditDeviceForm : Form
{
    private readonly IDeviceService service;
    private readonly Device? _device;
    public AddEditDeviceForm(IDeviceService service, Device? device)
    {
        InitializeComponent();
        this.service = service;
        _device = device;
        if (_device != null)
        {
            txtName.Text = _device.Name;
            txtSerialNumber.Text = _device.SerialNumber;
            txtVersion.Text = _device.Version;
            txtPassword.Text = _device.Password;
            txtEncryptionKey.Text = _device.EncryptionKey;
            chkIsActive.Checked = _device.IsActive;
            Text = "Edit Device";
        }
        else
        {
            Text = "Add Device";
        }
    }
    private void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtSerialNumber.Text))
        {
            MessageBox.Show("Name and Serial Number are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (_device == null)
        {
            _context.Devices.Add(new Device
            {
                Name = txtName.Text,
                SerialNumber = txtSerialNumber.Text,
                Version = txtVersion.Text,
                Password = txtPassword.Text,
                EncryptionKey = txtEncryptionKey.Text,
                IsActive = chkIsActive.Checked
            });
        }
        else
        {
            _device.Name = txtName.Text;
            _device.SerialNumber = txtSerialNumber.Text;
            _device.Version = txtVersion.Text;
            _device.Password = txtPassword.Text;
            _device.EncryptionKey = txtEncryptionKey.Text;
            _device.IsActive = chkIsActive.Checked;
            _device.UpdatedAt = DateTime.UtcNow;
        }
        try
        {
            _context.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving device: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}