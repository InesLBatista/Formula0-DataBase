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
                Size = new Size(850, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);
            

            // --- 3. Criar Botões ---
            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add New", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete Selected", new Point(280, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(420, 5));
            Button btnViewGPs = CreateActionButton("View Season GPs", new Point(560, 5));
            Button btnViewStandings = CreateActionButton("View Standings", new Point(710, 5), Color.FromArgb(0, 102, 204));

            // --- Ligar Eventos ---
            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewGPs.Click += btnViewGPs_Click;
            btnViewStandings.Click += btnViewStandings_Click;
            
            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewGPs);
            pnlStaffActions.Controls.Add(btnViewStandings);

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
                
                // Criar um painel separado para os botões de visualização (Guest)
                Panel viewOnlyPanel = new Panel
                {
                    Location = new Point(10, 500),
                    Size = new Size(300, 50),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left
                };
                
                Button viewGPsBtn = CreateActionButton("View Season GPs", new Point(0, 5));
                viewGPsBtn.Click += btnViewGPs_Click;
                viewOnlyPanel.Controls.Add(viewGPsBtn);
                
                Button viewStandingsBtn = CreateActionButton("View Standings", new Point(150, 5), Color.FromArgb(0, 102, 204));
                viewStandingsBtn.Click += btnViewStandings_Click;
                viewOnlyPanel.Controls.Add(viewStandingsBtn);
                
                this.Controls.Add(viewOnlyPanel);
            }
        }
        
        private Button CreateActionButton(string text, Point location, Color? backColor = null)
        {
            Button btn = new Button 
            { 
                Text = text, 
                Location = location, 
                Size = new Size(130, 40), 
                BackColor = backColor ?? Color.FromArgb(204, 0, 0),
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
                    ISNULL(gp.GPCount, 0) as NumCorridas
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

                // Configurar cabeçalhos
                if (dgvSeasons != null)
                {
                    if (dgvSeasons.Columns.Contains("Ano"))
                    {
                        dgvSeasons.Columns["Ano"]!.HeaderText = "Year";
                        dgvSeasons.Columns["Ano"]!.ReadOnly = true;
                    }
                    if (dgvSeasons.Columns.Contains("NumCorridas"))
                    {
                        dgvSeasons.Columns["NumCorridas"]!.HeaderText = "Races Count (Auto)";
                        dgvSeasons.Columns["NumCorridas"]!.ReadOnly = true;
                        dgvSeasons.Columns["NumCorridas"]!.DefaultCellStyle.Format = "N0";
                        dgvSeasons.Columns["NumCorridas"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
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

        private void btnViewStandings_Click(object? sender, EventArgs e)
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
                    using (var standingsDialog = new StandingsViewerDialog(selectedYear))
                    {
                        standingsDialog.ShowDialog();
                    }
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
            if (dgvSeasons.Columns == null || e.ColumnIndex >= dgvSeasons.Columns.Count) return;
            
            string? columnName = dgvSeasons.Columns[e.ColumnIndex]?.Name;
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
            if (dgvSeasons.Columns == null || e.ColumnIndex >= dgvSeasons.Columns.Count) return;
            
            dgvSeasons.Rows[e.RowIndex].ErrorText = "";
            
            string? columnName = dgvSeasons.Columns[e.ColumnIndex]?.Name;
            
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

    // ============================================================================
    // STANDINGS VIEWER DIALOG
    // ============================================================================
    public class StandingsViewerDialog : Form
    {
        private TabControl? tabControl;
        private DataGridView? dgvDriverStandings;
        private DataGridView? dgvTeamStandings;
        private int year;

        public StandingsViewerDialog(int year)
        {
            this.year = year;
            
            this.Text = $"Standings - {year} Season";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            LoadStandings();
        }

        private void SetupUI()
        {
            Label lblTitle = new Label
            {
                Text = $"{year} Season Standings",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            tabControl = new TabControl
            {
                Location = new Point(20, 60),
                Size = new Size(940, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Driver Standings Tab
            TabPage driverTab = new TabPage("Driver Standings");
            dgvDriverStandings = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(910, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            driverTab.Controls.Add(dgvDriverStandings);
            tabControl.TabPages.Add(driverTab);

            // Team Standings Tab
            TabPage teamTab = new TabPage("Team Standings");
            dgvTeamStandings = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(910, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            teamTab.Controls.Add(dgvTeamStandings);
            tabControl.TabPages.Add(teamTab);

            this.Controls.Add(tabControl);

            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(860, 520),
                Size = new Size(100, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnClose);
        }

        private void LoadStandings()
        {
            LoadDriverStandings();
            LoadTeamStandings();
        }

        private void LoadDriverStandings()
        {
            if (dgvDriverStandings == null) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
                            ISNULL(m.Nome, 'Unknown Driver') AS Driver,
                            ISNULL(e.Nome, 'No Team') AS Team,
                            ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
                            COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
                            COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums
                        FROM Piloto p
                        INNER JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                        INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
                        INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
                        WHERE r.NomeSessão = 'Race' AND gp.Ano_Temporada = @Year
                        GROUP BY p.ID_Piloto, m.Nome, e.Nome
                        HAVING ISNULL(SUM(r.Pontos), 0) > 0
                        ORDER BY TotalPoints DESC, Wins DESC";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Year", year);
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    
                    dgvDriverStandings.DataSource = table;
                    
                    dgvDriverStandings.Refresh();
                    Application.DoEvents();

                    if (dgvDriverStandings.Columns != null)
                    {
                        if (dgvDriverStandings.Columns.Contains("Position") && dgvDriverStandings.Columns["Position"] != null)
                            dgvDriverStandings.Columns["Position"]!.Width = 60;
                        if (dgvDriverStandings.Columns.Contains("TotalPoints") && dgvDriverStandings.Columns["TotalPoints"] != null)
                            dgvDriverStandings.Columns["TotalPoints"]!.HeaderText = "Points";
                        if (dgvDriverStandings.Columns.Contains("Wins") && dgvDriverStandings.Columns["Wins"] != null)
                            dgvDriverStandings.Columns["Wins"]!.Width = 60;
                        if (dgvDriverStandings.Columns.Contains("Podiums") && dgvDriverStandings.Columns["Podiums"] != null)
                            dgvDriverStandings.Columns["Podiums"]!.Width = 80;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading driver standings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTeamStandings()
        {
            if (dgvTeamStandings == null) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY ISNULL(SUM(r.Pontos), 0) DESC, COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) DESC) AS Position,
                            e.Nome AS Team,
                            ISNULL(SUM(r.Pontos), 0) AS TotalPoints,
                            COUNT(CASE WHEN r.PosiçãoFinal = 1 THEN 1 END) AS Wins,
                            COUNT(CASE WHEN r.PosiçãoFinal <= 3 THEN 1 END) AS Podiums
                        FROM Equipa e
                        INNER JOIN Piloto p ON e.ID_Equipa = p.ID_Equipa
                        INNER JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
                        INNER JOIN Grande_Prémio gp ON r.NomeGP = gp.NomeGP
                        WHERE r.NomeSessão = 'Race' AND gp.Ano_Temporada = @Year
                        GROUP BY e.ID_Equipa, e.Nome
                        HAVING ISNULL(SUM(r.Pontos), 0) > 0
                        ORDER BY TotalPoints DESC, Wins DESC";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Year", year);
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    
                    dgvTeamStandings.DataSource = table;
                    
                    dgvTeamStandings.Refresh();
                    Application.DoEvents();

                    if (dgvTeamStandings.Columns != null)
                    {
                        if (dgvTeamStandings.Columns.Contains("Position") && dgvTeamStandings.Columns["Position"] != null)
                            dgvTeamStandings.Columns["Position"]!.Width = 60;
                        if (dgvTeamStandings.Columns.Contains("TotalPoints") && dgvTeamStandings.Columns["TotalPoints"] != null)
                            dgvTeamStandings.Columns["TotalPoints"]!.HeaderText = "Points";
                        if (dgvTeamStandings.Columns.Contains("Wins") && dgvTeamStandings.Columns["Wins"] != null)
                            dgvTeamStandings.Columns["Wins"]!.Width = 60;
                        if (dgvTeamStandings.Columns.Contains("Podiums") && dgvTeamStandings.Columns["Podiums"] != null)
                            dgvTeamStandings.Columns["Podiums"]!.Width = 80;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team standings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}