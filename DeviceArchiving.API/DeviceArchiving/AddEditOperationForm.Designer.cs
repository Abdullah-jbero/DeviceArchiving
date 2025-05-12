namespace DeviceArchiving
{
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
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label lblBrotherName;
        private System.Windows.Forms.TextBox txtBrotherName;
        private System.Windows.Forms.Label lblLaptopName;
        private System.Windows.Forms.TextBox txtLaptopName;
        private System.Windows.Forms.Label lblSystemPassword;
        private System.Windows.Forms.TextBox txtSystemPassword;
        private System.Windows.Forms.Label lblWindowsPassword;
        private System.Windows.Forms.TextBox txtWindowsPassword;
        private System.Windows.Forms.Label lblHardDrivePassword;
        private System.Windows.Forms.TextBox txtHardDrivePassword;
        private System.Windows.Forms.Label lblFreezePassword;
        private System.Windows.Forms.TextBox txtFreezePassword;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.Label lblCard;
        private System.Windows.Forms.TextBox txtCard;
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
            lblSource = new System.Windows.Forms.Label();
            txtSource = new System.Windows.Forms.TextBox();
            lblBrotherName = new System.Windows.Forms.Label();
            txtBrotherName = new System.Windows.Forms.TextBox();
            lblLaptopName = new System.Windows.Forms.Label();
            txtLaptopName = new System.Windows.Forms.TextBox();
            lblSystemPassword = new System.Windows.Forms.Label();
            txtSystemPassword = new System.Windows.Forms.TextBox();
            lblWindowsPassword = new System.Windows.Forms.Label();
            txtWindowsPassword = new System.Windows.Forms.TextBox();
            lblHardDrivePassword = new System.Windows.Forms.Label();
            txtHardDrivePassword = new System.Windows.Forms.TextBox();
            lblFreezePassword = new System.Windows.Forms.Label();
            txtFreezePassword = new System.Windows.Forms.TextBox();
            lblCode = new System.Windows.Forms.Label();
            txtCode = new System.Windows.Forms.TextBox();
            lblType = new System.Windows.Forms.Label();
            txtType = new System.Windows.Forms.TextBox();
            lblCard = new System.Windows.Forms.Label();
            txtCard = new System.Windows.Forms.TextBox();
            chkIsActive = new System.Windows.Forms.CheckBox();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();

            // إعداد العناصر المختلفة
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(12, 20);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(35, 13);
            lblName.Text = "اسم:";

            txtName.Location = new System.Drawing.Point(120, 17);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(200, 20);

            lblSerialNumber.AutoSize = true;
            lblSerialNumber.Location = new System.Drawing.Point(12, 50);
            lblSerialNumber.Name = "lblSerialNumber";
            lblSerialNumber.Size = new System.Drawing.Size(74, 13);
            lblSerialNumber.Text = "رقم السيريال:";

            txtSerialNumber.Location = new System.Drawing.Point(120, 47);
            txtSerialNumber.Name = "txtSerialNumber";
            txtSerialNumber.Size = new System.Drawing.Size(200, 20);

            lblVersion.AutoSize = true;
            lblVersion.Location = new System.Drawing.Point(12, 80);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new System.Drawing.Size(45, 13);
            lblVersion.Text = "الإصدار:";

            txtVersion.Location = new System.Drawing.Point(120, 77);
            txtVersion.Name = "txtVersion";
            txtVersion.Size = new System.Drawing.Size(200, 20);

            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(12, 110);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(56, 13);
            lblPassword.Text = "كلمة السر:";

            txtPassword.Location = new System.Drawing.Point(120, 107);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(200, 20);

            lblEncryptionKey.AutoSize = true;
            lblEncryptionKey.Location = new System.Drawing.Point(12, 140);
            lblEncryptionKey.Name = "lblEncryptionKey";
            lblEncryptionKey.Size = new System.Drawing.Size(80, 13);
            lblEncryptionKey.Text = "مفتاح التشفير:";

            txtEncryptionKey.Location = new System.Drawing.Point(120, 137);
            txtEncryptionKey.Name = "txtEncryptionKey";
            txtEncryptionKey.Size = new System.Drawing.Size(200, 20);

            // إضافة حقول جديدة
            lblSource.AutoSize = true;
            lblSource.Location = new System.Drawing.Point(12, 170);
            lblSource.Name = "lblSource";
            lblSource.Size = new System.Drawing.Size(35, 13);
            lblSource.Text = "المصدر:";

            txtSource.Location = new System.Drawing.Point(120, 167);
            txtSource.Name = "txtSource";
            txtSource.Size = new System.Drawing.Size(200, 20);

            lblBrotherName.AutoSize = true;
            lblBrotherName.Location = new System.Drawing.Point(12, 200);
            lblBrotherName.Name = "lblBrotherName";
            lblBrotherName.Size = new System.Drawing.Size(74, 13);
            lblBrotherName.Text = "اسم الأخ:";

            txtBrotherName.Location = new System.Drawing.Point(120, 197);
            txtBrotherName.Name = "txtBrotherName";
            txtBrotherName.Size = new System.Drawing.Size(200, 20);

            lblLaptopName.AutoSize = true;
            lblLaptopName.Location = new System.Drawing.Point(12, 230);
            lblLaptopName.Name = "lblLaptopName";
            lblLaptopName.Size = new System.Drawing.Size(74, 13);
            lblLaptopName.Text = "اسم اللابتوب:";

            txtLaptopName.Location = new System.Drawing.Point(120, 227);
            txtLaptopName.Name = "txtLaptopName";
            txtLaptopName.Size = new System.Drawing.Size(200, 20);

            lblSystemPassword.AutoSize = true;
            lblSystemPassword.Location = new System.Drawing.Point(12, 260);
            lblSystemPassword.Name = "lblSystemPassword";
            lblSystemPassword.Size = new System.Drawing.Size(86, 13);
            lblSystemPassword.Text = "كلمة سر النظام:";

            txtSystemPassword.Location = new System.Drawing.Point(120, 257);
            txtSystemPassword.Name = "txtSystemPassword";
            txtSystemPassword.Size = new System.Drawing.Size(200, 20);

            lblWindowsPassword.AutoSize = true;
            lblWindowsPassword.Location = new System.Drawing.Point(12, 290);
            lblWindowsPassword.Name = "lblWindowsPassword";
            lblWindowsPassword.Size = new System.Drawing.Size(96, 13);
            lblWindowsPassword.Text = "كلمة سر الويندوز:";

            txtWindowsPassword.Location = new System.Drawing.Point(120, 287);
            txtWindowsPassword.Name = "txtWindowsPassword";
            txtWindowsPassword.Size = new System.Drawing.Size(200, 20);

            lblHardDrivePassword.AutoSize = true;
            lblHardDrivePassword.Location = new System.Drawing.Point(12, 320);
            lblHardDrivePassword.Name = "lblHardDrivePassword";
            lblHardDrivePassword.Size = new System.Drawing.Size(97, 13);
            lblHardDrivePassword.Text = "كلمة سر الهارد:";

            txtHardDrivePassword.Location = new System.Drawing.Point(120, 317);
            txtHardDrivePassword.Name = "txtHardDrivePassword";
            txtHardDrivePassword.Size = new System.Drawing.Size(200, 20);

            lblFreezePassword.AutoSize = true;
            lblFreezePassword.Location = new System.Drawing.Point(12, 350);
            lblFreezePassword.Name = "lblFreezePassword";
            lblFreezePassword.Size = new System.Drawing.Size(77, 13);
            lblFreezePassword.Text = "كلمة سر التجميد:";

            txtFreezePassword.Location = new System.Drawing.Point(120, 347);
            txtFreezePassword.Name = "txtFreezePassword";
            txtFreezePassword.Size = new System.Drawing.Size(200, 20);

            lblCode.AutoSize = true;
            lblCode.Location = new System.Drawing.Point(12, 380);
            lblCode.Name = "lblCode";
            lblCode.Size = new System.Drawing.Size(35, 13);
            lblCode.Text = "الكود:";

            txtCode.Location = new System.Drawing.Point(120, 377);
            txtCode.Name = "txtCode";
            txtCode.Size = new System.Drawing.Size(200, 20);

            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(12, 410);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(35, 13);
            lblType.Text = "النوع:";

            txtType.Location = new System.Drawing.Point(120, 407);
            txtType.Name = "txtType";
            txtType.Size = new System.Drawing.Size(200, 20);

            lblCard.AutoSize = true;
            lblCard.Location = new System.Drawing.Point(12, 440);
            lblCard.Name = "lblCard";
            lblCard.Size = new System.Drawing.Size(35, 13);
            lblCard.Text = "الكرت:";

            txtCard.Location = new System.Drawing.Point(120, 437);
            txtCard.Name = "txtCard";
            txtCard.Size = new System.Drawing.Size(200, 20);

            // إعداد chkIsActive
            chkIsActive.AutoSize = true;
            chkIsActive.Location = new System.Drawing.Point(120, 467);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new System.Drawing.Size(80, 17);
            chkIsActive.Text = "نشط";
            chkIsActive.Checked = true;

            // إعداد الأزرار
            btnSave.Location = new System.Drawing.Point(120, 500);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.Text = "حفظ";
            btnSave.Click += new System.EventHandler(btnSave_Click);

            btnCancel.Location = new System.Drawing.Point(201, 500);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.Text = "إلغاء";
            btnCancel.Click += new System.EventHandler(btnCancel_Click);

            // إعداد النموذج
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(350, 550);
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
            Controls.Add(lblSource);
            Controls.Add(txtSource);
            Controls.Add(lblBrotherName);
            Controls.Add(txtBrotherName);
            Controls.Add(lblLaptopName);
            Controls.Add(txtLaptopName);
            Controls.Add(lblSystemPassword);
            Controls.Add(txtSystemPassword);
            Controls.Add(lblWindowsPassword);
            Controls.Add(txtWindowsPassword);
            Controls.Add(lblHardDrivePassword);
            Controls.Add(txtHardDrivePassword);
            Controls.Add(lblFreezePassword);
            Controls.Add(txtFreezePassword);
            Controls.Add(lblCode);
            Controls.Add(txtCode);
            Controls.Add(lblType);
            Controls.Add(txtType);
            Controls.Add(lblCard);
            Controls.Add(txtCard);
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
}