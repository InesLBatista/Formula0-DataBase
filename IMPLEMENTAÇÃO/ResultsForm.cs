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

            dgvResults = new DataGridView
            {
                Name = "dgvResults",
                Location = new Point(20, 70),
                Size = new Size(1340, 470),
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

            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;

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


        private void LoadResults()
        {
            string connectionString = DbConfig.ConnectionString;
            
            // Build query based on filter mode
            string query = @"
                SELECT 
                    r.ID_Resultado,
                    r.PosiçãoGrid,
                    r.TempoFinal,
                    r.PosiçãoFinal,
                    r.Status,
                    r.Pontos,
                    r.NomeSessão,
                    r.ID_Piloto,
                    p.NumeroPermanente,
                    p.Abreviação AS DriverCode,
                    m.Nome AS DriverName,
                    e.Nome AS TeamName,
                    ISNULL(r.NomeGP, s.NomeGP) AS GrandPrix
                FROM Resultados r
                INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
                LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                LEFT JOIN Sessões s ON r.NomeSessão = s.NomeSessão 
                    AND (r.NomeGP = s.NomeGP OR r.NomeGP IS NULL)
                WHERE ";

            // Add filter condition based on mode
            switch (filterMode)
            {
                case ResultsFilterMode.BySession:
                    if (!string.IsNullOrEmpty(gpNameFilter))
                    {
                        // Filter by BOTH GP and Session to get specific session
                        query += "r.NomeSessão = @FilterValue AND (r.NomeGP = @GPName OR s.NomeGP = @GPName) ORDER BY r.PosiçãoFinal ASC";
                    }
                    else
                    {
                        // Filter by session name only (all GPs)
                        query += "r.NomeSessão = @FilterValue ORDER BY r.PosiçãoFinal ASC";
                    }
                    break;
                case ResultsFilterMode.ByTeam:
                    query += "e.Nome = @FilterValue ORDER BY s.NomeGP, r.NomeSessão, r.PosiçãoFinal ASC";
                    break;
                case ResultsFilterMode.ByDriver:
                    query += "p.ID_Piloto = @FilterValue ORDER BY s.NomeGP, r.NomeSessão, r.PosiçãoFinal ASC";
                    break;
            }

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                
                // Add parameter based on filter mode
                if (filterMode == ResultsFilterMode.ByDriver)
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@FilterValue", int.Parse(filterValue));
                }
                else
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@FilterValue", filterValue);
                }
                
                // Add GPName parameter if filtering by specific GP + Session
                if (!string.IsNullOrEmpty(gpNameFilter))
                {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@GPName", gpNameFilter);
                }
                
                resultsTable = new DataTable();
                dataAdapter.Fill(resultsTable);
                
                if (dgvResults != null)
                {
                    dgvResults.DataSource = resultsTable;

                    // Configure columns using BaseForm helper methods
                    MakeColumnReadOnly(dgvResults, "ID_Resultado");
                    SetColumnHeader(dgvResults, "ID_Resultado", "Result ID");
                    if (dgvResults.Columns.Contains("ID_Resultado"))
                        dgvResults.Columns["ID_Resultado"]!.Width = 80;
                    
                    SetColumnHeader(dgvResults, "PosiçãoGrid", "Grid Position");
                    SetColumnHeader(dgvResults, "TempoFinal", "Final Time");
                    SetColumnHeader(dgvResults, "PosiçãoFinal", "Final Position");
                    SetColumnHeader(dgvResults, "Status", "Status");
                    SetColumnHeader(dgvResults, "Pontos", "Points");
                    
                    // Show/hide session and GP columns based on filter mode
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
                        dgvResults.Columns["NumeroPermanente"]!.Width = 50;
                    
                    MakeColumnReadOnly(dgvResults, "DriverCode");
                    SetColumnHeader(dgvResults, "DriverCode", "Driver");
                    if (dgvResults.Columns.Contains("DriverCode"))
                        dgvResults.Columns["DriverCode"]!.Width = 70;
                    
                    MakeColumnReadOnly(dgvResults, "DriverName");
                    SetColumnHeader(dgvResults, "DriverName", "Name");
                    
                    MakeColumnReadOnly(dgvResults, "TeamName");
                    SetColumnHeader(dgvResults, "TeamName", "Team");
                }
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(sqlEx, "loading results");
            }
            catch (Exception ex)
            {
                ShowError($"Error loading results: {ex.Message}");
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
                        {
                            dgvResults.EndEdit();
                        }

                        connection.Open();
                        
                        dataAdapter.UpdateCommand = new SqlCommand(
                            @"UPDATE Resultados 
                              SET PosiçãoGrid = @PosiçãoGrid, 
                                  TempoFinal = @TempoFinal, 
                                  PosiçãoFinal = @PosiçãoFinal,
                                  Status = @Status,
                                  Pontos = @Pontos,
                                  ID_Piloto = @ID_Piloto
                              WHERE ID_Resultado = @ID_Resultado", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@PosiçãoGrid", SqlDbType.Int, 0, "PosiçãoGrid");
                        dataAdapter.UpdateCommand.Parameters.Add("@TempoFinal", SqlDbType.Time, 0, "TempoFinal");
                        dataAdapter.UpdateCommand.Parameters.Add("@PosiçãoFinal", SqlDbType.Int, 0, "PosiçãoFinal");
                        dataAdapter.UpdateCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50, "Status");
                        dataAdapter.UpdateCommand.Parameters.Add("@Pontos", SqlDbType.Decimal, 0, "Pontos");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Piloto", SqlDbType.Int, 0, "ID_Piloto");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Resultado", SqlDbType.Int, 0, "ID_Resultado");
                        
                        dataAdapter.InsertCommand = new SqlCommand(
                            @"INSERT INTO Resultados (PosiçãoGrid, TempoFinal, PosiçãoFinal, Status, Pontos, NomeSessão, ID_Piloto) 
                              VALUES (@PosiçãoGrid, @TempoFinal, @PosiçãoFinal, @Status, @Pontos, @NomeSessão, @ID_Piloto)", connection);
                        dataAdapter.InsertCommand.Parameters.Add("@PosiçãoGrid", SqlDbType.Int, 0, "PosiçãoGrid");
                        dataAdapter.InsertCommand.Parameters.Add("@TempoFinal", SqlDbType.Time, 0, "TempoFinal");
                        dataAdapter.InsertCommand.Parameters.Add("@PosiçãoFinal", SqlDbType.Int, 0, "PosiçãoFinal");
                        dataAdapter.InsertCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50, "Status");
                        dataAdapter.InsertCommand.Parameters.Add("@Pontos", SqlDbType.Decimal, 0, "Pontos");
                        dataAdapter.InsertCommand.Parameters.Add("@NomeSessão", SqlDbType.NVarChar, 100, "NomeSessão");
                        dataAdapter.InsertCommand.Parameters.Add("@ID_Piloto", SqlDbType.Int, 0, "ID_Piloto");
                        
                        dataAdapter.DeleteCommand = new SqlCommand(
                            @"DELETE FROM Resultados WHERE ID_Resultado = @ID_Resultado", connection);
                        dataAdapter.DeleteCommand.Parameters.Add("@ID_Resultado", SqlDbType.Int, 0, "ID_Resultado");

                        int rowsAffected = dataAdapter.Update(resultsTable);
                        ShowSuccess($"{rowsAffected} row(s) updated successfully!");
                        
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
                    // Select driver
                    int selectedDriverId = 0;
                    using (SqlConnection connection = new SqlConnection(DbConfig.ConnectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(
                            @"SELECT p.ID_Piloto, p.NumeroPermanente, p.Abreviação, m.Nome, e.Nome AS Team
                              FROM Piloto p
                              LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                              LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                              ORDER BY e.Nome, m.Nome", connection);
                        SqlDataReader reader = cmd.ExecuteReader();
                        
                        var drivers = new System.Collections.Generic.List<DriverItem>();
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int number = reader.GetInt32(1);
                            string code = reader.GetString(2);
                            string name = reader.IsDBNull(3) ? "Unknown" : reader.GetString(3);
                            string team = reader.IsDBNull(4) ? "No Team" : reader.GetString(4);
                            
                            drivers.Add(new DriverItem 
                            { 
                                ID = id,
                                DisplayText = $"#{number} {code} - {name} ({team})"
                            });
                        }
                        reader.Close();
                        
                        if (drivers.Count == 0)
                        {
                            ShowWarning("No drivers available. Please add drivers first.");
                            return;
                        }
                        
                        using (var driverSelector = new DriverSelectorDialog(drivers))
                        {
                            if (driverSelector.ShowDialog() == DialogResult.OK)
                            {
                                selectedDriverId = driverSelector.SelectedDriverId;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    
                    DataRow newRow = resultsTable.NewRow();
                    newRow["ID_Piloto"] = selectedDriverId;
                    newRow["NomeSessão"] = filterValue;  // Use filterValue which contains the session name
                    newRow["PosiçãoGrid"] = DBNull.Value;
                    newRow["TempoFinal"] = DBNull.Value;
                    newRow["PosiçãoFinal"] = DBNull.Value;
                    newRow["Status"] = "Finished";
                    newRow["Pontos"] = 0;
                    
                    resultsTable.Rows.Add(newRow);

                    if (dgvResults != null && dgvResults.Rows.Count > 0)
                    {
                        dgvResults.CurrentCell = dgvResults.Rows[dgvResults.Rows.Count - 1].Cells[1];
                    }
                    
                    ShowSuccess("Result added. Click 'Save Changes' to commit.");
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



        public class DriverItem
        {
            public int ID { get; set; }
            public string DisplayText { get; set; } = "";
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
}
