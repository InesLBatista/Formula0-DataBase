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
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(15, 15, 15); // Dark background
            this.Opacity = 0; // Start invisible for fade-in

            // Load logo
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("ProjetoFBD.logo.png"); 

            if (stream != null)
            {
                pbLogoFundo.Image = Image.FromStream(stream);
                pbLogoFundo.BackColor = Color.Transparent;
                pnlOverlay.BackColor = Color.FromArgb(180, 15, 15, 15); 
            }
            else
            {
                pbLogoFundo.BackColor = Color.FromArgb(30, 30, 30);
                pnlOverlay.BackColor = Color.FromArgb(200, 15, 15, 15);
            }

            // Center elements dynamically
            CenterElements();
            this.Resize += (s, e) => CenterElements();

            // Bring to front
            Label0.BringToFront();
            Label1.BringToFront();
            Label2.BringToFront();
            Label3.BringToFront();
            Label5.BringToFront();
            ProgressBar7.BringToFront();
            
            // Configure progress bar
            ProgressBar7.Minimum = 0;
            ProgressBar7.Maximum = totalSteps;
            ProgressBar7.Value = 0;
            ProgressBar7.Style = ProgressBarStyle.Continuous;

            // Start fade-in animation
            System.Windows.Forms.Timer fadeIn = new System.Windows.Forms.Timer { Interval = 20 };
            fadeIn.Tick += (s, e) => 
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                    ((System.Windows.Forms.Timer)s!).Stop();
            };
            fadeIn.Start();

            // Start loading timer
            Timer8.Interval = interval;
            Timer8.Tick += Timer8_Tick;
            Timer8.Start();
        }

        private void CenterElements()
        {
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            // Center "Fórmula 0" title
            int titleWidth = Label0.Width + Label1.Width;
            Label0.Location = new Point(centerX - titleWidth / 2, centerY - 80);
            Label1.Location = new Point(Label0.Right + 5, centerY - 80);

            // Center progress bar below title
            ProgressBar7.Location = new Point(centerX - ProgressBar7.Width / 2, centerY - 10);
            
            // Center "Loading..." text
            Label2.Location = new Point(centerX - Label2.Width / 2, centerY + 20);

            // Position credits in corners
            Label3.Location = new Point(this.ClientSize.Width - Label3.Width - 10, 10);
            Label5.Location = new Point(this.ClientSize.Width - Label5.Width - 10, 35);
        }

        private void Timer8_Tick(object? sender, EventArgs e)
        {
            steps++;
            if (steps <= totalSteps)
            {
                ProgressBar7.Value = steps;
                
                // Pulsing effect on loading text
                if (steps % 10 < 5)
                    Label2.ForeColor = Color.FromArgb(204, 0, 0);
                else
                    Label2.ForeColor = Color.White;
            }

            if (steps >= totalSteps)
            {
                Timer8.Stop();
                
                // Fade-out animation
                System.Windows.Forms.Timer fadeOut = new System.Windows.Forms.Timer { Interval = 20 };
                fadeOut.Tick += (s, ev) => 
                {
                    if (this.Opacity > 0)
                        this.Opacity -= 0.1;
                    else
                    {
                        ((System.Windows.Forms.Timer)s!).Stop();
                        this.Close();
                    }
                };
                fadeOut.Start();
            }
        }
    }
}