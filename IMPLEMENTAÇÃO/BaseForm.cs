using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace ProjetoFBD
{
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

        protected void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected void ShowSuccess(string message, string title = "Success")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected bool ShowConfirmation(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        protected void HandleSqlException(SqlException ex, string operation = "operation")
        {
            string message = $"Database error during {operation}:\n\n{ex.Message}\n\nError Number: {ex.Number}";
            
            switch (ex.Number)
            {
                case 547: 
                    message = $"Cannot perform {operation}: The record is referenced by other data.\n\n" +
                             "Please delete related records first or select a different record.\n\n" +
                             $"Technical details: {ex.Message}";
                    break;
                case 2627: 
                case 2601:
                    message = $"Cannot perform {operation}: A record with this value already exists.\n\n" +
                             $"Technical details: {ex.Message}";
                    break;
                case 515: 
                    message = $"Cannot perform {operation}: Required fields are missing.\n\n" +
                             $"Technical details: {ex.Message}";
                    break;
            }
            
            ShowError(message, "Database Error");
        }


        protected bool IsRowSelected(DataGridView dgv, string itemName = "item")
        {
            if (dgv.SelectedRows.Count == 0)
            {
                ShowWarning($"Please select a {itemName} first.");
                return false;
            }
            return true;
        }

        protected void ConfigureDataGridView(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.Fixed3D;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }

        protected void SetColumnHeader(DataGridView dgv, string columnName, string headerText)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]!.HeaderText = headerText;
            }
        }

        protected void HideColumn(DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]!.Visible = false;
            }
        }

        protected void MakeColumnReadOnly(DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]!.ReadOnly = true;
            }
        }
    }
}
