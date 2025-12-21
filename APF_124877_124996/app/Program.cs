using System;
using System.Windows.Forms;
using System.Threading;

namespace ProjetoFBD
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Global error handlers to exit cleanly on fatal errors
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext()); // Use AppContext, não LoadingPage!
        }

        private static void OnThreadException(object? sender, ThreadExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show($"An unexpected error occurred and the application will close.\n\n{e.Exception.Message}",
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Application.Exit();
            }
        }

        private static void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            try
            {
                if (ex != null)
                {
                    MessageBox.Show($"An unexpected error occurred and the application will close.\n\n{ex.Message}",
                        "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Ensure termination even if forms are blocked
                Environment.Exit(1);
            }
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