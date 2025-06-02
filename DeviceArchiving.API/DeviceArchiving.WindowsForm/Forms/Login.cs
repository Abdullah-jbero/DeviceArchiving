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
public partial class LoginForm : Form
{
    private readonly IAccountService _accountService;
    public AuthenticationResponse AuthResponse { get; private set; }
    public int LoggedInUserId => AuthResponse?.Id ?? 0;

    public LoginForm(IAccountService accountService)
    {
        _accountService = accountService;
        SetupUI();
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
    }

    private void SetupUI()
    {
        this.Text = "تسجيل الدخول";
        this.Size = new Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;

        Label lblUsername = new Label { Text = "اسم المستخدم:", Location = new Point(50, 50), AutoSize = true };
        TextBox txtUsername = new TextBox { Location = new Point(150, 50), Width = 200, Name = "txtUsername" };
        Label lblPassword = new Label { Text = "كلمة المرور:", Location = new Point(50, 100), AutoSize = true };
        TextBox txtPassword = new TextBox { Location = new Point(150, 100), Width = 200, UseSystemPasswordChar = true, Name = "txtPassword" };
        Button btnLogin = new Button { Text = "تسجيل الدخول", Location = new Point(150, 150), Width = 100 };
        Button btnSignup = new Button { Text = "إنشاء حساب", Location = new Point(260, 150), Width = 100 };

        btnLogin.Click += BtnLogin_Click;
        btnSignup.Click += BtnSignup_Click;

        this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, btnLogin, btnSignup });
    }

    private async void BtnLogin_Click(object sender, EventArgs e)
    {
        TextBox txtUsername = this.Controls["txtUsername"] as TextBox;
        TextBox txtPassword = this.Controls["txtPassword"] as TextBox;

        try
        {
            var request = new AuthenticationRequest { UserName = txtUsername.Text, Password = txtPassword.Text };
            var response = await _accountService.AuthenticateAsync(request);
            if (response.Success)
            {
                AppSession.CurrentUserId = response.Data.Id;
                MainForm mainForm = new MainForm(_accountService, response.Data);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(response.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSignup_Click(object sender, EventArgs e)
    {
        SignupForm signupForm = new SignupForm(_accountService);
        if (signupForm.ShowDialog() == DialogResult.OK)
        {
            MessageBox.Show("تم إنشاء الحساب بنجاح، يرجى تسجيل الدخول.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
