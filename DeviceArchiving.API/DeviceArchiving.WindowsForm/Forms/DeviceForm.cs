using DeviceArchiving.Data.Dto.Devices;
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

namespace DeviceArchiving.WindowsForm.Forms;
public partial class DeviceForm : Form
{
    private readonly IDeviceService _deviceService;
    private readonly GetAllDevicesDto _device;

    public DeviceForm(IDeviceService deviceService, GetAllDevicesDto device = null)
    {
        _deviceService = deviceService;
        _device = device;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        SetupUI();
    }

    private void SetupUI()
    {
        this.Text = _device == null ? "إضافة جهاز" : "تعديل جهاز";
        this.Size = new Size(450, 600); // حجم أكبر لاستيعاب الحقول الإضافية
        this.StartPosition = FormStartPosition.CenterScreen;

        // إعداد الحقول
        Label lblSource = new Label { Text = "المصدر:", Location = new Point(50, 30), AutoSize = true };
        TextBox txtSource = new TextBox { Location = new Point(150, 30), Width = 250, Name = "txtSource" };
        Label lblBrotherName = new Label { Text = "اسم الأخ:", Location = new Point(50, 60), AutoSize = true };
        TextBox txtBrotherName = new TextBox { Location = new Point(150, 60), Width = 250, Name = "txtBrotherName" };
        Label lblLaptopName = new Label { Text = "اسم اللاب توب:", Location = new Point(50, 90), AutoSize = true };
        TextBox txtLaptopName = new TextBox { Location = new Point(150, 90), Width = 250, Name = "txtLaptopName" };
        Label lblSystemPassword = new Label { Text = "كلمة مرور النظام:", Location = new Point(50, 120), AutoSize = true };
        TextBox txtSystemPassword = new TextBox { Location = new Point(150, 120), Width = 250, Name = "txtSystemPassword", UseSystemPasswordChar = true };
        Label lblWindowsPassword = new Label { Text = "كلمة مرور ويندوز:", Location = new Point(50, 150), AutoSize = true };
        TextBox txtWindowsPassword = new TextBox { Location = new Point(150, 150), Width = 250, Name = "txtWindowsPassword", UseSystemPasswordChar = true };
        Label lblHardDrivePassword = new Label { Text = "كلمة قرص الصلب:", Location = new Point(50, 180), AutoSize = true };
        TextBox txtHardDrivePassword = new TextBox { Location = new Point(150, 180), Width = 250, Name = "txtHardDrivePassword", UseSystemPasswordChar = true };
        Label lblFreezePassword = new Label { Text = "كلمة مرور التجميد:", Location = new Point(50, 210), AutoSize = true };
        TextBox txtFreezePassword = new TextBox { Location = new Point(150, 210), Width = 250, Name = "txtFreezePassword", UseSystemPasswordChar = true };
        Label lblCode = new Label { Text = "الكود:", Location = new Point(50, 240), AutoSize = true };
        TextBox txtCode = new TextBox { Location = new Point(150, 240), Width = 250, Name = "txtCode" };
        Label lblType = new Label { Text = "النوع:", Location = new Point(50, 270), AutoSize = true };
        TextBox txtType = new TextBox { Location = new Point(150, 270), Width = 250, Name = "txtType" };
        Label lblSerialNumber = new Label { Text = "رقم التسلسل:", Location = new Point(50, 300), AutoSize = true };
        TextBox txtSerialNumber = new TextBox { Location = new Point(150, 300), Width = 250, Name = "txtSerialNumber" };
        Label lblComment = new Label { Text = "التعليق:", Location = new Point(50, 330), AutoSize = true };
        TextBox txtComment = new TextBox { Location = new Point(150, 330), Width = 250, Name = "txtComment" };
        Label lblContactNumber = new Label { Text = "رقم التواصل:", Location = new Point(50, 360), AutoSize = true };
        TextBox txtContactNumber = new TextBox { Location = new Point(150, 360), Width = 250, Name = "txtContactNumber" };
        Label lblCard = new Label { Text = "البطاقة:", Location = new Point(50, 390), AutoSize = true };
        TextBox txtCard = new TextBox { Location = new Point(150, 390), Width = 250, Name = "txtCard" };
        Button btnSave = new Button { Text = "حفظ", Location = new Point(150, 430), Width = 100 };

        // تعبئة الحقول إذا كان الجهاز موجودًا (للتعديل)
        if (_device != null)
        {
            txtSource.Text = _device.Source;
            txtBrotherName.Text = _device.BrotherName;
            txtLaptopName.Text = _device.LaptopName;
            txtSystemPassword.Text = _device.SystemPassword;
            txtWindowsPassword.Text = _device.WindowsPassword;
            txtHardDrivePassword.Text = _device.HardDrivePassword;
            txtFreezePassword.Text = _device.FreezePassword;
            txtCode.Text = _device.Code;
            txtType.Text = _device.Type;
            txtSerialNumber.Text = _device.SerialNumber;
            txtComment.Text = _device.Comment;
            txtContactNumber.Text = _device.ContactNumber;
            txtCard.Text = _device.Card;
        }

        btnSave.Click += BtnSave_Click;

        this.Controls.AddRange(new Control[] {
            lblSource, txtSource,
            lblBrotherName, txtBrotherName,
            lblLaptopName, txtLaptopName,
            lblSystemPassword, txtSystemPassword,
            lblWindowsPassword, txtWindowsPassword,
            lblHardDrivePassword, txtHardDrivePassword,
            lblFreezePassword, txtFreezePassword,
            lblCode, txtCode,
            lblType, txtType,
            lblSerialNumber, txtSerialNumber,
            lblComment, txtComment,
            lblContactNumber, txtContactNumber,
            lblCard, txtCard,
            btnSave
        });
    }

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        TextBox txtLaptopName = this.Controls["txtLaptopName"] as TextBox;
        TextBox txtSerialNumber = this.Controls["txtSerialNumber"] as TextBox;
        TextBox txtSource = this.Controls["txtSource"] as TextBox;
        TextBox txtBrotherName = this.Controls["txtBrotherName"] as TextBox;
        TextBox txtSystemPassword = this.Controls["txtSystemPassword"] as TextBox;
        TextBox txtWindowsPassword = this.Controls["txtWindowsPassword"] as TextBox;
        TextBox txtHardDrivePassword = this.Controls["txtHardDrivePassword"] as TextBox;
        TextBox txtFreezePassword = this.Controls["txtFreezePassword"] as TextBox;
        TextBox txtCode = new TextBox { Location = new Point(150, 240), Width = 250, Name = "txtCode" };
        TextBox txtType = new TextBox { Location = new Point(150, 270), Width = 250, Name = "txtType" };
        TextBox txtComment = this.Controls["txtComment"] as TextBox;
        TextBox txtContactNumber = this.Controls["txtContactNumber"] as TextBox;
        TextBox txtCard = this.Controls["txtCard"] as TextBox;

        // التحقق من الحقول المطلوبة
        if (string.IsNullOrWhiteSpace(txtLaptopName.Text))
        {
            MessageBox.Show("اسم اللاب توب مطلوب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (string.IsNullOrWhiteSpace(txtSerialNumber.Text))
        {
            MessageBox.Show("رقم التسلسل مطلوب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            if (_device == null)
            {
                var dto = new CreateDeviceDto
                {
                    Source = txtSource.Text,
                    BrotherName = txtBrotherName.Text,
                    LaptopName = txtLaptopName.Text,
                    SystemPassword = txtSystemPassword.Text,
                    WindowsPassword = txtWindowsPassword.Text,
                    HardDrivePassword = txtHardDrivePassword.Text,
                    FreezePassword = txtFreezePassword.Text,
                    Code = txtCode.Text,
                    Type = txtType.Text,
                    SerialNumber = txtSerialNumber.Text,
                    Comment = txtComment.Text,
                    ContactNumber = txtContactNumber.Text,
                    Card = txtCard.Text
                };
                var response = await _deviceService.AddDeviceAsync(dto);
                if (response.Success)
                {
                    MessageBox.Show("تم إضافة الجهاز بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                var dto = new UpdateDeviceDto
                {
                    Source = txtSource.Text,
                    BrotherName = txtBrotherName.Text,
                    LaptopName = txtLaptopName.Text,
                    SystemPassword = txtSystemPassword.Text,
                    WindowsPassword = txtWindowsPassword.Text,
                    HardDrivePassword = txtHardDrivePassword.Text,
                    FreezePassword = txtFreezePassword.Text,
                    Code = txtCode.Text,
                    Type = txtType.Text,
                    SerialNumber = txtSerialNumber.Text,
                    Comment = txtComment.Text,
                    ContactNumber = txtContactNumber.Text,
                    Card = txtCard.Text
                };
                var response = await _deviceService.UpdateDeviceAsync(_device.Id, dto);
                if (response.Success)
                {
                    MessageBox.Show("تم تعديل الجهاز بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء حفظ الجهاز: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}