namespace DeviceArchiving;
partial class AddEditDeviceForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label lblSerialNumber;
    private System.Windows.Forms.TextBox txtSerialNumber;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.TextBox txtVersion;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Label lblEncryptionKey;
    private System.Windows.Forms.TextBox txtEncryptionKey;
    private System.Windows.Forms.CheckBox chkIsActive;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblName = new System.Windows.Forms.Label();
        txtName = new System.Windows.Forms.TextBox();
        lblSerialNumber = new System.Windows.Forms.Label();
        txtSerialNumber = new System.Windows.Forms.TextBox();
        lblVersion = new System.Windows.Forms.Label();
        txtVersion = new System.Windows.Forms.TextBox();
        lblPassword = new System.Windows.Forms.Label();
        txtPassword = new System.Windows.Forms.TextBox();
        lblEncryptionKey = new System.Windows.Forms.Label();
        txtEncryptionKey = new System.Windows.Forms.TextBox();
        chkIsActive = new System.Windows.Forms.CheckBox();
        btnSave = new System.Windows.Forms.Button();
        btnCancel = new System.Windows.Forms.Button();
        SuspendLayout();
        lblName.AutoSize = true;
        lblName.Location = new System.Drawing.Point(12, 20);
        lblName.Name = "lblName";
        lblName.Size = new System.Drawing.Size(35, 13);
        lblName.Text = "Name:";
        txtName.Location = new System.Drawing.Point(120, 17);
        txtName.Name = "txtName";
        txtName.Size = new System.Drawing.Size(200, 20);
        lblSerialNumber.AutoSize = true;
        lblSerialNumber.Location = new System.Drawing.Point(12, 50);
        lblSerialNumber.Name = "lblSerialNumber";
        lblSerialNumber.Size = new System.Drawing.Size(74, 13);
        lblSerialNumber.Text = "Serial Number:";
        txtSerialNumber.Location = new System.Drawing.Point(120, 47);
        txtSerialNumber.Name = "txtSerialNumber";
        txtSerialNumber.Size = new System.Drawing.Size(200, 20);
        lblVersion.AutoSize = true;
        lblVersion.Location = new System.Drawing.Point(12, 80);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new System.Drawing.Size(45, 13);
        lblVersion.Text = "Version:";
        txtVersion.Location = new System.Drawing.Point(120, 77);
        txtVersion.Name = "txtVersion";
        txtVersion.Size = new System.Drawing.Size(200, 20);
        lblPassword.AutoSize = true;
        lblPassword.Location = new System.Drawing.Point(12, 110);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new System.Drawing.Size(56, 13);
        lblPassword.Text = "Password:";
        txtPassword.Location = new System.Drawing.Point(120, 107);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new System.Drawing.Size(200, 20);
        lblEncryptionKey.AutoSize = true;
        lblEncryptionKey.Location = new System.Drawing.Point(12, 140);
        lblEncryptionKey.Name = "lblEncryptionKey";
        lblEncryptionKey.Size = new System.Drawing.Size(80, 13);
        lblEncryptionKey.Text = "Encryption Key:";
        txtEncryptionKey.Location = new System.Drawing.Point(120, 137);
        txtEncryptionKey.Name = "txtEncryptionKey";
        txtEncryptionKey.Size = new System.Drawing.Size(200, 20);
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new System.Drawing.Point(120, 167);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new System.Drawing.Size(80, 17);
        chkIsActive.Text = "Is Active";
        chkIsActive.Checked = true;
        btnSave.Location = new System.Drawing.Point(120, 200);
        btnSave.Name = "btnSave";
        btnSave.Size = new System.Drawing.Size(75, 23);
        btnSave.Text = "Save";
        btnSave.Click += new System.EventHandler(btnSave_Click);
        btnCancel.Location = new System.Drawing.Point(201, 200);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new System.Drawing.Size(75, 23);
        btnCancel.Text = "Cancel";
        //btnCancel.Click += btnCancel_Click;
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(350, 250);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblSerialNumber);
        Controls.Add(txtSerialNumber);
        Controls.Add(lblVersion);
        Controls.Add(txtVersion);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(lblEncryptionKey);
        Controls.Add(txtEncryptionKey);
        Controls.Add(chkIsActive);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AddEditDeviceForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ResumeLayout(false);
        PerformLayout();
    }
}