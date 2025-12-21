namespace ProjetoFBD
{
    partial class SessionForm
    {
        // Variável de designer necessária.
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
            this.SuspendLayout();
            // 
            // SessionForm
            // 
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "SessionForm";
            this.Text = "Sessions Management";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
