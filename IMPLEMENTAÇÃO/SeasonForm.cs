using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Configuration; 
using System.Linq;

namespace ProjetoFBD
{
    public partial class SeasonForm : Form
    {
        // Declare UI components
        private DataGridView? dgvSeasons;
        private Panel? pnlStaffActions;
        
        public SeasonForm() : this("Staff") { }
        private string userRole;
        private SqlDataAdapter? dataAdapter;
        private DataTable? seasonTable;

        public SeasonForm(string role)
        {
            InitializeComponent(); 
            this.userRole = role;
            
            this.Text = "Seasons Management";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupSeasonsLayout();
            LoadSeasonData();
        }

        // -------------------------------------------------------------------------
        // UI SETUP
        // -------------------------------------------------------------------------

        private void SetupSeasonsLayout()
        {
            // --- 1. DataGridView for Listing ---
            dgvSeasons = new DataGridView
            {
                Name = "dgvSeasons",
                Location = new Point(10, 10),
                Size = new Size(1160, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true 
            };
            this.Controls.Add(dgvSeasons);

            // --- 2. Staff Actions Panel ---
            pnlStaffActions = new Panel
            {
                Location = new Point(10, 500),
                Size = new Size(700, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);
            

            // --- 3. Criar Botões ---
            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add New", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete Selected", new Point(280, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(420, 5));
            Button btnViewGPs = CreateActionButton("View Season GPs", new Point(560, 5));

            // --- Ligar Eventos ---
            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewGPs.Click += btnViewGPs_Click;
            
            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewGPs);

            // --- 4. Role-Based Access Control (RBAC) ---
            if (this.userRole == "Staff")
            {
                dgvSeasons.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvSeasons.ReadOnly = true; 
                pnlStaffActions.Visible = false;
                
                // Criar um painel separado apenas para o botão de visualização
                Panel viewOnlyPanel = new Panel
                {
                    Location = new Point(10, 500),
                    Size = new Size(150, 50),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left
                };
                
                Button viewOnlyBtn = CreateActionButton("View Season GPs", new Point(0, 5));
                viewOnlyBtn.Click += btnViewGPs_Click;
                viewOnlyPanel.Controls.Add(viewOnlyBtn);
                this.Controls.Add(viewOnlyPanel);
            }
        }
        
        private Button CreateActionButton(string text, Point location)
        {
            Button btn = new Button 
            { 
                Text = text, 
                Location = location, 
                Size = new Size(130, 40), 
                BackColor = Color.FromArgb(204, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            return btn;
        }

        // -------------------------------------------------------------------------
        // DATA ACCESS METHODS (CRUD)
        // -------------------------------------------------------------------------

        private void LoadSeasonData()
        {
            string connectionString = DbConfig.ConnectionString;
            
            // Query que busca os dados da temporada E calcula o número de GPs
            string query = @"
                SELECT 
                    t.Ano,
                    ISNULL(gp.GPCount, 0) as NumCorridas, -- Calculado automaticamente
                    t.PontosPiloto,
                    t.PontosEquipa,
                    t.PosiçãoPiloto,
                    t.PosiçãoEquipa
                FROM Temporada t
                LEFT JOIN (
                    SELECT Ano_Temporada, COUNT(*) as GPCount
                    FROM Grande_Prémio
                    GROUP BY Ano_Temporada
                ) gp ON t.Ano = gp.Ano_Temporada
                ORDER BY t.Ano DESC";

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                seasonTable = new DataTable();
                
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                
                dataAdapter.Fill(seasonTable);
                
                // Ensure dgvSeasons is not null
                if (dgvSeasons == null)
                {
                    return;
                }
                
                dgvSeasons.DataSource = seasonTable;

                // --- Configurações de Coluna COM VERIFICAÇÃO DE NULOS ---
                if (seasonTable != null && seasonTable.Columns.Contains("PosiçãoPiloto"))
                {
                    var column = seasonTable.Columns["PosiçãoPiloto"];
                    if (column != null)
                    {
                        column.AllowDBNull = true;
                    }
                }
                
                if (seasonTable != null && seasonTable.Columns.Contains("PosiçãoEquipa"))
                {
                    var column = seasonTable.Columns["PosiçãoEquipa"];
                    if (column != null)
                    {
                        column.AllowDBNull = true;
                    }
                }
                
                // Configurar cabeçalhos
                if (dgvSeasons != null)
                {
                    if (dgvSeasons.Columns.Contains("Ano"))
                        dgvSeasons.Columns["Ano"]!.HeaderText = "Year";
                    if (dgvSeasons.Columns.Contains("NumCorridas"))
                    {
                        dgvSeasons.Columns["NumCorridas"]!.HeaderText = "Races Count (Auto)";
                        dgvSeasons.Columns["NumCorridas"]!.ReadOnly = true; // Tornar somente leitura

                    }
                    if (dgvSeasons.Columns.Contains("PontosPiloto"))
                        dgvSeasons.Columns["PontosPiloto"]!.HeaderText = "Driver Points";
                    if (dgvSeasons.Columns.Contains("PontosEquipa"))
                        dgvSeasons.Columns["PontosEquipa"]!.HeaderText = "Team Points";
                    if (dgvSeasons.Columns.Contains("PosiçãoPiloto"))
                        dgvSeasons.Columns["PosiçãoPiloto"]!.HeaderText = "Driver Position";
                    if (dgvSeasons.Columns.Contains("PosiçãoEquipa"))
                        dgvSeasons.Columns["PosiçãoEquipa"]!.HeaderText = "Team Position";
                    
                    // Tornar a coluna Ano somente leitura
                    if (dgvSeasons.Columns.Contains("Ano"))
                    {
                        dgvSeasons.Columns["Ano"]!.ReadOnly = true;
                    }
                    
                    // Configurar formatação para colunas numéricas
                    foreach (DataGridViewColumn column in dgvSeasons.Columns)
                    {
                        if (column.Name == "NumCorridas" || column.Name == "PontosPiloto" || 
                            column.Name == "PontosEquipa" || column.Name == "PosiçãoPiloto" || 
                            column.Name == "PosiçãoEquipa")
                        {
                            column.DefaultCellStyle.Format = "N0";
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                    
                    // Adicionar validação de célula
                    dgvSeasons.CellValidating += DgvSeasons_CellValidating;
                    dgvSeasons.CellEndEdit += DgvSeasons_CellEndEdit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Season data: {ex.Message}", "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && seasonTable != null && userRole == "Staff")
            {
                string connectionString = DbConfig.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        if (dgvSeasons != null)
                        {
                            dgvSeasons.EndEdit();
                        }
                        
                        // Antes de salvar, converter strings vazias para DBNull.Value
                        foreach (DataRow row in seasonTable.Rows)
                        {
                            if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                            {
                                // Para colunas numéricas
                                string[] numericColumns = { "PosiçãoPiloto", "PosiçãoEquipa", "PontosPiloto", "PontosEquipa" };
                                foreach (string col in numericColumns)
                                {
                                    if (seasonTable.Columns.Contains(col) && 
                                        row[col] != DBNull.Value && 
                                        string.IsNullOrWhiteSpace(row[col].ToString()))
                                    {
                                        row[col] = DBNull.Value;
                                    }
                                }
                                
                                // Atualizar o número de corridas automaticamente
                                if (row["Ano"] != DBNull.Value)
                                {
                                    int year = Convert.ToInt32(row["Ano"]);
                                    row["NumCorridas"] = GetGPsCountForYear(year);
                                }
                            }
                        }
                        
                        // Verificar se há erros
                        var errorRows = seasonTable.GetErrors();
                        if (errorRows.Length > 0)
                        {
                            MessageBox.Show($"Please fix errors before saving:\n{string.Join("\n", errorRows.Select(r => r.RowError))}", 
                                "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Atualizar comandos
                        SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                        dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                        dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                        dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                        connection.Open();
                        int rowsAffected = dataAdapter.Update(seasonTable);
                        
                        MessageBox.Show($"{rowsAffected} rows saved successfully!", "Success");
                        seasonTable.AcceptChanges();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Database error: {sqlEx.Message}\nCheck if year is unique and all required fields are filled.", 
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        seasonTable.RejectChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving data: {ex.Message}", 
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        seasonTable.RejectChanges();
                    }
                }
            }
        }
        
        // Método para obter o número de GPs de um ano específico
        private int GetGPsCountForYear(int year)
        {
            string connectionString = DbConfig.ConnectionString;
            string query = "SELECT COUNT(*) FROM Grande_Prémio WHERE Ano_Temporada = @Year";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Year", year);
                    
                    connection.Open();
                    object result = command.ExecuteScalar();
                    
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        
        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (seasonTable != null && userRole == "Staff")
            {
                // Solicitar o ano ao usuário
                using (var inputForm = new InputDialog("Add New Season", "Enter the year:"))
                {
                    if (inputForm.ShowDialog() == DialogResult.OK && 
                        !string.IsNullOrWhiteSpace(inputForm.InputValue))
                    {
                        string year = inputForm.InputValue.Trim();
                        
                        // Validar ano
                        if (!int.TryParse(year, out int yearInt) || 
                            yearInt < 1900 || yearInt > DateTime.Now.Year + 1)
                        {
                            MessageBox.Show($"Please enter a valid year between 1900 and {DateTime.Now.Year + 1}", 
                                "Invalid Year", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        // Verificar se o ano já existe
                        bool yearExists = false;
                        foreach (DataRow row in seasonTable.Rows)
                        {
                            if (row.RowState != DataRowState.Deleted && 
                                row["Ano"] != DBNull.Value && 
                                row["Ano"].ToString() == year)
                            {
                                yearExists = true;
                                break;
                            }
                        }
                        
                        if (yearExists)
                        {
                            MessageBox.Show($"Season for year {year} already exists!", 
                                "Duplicate Year", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        // Adicionar nova linha
                        try
                        {
                            DataRow newRow = seasonTable.NewRow();
                            newRow["Ano"] = yearInt;
                            newRow["NumCorridas"] = GetGPsCountForYear(yearInt); // Calcular automaticamente
                            newRow["PontosPiloto"] = DBNull.Value;
                            newRow["PontosEquipa"] = DBNull.Value;
                            newRow["PosiçãoPiloto"] = DBNull.Value;
                            newRow["PosiçãoEquipa"] = DBNull.Value;
                            
                            seasonTable.Rows.InsertAt(newRow, 0);
                            
                            // Mover foco para a primeira célula editável
                            if (dgvSeasons != null && dgvSeasons.Columns.Contains("PontosPiloto"))
                            {
                                dgvSeasons.CurrentCell = dgvSeasons.Rows[0].Cells["PontosPiloto"];
                                dgvSeasons.BeginEdit(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error adding new season: {ex.Message}", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff" && dgvSeasons != null && dgvSeasons.SelectedRows.Count > 0 && seasonTable != null)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete the selected row(s)? This action cannot be undone.", 
                    "Confirm Deletion", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvSeasons.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(r => r.Index))
                        {
                            DataRow? dataRow = (row.DataBoundItem as DataRowView)?.Row;
                            if (dataRow != null)
                            {
                                dataRow.Delete();
                            }
                        }
                        
                        btnSave_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during deletion: {ex.Message}", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        seasonTable.RejectChanges(); 
                    }
                }
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            // Atualizar os contadores de corridas antes de recarregar
            if (seasonTable != null)
            {
                foreach (DataRow row in seasonTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted && row["Ano"] != DBNull.Value)
                    {
                        int year = Convert.ToInt32(row["Ano"]);
                        row["NumCorridas"] = GetGPsCountForYear(year);
                    }
                }
            }
            
            if (seasonTable != null && seasonTable.GetChanges() != null)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to discard them and refresh the data?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    seasonTable.RejectChanges();
                    LoadSeasonData();
                }
            }
            else
            {
                LoadSeasonData();
            }
        }
        
        private void btnViewGPs_Click(object? sender, EventArgs e)
        {
            if (dgvSeasons == null || dgvSeasons.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a season first.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            DataGridViewRow selectedRow = dgvSeasons.SelectedRows[0];
            
            var anoCell = selectedRow.Cells["Ano"];
            if (anoCell != null && anoCell.Value != null)
            {
                if (int.TryParse(anoCell.Value.ToString(), out int selectedYear))
                {
                    OpenGPListForSeason(selectedYear);
                }
                else
                {
                    MessageBox.Show("Invalid year format.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Could not retrieve the selected year.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DgvSeasons_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvSeasons == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            
            string? columnName = dgvSeasons.Columns[e.ColumnIndex].Name;
            string value = e.FormattedValue?.ToString() ?? "";
            
            try
            {
                // Não validar NumCorridas pois é calculado automaticamente
                if (columnName == "NumCorridas")
                {
                    return;
                }
                
                // Validar outras colunas numéricas
                if (columnName == "Ano" || columnName == "PontosPiloto" || columnName == "PontosEquipa" ||
                    columnName == "PosiçãoPiloto" || columnName == "PosiçãoEquipa")
                {
                    // Se estiver vazio, permitir (será convertido para DBNull.Value)
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        dgvSeasons.Rows[e.RowIndex].ErrorText = "";
                        return;
                    }
                    
                    // Tentar converter para inteiro
                    if (!int.TryParse(value, out int intValue))
                    {
                        dgvSeasons.Rows[e.RowIndex].ErrorText = $"Please enter a valid integer for {columnName}";
                        e.Cancel = true;
                        return;
                    }
                    
                    // Validações específicas por coluna
                    if (columnName == "Ano" && (intValue < 1900 || intValue > DateTime.Now.Year + 1))
                    {
                        dgvSeasons.Rows[e.RowIndex].ErrorText = $"Year must be between 1900 and {DateTime.Now.Year + 1}";
                        e.Cancel = true;
                        return;
                    }
                    
                    if ((columnName == "PontosPiloto" || columnName == "PontosEquipa" || 
                         columnName == "PosiçãoPiloto" || columnName == "PosiçãoEquipa") && intValue < 0)
                    {
                        dgvSeasons.Rows[e.RowIndex].ErrorText = "Value cannot be negative";
                        e.Cancel = true;
                        return;
                    }
                    
                    dgvSeasons.Rows[e.RowIndex].ErrorText = "";
                }
            }
            catch (Exception ex)
            {
                if (dgvSeasons != null)
                {
                    dgvSeasons.Rows[e.RowIndex].ErrorText = $"Validation error: {ex.Message}";
                }
                e.Cancel = true;
            }
        }

        private void DgvSeasons_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (dgvSeasons == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            
            dgvSeasons.Rows[e.RowIndex].ErrorText = "";
            
            string? columnName = dgvSeasons.Columns[e.ColumnIndex].Name;
            
            if (!string.IsNullOrEmpty(columnName) && 
                (columnName == "PosiçãoPiloto" || columnName == "PosiçãoEquipa" ||
                 columnName == "PontosPiloto" || columnName == "PontosEquipa"))
            {
                var cell = dgvSeasons.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value != null && cell.Value.ToString() == "")
                {
                    cell.Value = DBNull.Value;
                }
            }
        }

        private void OpenGPListForSeason(int year)
        {
            try
            {
                GPListForm gpList = new GPListForm(this.userRole, year);
                NavigationHelper.NavigateTo(gpList, "GP LIST - " + year);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open GP list form: {ex.Message}", 
                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // InputDialog class for Add functionality
        public class InputDialog : Form
        {
            private TextBox textBox;
            public string InputValue { get; private set; } = "";
            
            public InputDialog(string title, string prompt)
            {
                this.Text = title;
                this.Size = new Size(300, 150);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                
                Label label = new Label
                {
                    Text = prompt,
                    Location = new Point(10, 20),
                    Size = new Size(260, 20)
                };
                
                textBox = new TextBox
                {
                    Location = new Point(10, 50),
                    Size = new Size(260, 20)
                };
                
                Button okButton = new Button
                {
                    Text = "OK",
                    Location = new Point(70, 80),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.OK
                };
                
                Button cancelButton = new Button
                {
                    Text = "Cancel",
                    Location = new Point(155, 80),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.Cancel
                };
                
                okButton.Click += (s, e) => 
                {
                    InputValue = textBox.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                
                cancelButton.Click += (s, e) => 
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                };
                
                this.Controls.Add(label);
                this.Controls.Add(textBox);
                this.Controls.Add(okButton);
                this.Controls.Add(cancelButton);
                
                this.AcceptButton = okButton;
                this.CancelButton = cancelButton;
            }
        }
    }
}