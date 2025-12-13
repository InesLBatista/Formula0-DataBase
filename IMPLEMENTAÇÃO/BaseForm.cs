using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace ProjetoFBD
{
    /// <summary>
    /// Base form class with shared utilities for all management forms
    /// </summary>
    public class BaseForm : Form
    {
        protected string userRole;

        public BaseForm()
        {
            this.userRole = "Guest";
        }

        public BaseForm(string role)
        {
            this.userRole = role;
        }

        /// <summary>
        /// Creates a standardized action button with Formula 1 styling
        /// </summary>
        protected Button CreateActionButton(string text, Point location, Color? backgroundColor = null)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(130, 40),
                BackColor = backgroundColor ?? Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        /// <summary>
        /// Shows a standardized error message
        /// </summary>
        protected void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a standardized success message
        /// </summary>
        protected void ShowSuccess(string message, string title = "Success")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a standardized warning message
        /// </summary>
        protected void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        protected bool ShowConfirmation(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// Handles SQL exceptions with detailed error messages
        /// </summary>
        protected void HandleSqlException(SqlException ex, string operation = "operation")
        {
            string message = $"Database error during {operation}:\n\n{ex.Message}\n\nError Number: {ex.Number}";
            
            // Provide specific messages for common SQL errors
            switch (ex.Number)
            {
                case 547: // FK constraint violation
                    message = $"Cannot perform {operation}: The record is referenced by other data.\n\n" +
                             "Please delete related records first or select a different record.\n\n" +
                             $"Technical details: {ex.Message}";
                    break;
                case 2627: // Unique constraint violation
                case 2601:
                    message = $"Cannot perform {operation}: A record with this value already exists.\n\n" +
                             $"Technical details: {ex.Message}";
                    break;
                case 515: // Cannot insert NULL
                    message = $"Cannot perform {operation}: Required fields are missing.\n\n" +
                             $"Technical details: {ex.Message}";
                    break;
            }
            
            ShowError(message, "Database Error");
        }

        /// <summary>
        /// Checks if a row is selected in a DataGridView
        /// </summary>
        protected bool IsRowSelected(DataGridView dgv, string itemName = "item")
        {
            if (dgv.SelectedRows.Count == 0)
            {
                ShowWarning($"Please select a {itemName} first.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Configures standard DataGridView appearance
        /// </summary>
        protected void ConfigureDataGridView(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            
            // Alternate row colors for better readability
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }

        /// <summary>
        /// Sets column header text with English translation
        /// </summary>
        protected void SetColumnHeader(DataGridView dgv, string columnName, string headerText)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]!.HeaderText = headerText;
            }
        }

        /// <summary>
        /// Hides a column if it exists
        /// </summary>
        protected void HideColumn(DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]!.Visible = false;
            }
        }

        /// <summary>
        /// Makes a column read-only if it exists
        /// </summary>
        protected void MakeColumnReadOnly(DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]!.ReadOnly = true;
            }
        }
    }
}
