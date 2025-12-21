namespace ProjetoFBD
{
    partial class LoginForm : System.Windows.Forms.Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // Instanciação de Componentes
            this.btnVoltar = new System.Windows.Forms.Button();
            this.btnStaff = new System.Windows.Forms.Button();
            this.btnGuest = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.components = new System.ComponentModel.Container();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pnlStaffFields = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();

            // BeginInit
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.pnlStaffFields.SuspendLayout();
            this.tableMain.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.contentLayout.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();

            // --- Painel Principal ---
            this.pnlMain.BackColor = System.Drawing.Color.White;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.TabIndex = 0;
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pnlMain.Controls.Add(this.tableMain);

            // --- Layout Principal ---
            this.tableMain.ColumnCount = 3;
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableMain.RowCount = 3;
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMain.Name = "tableMain";
            this.tableMain.TabIndex = 9;
            this.tableMain.Controls.Add(this.contentLayout, 1, 1);

            // --- Content Layout ---
            this.contentLayout.AutoSize = true;
            this.contentLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.contentLayout.ColumnCount = 1;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contentLayout.RowCount = 5;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.TabIndex = 10;
            this.contentLayout.Anchor = System.Windows.Forms.AnchorStyles.None;

            // --- Título ---
            this.lblTitle.AutoSize = true;
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.lblTitle.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "LOGIN";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.contentLayout.Controls.Add(this.lblTitle, 0, 0);

            // --- Ícone ---
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(80, 80);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbIcon.TabIndex = 2;
            this.pbIcon.TabStop = false;
            this.pbIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbIcon.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.contentLayout.Controls.Add(this.pbIcon, 0, 1);

            // --- Botões de Seleção (Staff / Guest) ---
            this.btnStaff.BackColor = System.Drawing.Color.FromArgb(220, 20, 20);
            this.btnStaff.FlatAppearance.BorderSize = 0;
            this.btnStaff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStaff.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStaff.ForeColor = System.Drawing.Color.White;
            this.btnStaff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStaff.Size = new System.Drawing.Size(150, 45);
            this.btnStaff.Text = "STAFF";
            this.btnStaff.UseVisualStyleBackColor = false;
            this.btnStaff.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(180, 10, 10);
            this.btnStaff.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(240, 30, 30);
            this.btnStaff.Margin = new System.Windows.Forms.Padding(0, 0, 15, 0);

            this.btnGuest.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.btnGuest.FlatAppearance.BorderSize = 0;
            this.btnGuest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuest.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGuest.ForeColor = System.Drawing.Color.White;
            this.btnGuest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuest.Size = new System.Drawing.Size(150, 45);
            this.btnGuest.Text = "GUEST";
            this.btnGuest.UseVisualStyleBackColor = false;
            this.btnGuest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnGuest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.btnGuest.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);

            this.flowButtons.AutoSize = true;
            this.flowButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowButtons.WrapContents = false;
            this.flowButtons.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.TabIndex = 11;
            this.flowButtons.Controls.Add(this.btnStaff);
            this.flowButtons.Controls.Add(this.btnGuest);
            this.flowButtons.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.contentLayout.Controls.Add(this.flowButtons, 0, 2);

            // --- Painel de Campos Staff (Oculto) ---
            this.pnlStaffFields.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.pnlStaffFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStaffFields.Name = "pnlStaffFields";
            this.pnlStaffFields.Size = new System.Drawing.Size(320, 100);
            this.pnlStaffFields.TabIndex = 4;
            this.pnlStaffFields.Visible = false;
            this.pnlStaffFields.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.pnlStaffFields.Anchor = System.Windows.Forms.AnchorStyles.None;

            this.lblUsername.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUsername.Location = new System.Drawing.Point(10, 12);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(75, 20);
            this.lblUsername.Text = "Staff ID:";
            this.pnlStaffFields.Controls.Add(this.lblUsername);

            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.BackColor = System.Drawing.Color.White;
            this.txtUsername.Font = new System.Drawing.Font("Arial", 11F);
            this.txtUsername.Location = new System.Drawing.Point(100, 10);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 26);
            this.txtUsername.TabIndex = 5;
            this.pnlStaffFields.Controls.Add(this.txtUsername);

            this.lblPassword.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPassword.Location = new System.Drawing.Point(10, 50);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(75, 20);
            this.lblPassword.Text = "Password:";
            this.pnlStaffFields.Controls.Add(this.lblPassword);

            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.BackColor = System.Drawing.Color.White;
            this.txtPassword.Font = new System.Drawing.Font("Arial", 11F);
            this.txtPassword.Location = new System.Drawing.Point(100, 48);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 26);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.UseSystemPasswordChar = true;
            this.pnlStaffFields.Controls.Add(this.txtPassword);

            this.contentLayout.Controls.Add(this.pnlStaffFields, 0, 3);

            // --- Botões de Ação (Voltar) ---
            this.btnVoltar.BackColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.btnVoltar.FlatAppearance.BorderSize = 0;
            this.btnVoltar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVoltar.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnVoltar.ForeColor = System.Drawing.Color.White;
            this.btnVoltar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVoltar.Size = new System.Drawing.Size(150, 45);
            this.btnVoltar.Text = "VOLTAR";
            this.btnVoltar.UseVisualStyleBackColor = false;
            this.btnVoltar.Visible = false;
            this.btnVoltar.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btnVoltar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.contentLayout.Controls.Add(this.btnVoltar, 0, 4);

            // --- Configuração do Formulário ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.pnlMain);
            this.Name = "LoginForm";
            this.Text = "Login - Formula 0";
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.pnlStaffFields.ResumeLayout(false);
            this.tableMain.ResumeLayout(false);
            this.tableMain.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.contentLayout.ResumeLayout(false);
            this.contentLayout.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // Variáveis de Controlo
        private System.Windows.Forms.Button btnStaff;
        private System.Windows.Forms.Button btnGuest;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Panel pnlStaffFields;
        private System.Windows.Forms.Button btnVoltar;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel tableMain;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.TableLayoutPanel contentLayout;
    }
    
}
