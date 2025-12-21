using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace ProjetoFBD
{
    public partial class LoginForm : Form
    {
        private bool areFieldsVisible = false;
        string connectionString = DbConfig.ConnectionString;

        public LoginForm()
        {
            InitializeComponent(); 
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 500);
            
            // Attach event handlers
            this.btnStaff.Click += btnStaff_Click;
            this.btnGuest.Click += btnGuest_Click;
            this.btnVoltar.Click += btnVoltar_Click;
            
            // Permitir Enter nos TextBox para submeter
            this.txtUsername.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Return) { btnStaff_Click(null, EventArgs.Empty); e.Handled = true; } };
            this.txtPassword.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Return) { btnStaff_Click(null, EventArgs.Empty); e.Handled = true; } };
        }

        private void btnStaff_Click(object? sender, EventArgs e) 
        {
            if (!areFieldsVisible)
            {
                this.SuspendLayout();
                
                pnlStaffFields.Visible = true;
                areFieldsVisible = true;
                
                btnGuest.Visible = false; 
                btnVoltar.Visible = true; 
                
                btnStaff.Text = "CONTINUAR";
                
                this.ResumeLayout(false);
                txtUsername.Focus();
            }
            else
            {
                AuthenticateStaff(txtUsername.Text, txtPassword.Text);
            }
        }

        private void btnVoltar_Click(object? sender, EventArgs e)
        {
            this.SuspendLayout();

            pnlStaffFields.Visible = false;
            areFieldsVisible = false;
            
            btnStaff.Text = "STAFF";
            
            btnGuest.Visible = true;
            btnVoltar.Visible = false;

            // Limpar campos
            txtUsername.Clear();
            txtPassword.Clear();

            this.ResumeLayout(false);
        }
        
        private void btnGuest_Click(object? sender, EventArgs e) 
        {
            OpenHomePage("Guest");
        }

        private void AuthenticateStaff(string staffIdText, string password)
        {
            if (!int.TryParse(staffIdText.Trim(), out int staffId))
            {
                MessageBox.Show("O Staff ID deve ser um número.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Focus();
                return;
            }

            string cleanedPassword = password.Trim();

            string query = "SELECT COUNT(1) FROM Staff WHERE StaffID = @StaffID AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    command.Parameters.AddWithValue("@StaffID", staffId);
                    command.Parameters.AddWithValue("@Password", cleanedPassword);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    if (count == 1)
                    {
                        OpenHomePage("Staff");
                    }
                    else
                    {
                        MessageBox.Show("Staff ID ou Password incorretos.", 
                            "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        txtPassword.Clear();
                        txtUsername.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro na Base de Dados: {ex.Message}",
                        "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenHomePage(string userRole)
        {
            // 1. Esconde o formulário de Login (não o fecha, apenas esconde)
            this.Hide(); 
            
            // 2. Cria e mostra a nova Home Page, passando o papel do utilizador
            HomePage homePage = new HomePage(userRole);
            
            // 3. Liga um evento para saber quando a Home Page é fechada
            homePage.FormClosed += (s, args) => this.Close(); // Fecha o LoginForm (o programa) quando a HomePage é fechada
            
            homePage.Show(); 
        }
    }
}
