using DeviceArchiving.Data.Dto.Users;
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

public partial class SignupForm : Form
{
    private readonly IAccountService _accountService;

    public SignupForm(IAccountService accountService)
    {
        _accountService = accountService;
        InitializeComponentSignupForm();
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
    }

    private void InitializeComponentSignupForm()
    {
        this.Text = "إنشاء حساب جديد";
        this.Size = new Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;

        Label lblUsername = new Label { Text = "اسم المستخدم", Location = new Point(50, 50), AutoSize = true };
        TextBox txtUsername = new TextBox { Location = new Point(150, 50), Width = 200, Name = "txtUsername" };
        Label lblPassword = new Label { Text = "كلمة المرور", Location = new Point(50, 100), AutoSize = true };
        TextBox txtPassword = new TextBox { Location = new Point(150, 100), Width = 200, UseSystemPasswordChar = true, Name = "txtPassword" };
        Button btnSave = new Button { Text = "إنشاء", Location = new Point(150, 150), Width = 100 };

        btnSave.Click += BtnSave_Click;

        this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, btnSave });
    }

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        TextBox txtUsername = this.Controls["txtUsername"] as TextBox;
        TextBox txtPassword = this.Controls["txtPassword"] as TextBox;

        try
        {
            var request = new AuthenticationRequest { UserName = txtUsername.Text, Password = txtPassword.Text };
            var response = await _accountService.AddUserAsync(request);
            if (response.Success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(response.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ أثناء إنشاء الحساب: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}