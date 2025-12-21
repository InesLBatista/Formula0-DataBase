namespace ProjetoFBD
{
    partial class HomePage
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlGrandPrix;
        private System.Windows.Forms.Panel pnlSeasons;
        private System.Windows.Forms.Panel pnlGrid;

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
            this.pbFundo = new System.Windows.Forms.PictureBox(); // Instanciar
            this.components = new System.ComponentModel.Container();
            // INICIALIZAÇÃO DE CAMPOS (Mínimo necessário, mas não é obrigatório para código manual)
            this.pnlGrandPrix = new System.Windows.Forms.Panel();
            this.pnlSeasons = new System.Windows.Forms.Panel();
            this.pnlGrid = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbFundo)).BeginInit(); // NOVO
            this.SuspendLayout();

            // --- Configuração do PictureBox de Fundo ---
    this.pbFundo.Name = "pbFundo";
    this.pbFundo.Dock = System.Windows.Forms.DockStyle.Fill; // CRÍTICO: Preenche todo o formulário
    this.pbFundo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage; // Ajusta a imagem
    this.pbFundo.TabIndex = 0; // Posição mais baixa
    this.pbFundo.TabStop = false;

            // 
            // HomePage
            // 
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "HomePage";
            this.Text = "Home Page";
            this.Controls.Add(this.pbFundo); // NOVO
            ((System.ComponentModel.ISupportInitialize)(this.pbFundo)).EndInit(); // NOVO
            this.ResumeLayout(false);
        }

        #endregion


        // Variável de Controlo do Background
        private System.Windows.Forms.PictureBox pbFundo; // NOVO
    }
}