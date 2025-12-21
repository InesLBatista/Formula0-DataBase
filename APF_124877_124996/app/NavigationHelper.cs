using System;
using System.Windows.Forms;

namespace ProjetoFBD
{
    // Helper global de navegação para sistema de janela única.
    public static class NavigationHelper
    {
        private static HomePage? _mainWindow;
        
        // Inicializa navegação com referência à HomePage principal.
        public static void Initialize(HomePage mainWindow)
        {
            _mainWindow = mainWindow;
        }
        
        // Navega para um formulário dentro do painel de conteúdo da janela principal.
        public static void NavigateTo(Form form, string title)
        {
            if (_mainWindow != null)
            {
                _mainWindow.NavigateToForm(form, title);
            }
            else
            {
                // Fallback to traditional window if not initialized
                form.ShowDialog();
            }
        }
        
        // Volta para a view anterior.
        /// </summary>
        public static void GoBack()
        {
            if (_mainWindow != null)
            {
                _mainWindow.NavigateBack();
            }
        }
    }
}
