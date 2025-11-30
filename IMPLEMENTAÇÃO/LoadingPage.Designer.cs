namespace ProjetoFBD
{
    partial class LoadingPage
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
            this.components = new System.ComponentModel.Container();

            this.pbLogoFundo = new System.Windows.Forms.PictureBox();
            this.pnlOverlay = new System.Windows.Forms.Panel();

            this.Label0 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.ImageList4 = new System.Windows.Forms.ImageList(this.components);
            this.Label5 = new System.Windows.Forms.Label();
            this.ProgressBar7 = new GradientProgressBar();
            this.Timer8 = new System.Windows.Forms.Timer(this.components);
            
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoFundo)).BeginInit();
            this.SuspendLayout();

            this.pbLogoFundo.BackColor = System.Drawing.Color.White;
            this.pbLogoFundo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLogoFundo.Name = "pbLogoFundo";
            this.pbLogoFundo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLogoFundo.TabIndex = 8;
            this.pbLogoFundo.TabStop = false;

            this.pnlOverlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOverlay.Name = "pnlOverlay";
            this.pnlOverlay.TabIndex = 9;

            this.Label0.AutoSize = true;
            this.Label0.Font = new System.Drawing.Font("Century Gothic", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label0.Location = new System.Drawing.Point(260, 168); 
            this.Label0.Name = "Label0";
            this.Label0.Size = new System.Drawing.Size(219, 59);
            this.Label0.TabIndex = 0;
            this.Label0.Text = "Fórmula";
            this.Label0.ForeColor = Color.Black; 
            this.Label0.BackColor = Color.Transparent;
            
            // Label1
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Century Gothic", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label1.Location = new System.Drawing.Point(470, 168); 
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(53, 59);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "0";
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(204, 0, 0); 
            this.Label1.BackColor = Color.Transparent;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label2.Location = new System.Drawing.Point(12, 395); // ou 340, experimenta
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(95, 21);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Loading...";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label3.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.Label3.Location = new System.Drawing.Point(556, 4);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(170, 21);
            this.Label3.TabIndex = 3;
            this.Label3.Text = "Inês Batista, 124877";
            // 
            // ImageList4
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label5.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.Label5.Location = new System.Drawing.Point(524, 24);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(203, 21);
            this.Label5.TabIndex = 5;
            this.Label5.Text = "Maria Quinteiro, 124996";
            //
            // ProgressBar7
            // 
            this.ProgressBar7.StartColor = Color.DarkRed; 
            this.ProgressBar7.EndColor = Color.Red;         
            this.ProgressBar7.ForeColor = System.Drawing.Color.Firebrick;
            this.ProgressBar7.Location = new System.Drawing.Point(260, 235);
            this.ProgressBar7.Name = "ProgressBar7";
            this.ProgressBar7.Size = new System.Drawing.Size(220, 15);
            this.ProgressBar7.TabIndex = 7;
            this.ProgressBar7.Value = 50;
            this.ProgressBar7.BackColor = Color.LightGray; 

            // Timer8
            this.Timer8.Interval = 1000;
            
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 424);
            this.Controls.Clear();

            this.Controls.Add(this.pbLogoFundo);

            this.Controls.Add(this.pnlOverlay);

            this.Controls.Add(this.ProgressBar7);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label0);
            
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 424);
            // ...
            this.Name = "Form1"; 
            this.Text = "Loading Page";
            
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoFundo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label Label0;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.ImageList ImageList4;
        private System.Windows.Forms.Label Label5;
        private GradientProgressBar ProgressBar7;
        private System.Windows.Forms.Timer Timer8;
        
        private System.Windows.Forms.PictureBox pbLogoFundo;
        private System.Windows.Forms.Panel pnlOverlay;
    }
}