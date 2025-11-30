using System;
using System.Windows.Forms;

namespace ProjetoFBD
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext()); // Use AppContext, não LoadingPage!
        }
    }

    // Esta classe irá gerenciar a transição entre telas
    public class AppContext : ApplicationContext
    {
        // No ficheiro AppContext.cs (DENTRO da classe AppContext)
        public AppContext()
        {
            // O código de configuração padrão DEVE ser feito ANTES do Application.Run, 
            // no Program.cs, mas para debugging, podemos deixá-lo aqui (não é o ideal).
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 1. Inicia o Loading Screen
            LoadingPage loading = new LoadingPage();
            
            // 2. Associa um evento ao fecho do Loading Screen
            loading.FormClosed += LoadingClosed;
            
            // 3. Define o formulário principal atual e mostra-o
            this.MainForm = loading;
            loading.Show();
            
            // REMOVA:
            // Application.Run(new AppContext()); // ESTA LINHA CAUSA O ERRO FATAL
        }


        private void LoadingClosed(object? sender, FormClosedEventArgs e)
        {
            // O Loading Page fechou. Vamos abrir o Login Form.
            if (sender is LoadingPage)
            {
                LoginForm login = new LoginForm();
                
                // Associa um evento ao fecho do Login Form para fechar a aplicação
                login.FormClosed += LoginClosed;
                
                this.MainForm = login;
                login.Show();
            }
        }

        private void LoginClosed(object? sender, FormClosedEventArgs e)
        {
            // Quando o formulário de login (que é a nossa Main Form) fechar,
            // fechamos toda a aplicação (isto é padrão).
            if (sender is LoginForm)
            {
                if (this.MainForm == sender)
                {
                    ExitThread();
                }
            }
        }
    }
}