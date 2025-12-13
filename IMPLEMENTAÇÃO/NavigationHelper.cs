using System;
using System.Windows.Forms;

namespace ProjetoFBD
{
    /// <summary>
    /// Global navigation helper for single-window navigation system.
    /// Allows child forms to navigate without opening new windows.
    /// </summary>
    public static class NavigationHelper
    {
        private static HomePage? _mainWindow;
        
        /// <summary>
        /// Initialize navigation with reference to main HomePage.
        /// Should be called once when HomePage is created.
        /// </summary>
        public static void Initialize(HomePage mainWindow)
        {
            _mainWindow = mainWindow;
        }
        
        /// <summary>
        /// Navigate to a form within the main window's content panel.
        /// </summary>
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
        
        /// <summary>
        /// Navigate back to previous view.
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
