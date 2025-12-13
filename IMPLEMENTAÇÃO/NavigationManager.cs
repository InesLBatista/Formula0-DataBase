using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProjetoFBD
{
    /// <summary>
    /// Manages navigation between different views in the application
    /// </summary>
    public class NavigationManager
    {
        private Panel contentPanel;
        private Stack<Control> navigationStack;
        private Button btnBack;
        private Label lblCurrentView;
        private Form parentForm;

        public NavigationManager(Form parent, Panel content, Button backButton, Label viewLabel)
        {
            this.parentForm = parent;
            this.contentPanel = content;
            this.btnBack = backButton;
            this.lblCurrentView = viewLabel;
            this.navigationStack = new Stack<Control>();

            btnBack.Click += BtnBack_Click;
            UpdateBackButton();
        }

        /// <summary>
        /// Navigate to a new view
        /// </summary>
        public void NavigateTo(Control view, string title)
        {
            // Save current view to stack if exists
            if (contentPanel.Controls.Count > 0)
            {
                var currentView = contentPanel.Controls[0];
                navigationStack.Push(currentView);
                contentPanel.Controls.Clear();
            }

            // Add new view
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
            
            // Update title
            if (lblCurrentView != null)
            {
                lblCurrentView.Text = title;
            }

            UpdateBackButton();
        }

        /// <summary>
        /// Go back to previous view
        /// </summary>
        public void GoBack()
        {
            if (navigationStack.Count > 0)
            {
                // Remove current view
                contentPanel.Controls.Clear();

                // Restore previous view
                var previousView = navigationStack.Pop();
                previousView.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(previousView);

                // Update title if possible
                if (lblCurrentView != null && previousView is INavigableView navigable)
                {
                    lblCurrentView.Text = navigable.ViewTitle;
                }

                UpdateBackButton();
            }
        }

        /// <summary>
        /// Clear navigation history and return to home
        /// </summary>
        public void NavigateHome()
        {
            navigationStack.Clear();
            contentPanel.Controls.Clear();
            
            if (lblCurrentView != null)
            {
                lblCurrentView.Text = "";
            }

            UpdateBackButton();
        }

        /// <summary>
        /// Check if can go back
        /// </summary>
        public bool CanGoBack => navigationStack.Count > 0;

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            GoBack();
        }

        private void UpdateBackButton()
        {
            if (btnBack != null)
            {
                btnBack.Visible = CanGoBack;
                btnBack.Enabled = CanGoBack;
            }
        }
    }

    /// <summary>
    /// Interface for views that can be navigated
    /// </summary>
    public interface INavigableView
    {
        string ViewTitle { get; }
    }
}
