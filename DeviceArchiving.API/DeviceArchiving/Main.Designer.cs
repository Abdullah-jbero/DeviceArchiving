namespace DeviceArchiving
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridViewDevices;
        private System.Windows.Forms.DataGridView dataGridViewOperations;
        private System.Windows.Forms.Button btnAddDevice;
        private System.Windows.Forms.Button btnEditDevice;
        private System.Windows.Forms.Button btnDeleteDevice;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox txtSearchOperations;

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
            dataGridViewDevices = new DataGridView();
            dataGridViewOperations = new DataGridView();
            btnAddDevice = new Button();
            btnEditDevice = new Button();
            btnDeleteDevice = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnEditOperation = new Button();
            btnDeleteOperation = new Button();
            btnAddOperation = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            lblSearchDevices = new Label();
            txtSearchDevices = new TextBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            lblSearchOperations = new Label();
            txtSearchOperations = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewDevices).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOperations).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridViewDevices
            // 
            dataGridViewDevices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewDevices.Dock = DockStyle.Fill;
            dataGridViewDevices.Location = new Point(4, 53);
            dataGridViewDevices.Margin = new Padding(4, 3, 4, 3);
            dataGridViewDevices.Name = "dataGridViewDevices";
            dataGridViewDevices.Size = new Size(656, 347);
            dataGridViewDevices.TabIndex = 0;
            dataGridViewDevices.SelectionChanged += dataGridViewDevices_SelectionChanged;
            // 
            // dataGridViewOperations
            // 
            dataGridViewOperations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOperations.Dock = DockStyle.Fill;
            dataGridViewOperations.Location = new Point(668, 53);
            dataGridViewOperations.Margin = new Padding(4, 3, 4, 3);
            dataGridViewOperations.Name = "dataGridViewOperations";
            dataGridViewOperations.Size = new Size(318, 347);
            dataGridViewOperations.TabIndex = 1;
            // 
            // btnAddDevice
            // 
            btnAddDevice.Dock = DockStyle.Fill;
            btnAddDevice.Location = new Point(4, 3);
            btnAddDevice.Margin = new Padding(4, 3, 4, 3);
            btnAddDevice.Name = "btnAddDevice";
            btnAddDevice.Size = new Size(211, 38);
            btnAddDevice.TabIndex = 2;
            btnAddDevice.Text = "Add Device";
            btnAddDevice.UseVisualStyleBackColor = true;
            btnAddDevice.Click += btnAddDevice_Click;
            // 
            // btnEditDevice
            // 
            btnEditDevice.Dock = DockStyle.Fill;
            btnEditDevice.Location = new Point(223, 3);
            btnEditDevice.Margin = new Padding(4, 3, 4, 3);
            btnEditDevice.Name = "btnEditDevice";
            btnEditDevice.Size = new Size(211, 38);
            btnEditDevice.TabIndex = 3;
            btnEditDevice.Text = "Edit Device";
            btnEditDevice.UseVisualStyleBackColor = true;
            btnEditDevice.Click += btnEditDevice_Click;
            // 
            // btnDeleteDevice
            // 
            btnDeleteDevice.Dock = DockStyle.Fill;
            btnDeleteDevice.Location = new Point(442, 3);
            btnDeleteDevice.Margin = new Padding(4, 3, 4, 3);
            btnDeleteDevice.Name = "btnDeleteDevice";
            btnDeleteDevice.Size = new Size(212, 38);
            btnDeleteDevice.TabIndex = 4;
            btnDeleteDevice.Text = "Delete Device";
            btnDeleteDevice.UseVisualStyleBackColor = true;
            btnDeleteDevice.Click += btnDeleteDevice_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67.07071F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32.9292946F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
            tableLayoutPanel1.Controls.Add(dataGridViewOperations, 1, 1);
            tableLayoutPanel1.Controls.Add(dataGridViewDevices, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel5, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(990, 453);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.Controls.Add(btnEditOperation, 1, 0);
            tableLayoutPanel3.Controls.Add(btnDeleteOperation, 2, 0);
            tableLayoutPanel3.Controls.Add(btnAddOperation, 0, 0);
            tableLayoutPanel3.Location = new Point(667, 406);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(320, 44);
            tableLayoutPanel3.TabIndex = 15;
            // 
            // btnEditOperation
            // 
            btnEditOperation.Dock = DockStyle.Fill;
            btnEditOperation.Location = new Point(110, 3);
            btnEditOperation.Margin = new Padding(4, 3, 4, 3);
            btnEditOperation.Name = "btnEditOperation";
            btnEditOperation.Size = new Size(98, 38);
            btnEditOperation.TabIndex = 9;
            btnEditOperation.Text = "Edit Operation";
            btnEditOperation.UseVisualStyleBackColor = true;
            // 
            // btnDeleteOperation
            // 
            btnDeleteOperation.Dock = DockStyle.Fill;
            btnDeleteOperation.Location = new Point(216, 3);
            btnDeleteOperation.Margin = new Padding(4, 3, 4, 3);
            btnDeleteOperation.Name = "btnDeleteOperation";
            btnDeleteOperation.Size = new Size(100, 38);
            btnDeleteOperation.TabIndex = 8;
            btnDeleteOperation.Text = "Delete Operation";
            btnDeleteOperation.UseVisualStyleBackColor = true;
            // 
            // btnAddOperation
            // 
            btnAddOperation.Dock = DockStyle.Fill;
            btnAddOperation.Location = new Point(4, 3);
            btnAddOperation.Margin = new Padding(4, 3, 4, 3);
            btnAddOperation.Name = "btnAddOperation";
            btnAddOperation.Size = new Size(98, 38);
            btnAddOperation.TabIndex = 6;
            btnAddOperation.Text = "Add Operation";
            btnAddOperation.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.Controls.Add(btnAddDevice, 0, 0);
            tableLayoutPanel2.Controls.Add(btnEditDevice, 1, 0);
            tableLayoutPanel2.Controls.Add(btnDeleteDevice, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 406);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(658, 44);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(lblSearchDevices, 0, 0);
            tableLayoutPanel4.Controls.Add(txtSearchDevices, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(658, 44);
            tableLayoutPanel4.TabIndex = 13;
            // 
            // lblSearchDevices
            // 
            lblSearchDevices.AutoSize = true;
            lblSearchDevices.Location = new Point(3, 0);
            lblSearchDevices.Name = "lblSearchDevices";
            lblSearchDevices.Size = new Size(88, 15);
            lblSearchDevices.TabIndex = 10;
            lblSearchDevices.Text = "Search Devices:";
            // 
            // txtSearchDevices
            // 
            txtSearchDevices.AllowDrop = true;
            txtSearchDevices.Dock = DockStyle.Fill;
            txtSearchDevices.Location = new Point(97, 3);
            txtSearchDevices.Name = "txtSearchDevices";
            txtSearchDevices.Size = new Size(558, 23);
            txtSearchDevices.TabIndex = 8;
            txtSearchDevices.TextChanged += txtSearchDevices_TextChanged;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Controls.Add(lblSearchOperations, 0, 0);
            tableLayoutPanel5.Controls.Add(txtSearchOperations, 1, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(667, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(320, 44);
            tableLayoutPanel5.TabIndex = 14;
            // 
            // lblSearchOperations
            // 
            lblSearchOperations.AllowDrop = true;
            lblSearchOperations.Anchor = AnchorStyles.None;
            lblSearchOperations.AutoSize = true;
            lblSearchOperations.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSearchOperations.Location = new Point(3, 15);
            lblSearchOperations.Name = "lblSearchOperations";
            lblSearchOperations.Size = new Size(105, 13);
            lblSearchOperations.TabIndex = 11;
            lblSearchOperations.Text = "Search Operations:";
            // 
            // txtSearchOperations
            // 
            txtSearchOperations.Dock = DockStyle.Fill;
            txtSearchOperations.Location = new Point(114, 3);
            txtSearchOperations.Name = "txtSearchOperations";
            txtSearchOperations.Size = new Size(203, 23);
            txtSearchOperations.TabIndex = 10;
            txtSearchOperations.TextChanged += txtSearchOperations_TextChanged;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(990, 453);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Main";
            Text = "Device Archiving";
            ((System.ComponentModel.ISupportInitialize)dataGridViewDevices).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOperations).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private TextBox txtSearchDevices;
        private TableLayoutPanel tableLayoutPanel4;
        private Label lblSearchDevices;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnEditOperation;
        private Button btnDeleteOperation;
        private Button btnAddOperation;
        private TableLayoutPanel tableLayoutPanel5;
        private Label lblSearchOperations;
    }
}