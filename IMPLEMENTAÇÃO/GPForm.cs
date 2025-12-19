using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace ProjetoFBD
{
    public partial class GPForm : Form
    {
        public GPForm() : this("Staff") { }
        private string userRole;
        private SqlDataAdapter? dataAdapter;
        private DataTable? gpTable;

        // Search controls (match Circuits page pattern)
        private Panel? pnlSearch;
        private Label? lblSearch;
        private TextBox? txtSearch;

        public GPForm(string role)
        {
            InitializeComponent(); // DEVE ser chamado primeiro
            
            this.userRole = role;
            this.Text = "Grand Prix Management";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupGPLayout();
            LoadGPData();
        }

        // -------------------------------------------------------------------------
        // UI SETUP - ATUALIZADO (sem criar novos controles)
        // -------------------------------------------------------------------------

        private void SetupGPLayout()
        {
            // --- 1. Configurar DataGridView existente ---
            if (dgvGrandPrix != null)
            {
                dgvGrandPrix.Location = new Point(10, 60); // leave space for search bar
                dgvGrandPrix.Size = new Size(1160, 430);
                dgvGrandPrix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                dgvGrandPrix.AllowUserToAddRows = false;
                dgvGrandPrix.ReadOnly = true;

                // Preencher o ecrã e evitar quebras de linha
                dgvGrandPrix.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvGrandPrix.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgvGrandPrix.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                dgvGrandPrix.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

                // Configure columns only after binding completes (prevents null refs)
                dgvGrandPrix.DataBindingComplete -= DgvGrandPrix_DataBindingComplete;
                dgvGrandPrix.DataBindingComplete += DgvGrandPrix_DataBindingComplete;
            }

            // --- 0. Barra de Pesquisa (Topo) ---
            pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(pnlSearch);

            lblSearch = new Label
            {
                Text = "Search:",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            pnlSearch.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Size = new Size(380, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            txtSearch.TextChanged += txtSearch_TextChanged;
            pnlSearch.Controls.Add(txtSearch);

            pnlSearch.SizeChanged += (s, e) => PositionSearchControls();
            PositionSearchControls();

            // --- 2. Configurar Painéis existentes ---
            if (pnlStaffActions != null)
            {
                pnlStaffActions.Location = new Point(10, 500);
                pnlStaffActions.Size = new Size(570, 50);
                pnlStaffActions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                
                // Criar e adicionar botões ao painel Staff
                Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
                Button btnAdd = CreateActionButton("Add New GP", new Point(0, 5));
                Button btnDelete = CreateActionButton("Delete GP", new Point(0, 5));
                Button btnRefresh = CreateActionButton("Refresh", new Point(0, 5));

                btnSave.Click += btnSave_Click;
                btnAdd.Click += btnAdd_Click;
                btnDelete.Click += btnDelete_Click;
                btnRefresh.Click += btnRefresh_Click;

                pnlStaffActions.Controls.Add(btnSave);
                pnlStaffActions.Controls.Add(btnAdd);
                pnlStaffActions.Controls.Add(btnDelete);
                pnlStaffActions.Controls.Add(btnRefresh);

                // Distribuir botões de forma equidistante dentro do painel
                DistributeButtons(pnlStaffActions);
                pnlStaffActions.Resize += (s, e) => DistributeButtons(pnlStaffActions);
            }

            if (pnlAdditionalActions != null)
            {
                pnlAdditionalActions.Location = new Point(600, 500);
                pnlAdditionalActions.Size = new Size(570, 50);
                pnlAdditionalActions.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                
                // Criar botão de gerenciar sessões
                Button btnManageSessions = CreateActionButton("Manage Sessions", new Point(0, 5));
                btnManageSessions.BackColor = Color.FromArgb(0, 102, 204);
                btnManageSessions.Click += btnManageSessions_Click;
                pnlAdditionalActions.Controls.Add(btnManageSessions);

                // Centrar/Distribuir botão no painel
                DistributeButtons(pnlAdditionalActions);
                pnlAdditionalActions.Resize += (s, e) => DistributeButtons(pnlAdditionalActions);
            }

            // --- 3. Role-Based Access Control (RBAC) ---
            if (dgvGrandPrix != null)
            {
                dgvGrandPrix.ReadOnly = (this.userRole != "Staff");
            }
            
            if (pnlStaffActions != null)
            {
                pnlStaffActions.Visible = (this.userRole == "Staff");
            }
            
            // Manage Sessions deve estar visível para todos os utilizadores
            if (pnlAdditionalActions != null)
            {
                pnlAdditionalActions.Visible = true;
            }
        }

        private Button CreateActionButton(string text, Point location)
        {
            return new Button 
            { 
                Text = text, 
                Location = location, 
                Size = new Size(130, 40), 
                BackColor = Color.FromArgb(204, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        // -------------------------------------------------------------------------
        // DATA ACCESS METHODS (CRUD) - SIMPLIFICADO (apenas 4 colunas)
        // -------------------------------------------------------------------------

        private void LoadGPData()
        {
            string connectionString = ProjetoFBD.DbConfig.ConnectionString;
            
            // Query com JOIN para trazer nome do circuito
            string query = @"
                SELECT 
                    gp.NomeGP,
                    gp.DataCorrida,
                    c.Nome AS Circuit,
                    gp.ID_Circuito,
                    gp.Ano_Temporada AS Season
                FROM Grande_Prémio gp
                INNER JOIN Circuito c ON gp.ID_Circuito = c.ID_Circuito
                ORDER BY gp.Ano_Temporada DESC, gp.DataCorrida ASC";

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                gpTable = new DataTable();
                
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.Fill(gpTable);
                
                if (dgvGrandPrix != null)
                {
                    dgvGrandPrix.AutoGenerateColumns = true;
                    dgvGrandPrix.DataSource = gpTable;

                    // Configure columns once binding finishes to avoid nulls
                    // (handler set in SetupGPLayout)

                    // Remover handlers existentes antes de adicionar novos
                    dgvGrandPrix.CellValidating -= DgvGrandPrix_CellValidating;
                    dgvGrandPrix.CellEndEdit -= DgvGrandPrix_CellEndEdit;
                    
                    dgvGrandPrix.CellValidating += DgvGrandPrix_CellValidating;
                    dgvGrandPrix.CellEndEdit += DgvGrandPrix_CellEndEdit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading GP data: {ex.Message}", "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureColumnHeaders()
        {
            if (dgvGrandPrix == null) return;
            
            // Esconde ID_Circuito, mostra nome do circuito
            if (dgvGrandPrix.Columns.Contains("ID_Circuito"))
            {
                dgvGrandPrix.Columns["ID_Circuito"]!.Visible = false;
            }
            
            var columnHeaders = new Dictionary<string, string>
            {
                { "NomeGP", "Grand Prix Name" },
                { "DataCorrida", "Race Date" },
                { "Circuit", "Circuit" },
                { "Season", "Season" }
            };
            
            foreach (var mapping in columnHeaders)
            {
                if (dgvGrandPrix.Columns.Contains(mapping.Key) && dgvGrandPrix.Columns[mapping.Key] != null)
                {
                    DataGridViewColumn? column = dgvGrandPrix.Columns[mapping.Key];
                    if (column != null)
                    {
                        column.HeaderText = mapping.Value;
                        
                        // Centrar todos os dados e cabeçalhos
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        
                        // NomeGP e Circuit são read-only
                        if (mapping.Key == "NomeGP" || mapping.Key == "Circuit")
                        {
                            column.ReadOnly = true;
                        }
                        
                        // Formatar data
                        if (mapping.Key == "DataCorrida")
                        {
                            column.DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                        
                        // Formatar números
                        if (mapping.Key == "Season")
                        {
                            column.DefaultCellStyle.Format = "N0";
                        }

                        // Ajustar tamanhos para preencher o ecrã
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        if (mapping.Key == "NomeGP")
                        {
                            column.FillWeight = 200;
                            column.MinimumWidth = 200;
                            if (column.DefaultCellStyle != null)
                                column.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                        }
                        else if (mapping.Key == "Circuit")
                        {
                            column.FillWeight = 150;
                            column.MinimumWidth = 140;
                        }
                        else if (mapping.Key == "DataCorrida")
                        {
                            column.FillWeight = 110;
                            column.MinimumWidth = 110;
                        }
                        else // Season
                        {
                            column.FillWeight = 90;
                            column.MinimumWidth = 90;
                        }
                    }
                }
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && gpTable != null && userRole == "Staff")
            {
                string connectionString = ProjetoFBD.DbConfig.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        if (dgvGrandPrix != null)
                        {
                            dgvGrandPrix.EndEdit();
                        }
                        
                        // Validações
                        foreach (DataRow row in gpTable.Rows)
                        {
                            if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                            {
                                // Validar DataCorrida
                                if (row["DataCorrida"] == DBNull.Value || string.IsNullOrWhiteSpace(row["DataCorrida"].ToString()))
                                {
                                    MessageBox.Show("Race Date is required!", "Validation Error", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                
                                // Validar chave estrangeira obrigatória
                                if (row["ID_Circuito"] == DBNull.Value || string.IsNullOrWhiteSpace(row["ID_Circuito"].ToString()))
                                {
                                    MessageBox.Show("Circuit ID is required!", "Validation Error", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                
                                // Alterado de "Ano_Temporada" para "Season"
                                if (row["Season"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Season"].ToString()))
                                {
                                    MessageBox.Show("Season is required!", "Validation Error", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        // Verificar erros
                        var errorRows = gpTable.GetErrors();
                        if (errorRows.Length > 0)
                        {
                            MessageBox.Show($"Please fix errors before saving:\n{string.Join("\n", errorRows.Select(r => r.RowError))}", 
                                "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                        dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                        dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                        dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                        connection.Open();
                        int rowsAffected = dataAdapter.Update(gpTable);
                        
                        MessageBox.Show($"{rowsAffected} rows saved successfully!", "Success");
                        gpTable.AcceptChanges();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Database error: {sqlEx.Message}\nCheck if GP name is unique and foreign keys exist.", 
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        gpTable.RejectChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving data: {ex.Message}", 
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        gpTable.RejectChanges();
                    }
                }
            }
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (gpTable != null && userRole == "Staff")
            {
                using (var addDialog = new AddGPDialog())
                {
                    if (addDialog.ShowDialog() == DialogResult.OK)
                    {
                        string gpName = addDialog.GPName;
                        int circuitId = addDialog.CircuitID;
                        int season = addDialog.Season;
                        DateTime? raceDate = addDialog.RaceDate;

                        // Verificar se o GP já existe
                        bool gpExists = false;
                        foreach (DataRow row in gpTable.Rows)
                        {
                            if (row.RowState != DataRowState.Deleted && 
                                row["NomeGP"] != DBNull.Value && 
                                row["NomeGP"].ToString() == gpName)
                            {
                                gpExists = true;
                                break;
                            }
                        }
                        
                        if (gpExists)
                        {
                            MessageBox.Show($"Grand Prix '{gpName}' already exists!", 
                                "Duplicate GP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        try
                        {
                            DataRow newRow = gpTable.NewRow();
                            newRow["NomeGP"] = gpName;
                            newRow["DataCorrida"] = raceDate.HasValue ? (object)raceDate.Value : DBNull.Value;
                            newRow["ID_Circuito"] = circuitId;
                            newRow["Season"] = season;
                            
                            gpTable.Rows.InsertAt(newRow, 0);
                            
                            // Salvar automaticamente
                            btnSave_Click(sender, e);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error adding new GP: {ex.Message}", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff" && dgvGrandPrix != null && dgvGrandPrix.SelectedRows.Count > 0 && gpTable != null)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete the selected Grand Prix? This will also delete all related sessions.", 
                    "Confirm Deletion", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvGrandPrix.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(r => r.Index))
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
                        MessageBox.Show($"Error during deletion: {ex.Message}", 
                            "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        gpTable.RejectChanges();
                    }
                }
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            if (gpTable != null && gpTable.GetChanges() != null)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to discard them and refresh the data?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    gpTable.RejectChanges();
                    LoadGPData();
                }
            }
            else
            {
                LoadGPData();
            }
        }

        private void btnManageSessions_Click(object? sender, EventArgs e)
        {
            if (dgvGrandPrix == null || dgvGrandPrix.CurrentRow == null || dgvGrandPrix.CurrentRow.Cells["NomeGP"].Value == null)
            {
                MessageBox.Show("Please select a Grand Prix to manage its sessions.", 
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string? selectedGP = dgvGrandPrix.CurrentRow.Cells["NomeGP"].Value?.ToString();
            
            // Adicione verificação para nulo
            if (string.IsNullOrEmpty(selectedGP))
            {
                MessageBox.Show("Invalid Grand Prix selection.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Abrir o SessionForm para gerenciar sessões deste GP
            try
            {
                SessionForm sessionForm = new SessionForm(this.userRole, selectedGP);
                NavigationHelper.NavigateTo(sessionForm, "SESSIONS - " + selectedGP);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening session management: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------------------------------------------------------
        // VALIDATION METHODS - ATUALIZADO
        // -------------------------------------------------------------------------

        private void DgvGrandPrix_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvGrandPrix == null) return;
            
            string? columnName = dgvGrandPrix.Columns[e.ColumnIndex].Name;
            string value = e.FormattedValue?.ToString() ?? "";
            
            try
            {
                if (columnName == "NomeGP")
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = "Grand Prix name cannot be empty!";
                        e.Cancel = true;
                        return;
                    }
                    
                    // Validação: não permitir apenas números
                    if (value.All(char.IsDigit))
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = "Grand Prix name cannot contain only numbers!";
                        e.Cancel = true;
                        return;
                    }
                    
                    // Verificar duplicatas
                    for (int i = 0; i < dgvGrandPrix.Rows.Count; i++)
                    {
                        if (i != e.RowIndex && 
                            dgvGrandPrix.Rows[i].Cells["NomeGP"].Value?.ToString() == value &&
                            !dgvGrandPrix.Rows[i].IsNewRow)
                        {
                            dgvGrandPrix.Rows[e.RowIndex].ErrorText = "Grand Prix name must be unique!";
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                
                if (columnName == "DataCorrida")
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = "Race Date is required!";
                        e.Cancel = true;
                        return;
                    }
                    
                    if (!DateTime.TryParse(value, out DateTime dateValue))
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = "Please enter a valid date (dd/MM/yyyy)";
                        e.Cancel = true;
                        return;
                    }
                    
                    if (dateValue.Year < 1900 || dateValue.Year > DateTime.Now.Year + 1)
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"Year must be between 1900 and {DateTime.Now.Year + 1}";
                        e.Cancel = true;
                        return;
                    }
                }
                
                // Alterado: "Ano_Temporada" → "Season"
                if (columnName == "ID_Circuito" || columnName == "Season")
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"{columnName} is required!";
                        e.Cancel = true;
                        return;
                    }
                    
                    if (!int.TryParse(value, out int intValue))
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"Please enter a valid integer for {columnName}";
                        e.Cancel = true;
                        return;
                    }
                    
                    if (intValue <= 0)
                    {
                        dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"{columnName} must be positive";
                        e.Cancel = true;
                        return;
                    }
                    
                    // Validar se a temporada existe
                    if (columnName == "Season")
                    {
                        if (!CheckIfSeasonExists(intValue))
                        {
                            dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"Season {intValue} does not exist in the database";
                            e.Cancel = true;
                            return;
                        }
                    }
                    
                    // Validar se o circuito existe
                    if (columnName == "ID_Circuito")
                    {
                        if (!CheckIfCircuitExists(intValue))
                        {
                            dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"Circuit ID {intValue} does not exist in the database";
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                
                dgvGrandPrix.Rows[e.RowIndex].ErrorText = "";
            }
            catch (Exception ex)
            {
                dgvGrandPrix.Rows[e.RowIndex].ErrorText = $"Validation error: {ex.Message}";
                e.Cancel = true;
            }
        }

        private void DgvGrandPrix_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (dgvGrandPrix == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            
            dgvGrandPrix.Rows[e.RowIndex].ErrorText = "";
        }

        private void DgvGrandPrix_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                ConfigureColumnHeaders();
                ApplySearchFilter();
            }
            catch { /* avoid bubbling to UI */ }
        }

        // -------------------------------------------------------------------------
        // HELPER METHODS
        // -------------------------------------------------------------------------

        private void DistributeButtons(Panel panel)
        {
            if (panel == null) return;
            var buttons = panel.Controls.OfType<Button>().ToList();
            if (buttons.Count == 0) return;

            int panelWidth = panel.ClientSize.Width;
            int panelHeight = panel.ClientSize.Height;
            int totalButtonsWidth = buttons.Sum(b => b.Width);
            int gaps = buttons.Count + 1;
            int spacing = (panelWidth - totalButtonsWidth) / Math.Max(1, gaps);
            if (spacing < 5) spacing = 5; // espaçamento mínimo

            int x = spacing;
            foreach (var btn in buttons)
            {
                int y = Math.Max(0, (panelHeight - btn.Height) / 2);
                btn.Location = new Point(x, y);
                btn.Anchor = AnchorStyles.Top; // manter posição relativa; redistribuímos no Resize
                x += btn.Width + spacing;
            }
        }

        private void PositionSearchControls()
        {
            if (pnlSearch == null || txtSearch == null || lblSearch == null) return;
            int margin = 12;
            int txtWidth = Math.Min(350, Math.Max(200, pnlSearch.ClientSize.Width / 4));
            txtSearch.Size = new Size(txtWidth, txtSearch.Height);
            int centerY = (pnlSearch.Height - txtSearch.Height) / 2;

            int lblWidth = lblSearch.PreferredWidth + 6;
            int startX = Math.Max(margin, 20);
            lblSearch.Location = new Point(startX, centerY);
            txtSearch.Location = new Point(startX + lblWidth, centerY);
        }

        private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            if (gpTable == null) return;
            string term = txtSearch?.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
            {
                gpTable.DefaultView.RowFilter = string.Empty;
                return;
            }

            string escaped = term.Replace("'", "''");
            string filter =
                $"Convert([NomeGP], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([Circuit], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([Season], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([DataCorrida], 'System.String') LIKE '%{escaped}%'";

            gpTable.DefaultView.RowFilter = filter;
        }

        private bool CheckIfSeasonExists(int year)
        {
            try
            {
                string connectionString = ProjetoFBD.DbConfig.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM Temporada WHERE Ano = @Year";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Year", year);
                    
                    object? result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool CheckIfCircuitExists(int circuitId)
        {
            try
            {
                string connectionString = ProjetoFBD.DbConfig.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM Circuito WHERE ID_Circuito = @CircuitId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CircuitId", circuitId);
                    
                    object? result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    // -------------------------------------------------------------------------
    // DIALOG FOR ADDING NEW GP
    // -------------------------------------------------------------------------
    
    public class AddGPDialog : Form
    {
        private TextBox txtGPName = null!;
        private ComboBox cmbCircuit = null!;
        private ComboBox cmbSeason = null!;
        private DateTimePicker dtpRaceDate = null!;
        private Button btnOK = null!;
        private Button btnCancel = null!;

        public string GPName => txtGPName?.Text?.Trim() ?? "";
        public int CircuitID { get; private set; }
        public int Season { get; private set; }
        public DateTime? RaceDate => dtpRaceDate?.Checked == true ? dtpRaceDate.Value : (DateTime?)null;

        public AddGPDialog()
        {
            InitializeComponent();
            LoadCircuits();
            LoadSeasons();
        }

        private void InitializeComponent()
        {
            this.Text = "Add New Grand Prix";
            this.Size = new Size(500, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // GP Name
            Label lblGPName = new Label
            {
                Text = "Grand Prix Name:",
                Location = new Point(20, 20),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblGPName);

            txtGPName = new TextBox
            {
                Location = new Point(150, 18),
                Size = new Size(310, 25)
            };
            this.Controls.Add(txtGPName);

            // Circuit
            Label lblCircuit = new Label
            {
                Text = "Circuit:",
                Location = new Point(20, 60),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblCircuit);

            cmbCircuit = new ComboBox
            {
                Location = new Point(150, 58),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            this.Controls.Add(cmbCircuit);

            // Season
            Label lblSeason = new Label
            {
                Text = "Season:",
                Location = new Point(20, 100),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblSeason);

            cmbSeason = new ComboBox
            {
                Location = new Point(150, 98),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbSeason);

            // Race Date
            Label lblRaceDate = new Label
            {
                Text = "Race Date:",
                Location = new Point(20, 140),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblRaceDate);

            dtpRaceDate = new DateTimePicker
            {
                Location = new Point(150, 138),
                Size = new Size(310, 25),
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true,
                Checked = false
            };
            this.Controls.Add(dtpRaceDate);

            // Buttons
            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(260, 230),
                Size = new Size(90, 35),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(370, 230),
                Size = new Size(90, 35),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadCircuits()
        {
            if (cmbCircuit == null) return;
            
            try
            {
                string connectionString = ProjetoFBD.DbConfig.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID_Circuito, Nome, Cidade, Pais FROM Circuito ORDER BY Nome";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nome = reader.GetString(1);
                            string cidade = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            string pais = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            
                            string displayText = $"{nome} - {cidade}, {pais}";
                            cmbCircuit.Items.Add(new CircuitItem { ID = id, DisplayText = displayText });
                        }
                    }
                }

                if (cmbCircuit.Items.Count > 0)
                {
                    cmbCircuit.DisplayMember = "DisplayText";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading circuits: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSeasons()
        {
            if (cmbSeason == null) return;
            
            try
            {
                string connectionString = ProjetoFBD.DbConfig.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Ano FROM Temporada ORDER BY Ano DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int year = reader.GetInt32(0);
                            cmbSeason.Items.Add(year);
                        }
                    }
                }

                if (cmbSeason.Items.Count > 0)
                {
                    cmbSeason.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading seasons: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            // Validações
            if (txtGPName == null || string.IsNullOrWhiteSpace(txtGPName.Text))
            {
                MessageBox.Show("Please enter a Grand Prix name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtGPName.Text.Trim().All(char.IsDigit))
            {
                MessageBox.Show("Grand Prix name cannot contain only numbers!", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (cmbCircuit == null || cmbCircuit.SelectedItem == null)
            {
                MessageBox.Show("Please select a circuit.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (cmbSeason == null || cmbSeason.SelectedItem == null)
            {
                MessageBox.Show("Please select a season.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            CircuitID = ((CircuitItem)cmbCircuit.SelectedItem).ID;
            Season = (int)cmbSeason.SelectedItem;
        }

        private class CircuitItem
        {
            public int ID { get; set; }
            public string DisplayText { get; set; } = "";
        }
    }
}