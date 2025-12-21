using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjetoFBD
{
    public enum ResultsFilterMode
    {
        BySession,
        ByTeam,
        ByDriver
    }

    public partial class ResultsForm : BaseForm
    {
        private DataGridView? dgvResults;
        private Panel? pnlStaffActions;
        
        // Search controls
        private Panel? pnlSearch;
        private Label? lblSearch;
        private TextBox? txtSearch;
        
        private SqlDataAdapter? dataAdapter;
        private DataTable? resultsTable;
        private ResultsFilterMode filterMode;
        private string filterValue;
        private string? gpNameFilter; // For filtering by specific GP + Session

        // Constructor for filtering by session (all GPs with this session name)
        public ResultsForm(string role, string sessionName) : base(role)
        {
            InitializeComponent();
            
            this.filterMode = ResultsFilterMode.BySession;
            this.filterValue = sessionName;
            this.gpNameFilter = null; // null means all GPs
            
            this.Text = $"Session Results - {sessionName}";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadResults();
        }

        // Constructor for filtering by SPECIFIC session in a SPECIFIC GP
        public ResultsForm(string role, string gpName, string sessionName) : base(role)
        {
            InitializeComponent();
            
            this.filterMode = ResultsFilterMode.BySession;
            this.filterValue = sessionName;
            this.gpNameFilter = gpName; // Filter by this specific GP
            
            this.Text = $"Results - {gpName} - {sessionName}";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadResults();
        }

        // Constructor for filtering by team
        public ResultsForm(string role, string teamName, bool isTeamFilter) : base(role)
        {
            InitializeComponent();
            
            this.filterMode = ResultsFilterMode.ByTeam;
            this.filterValue = teamName;
            this.gpNameFilter = null;
            
            this.Text = $"Team Results - {teamName}";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadResults();
        }

        // Constructor for filtering by driver
        public ResultsForm(string role, int driverId, string driverName) : base(role)
        {
            InitializeComponent();
            
            this.filterMode = ResultsFilterMode.ByDriver;
            this.filterValue = driverId.ToString();
            this.gpNameFilter = null;
            
            this.Text = $"Driver Results - {driverName}";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadResults();
        }

        private void SetupLayout()
        {
            string title = filterMode switch
            {
                ResultsFilterMode.BySession => $"Session Results - {filterValue}",
                ResultsFilterMode.ByTeam => $"Team Results - {filterValue}",
                ResultsFilterMode.ByDriver => $"Driver Results - {filterValue}",
                _ => "Results"
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(20, 20),
                Size = new Size(800, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // --- Barra de Pesquisa ---
            pnlSearch = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1340, 45),
                BackColor = Color.WhiteSmoke,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(pnlSearch);

            lblSearch = new Label
            {
                Text = "Search:",
                AutoSize = true,
                Location = new Point(10, 12)
            };
            pnlSearch.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Size = new Size(380, 27),
                Location = new Point(70, 10)
            };
            txtSearch.TextChanged += txtSearch_TextChanged;
            pnlSearch.Controls.Add(txtSearch);

            dgvResults = new DataGridView
            {
                Name = "dgvResults",
                Location = new Point(20, 115),
                Size = new Size(1340, 425),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = false,
                AutoGenerateColumns = true
            };
            ConfigureDataGridView(dgvResults);
            this.Controls.Add(dgvResults);

            pnlStaffActions = new Panel
            {
                Location = new Point(20, 560),
                Size = new Size(840, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add Result", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(270, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(380, 5));
            
            // Adicionar botão de pitstops apenas para Race sessions
            Button? btnViewPitstops = null;
            if (filterMode == ResultsFilterMode.BySession && 
                (filterValue == "Race" || filterValue == "Sprint Race"))
            {
                btnViewPitstops = CreateActionButton("View Pitstops", new Point(510, 5), Color.FromArgb(128, 0, 128));
                btnViewPitstops.Click += btnViewPitstops_Click;
                pnlStaffActions.Controls.Add(btnViewPitstops);
            }

            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            
            // Auto-fill GrandPrix and Session when user adds rows directly in grid
            dgvResults.UserAddedRow += DgvResults_UserAddedRow;

            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);

            if (userRole == "Staff")
            {
                dgvResults.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvResults.ReadOnly = true;
                pnlStaffActions.Visible = false;
            }
        }


        private static bool SessionUsesGridPosition(string sessionName)
        {
            return sessionName.Equals("Race", StringComparison.OrdinalIgnoreCase)
                || sessionName.Equals("Sprint Race", StringComparison.OrdinalIgnoreCase);
        }

        private void LoadResults()
        {
            string connectionString = DbConfig.ConnectionString;
            string filter = "";
            string order = "";
            var table = new DataTable();

            // Montar filtro conforme o modo
            switch (filterMode)
            {
                case ResultsFilterMode.BySession:
                    if (!string.IsNullOrEmpty(gpNameFilter))
                    {
                        filter = $"NomeSessão = @FilterValue AND GrandPrix = @GPName";
                        order = "ORDER BY PosiçãoFinal ASC";
                    }
                    else
                    {
                        filter = "NomeSessão = @FilterValue";
                        order = "ORDER BY PosiçãoFinal ASC";
                    }
                    break;
                case ResultsFilterMode.ByTeam:
                    filter = "TeamName = @FilterValue";
                    order = "ORDER BY GrandPrix, NomeSessão, PosiçãoFinal ASC";
                    break;
                case ResultsFilterMode.ByDriver:
                    filter = "ID_Piloto = @FilterValue";
                    order = "ORDER BY GrandPrix, NomeSessão, PosiçãoFinal ASC";
                    break;
            }

            string sql = $"SELECT * FROM vw_Results_Detailed WHERE {filter} {order}";
            try
            {
                dataAdapter = new SqlDataAdapter(sql, connectionString);
                if (filterMode == ResultsFilterMode.ByDriver)
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@FilterValue", int.Parse(filterValue));
                else
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@FilterValue", filterValue);
                if (!string.IsNullOrEmpty(gpNameFilter))
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@GPName", gpNameFilter);
                resultsTable = new DataTable();
                dataAdapter.Fill(resultsTable);
                if (dgvResults != null)
                {
                    dgvResults.DataSource = resultsTable;
                    dgvResults.Refresh();
                    Application.DoEvents();
                    try
                    {
                        foreach (DataGridViewColumn col in dgvResults.Columns)
                        {
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        MakeColumnReadOnly(dgvResults, "ID_Resultado");
                        HideColumn(dgvResults, "ID_Resultado");
                        SetColumnHeader(dgvResults, "PosiçãoGrid", "Grid Position");
                        SetColumnHeader(dgvResults, "TempoFinal", "Final Time");
                        SetColumnHeader(dgvResults, "PosiçãoFinal", "Final Position");
                        SetColumnHeader(dgvResults, "Status", "Status");
                        SetColumnHeader(dgvResults, "Pontos", "Points");
                        if (filterMode == ResultsFilterMode.BySession && !SessionUsesGridPosition(filterValue))
                        {
                            HideColumn(dgvResults, "PosiÇõÇœoGrid");
                        }
                        if (filterMode == ResultsFilterMode.BySession)
                        {
                            HideColumn(dgvResults, "NomeSessão");
                            HideColumn(dgvResults, "GrandPrix");
                        }
                        else
                        {
                            SetColumnHeader(dgvResults, "NomeSessão", "Session");
                            SetColumnHeader(dgvResults, "GrandPrix", "Grand Prix");
                            MakeColumnReadOnly(dgvResults, "NomeSessão");
                            MakeColumnReadOnly(dgvResults, "GrandPrix");
                        }
                        HideColumn(dgvResults, "ID_Piloto");
                        MakeColumnReadOnly(dgvResults, "NumeroPermanente");
                        SetColumnHeader(dgvResults, "NumeroPermanente", "#");
                        if (dgvResults.Columns.Contains("NumeroPermanente"))
                        {
                            var numCol = dgvResults.Columns["NumeroPermanente"];
                            if (numCol != null)
                                numCol.Width = 50;
                        }
                        MakeColumnReadOnly(dgvResults, "DriverCode");
                        SetColumnHeader(dgvResults, "DriverCode", "Driver");
                        if (dgvResults.Columns.Contains("DriverCode"))
                        {
                            var driverCol = dgvResults.Columns["DriverCode"];
                            if (driverCol != null)
                                driverCol.Width = 70;
                        }
                        MakeColumnReadOnly(dgvResults, "DriverName");
                        SetColumnHeader(dgvResults, "DriverName", "Name");
                        MakeColumnReadOnly(dgvResults, "TeamName");
                        SetColumnHeader(dgvResults, "TeamName", "Team");
                    }
                    catch (Exception colEx)
                    {
                        Console.WriteLine($"Error configuring columns: {colEx.Message}");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(sqlEx, "loading results");
            }
            catch (Exception ex)
            {
                ShowError($"Error loading results: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && resultsTable != null && userRole == "Staff")
            {
                string connectionString = DbConfig.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        if (dgvResults != null)
                            dgvResults.EndEdit();
                        connection.Open();
                        foreach (DataRow row in resultsTable.Rows)
                        {
                            string rowSession = filterMode == ResultsFilterMode.BySession
                                ? filterValue
                                : row["NomeSessÇœo"]?.ToString() ?? string.Empty;
                            object gridValue = SessionUsesGridPosition(rowSession) && row.Table.Columns.Contains("PosiÇõÇœoGrid")
                                ? row["PosiÇõÇœoGrid"]
                                : DBNull.Value;

                            if (row.RowState == DataRowState.Added)
                            {
                                using (SqlCommand cmd = new SqlCommand("sp_InsertResult", connection))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@PosicaoGrid", gridValue ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@TempoFinal", row["TempoFinal"] == DBNull.Value ? (object)DBNull.Value : row["TempoFinal"]);
                                    cmd.Parameters.AddWithValue("@PosicaoFinal", row["PosiÇõÇœoFinal"]);
                                    cmd.Parameters.AddWithValue("@Status", row["Status"]);
                                    cmd.Parameters.AddWithValue("@Pontos", row["Pontos"]);
                                    cmd.Parameters.AddWithValue("@NomeSessao", row["NomeSessÇœo"]);
                                    cmd.Parameters.AddWithValue("@NomeGP", row["GrandPrix"]);
                                    cmd.Parameters.AddWithValue("@ID_Piloto", row["ID_Piloto"]);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else if (row.RowState == DataRowState.Modified)
                            {
                                using (SqlCommand cmd = new SqlCommand("sp_UpdateResult", connection))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@ID_Resultado", row["ID_Resultado"]);
                                    cmd.Parameters.AddWithValue("@PosicaoGrid", gridValue ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@TempoFinal", row["TempoFinal"] == DBNull.Value ? (object)DBNull.Value : row["TempoFinal"]);
                                    cmd.Parameters.AddWithValue("@PosicaoFinal", row["PosiÇõÇœoFinal"]);
                                    cmd.Parameters.AddWithValue("@Status", row["Status"]);
                                    cmd.Parameters.AddWithValue("@Pontos", row["Pontos"]);
                                    cmd.Parameters.AddWithValue("@NomeGP", row["GrandPrix"]);
                                    cmd.Parameters.AddWithValue("@ID_Piloto", row["ID_Piloto"]);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else if (row.RowState == DataRowState.Deleted)
                            {
                                using (SqlCommand cmd = new SqlCommand("sp_DeleteResult", connection))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@ID_Resultado", row["ID_Resultado", DataRowVersion.Original]);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        ShowSuccess("Alterações salvas com sucesso!");
                        resultsTable.AcceptChanges();
                        LoadResults();
                    }
                    catch (SqlException sqlEx)
                    {
                        HandleSqlException(sqlEx, "saving changes");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error saving changes: {ex.Message}");
                    }
                }
            }
        }

        private void DgvResults_UserAddedRow(object? sender, DataGridViewRowEventArgs e)
        {
            // Auto-fill GrandPrix and Session when user adds rows directly
            if (resultsTable != null && filterMode == ResultsFilterMode.BySession)
            {
                int newRowIndex = e.Row.Index;
                if (newRowIndex >= 0 && newRowIndex < resultsTable.Rows.Count)
                {
                    DataRow row = resultsTable.Rows[newRowIndex];
                    
                    // Auto-fill session name
                    if (row["NomeSessão"] == DBNull.Value || string.IsNullOrWhiteSpace(row["NomeSessão"].ToString()))
                    {
                        row["NomeSessão"] = filterValue;
                    }
                    
                    // Auto-fill GP name if available
                    if (resultsTable.Columns.Contains("GrandPrix") && !string.IsNullOrEmpty(gpNameFilter))
                    {
                        if (row["GrandPrix"] == DBNull.Value || string.IsNullOrWhiteSpace(row["GrandPrix"].ToString()))
                        {
                            row["GrandPrix"] = gpNameFilter;
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            // Only allow adding results when filtering by session
            if (filterMode != ResultsFilterMode.BySession)
            {
                ShowWarning("Can only add results from the Session view. Please open results from a specific session.");
                return;
            }

            if (userRole == "Staff" && resultsTable != null)
            {
                try
                {
                    string sessionName = filterValue;
                    string? gpName = gpNameFilter;

                    // Validação de existência de sessão e qualificação pode ser feita via views ou SPs, mas aqui apenas chamada do dialog
                    using (var dialog = new AddResultDialog(sessionName, gpName))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            DataRow newRow = resultsTable.NewRow();
                            newRow["ID_Piloto"] = dialog.SelectedDriverId;
                            newRow["NomeSessão"] = sessionName;
                            newRow["PosiçãoGrid"] = dialog.GridPosition;
                            newRow["TempoFinal"] = string.IsNullOrWhiteSpace(dialog.FinalTime) ? DBNull.Value : (object)TimeSpan.Parse(dialog.FinalTime);
                            newRow["PosiçãoFinal"] = dialog.FinalPosition;
                            newRow["Status"] = dialog.Status;
                            newRow["Pontos"] = dialog.Points;
                            if (resultsTable.Columns.Contains("GrandPrix"))
                            {
                                if (!string.IsNullOrEmpty(gpName))
                                    newRow["GrandPrix"] = gpName;
                                else
                                {
                                    ShowError("Cannot add result: Grand Prix name is missing. Please open results from a specific GP session.");
                                    return;
                                }
                            }
                            resultsTable.Rows.Add(newRow);
                            if (dgvResults != null && dgvResults.Rows.Count > 0)
                                dgvResults.CurrentCell = dgvResults.Rows[dgvResults.Rows.Count - 1].Cells[1];
                            ShowSuccess("Result added. Click 'Save Changes' to commit.");
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    HandleSqlException(sqlEx, "adding result");
                }
                catch (Exception ex)
                {
                    ShowError($"Error adding result: {ex.Message}");
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole != "Staff" || dgvResults == null || resultsTable == null)
                return;

            if (!IsRowSelected(dgvResults, "result"))
                return;

            if (ShowConfirmation($"Are you sure you want to delete {dgvResults.SelectedRows.Count} result(s)?\n\nThis action cannot be undone.", "Confirm Deletion"))
            {
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvResults.SelectedRows)
                        {
                            if (row.Index >= 0 && row.Index < resultsTable.Rows.Count)
                            {
                                resultsTable.Rows[row.Index].Delete();
                            }
                        }
                        ShowSuccess("Selected row(s) marked for deletion. Click 'Save Changes' to commit.");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error deleting result: {ex.Message}");
                    }
                }
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            if (resultsTable != null && resultsTable.GetChanges() != null)
            {
                if (ShowConfirmation("You have unsaved changes. Do you want to discard them and refresh?", "Unsaved Changes"))
                {
                    resultsTable.RejectChanges();
                    LoadResults();
                }
            }
            else
            {
                LoadResults();
            }
        }

        private void btnViewPitstops_Click(object? sender, EventArgs e)
        {
            if (filterMode == ResultsFilterMode.BySession && !string.IsNullOrEmpty(gpNameFilter))
            {
                try
                {
                    PitstopViewerDialog pitstopDialog = new PitstopViewerDialog(filterValue, gpNameFilter, this.userRole);
                    pitstopDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    ShowError($"Error opening pitstops: {ex.Message}");
                }
            }
            else
            {
                ShowWarning("Pitstops can only be viewed from a specific Race session.");
            }
        }

        private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            if (resultsTable == null) return;
            string term = txtSearch?.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
            {
                resultsTable.DefaultView.RowFilter = string.Empty;
                return;
            }

            string escaped = term.Replace("'", "''");
            
            // Filtrar por driver name, code, team, position, status
            string filter =
                $"Convert([DriverName], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([DriverCode], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([TeamName], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([Status], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([NumeroPermanente], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([PosiçãoFinal], 'System.String') LIKE '%{escaped}%' " +
                $"OR Convert([PosiçãoGrid], 'System.String') LIKE '%{escaped}%'";
            
            // Adicionar filtros por GP e Session se as colunas não estiverem escondidas
            if (filterMode != ResultsFilterMode.BySession && resultsTable.Columns.Contains("NomeSessão"))
            {
                filter += $" OR Convert([NomeSessão], 'System.String') LIKE '%{escaped}%'";
            }
            if (filterMode != ResultsFilterMode.BySession && resultsTable.Columns.Contains("GrandPrix"))
            {
                filter += $" OR Convert([GrandPrix], 'System.String') LIKE '%{escaped}%'";
            }

            resultsTable.DefaultView.RowFilter = filter;
        }
    }

    // Helper class for driver selection
    public class DriverItem
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Team { get; set; } = "";
        public string DisplayText { get; set; } = "";

        public override string ToString() => DisplayText;
    }

    // Dialog for adding race results with validation and auto-calculation
    public class AddResultDialog : Form
    {
        public int SelectedDriverId { get; private set; }
        public int GridPosition { get; private set; }
        public string FinalTime { get; private set; } = "";
        public int FinalPosition { get; private set; }
        public string Status { get; private set; } = "Finished";
        public decimal Points { get; private set; }

        private ComboBox cmbDriver = null!;
        private NumericUpDown nudGridPosition = null!;
        private TextBox txtFinalTime = null!;
        private NumericUpDown nudFinalPosition = null!;
        private ComboBox cmbStatus = null!;
        private Label lblPoints = null!;
        private Label lblGridPositionInfo = null!;
        private string sessionName;
        private string? gpName;

        public AddResultDialog(string sessionName, string? gpName)
        {
            this.sessionName = sessionName;
            this.gpName = gpName;
            
            InitializeComponents();
            LoadDrivers();
            
            if (sessionName == "Race" || sessionName == "Sprint Race")
            {
                LoadQualificationPositions();
            }
        }

        private void InitializeComponents()
        {
            this.Text = $"Add Result - {sessionName}";
            this.Size = new Size(550, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;
            int labelWidth = 120;
            int controlWidth = 380;

            // Driver selection
            Label lblDriver = new Label
            {
                Text = "Driver:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblDriver);

            cmbDriver = new ComboBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };
            cmbDriver.SelectedIndexChanged += (s, e) =>
            {
                if (sessionName == "Race" || sessionName == "Sprint Race")
                {
                    AutoFillGridPosition();
                }
            };
            this.Controls.Add(cmbDriver);
            yPos += 45;

            // Grid Position
            Label lblGrid = new Label
            {
                Text = "Grid Position:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblGrid);

            nudGridPosition = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(100, 30),
                Minimum = 1,
                Maximum = 30,
                Value = 1,
                Font = new Font("Arial", 10)
            };
            if (sessionName == "Race" || sessionName == "Sprint Race")
            {
                nudGridPosition.ReadOnly = true;
                nudGridPosition.BackColor = Color.LightGray;
            }
            this.Controls.Add(nudGridPosition);

            lblGridPositionInfo = new Label
            {
                Location = new Point(260, yPos),
                Size = new Size(270, 40),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = sessionName == "Race" ? "Auto-filled from Qualification" :
                       sessionName == "Sprint Race" ? "Auto-filled from Sprint Qualification" :
                       "Starting position on track"
            };
            this.Controls.Add(lblGridPositionInfo);
            yPos += 55;

            // Final Position
            Label lblFinal = new Label
            {
                Text = "Final Position:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblFinal);

            nudFinalPosition = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(100, 30),
                Minimum = 1,
                Maximum = 30,
                Value = 1,
                Font = new Font("Arial", 10)
            };
            nudFinalPosition.ValueChanged += (s, e) => CalculatePoints();
            this.Controls.Add(nudFinalPosition);
            yPos += 45;

            // Final Time
            Label lblTime = new Label
            {
                Text = "Final Time:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblTime);

            txtFinalTime = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 30),
                Font = new Font("Arial", 10),
                PlaceholderText = "HH:MM:SS.mmm"
            };
            this.Controls.Add(txtFinalTime);

            Label lblTimeHelp = new Label
            {
                Location = new Point(310, yPos),
                Size = new Size(220, 40),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = "Format: 01:25:45.123\n(Hours:Minutes:Seconds.Milliseconds)"
            };
            this.Controls.Add(lblTimeHelp);
            yPos += 55;

            // Status
            Label lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                Location = new Point(150, yPos),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };
            cmbStatus.Items.AddRange(new string[] { "Finished", "DNF", "DNS", "DSQ", "Retired" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            yPos += 45;

            // Points (auto-calculated)
            Label lblPointsLabel = new Label
            {
                Text = "Points:",
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblPointsLabel);

            lblPoints = new Label
            {
                Location = new Point(150, yPos),
                Size = new Size(100, 30),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20),
                Text = "0",
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblPoints);

            Label lblAutoCalc = new Label
            {
                Location = new Point(260, yPos),
                Size = new Size(270, 25),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = "Auto-calculated based on position"
            };
            this.Controls.Add(lblAutoCalc);
            yPos += 50;

            // Buttons
            Button btnOK = new Button
            {
                Text = "Add Result",
                Location = new Point(300, yPos),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold),
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(420, yPos),
                Size = new Size(100, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            // Initial points calculation
            CalculatePoints();
        }

        private void LoadDrivers()
        {
            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ID_Piloto, NumeroPermanente, Abreviação, DriverName, Team FROM vw_Drivers_List ORDER BY Team, DriverName", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new DriverItem
                    {
                        ID = reader.GetInt32(0),
                        Number = reader.GetInt32(1),
                        Code = reader.GetString(2),
                        Name = reader.IsDBNull(3) ? "Unknown" : reader.GetString(3),
                        Team = reader.IsDBNull(4) ? "No Team" : reader.GetString(4)
                    };
                    item.DisplayText = $"#{item.Number} {item.Code} - {item.Name} ({item.Team})";
                    cmbDriver.Items.Add(item);
                }
                reader.Close();
            }
            if (cmbDriver.Items.Count > 0)
                cmbDriver.SelectedIndex = 0;
        }

        private void LoadQualificationPositions()
        {
            // This will store qualification results to auto-fill grid positions
            string qualSession = sessionName == "Race" ? "Qualification" : "Sprint Qualification";
            
            lblGridPositionInfo.Text = $"Will be auto-filled from {qualSession} results";
        }

        private void AutoFillGridPosition()
        {
            if (cmbDriver.SelectedItem == null || string.IsNullOrEmpty(gpName))
                return;

            var selectedDriver = (DriverItem)cmbDriver.SelectedItem;
            string qualSession = sessionName == "Race" ? "Qualification" : "Sprint Qualification";

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                string query = @"SELECT PosiçãoFinal FROM vw_Qualification_Positions WHERE ID_Piloto = @DriverId AND NomeSessão = @QualSession AND NomeGP = @GPName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DriverId", selectedDriver.ID);
                cmd.Parameters.AddWithValue("@QualSession", qualSession);
                cmd.Parameters.AddWithValue("@GPName", gpName);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    nudGridPosition.Value = Convert.ToInt32(result);
                    lblGridPositionInfo.Text = $"From {qualSession}: P{result}";
                    lblGridPositionInfo.ForeColor = Color.Green;
                }
                else
                {
                    lblGridPositionInfo.Text = $"No {qualSession} result found for this driver!";
                    lblGridPositionInfo.ForeColor = Color.Red;
                }
            }
        }

        private void CalculatePoints()
        {
            int position = (int)nudFinalPosition.Value;
            decimal points = 0;

            if (sessionName == "Race")
            {
                // Race points: 25, 18, 15, 12, 10, 8, 6, 4, 2, 1
                points = position switch
                {
                    1 => 25,
                    2 => 18,
                    3 => 15,
                    4 => 12,
                    5 => 10,
                    6 => 8,
                    7 => 6,
                    8 => 4,
                    9 => 2,
                    10 => 1,
                    _ => 0
                };
            }
            else if (sessionName == "Sprint Race")
            {
                // Sprint Race points: 8, 7, 6, 5, 4, 3, 2, 1
                points = position switch
                {
                    1 => 8,
                    2 => 7,
                    3 => 6,
                    4 => 5,
                    5 => 4,
                    6 => 3,
                    7 => 2,
                    8 => 1,
                    _ => 0
                };
            }

            lblPoints.Text = points.ToString();
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            if (cmbDriver.SelectedItem == null)
            {
                MessageBox.Show("Please select a driver.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedDriver = (DriverItem)cmbDriver.SelectedItem;
            SelectedDriverId = selectedDriver.ID;
            GridPosition = (int)nudGridPosition.Value;
            FinalPosition = (int)nudFinalPosition.Value;
            FinalTime = txtFinalTime.Text;
            Status = cmbStatus.SelectedItem?.ToString() ?? "Finished";
            Points = decimal.Parse(lblPoints.Text);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public class DriverSelectorDialog : Form
    {
        public int SelectedDriverId { get; private set; }
        private ComboBox cmbDrivers;

        public DriverSelectorDialog(System.Collections.Generic.List<DriverItem> drivers)
        {
            this.Text = "Select Driver";
            this.Size = new Size(500, 180);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                Label lblPrompt = new Label
                {
                    Text = "Select a driver:",
                    Location = new Point(20, 20),
                    Size = new Size(450, 20),
                    Font = new Font("Arial", 10)
                };
                this.Controls.Add(lblPrompt);

                cmbDrivers = new ComboBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(440, 25),
                    Font = new Font("Arial", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                
                foreach (var driver in drivers)
                {
                    cmbDrivers.Items.Add(driver);
                }
                cmbDrivers.DisplayMember = "DisplayText";
                
                if (cmbDrivers.Items.Count > 0)
                    cmbDrivers.SelectedIndex = 0;
                
                this.Controls.Add(cmbDrivers);

                Button btnOK = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(280, 90),
                    Size = new Size(80, 30),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOK.FlatAppearance.BorderSize = 0;
                btnOK.Click += (s, e) =>
                {
                    if (cmbDrivers.SelectedItem is DriverItem selected)
                    {
                        SelectedDriverId = selected.ID;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                };
                this.Controls.Add(btnOK);

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(370, 90),
                    Size = new Size(80, 30),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.FlatAppearance.BorderSize = 0;
                this.Controls.Add(btnCancel);

                this.AcceptButton = btnOK;
                this.CancelButton = btnCancel;
            }
        }
    }
