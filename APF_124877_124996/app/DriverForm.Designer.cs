namespace ProjetoFBD
{
    partial class DriverForm
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
            // DriverForm
            // 
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "DriverForm";
            this.Text = "Drivers Management";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
