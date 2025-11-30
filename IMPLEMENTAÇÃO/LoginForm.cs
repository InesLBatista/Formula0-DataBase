using System;
using System.Windows.Forms;
using System.Drawing;

namespace ProjetoFBD
{
    // CRÍTICO: Deve ser 'partial' e herdar de 'Form'
    public partial class LoginForm : Form
    {
        private bool areFieldsVisible = false;

        public LoginForm()
        {
            InitializeComponent(); // Chamada obrigatória ao designer
            
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // --- LIGAÇÕES DE EVENTOS (CS0103 resolvido por ligar os eventos) ---
            this.btnStaff.Click += new System.EventHandler(this.btnStaff_Click);
            this.btnGuest.Click += new System.EventHandler(this.btnGuest_Click);
            this.btnVoltar.Click += new System.EventHandler(this.btnVoltar_Click);
            
            // O botão 'Voltar' deve estar invisível no estado inicial
            this.btnVoltar.Visible = false; 
        }

        // -------------------------------------------------------------------------
        // LÓGICA DE AÇÃO (STAFF / CONTINUAR)
        // -------------------------------------------------------------------------
        
        // Correção CS8622: Adicionar '?' ao object sender
        private void btnStaff_Click(object? sender, EventArgs e) 
        {
            if (!areFieldsVisible)
            {
                // ESTADO 1: Mudar para o estado de ENTRADA (Mostrar campos)
                this.SuspendLayout();
                
                // Mostrar campos de Staff e alterar estado
                pnlStaffFields.Visible = true;
                areFieldsVisible = true;
                btnStaff.Text = "CONTINUAR"; 
                
                // CRÍTICO: Esconder o botão GUEST e mover/mostrar o botão VOLTAR
                btnGuest.Visible = false; 
                btnVoltar.Visible = true; 
                
                // Mover os botões para a posição de submissão (em baixo)
                btnVoltar.Location = new Point(80, 340);
                btnStaff.Location = new Point(250, 340); 
                
                this.ResumeLayout(false);
            }
            else
            {
                // ESTADO 2: Tentar autenticação (CONTINUAR)
                AuthenticateStaff(txtUsername.Text, txtPassword.Text);
            }
        }

        // -------------------------------------------------------------------------
        // LÓGICA DE VOLTAR AO ESTADO INICIAL
        // -------------------------------------------------------------------------
        
        private void btnVoltar_Click(object? sender, EventArgs e)
        {
            this.SuspendLayout();

            // 1. Esconde os campos de Staff
            pnlStaffFields.Visible = false;
            areFieldsVisible = false;
            
            // 2. Repõe o texto e posição dos botões STAFF/GUEST (Estado 1)
            btnStaff.Text = "STAFF";
            btnStaff.Location = new Point(80, 270); // Posição Original
            btnGuest.Location = new Point(250, 270); // Posição Original
            
            // 3. Mostra o GUEST e esconde o VOLTAR
            btnGuest.Visible = true;
            btnVoltar.Visible = false; 

            this.ResumeLayout(false);
        }

        // -------------------------------------------------------------------------
        // LÓGICA DE ACESSO (GUEST E AUTHENTICATE)
        // -------------------------------------------------------------------------
        
        private void btnGuest_Click(object? sender, EventArgs e) 
        {
            OpenHomePage("Guest");
        }

        private void AuthenticateStaff(string username, string password)
        {
            if (username == "admin" && password == "1234") 
            {
                OpenHomePage("Staff");
            }
            else
            {
                MessageBox.Show("Credenciais inválidas.", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear(); 
                txtUsername.Focus();
            }
        }

        private void OpenHomePage(string userRole)
        {
            // Este é o ponto onde o seu código deve abrir a HomePage
            MessageBox.Show($"Login bem-sucedido como {userRole}. PRÓXIMA TELA: HOME PAGE.", "Sucesso!");
            this.Close(); 
        }
        
        // O método 'LoginForm_Load' problemático foi removido.
    }
}