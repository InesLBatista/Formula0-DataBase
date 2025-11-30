using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace ProjetoFBD
{
    public partial class LoadingPage : Form
    {
        private int steps = 0;
        private const int totalSteps = 100;
        private const int interval = 50; 

        public LoadingPage()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White; 
            this.BackgroundImage = null;

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("ProjetoFBD.logo.png"); 

            if (stream != null)
            {
            pbLogoFundo.Image = Image.FromStream(stream); 
    
            pnlOverlay.BackColor = Color.FromArgb(128, Color.White); 
            }
            else
            {

            pbLogoFundo.BackColor = Color.Red; 
            }

            Label0.BringToFront();
            Label1.BringToFront();
            Label2.BringToFront();
            Label3.BringToFront();
            Label5.BringToFront();
            ProgressBar7.BringToFront();
            
            ProgressBar7.Minimum = 0;
            ProgressBar7.Maximum = totalSteps;
            ProgressBar7.Value = 0;
            ProgressBar7.Style = ProgressBarStyle.Continuous;

            Timer8.Interval = interval;
            Timer8.Tick += Timer8_Tick;
            Timer8.Start();
        }

        private void Timer8_Tick(object? sender, EventArgs e)
        {
            steps++;
            if (steps <= totalSteps)
            {
                ProgressBar7.Value = steps;
            }

            if (steps >= totalSteps)
            {
                Timer8.Stop();
                
                this.Close();                   
            }
        }

    }
}