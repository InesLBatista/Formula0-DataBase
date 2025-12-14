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
                    
                    // Wait for columns to be auto-generated
                    dgvResults.Refresh();
                    Application.DoEvents();

                    try
                    {
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
                    catch (Exception colEx)
                    {
                        Console.WriteLine($"Error configuring columns: {colEx.Message}");
                        // Continue - column configuration is not critical for basic functionality
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
                                  ID_Piloto = @ID_Piloto,
                                  NomeGP = @NomeGP
                              WHERE ID_Resultado = @ID_Resultado", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@PosiçãoGrid", SqlDbType.Int, 0, "PosiçãoGrid");
                        dataAdapter.UpdateCommand.Parameters.Add("@TempoFinal", SqlDbType.Time, 0, "TempoFinal");
                        dataAdapter.UpdateCommand.Parameters.Add("@PosiçãoFinal", SqlDbType.Int, 0, "PosiçãoFinal");
                        dataAdapter.UpdateCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50, "Status");
                        dataAdapter.UpdateCommand.Parameters.Add("@Pontos", SqlDbType.Decimal, 0, "Pontos");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Piloto", SqlDbType.Int, 0, "ID_Piloto");
                        dataAdapter.UpdateCommand.Parameters.Add("@NomeGP", SqlDbType.NVarChar, 100, "GrandPrix");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Resultado", SqlDbType.Int, 0, "ID_Resultado");
                        
                        dataAdapter.InsertCommand = new SqlCommand(
                            @"INSERT INTO Resultados (PosiçãoGrid, TempoFinal, PosiçãoFinal, Status, Pontos, NomeSessão, NomeGP, ID_Piloto) 
                              VALUES (@PosiçãoGrid, @TempoFinal, @PosiçãoFinal, @Status, @Pontos, @NomeSessão, @NomeGP, @ID_Piloto)", connection);
                        dataAdapter.InsertCommand.Parameters.Add("@PosiçãoGrid", SqlDbType.Int, 0, "PosiçãoGrid");
                        dataAdapter.InsertCommand.Parameters.Add("@TempoFinal", SqlDbType.Time, 0, "TempoFinal");
                        dataAdapter.InsertCommand.Parameters.Add("@PosiçãoFinal", SqlDbType.Int, 0, "PosiçãoFinal");
                        dataAdapter.InsertCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50, "Status");
                        dataAdapter.InsertCommand.Parameters.Add("@Pontos", SqlDbType.Decimal, 0, "Pontos");
                        dataAdapter.InsertCommand.Parameters.Add("@NomeSessão", SqlDbType.NVarChar, 100, "NomeSessão");
                        dataAdapter.InsertCommand.Parameters.Add("@NomeGP", SqlDbType.NVarChar, 100, "GrandPrix");
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

                    // CRITICAL: Verify that this session exists in the database
                    if (!string.IsNullOrEmpty(gpName))
                    {
                        using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                        {
                            conn.Open();
                            string checkSession = "SELECT COUNT(*) FROM Sessões WHERE NomeSessão = @SessionName AND NomeGP = @GPName";
                            SqlCommand cmd = new SqlCommand(checkSession, conn);
                            cmd.Parameters.AddWithValue("@SessionName", sessionName);
                            cmd.Parameters.AddWithValue("@GPName", gpName);
                            
                            int sessionCount = (int)cmd.ExecuteScalar();
                            if (sessionCount == 0)
                            {
                                ShowError($"Cannot add results!\n\nThe session '{sessionName}' does not exist for '{gpName}'.\n\nPlease create the session first in the Sessions view.");
                                return;
                            }
                        }
                    }

                    // Check if this is a race/sprint that requires qualification
                    if (sessionName == "Race" || sessionName == "Sprint Race")
                    {
                        string qualificationSession = sessionName == "Race" ? "Qualification" : "Sprint Qualification";
                        
                        // Check if qualification exists
                        using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                        {
                            conn.Open();
                            string checkQual = "SELECT COUNT(*) FROM Sessões WHERE NomeSessão = @QualSession";
                            if (!string.IsNullOrEmpty(gpName))
                            {
                                checkQual += " AND NomeGP = @GPName";
                            }
                            
                            SqlCommand cmd = new SqlCommand(checkQual, conn);
                            cmd.Parameters.AddWithValue("@QualSession", qualificationSession);
                            if (!string.IsNullOrEmpty(gpName))
                            {
                                cmd.Parameters.AddWithValue("@GPName", gpName);
                            }
                            
                            int qualCount = (int)cmd.ExecuteScalar();
                            if (qualCount == 0)
                            {
                                ShowWarning($"Cannot add {sessionName} results!\n\n{qualificationSession} session must exist first for this Grand Prix.\n\nPlease create {qualificationSession} session and add results before adding {sessionName} results.");
                                return;
                            }
                        }
                    }

                    // Show custom dialog for adding result
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
                            
                            // Add GrandPrix (NomeGP) - this is REQUIRED for the database
                            if (resultsTable.Columns.Contains("GrandPrix"))
                            {
                                if (!string.IsNullOrEmpty(gpName))
                                {
                                    newRow["GrandPrix"] = gpName;
                                }
                                else
                                {
                                    ShowError("Cannot add result: Grand Prix name is missing. Please open results from a specific GP session.");
                                    return;
                                }
                            }
                            
                            resultsTable.Rows.Add(newRow);

                            if (dgvResults != null && dgvResults.Rows.Count > 0)
                            {
                                dgvResults.CurrentCell = dgvResults.Rows[dgvResults.Rows.Count - 1].Cells[1];
                            }
                            
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
                    @"SELECT p.ID_Piloto, p.NumeroPermanente, p.Abreviação, m.Nome, e.Nome AS Team
                      FROM Piloto p
                      LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                      LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                      ORDER BY e.Nome, m.Nome", conn);
                
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
                string query = @"
                    SELECT PosiçãoFinal 
                    FROM Resultados r
                    INNER JOIN Sessões s ON r.NomeSessão = s.NomeSessão 
                        AND (r.NomeGP = s.NomeGP OR r.NomeGP IS NULL)
                    WHERE r.ID_Piloto = @DriverId 
                        AND s.NomeSessão = @QualSession 
                        AND s.NomeGP = @GPName";

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
