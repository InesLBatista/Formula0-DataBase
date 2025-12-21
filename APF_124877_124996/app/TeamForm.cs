using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProjetoFBD
{
    public partial class TeamForm : Form
    {
        private DataGridView? dgvTeams;
        private Panel? pnlStaffActions;
        private Chart? chartTeamPoints;
        private TextBox? txtSearch;
        private DataTable? teamDataComplete;
        private string userRole;
        private SqlDataAdapter? dataAdapter;
        private DataTable? teamTable;

        public TeamForm(string role)
        {
            InitializeComponent();
            
            this.userRole = role;
            
            this.Text = "Teams Management";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadTeamData();
            LoadTeamPointsChart();
        }

        // -------------------------------------------------------------------------
        // UI SETUP
        // -------------------------------------------------------------------------

        private void SetupLayout()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "Teams Management",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // Search bar
            Label lblSearch = new Label
            {
                Text = "Search Team:",
                Location = new Point(20, 58),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(125, 55),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Type team name or base..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // DataGridView for listing teams
            dgvTeams = new DataGridView
            {
                Name = "dgvTeams",
                Location = new Point(20, 90),
                Size = new Size(750, 460),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            
            // Centralizar conteudo das celulas
            dgvTeams.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTeams.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            this.Controls.Add(dgvTeams);

            // Chart para pontos das equipas
            chartTeamPoints = new Chart
            {
                Name = "chartTeamPoints",
                Location = new Point(790, 90),
                Size = new Size(580, 460),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.White
            };

            // Configurar Chart Area
            ChartArea chartArea = new ChartArea
            {
                Name = "MainArea",
                BackColor = Color.WhiteSmoke,
                AxisX = 
                { 
                    Title = "Teams",
                    Interval = 1,
                    LabelStyle = { Angle = -45, Font = new Font("Arial", 8) }
                },
                AxisY = 
                { 
                    Title = "Points",
                    LabelStyle = { Font = new Font("Arial", 9) }
                }
            };
            chartTeamPoints.ChartAreas.Add(chartArea);

            // Configurar Series
            Series series = new Series
            {
                Name = "Points",
                ChartType = SeriesChartType.Bar,
                Color = Color.FromArgb(220, 20, 20),
                IsValueShownAsLabel = true,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            chartTeamPoints.Series.Add(series);

            // Título do gráfico
            Title chartTitle = new Title
            {
                Text = "Team Career Points",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            chartTeamPoints.Titles.Add(chartTitle);

            this.Controls.Add(chartTeamPoints);

            // Painel de ações
            pnlStaffActions = new Panel
            {
                Location = new Point(20, 570),
                Size = new Size(840, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            // Create Buttons - equidistantes
            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add Team", new Point(145, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(290, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(435, 5));
            Button btnViewResults = CreateActionButton("View Results", new Point(580, 5));
            Button btnViewDetails = CreateActionButton("View Details", new Point(725, 5));
            
            // Style special buttons
            btnViewDetails.BackColor = Color.FromArgb(0, 102, 204);
            btnViewResults.BackColor = Color.FromArgb(0, 153, 76);

            // Ligar Eventos
            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewResults.Click += btnViewResults_Click;
            btnViewDetails.Click += btnViewDetails_Click;

            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewResults);
            pnlStaffActions.Controls.Add(btnViewDetails);
            
            pnlStaffActions.Size = new Size(870, 50);

            // Role-Based Access Control
            if (this.userRole == "Staff")
            {
                dgvTeams.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvTeams.ReadOnly = true;
                btnSave.Visible = false;
                btnAdd.Visible = false;
                btnDelete.Visible = false;
            }
        }

        private Button CreateActionButton(string text, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            return btn;
        }


        // -------------------------------------------------------------------------
        // DATA ACCESS METHODS (CRUD)
        // -------------------------------------------------------------------------

        private void LoadTeamData()
        {
            string connectionString = DbConfig.ConnectionString;
            try
            {
                string query = "SELECT * FROM vw_Teams_List_Grid ORDER BY Nome ASC";
                dataAdapter = new SqlDataAdapter(query, connectionString);
                teamTable = new DataTable();
                dataAdapter.Fill(teamTable);
                teamDataComplete = teamTable.Copy();
                if (dgvTeams != null)
                {
                    dgvTeams.DataSource = teamTable;
                    try
                    {
                        ApplyEnglishHeaders(dgvTeams);
                        if (dgvTeams.Columns.Contains("ID_Equipa"))
                        {
                            var col = dgvTeams.Columns["ID_Equipa"];
                            if (col != null)
                            {
                                col.HeaderText = "Team ID";
                                col.ReadOnly = true;
                                col.Width = 80;
                            }
                        }
                        if (dgvTeams.Columns.Contains("Nome"))
                        {
                            var col = dgvTeams.Columns["Nome"];
                            if (col != null)
                            {
                                col.HeaderText = "Name";
                                col.Width = 200;
                            }
                        }
                        if (dgvTeams.Columns.Contains("Nacionalidade"))
                        {
                            var col = dgvTeams.Columns["Nacionalidade"];
                            if (col != null)
                            {
                                col.HeaderText = "Nationality";
                                col.Width = 150;
                            }
                        }
                        if (dgvTeams.Columns.Contains("AnoEstreia"))
                        {
                            var col = dgvTeams.Columns["AnoEstreia"];
                            if (col != null)
                            {
                                col.HeaderText = "Debut Year";
                                col.Width = 100;
                            }
                        }
                        foreach (DataGridViewColumn col in dgvTeams.Columns)
                        {
                            if (col.Name == "ID_Equipa" || col.Name == "Nome" || col.Name == "Nacionalidade" || col.Name == "AnoEstreia")
                                continue;
                            if (col.HeaderText == "ID_Equipa" || col.HeaderText == "Nome" || col.HeaderText == "Nacionalidade" || col.HeaderText == "AnoEstreia")
                                col.Visible = false;
                        }
                    }
                    catch (Exception colEx)
                    {
                        Console.WriteLine($"Column configuration error: {colEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Team data: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ApplyEnglishHeaders(DataGridView grid)
        {
            var mappings = new (string key, string header)[]
            {
                ("ID_Equipa", "Team ID"),
                ("Nome", "Name"),
                ("Nacionalidade", "Nationality"),
                ("AnoEstreia", "Debut Year"),
                ("ChefeEquipa", "Team Principal"),
                ("ChefeTécnico", "Technical Chief"),
                ("ModeloChassis", "Chassis Model"),
                ("Power_Unit", "Power Unit"),
                ("PilotosReserva", "Reserve Drivers")
            };

            foreach (var (key, header) in mappings)
            {
                if (grid.Columns.Contains(key))
                {
                    var col = grid.Columns[key];
                    if (col != null)
                    {
                        col.HeaderText = header;
                    }
                }
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && teamTable != null && userRole == "Staff")
            {
                string connectionString = DbConfig.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        // Update
                        dataAdapter.UpdateCommand = new SqlCommand("sp_UpdateTeamGrid", connection);
                        dataAdapter.UpdateCommand.CommandType = CommandType.StoredProcedure;
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        dataAdapter.UpdateCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 100, "Nome");
                        dataAdapter.UpdateCommand.Parameters.Add("@Nacionalidade", SqlDbType.NVarChar, 100, "Nacionalidade");
                        dataAdapter.UpdateCommand.Parameters.Add("@AnoEstreia", SqlDbType.Int, 0, "AnoEstreia");
                        // Insert
                        dataAdapter.InsertCommand = new SqlCommand("sp_InsertTeamGrid", connection);
                        dataAdapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                        dataAdapter.InsertCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 100, "Nome");
                        dataAdapter.InsertCommand.Parameters.Add("@Nacionalidade", SqlDbType.NVarChar, 100, "Nacionalidade");
                        dataAdapter.InsertCommand.Parameters.Add("@AnoEstreia", SqlDbType.Int, 0, "AnoEstreia");
                        // Delete
                        dataAdapter.DeleteCommand = new SqlCommand("sp_DeleteTeamGrid", connection);
                        dataAdapter.DeleteCommand.CommandType = CommandType.StoredProcedure;
                        dataAdapter.DeleteCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        int rowsAffected = dataAdapter.Update(teamTable);
                        MessageBox.Show($"{rowsAffected} row(s) updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        teamTable.AcceptChanges();
                        LoadTeamPointsChart();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Database error: {sqlEx.Message}\n\nError Number: {sqlEx.Number}", "Database Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff" && teamTable != null)
            {
                try
                {
                    DataRow newRow = teamTable.NewRow();
                    
                    // Nome (obrigatório)
                    using (var inputForm = new InputDialog("Add Team", "Team name:"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue))
                        {
                            newRow["Nome"] = inputForm.InputValue.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Team name is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Nacionalidade (obrigatório)
                    using (var inputForm = new InputDialog("Add Team", "Nationality:"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue))
                        {
                            newRow["Nacionalidade"] = inputForm.InputValue.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Nationality is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Ano Estreia (obrigatório)
                    using (var inputForm = new InputDialog("Add Team", "Debut year:"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue) &&
                            int.TryParse(inputForm.InputValue, out int year))
                        {
                            newRow["AnoEstreia"] = year;
                        }
                        else
                        {
                            MessageBox.Show("Valid debut year is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    teamTable.Rows.Add(newRow);

                    // Focar na nova linha
                    if (dgvTeams != null && dgvTeams.Rows.Count > 0)
                    {
                        dgvTeams.CurrentCell = dgvTeams.Rows[dgvTeams.Rows.Count - 1].Cells[1];
                    }
                    
                    MessageBox.Show("Team added. Click 'Save Changes' to save to database.\n\n" +
                        "Use 'View Details' to fill in remaining information (Base, Team Principal, etc.).",
                        "Team Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding team: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
       
        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff" && dgvTeams != null && dgvTeams.SelectedRows.Count > 0 && teamTable != null)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete the selected team(s)? This action cannot be undone.",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvTeams.SelectedRows)
                        {
                            if (row.Index >= 0 && row.Index < teamTable.Rows.Count)
                            {
                                teamTable.Rows[row.Index].Delete();
                            }
                        }
                        MessageBox.Show("Selected row(s) marked for deletion. Click 'Save Changes' to commit.",
                            "Deletion Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting team: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            if (teamTable != null && teamTable.GetChanges() != null)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to discard them and refresh the data?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
        
                if (result == DialogResult.Yes)
                {
                    teamTable.RejectChanges();
                    LoadTeamData();
                }
            }
            else
            {
                LoadTeamData();
            }
            
            LoadTeamPointsChart();
        }

        private void LoadTeamPointsChart()
        {
            if (chartTeamPoints == null) return;

            string connectionString = DbConfig.ConnectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_GetTeamCareerPoints", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            chartTeamPoints.Series["Points"].Points.Clear();
                            var teamColors = new System.Collections.Generic.Dictionary<string, Color>
                            {
                                { "Alpine", Color.FromArgb(0, 144, 255) },
                                { "Aston Martin", Color.FromArgb(0, 111, 98) },
                                { "Scuderia Ferrari", Color.FromArgb(220, 0, 0) },
                                { "Red Bull Racing", Color.FromArgb(30, 65, 174) },
                                { "Mercedes", Color.FromArgb(0, 210, 190) },
                                { "McLaren", Color.FromArgb(255, 135, 0) },
                                { "Williams", Color.FromArgb(0, 82, 147) },
                                { "Racing Bulls", Color.FromArgb(70, 155, 255) },
                                { "Haas", Color.FromArgb(182, 186, 189) },
                                { "Kick Sauber", Color.FromArgb(82, 226, 82) }
                            };
                            int index = 0;
                            while (reader.Read())
                            {
                                string teamName = reader["TeamName"].ToString() ?? "Unknown";
                                int totalPoints = Convert.ToInt32(reader["TotalPoints"]);
                                int pointIndex = chartTeamPoints.Series["Points"].Points.AddXY(teamName, totalPoints);
                                Color teamColor;
                                if (teamColors.TryGetValue(teamName, out Color specificColor))
                                {
                                    teamColor = specificColor;
                                }
                                else
                                {
                                    teamColor = Color.FromArgb(220, 20, 20);
                                }
                                chartTeamPoints.Series["Points"].Points[pointIndex].Color = teamColor;
                                chartTeamPoints.Series["Points"].Points[pointIndex].ToolTip = $"#{index + 1} {teamName}\n{totalPoints} points\nClick for details";
                                chartTeamPoints.Series["Points"].Points[pointIndex].Label = totalPoints.ToString();
                                chartTeamPoints.Series["Points"].Points[pointIndex].Font = new Font("Arial", 9, FontStyle.Bold);
                                chartTeamPoints.Series["Points"].Points[pointIndex].LabelForeColor = Color.White;
                                if (index == 0)
                                {
                                    chartTeamPoints.Series["Points"].Points[pointIndex].BorderWidth = 4;
                                    chartTeamPoints.Series["Points"].Points[pointIndex].BorderColor = Color.Gold;
                                }
                                else if (index < 3)
                                {
                                    chartTeamPoints.Series["Points"].Points[pointIndex].BorderWidth = 3;
                                    chartTeamPoints.Series["Points"].Points[pointIndex].BorderColor = Color.Silver;
                                }
                                index++;
                            }
                        }
                    }
                }
                if (chartTeamPoints.ChartAreas.Count > 0)
                {
                    chartTeamPoints.ChartAreas[0].Area3DStyle.Enable3D = true;
                    chartTeamPoints.ChartAreas[0].Area3DStyle.Rotation = -5;
                    chartTeamPoints.ChartAreas[0].Area3DStyle.Inclination = 10;
                    chartTeamPoints.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;
                    chartTeamPoints.ChartAreas[0].BackGradientStyle = GradientStyle.LeftRight;
                    chartTeamPoints.ChartAreas[0].BackColor = Color.FromArgb(245, 245, 245);
                    chartTeamPoints.ChartAreas[0].BackSecondaryColor = Color.White;
                    chartTeamPoints.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(200, 200, 200);
                    chartTeamPoints.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(200, 200, 200);
                    chartTeamPoints.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                    chartTeamPoints.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.FromArgb(80, 80, 80);
                    chartTeamPoints.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.FromArgb(80, 80, 80);
                }
                if (chartTeamPoints.Legends.Count == 0)
                {
                    Legend legend = new Legend
                    {
                        Name = "Legend",
                        Docking = Docking.Bottom,
                        Alignment = StringAlignment.Center,
                        BackColor = Color.Transparent,
                        Font = new Font("Arial", 8, FontStyle.Italic)
                    };
                    chartTeamPoints.Legends.Add(legend);
                }
                chartTeamPoints.Click -= ChartTeamPoints_Click;
                chartTeamPoints.Click += ChartTeamPoints_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team points chart: {ex.Message}", "Chart Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void ChartTeamPoints_Click(object? sender, EventArgs e)
        {
            if (sender is Chart chart && e is MouseEventArgs mouseEvent)
            {
                HitTestResult result = chart.HitTest(mouseEvent.X, mouseEvent.Y);
                
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    DataPoint point = result.Series.Points[result.PointIndex];
                    string team = point.AxisLabel;
                    double points = point.YValues[0];
                    
                    DialogResult dialogResult = MessageBox.Show(
                        $"Team: {team}\nCareer Points: {points}\n\nWould you like to view team details?",
                        "Team Career Points",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
                    
                    if (dialogResult == DialogResult.Yes)
                    {
                        // Procurar o ID da equipa e abrir detalhes
                        if (dgvTeams != null && dgvTeams.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in dgvTeams.Rows)
                            {
                                if (row.Cells["Nome"].Value?.ToString() == team)
                                {
                                    dgvTeams.ClearSelection();
                                    row.Selected = true;
                                    btnViewDetails_Click(null, EventArgs.Empty);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnViewDetails_Click(object? sender, EventArgs e)
        {
            if (dgvTeams == null || dgvTeams.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a team first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = dgvTeams.SelectedRows[0];
            int teamId = selectedRow.Cells["ID_Equipa"].Value != DBNull.Value
                ? Convert.ToInt32(selectedRow.Cells["ID_Equipa"].Value)
                : 0;

            if (teamId == 0)
            {
                MessageBox.Show("Invalid team selection.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Abrir formulário de detalhes
            TeamDetailsForm detailsForm = new TeamDetailsForm(teamId, this.userRole);
            NavigationHelper.NavigateTo(detailsForm, "TEAM DETAILS");
        }

        // InputDialog class for Add functionality
        public class InputDialog : Form
        {
            private TextBox textBox;
            public string InputValue { get; private set; } = "";

            public InputDialog(string title, string prompt)
            {
                this.Text = title;
                this.Size = new Size(400, 180);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                Label lblPrompt = new Label
                {
                    Text = prompt,
                    Location = new Point(20, 20),
                    Size = new Size(350, 20),
                    Font = new Font("Arial", 10)
                };
                this.Controls.Add(lblPrompt);

                textBox = new TextBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(340, 25),
                    Font = new Font("Arial", 10)
                };
                this.Controls.Add(textBox);

                Button btnOK = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(200, 90),
                    Size = new Size(80, 30),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOK.FlatAppearance.BorderSize = 0;
                btnOK.Click += (s, e) =>
                {
                    InputValue = textBox.Text;
                    this.Close();
                };
                this.Controls.Add(btnOK);

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(290, 90),
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

        private void btnViewResults_Click(object? sender, EventArgs e)
        {
            if (dgvTeams != null && dgvTeams.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedRow = dgvTeams.SelectedRows[0];
                    
                    // Check if the column exists
                    if (!dgvTeams.Columns.Contains("Nome"))
                    {
                        MessageBox.Show("Column 'Nome' not found. Please refresh the data.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    string? teamName = selectedRow.Cells["Nome"].Value?.ToString();

                    if (!string.IsNullOrEmpty(teamName))
                    {
                        ResultsForm resultsForm = new ResultsForm(userRole, teamName, true);
                        NavigationHelper.NavigateTo(resultsForm, "RESULTS - " + teamName);
                    }
                    else
                    {
                        MessageBox.Show("Please select a valid team.", "No Selection",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading results: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a team first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtSearch == null || teamDataComplete == null || dgvTeams == null)
                return;

            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                // Mostrar todos os dados
                dgvTeams.DataSource = teamDataComplete;
            }
            else
            {
                // Filtrar dados
                try
                {
                    DataView dv = new DataView(teamDataComplete);
                    string escapedSearch = searchText.Replace("'", "''");
                    
                    // Filtro que procura em Nome e Nacionalidade
                    dv.RowFilter = $"Nome LIKE '%{escapedSearch}%' OR Nacionalidade LIKE '%{escapedSearch}%' OR CONVERT(AnoEstreia, 'System.String') LIKE '%{escapedSearch}%'";
                    
                    dgvTeams.DataSource = dv.ToTable();
                }
                catch (Exception ex)
                {
                    // Se houver erro no filtro, mostrar todos os dados
                    Console.WriteLine($"Filter error: {ex.Message}");
                    dgvTeams.DataSource = teamDataComplete;
                }
            }
        }
    }
}
