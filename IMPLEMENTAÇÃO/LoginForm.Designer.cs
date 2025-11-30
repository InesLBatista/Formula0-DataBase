namespace ProjetoFBD // Use o seu namespace correto aqui
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
            this.btnStaff = new System.Windows.Forms.Button(); // Tem que estar aqui!
            this.btnGuest = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnStaff.Text = "CONTINUAR"; // Texto inicial
            this.components = new System.ComponentModel.Container();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pnlStaffFields = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();

            // 2. CONFIGURAÇÃO (BeginInit)
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.pnlStaffFields.SuspendLayout();
            this.SuspendLayout();

            // --- Configuração dos Botões de Ação ---
            // No InitializeComponent()

            // btnVoltar (CANCELAR/VOLTAR)
            this.btnStaff.Location = new System.Drawing.Point(260, 340);
            this.btnVoltar.BackColor = System.Drawing.Color.Gray; // Cor Cinza
            this.btnVoltar.FlatAppearance.BorderSize = 0;
            this.btnVoltar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVoltar.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnVoltar.ForeColor = System.Drawing.Color.White;
            this.btnVoltar.Cursor = System.Windows.Forms.Cursors.Hand; 
            this.btnVoltar.Location = new System.Drawing.Point(50, 340); // Posicionado abaixo dos campos
            this.btnVoltar.Name = "btnVoltar";
            this.btnVoltar.Size = new System.Drawing.Size(180, 50); 
            this.btnVoltar.TabIndex = 7; // Novo TabIndex
            this.btnVoltar.Text = "VOLTAR"; // Pode ser CANCELAR ou VOLTAR
            this.btnVoltar.UseVisualStyleBackColor = false;

            // btnStaff
            this.btnStaff.BackColor = System.Drawing.Color.FromArgb(192, 0, 0); 
            this.btnStaff.FlatAppearance.BorderSize = 0;
            this.btnStaff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStaff.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStaff.ForeColor = System.Drawing.Color.White;
            this.btnStaff.Cursor = System.Windows.Forms.Cursors.Hand; // Novo: Cursor de mão
            this.btnStaff.Location = new System.Drawing.Point(60, 270); // Ajuste a posição para maior largura
            this.btnStaff.Size = new System.Drawing.Size(180, 50); // Maior largura
            this.btnStaff.Text = "STAFF";
            this.btnStaff.UseVisualStyleBackColor = false;

            // btnGuest
            this.btnGuest.BackColor = System.Drawing.Color.FromArgb(255, 50, 50); // Vermelho ligeiramente diferente
            this.btnGuest.FlatAppearance.BorderSize = 0;
            this.btnGuest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuest.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGuest.ForeColor = System.Drawing.Color.White;
            this.btnGuest.Cursor = System.Windows.Forms.Cursors.Hand; // Novo: Cursor de mão
            this.btnGuest.Location = new System.Drawing.Point(260, 270);
            this.btnGuest.Size = new System.Drawing.Size(180, 50); // Maior largura
            this.btnGuest.Text = "GUEST";
            this.btnGuest.UseVisualStyleBackColor = false;

            // --- Configuração do Título e Ícone ---
            this.pbIcon.Location = new System.Drawing.Point(100, 70);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(80, 80);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbIcon.TabIndex = 2;
            this.pbIcon.TabStop = false;

            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(190, 85);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(169, 59);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "LOGIN";

            // --- Configuração dos Campos Ocultos (Staff Fields) ---
            this.pnlStaffFields.Controls.Add(this.txtPassword);
            this.pnlStaffFields.Controls.Add(this.lblPassword);
            this.pnlStaffFields.Controls.Add(this.txtUsername);
            this.pnlStaffFields.Controls.Add(this.lblUsername);
            this.pnlStaffFields.Location = new System.Drawing.Point(60, 170);
            this.pnlStaffFields.Name = "pnlStaffFields";
            this.pnlStaffFields.Size = new System.Drawing.Size(360, 80);
            this.pnlStaffFields.TabIndex = 4;
            this.pnlStaffFields.Visible = false; // INICIALMENTE INVISÍVEL

            this.lblUsername.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point); // Negrito
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(10, 10);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(75, 20);
            this.lblUsername.Text = "STAFF ID:";

            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; // Novo: Borda Flat
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(250, 250, 250); // Fundo quase branco
            this.txtUsername.Location = new System.Drawing.Point(100, 7);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(240, 27);
            this.txtUsername.TabIndex = 5;

            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; // Novo: Borda Flat
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(250, 250, 250); // Fundo quase branco
            this.lblPassword.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point); // Negrito
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(10, 45);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(75, 20);
            this.lblPassword.Text = "Password:";

            this.txtPassword.Location = new System.Drawing.Point(100, 42);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(240, 27);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.UseSystemPasswordChar = true;

            // --- Configuração do Formulário ---
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245); // Cinza muito claro, quase branco
            this.ClientSize = new System.Drawing.Size(480, 400); // Ajuste se necessário devido aos botões maiores
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlStaffFields);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbIcon);
            this.Controls.Add(this.btnGuest);
            this.Controls.Add(this.btnStaff);
            this.Controls.Add(this.btnVoltar);
            this.Name = "LoginForm";
            this.Text = "Login";

            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.pnlStaffFields.ResumeLayout(false);
            this.pnlStaffFields.PerformLayout();
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
    }
    
}