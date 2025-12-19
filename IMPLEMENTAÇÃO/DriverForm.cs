using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProjetoFBD
{
    public partial class DriverForm : Form
    {
        private DataGridView? dgvDrivers;
        private Panel? pnlStaffActions;
        private Chart? chartDriverPoints;
        private TextBox? txtSearch;
        private DataTable? driverDataComplete;
        
        private string userRole;
        private SqlDataAdapter? dataAdapter;
        private DataTable? driverTable;

        public DriverForm(string role)
        {
            InitializeComponent();
            
            this.userRole = role;
            
            this.Text = "Drivers Management";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadDriverData();
            LoadDriverPointsChart();
        }

        private void SetupLayout()
        {
            Label lblTitle = new Label
            {
                Text = "Drivers Management",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // Search bar
            Label lblSearch = new Label
            {
                Text = "Search Driver:",
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
                PlaceholderText = "Type driver name, number, team..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // DataGridView for listing drivers
            dgvDrivers = new DataGridView
            {
                Name = "dgvDrivers",
                Location = new Point(20, 90),
                Size = new Size(750, 460),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            
            // Centralizar 
            dgvDrivers.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDrivers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            this.Controls.Add(dgvDrivers);

            // Chart para pontos dos pilotos
            chartDriverPoints = new Chart
            {
                Name = "chartDriverPoints",
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
                    Title = "Drivers",
                    Interval = 1,
                    LabelStyle = { Angle = -45, Font = new Font("Arial", 8) }
                },
                AxisY = 
                { 
                    Title = "Points",
                    LabelStyle = { Font = new Font("Arial", 9) }
                }
            };
            chartDriverPoints.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                Name = "Points",
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(220, 20, 20),
                IsValueShownAsLabel = true,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            chartDriverPoints.Series.Add(series);

            Title chartTitle = new Title
            {
                Text = "Driver Career Points",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            chartDriverPoints.Titles.Add(chartTitle);

            this.Controls.Add(chartDriverPoints);

            pnlStaffActions = new Panel
            {
                Location = new Point(20, 570),
                Size = new Size(870, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            // Criar Botões
            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add New", new Point(145, 5));
            Button btnDelete = CreateActionButton("Delete Selected", new Point(290, 5));
            Button btnEdit = CreateActionButton("Edit", new Point(435, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(580, 5));
            Button btnViewResults = CreateActionButton("View Results", new Point(725, 5));

            btnViewResults.BackColor = Color.FromArgb(0, 153, 76);

            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnEdit.Click += btnEdit_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewResults.Click += btnViewResults_Click;

            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnEdit);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewResults);
            
            pnlStaffActions.Size = new Size(840, 50);

            // Role-Based Access Control
            if (this.userRole == "Staff")
            {
                dgvDrivers.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvDrivers.ReadOnly = true;
                btnSave.Visible = false;
                btnAdd.Visible = false;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
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

        private void LoadDriverData()
        {
            string connectionString = DbConfig.ConnectionString;

            // Use the view for driver details
            string query = "SELECT * FROM vw_DriverDetails ORDER BY NumeroPermanente ASC;";

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                
                driverTable = new DataTable();
                
                dataAdapter.Fill(driverTable);
                
                // Guardar cópia dos dados completos para filtro
                driverDataComplete = driverTable.Copy();
                
                if (dgvDrivers != null)
                {
                    dgvDrivers.DataSource = driverTable;

                    // Configurar colunas
                    if (dgvDrivers.Columns.Contains("ID_Piloto"))
                    {
                        dgvDrivers.Columns["ID_Piloto"]!.HeaderText = "Driver ID";
                        dgvDrivers.Columns["ID_Piloto"]!.ReadOnly = true;
                    }
                    
                    if (dgvDrivers.Columns.Contains("NumeroPermanente"))
                        dgvDrivers.Columns["NumeroPermanente"]!.HeaderText = "Permanent Number";
                    
                    if (dgvDrivers.Columns.Contains("Abreviação"))
                        dgvDrivers.Columns["Abreviação"]!.HeaderText = "Abbreviation";
                    
                    if (dgvDrivers.Columns.Contains("DriverName"))
                    {
                        dgvDrivers.Columns["DriverName"]!.HeaderText = "Driver Name";
                        dgvDrivers.Columns["DriverName"]!.ReadOnly = true;
                    }
                    
                    // Esconder ID_Equipa e mostrar TeamName
                    if (dgvDrivers.Columns.Contains("ID_Equipa"))
                    {
                        dgvDrivers.Columns["ID_Equipa"]!.Visible = false;
                    }
                    
                    if (dgvDrivers.Columns.Contains("TeamName"))
                    {
                        dgvDrivers.Columns["TeamName"]!.HeaderText = "Team";
                        dgvDrivers.Columns["TeamName"]!.ReadOnly = true;
                    }
                    
                    if (dgvDrivers.Columns.Contains("ID_Membro"))
                    {
                        dgvDrivers.Columns["ID_Membro"]!.HeaderText = "Member ID";
                        // Esconder ID_Membro para Guest
                        if (userRole != "Staff")
                        {
                            dgvDrivers.Columns["ID_Membro"]!.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Driver data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && driverTable != null && userRole == "Staff")
            {
                string connectionString = DbConfig.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        if (dgvDrivers != null)
                        {
                            dgvDrivers.EndEdit();
                        }

                        connection.Open();
                        
                        // Como temos uma coluna calculada (TeamName), precisamos criar comandos explícitos
                        dataAdapter.UpdateCommand = new SqlCommand(
                            @"UPDATE Piloto 
                              SET NumeroPermanente = @NumeroPermanente, 
                                  Abreviação = @Abreviação, 
                                  ID_Equipa = @ID_Equipa, 
                                  ID_Membro = @ID_Membro 
                              WHERE ID_Piloto = @ID_Piloto", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@NumeroPermanente", SqlDbType.Int, 0, "NumeroPermanente");
                        dataAdapter.UpdateCommand.Parameters.Add("@Abreviação", SqlDbType.NVarChar, 3, "Abreviação");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Membro", SqlDbType.Int, 0, "ID_Membro");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Piloto", SqlDbType.Int, 0, "ID_Piloto");
                        
                        dataAdapter.InsertCommand = new SqlCommand(
                            @"INSERT INTO Piloto (NumeroPermanente, Abreviação, ID_Equipa, ID_Membro) 
                              VALUES (@NumeroPermanente, @Abreviação, @ID_Equipa, @ID_Membro)", connection);
                        dataAdapter.InsertCommand.Parameters.Add("@NumeroPermanente", SqlDbType.Int, 0, "NumeroPermanente");
                        dataAdapter.InsertCommand.Parameters.Add("@Abreviação", SqlDbType.NVarChar, 3, "Abreviação");
                        dataAdapter.InsertCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        dataAdapter.InsertCommand.Parameters.Add("@ID_Membro", SqlDbType.Int, 0, "ID_Membro");
                        
                        dataAdapter.DeleteCommand = new SqlCommand(
                            @"DELETE FROM Piloto WHERE ID_Piloto = @ID_Piloto", connection);
                        dataAdapter.DeleteCommand.Parameters.Add("@ID_Piloto", SqlDbType.Int, 0, "ID_Piloto");

                        int rowsAffected = dataAdapter.Update(driverTable);
                        MessageBox.Show($"{rowsAffected} row(s) updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        driverTable.AcceptChanges();
                        LoadDriverData(); // Recarregar para atualizar os nomes das equipas
                        LoadDriverPointsChart(); // Atualizar gráfico
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
            if (driverTable != null && userRole == "Staff")
            {
                try
                {
                    DataRow newRow = driverTable.NewRow();
                    
                    // Get permanent number
                    using (var inputForm = new InputDialog("Add New Driver", "Enter permanent number:"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue) &&
                            int.TryParse(inputForm.InputValue, out int permNumber))
                        {
                            newRow["NumeroPermanente"] = permNumber;
                        }
                        else
                        {
                            MessageBox.Show("Permanent number is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Get abbreviation
                    using (var inputForm = new InputDialog("Add New Driver", "Enter abbreviation (3 letters):"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue))
                        {
                            string abbr = inputForm.InputValue.Trim().ToUpper();
                            if (abbr.Length != 3)
                            {
                                MessageBox.Show("Abbreviation must be exactly 3 characters.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            newRow["Abreviação"] = abbr;
                        }
                        else
                        {
                            MessageBox.Show("Abbreviation is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Get Team ID using a selection form
                    int selectedTeamId = 0;
                    using (SqlConnection connection = new SqlConnection(DbConfig.ConnectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("SELECT ID_Equipa, Nome FROM Equipa ORDER BY Nome", connection);
                        SqlDataReader reader = cmd.ExecuteReader();
                        
                        var teams = new System.Collections.Generic.List<(int Id, string Name)>();
                        while (reader.Read())
                        {
                            teams.Add((reader.GetInt32(0), reader.GetString(1)));
                        }
                        reader.Close();
                        
                        if (teams.Count == 0)
                        {
                            MessageBox.Show("No teams available. Please create a team first.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        // Show selection dialog
                        using (var teamSelector = new TeamSelectorDialog(teams))
                        {
                            if (teamSelector.ShowDialog() == DialogResult.OK)
                            {
                                selectedTeamId = teamSelector.SelectedTeamId;
                            }
                            else
                            {
                                MessageBox.Show("Team selection is required.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    
                    newRow["ID_Equipa"] = selectedTeamId;
                    
                    // Ask if user wants to create new member or select existing
                    DialogResult createNew = MessageBox.Show(
                        "Do you want to create a NEW team member for this driver?\n\n" +
                        "YES = Create new member with driver details\n" +
                        "NO = Select existing team member\n" +
                        "CANCEL = Skip (driver without member info)",
                        "Driver Member Info",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);
                    
                    if (createNew == DialogResult.Yes)
                    {
                        // Create new team member
                        string? driverName = null;
                        string? nationality = null;
                        DateTime? birthDate = null;
                        string? gender = null;
                        
                        // Get driver name
                        using (var inputForm = new InputDialog("Driver Details", "Enter driver name:"))
                        {
                            if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(inputForm.InputValue))
                            {
                                driverName = inputForm.InputValue.Trim();
                            }
                            else
                            {
                                MessageBox.Show("Driver name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        
                        // Get nationality
                        using (var inputForm = new InputDialog("Driver Details", "Enter nationality:"))
                        {
                            if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(inputForm.InputValue))
                            {
                                nationality = inputForm.InputValue.Trim();
                            }
                            else
                            {
                                MessageBox.Show("Nationality is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        
                        // Get birth date
                        using (var inputForm = new InputDialog("Driver Details", "Enter birth date (YYYY-MM-DD):"))
                        {
                            if (inputForm.ShowDialog() == DialogResult.OK && 
                                DateTime.TryParse(inputForm.InputValue, out DateTime parsedDate))
                            {
                                birthDate = parsedDate;
                            }
                            else
                            {
                                MessageBox.Show("Valid birth date is required (YYYY-MM-DD).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        
                        // Get gender
                        DialogResult genderResult = MessageBox.Show(
                            "Select driver gender:\n\nYES = Male (M)\nNO = Female (F)",
                            "Driver Gender",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        gender = genderResult == DialogResult.Yes ? "M" : "F";
                        
                        // Insert new member using stored procedure
                        using (SqlConnection connection = new SqlConnection(DbConfig.ConnectionString))
                        {
                            connection.Open();
                            SqlCommand insertCmd = new SqlCommand("sp_InsertTeamMember", connection);
                            insertCmd.CommandType = CommandType.StoredProcedure;

                            insertCmd.Parameters.AddWithValue("@Nome", driverName);
                            insertCmd.Parameters.AddWithValue("@Nacionalidade", nationality);
                            insertCmd.Parameters.AddWithValue("@DataNascimento", birthDate);
                            insertCmd.Parameters.AddWithValue("@Genero", gender);
                            insertCmd.Parameters.AddWithValue("@ID_Equipa", selectedTeamId);

                            SqlParameter outputIdParam = new SqlParameter("@ID_Membro", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            insertCmd.Parameters.Add(outputIdParam);

                            insertCmd.ExecuteNonQuery();
                            int newMemberId = (int)outputIdParam.Value;
                            newRow["ID_Membro"] = newMemberId;
                            newRow["DriverName"] = driverName;
                        }
                        
                        MessageBox.Show($"Team member '{driverName}' created successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (createNew == DialogResult.No)
                    {
                        // Select existing member
                        using (SqlConnection connection = new SqlConnection(DbConfig.ConnectionString))
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(
                                @"SELECT ID_Membro, Nome, Nacionalidade, Função 
                                  FROM Membros_da_Equipa 
                                  WHERE ID_Equipa = @TeamId
                                  ORDER BY Nome", 
                                connection);
                            cmd.Parameters.AddWithValue("@TeamId", selectedTeamId);
                            SqlDataReader reader = cmd.ExecuteReader();
                            
                            var members = new System.Collections.Generic.List<(int Id, string Display)>();
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string nat = reader.GetString(2);
                                string func = reader.GetString(3);
                                members.Add((id, $"{name} - {nat} ({func})"));
                            }
                            reader.Close();
                            
                            if (members.Count == 0)
                            {
                                MessageBox.Show("No team members found for selected team. Please create a new member.", 
                                    "No Members", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string selected = Microsoft.VisualBasic.Interaction.InputBox(
                                "Select member:\n\n" + string.Join("\n", members.Select((m, i) => $"{i + 1}. {m.Display}")),
                                "Select Team Member",
                                "1");
                            
                            if (int.TryParse(selected, out int index) && index > 0 && index <= members.Count)
                            {
                                newRow["ID_Membro"] = members[index - 1].Id;
                            }
                            else
                            {
                                MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    else
                    {
                        newRow["ID_Membro"] = DBNull.Value;
                        newRow["DriverName"] = DBNull.Value;
                    }

                    if (!newRow.Table.Columns.Contains("TeamName") || newRow["ID_Membro"] == DBNull.Value)
                    {
                        newRow["TeamName"] = DBNull.Value;
                    }
                    
                    driverTable.Rows.Add(newRow);
                    if (dgvDrivers != null && dgvDrivers.Rows.Count > 0)
                    {
                        dgvDrivers.CurrentCell = dgvDrivers.Rows[dgvDrivers.Rows.Count - 1].Cells[1];
                    }
                    
                    MessageBox.Show("Driver added. Click 'Save Changes' to commit to database.",
                        "Driver Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding driver: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff" && dgvDrivers != null && dgvDrivers.SelectedRows.Count > 0 && driverTable != null)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete the selected driver(s)? This action cannot be undone.",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvDrivers.SelectedRows)
                        {
                            if (row.Index >= 0 && row.Index < driverTable.Rows.Count)
                            {
                                driverTable.Rows[row.Index].Delete();
                            }
                        }
                        MessageBox.Show("Selected row(s) marked for deletion. Click 'Save Changes' to commit.",
                            "Deletion Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting driver: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvDrivers != null && dgvDrivers.SelectedRows.Count > 0)
            {
                int editableColumn = dgvDrivers.Columns["NumeroPermanente"]?.Index ?? 1;
                dgvDrivers.CurrentCell = dgvDrivers.SelectedRows[0].Cells[editableColumn];
                dgvDrivers.BeginEdit(true);
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            if (driverTable != null && driverTable.GetChanges() != null)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to discard them and refresh the data?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    driverTable.RejectChanges();
                    LoadDriverData();
                }
            }
            else
            {
                LoadDriverData();
            }
            
            LoadDriverPointsChart();
        }

        private void LoadDriverPointsChart()
        {
            if (chartDriverPoints == null) return;

            string connectionString = DbConfig.ConnectionString;
            
            // Use the view for driver points
            string query = "SELECT * FROM vw_DriverPoints;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            chartDriverPoints.Series["Points"].Points.Clear();

                            Color[] colors = new Color[]
                            {
                                Color.FromArgb(255, 215, 0),   
                                Color.FromArgb(192, 192, 192),  
                                Color.FromArgb(205, 127, 50),   
                                Color.FromArgb(220, 20, 60),    
                                Color.FromArgb(30, 144, 255),   
                                Color.FromArgb(50, 205, 50),    
                                Color.FromArgb(255, 140, 0),   
                                Color.FromArgb(138, 43, 226),   
                                Color.FromArgb(255, 20, 147),   
                                Color.FromArgb(0, 191, 255),    
                                Color.FromArgb(255, 69, 0),     
                                Color.FromArgb(34, 139, 34),    
                                Color.FromArgb(220, 20, 20),    
                                Color.FromArgb(128, 0, 128),    
                                Color.FromArgb(0, 128, 128)     
                            };
                            
                            int index = 0;
                            while (reader.Read())
                            {
                                string abbreviation = reader["Abreviação"].ToString() ?? "???";
                                string driverName = reader["DriverName"].ToString() ?? "Unknown";
                                string teamName = reader["TeamName"].ToString() ?? "No Team";
                                int totalPoints = Convert.ToInt32(reader["TotalPoints"]);

                                int pointIndex = chartDriverPoints.Series["Points"].Points.AddXY(abbreviation, totalPoints);
                                
                                chartDriverPoints.Series["Points"].Points[pointIndex].Color = colors[index % colors.Length];

                                chartDriverPoints.Series["Points"].Points[pointIndex].ToolTip = 
                                    $"#{index + 1} {driverName}\n{teamName}\n{totalPoints} points";
                                
                                // Label 
                                chartDriverPoints.Series["Points"].Points[pointIndex].Label = totalPoints.ToString();
                                chartDriverPoints.Series["Points"].Points[pointIndex].Font = new Font("Arial", 9, FontStyle.Bold);
                                
                                if (index < 3)
                                {
                                    chartDriverPoints.Series["Points"].Points[pointIndex].BorderWidth = 3;
                                    chartDriverPoints.Series["Points"].Points[pointIndex].BorderColor = Color.White;
                                }
                                
                                index++;
                            }
                        }
                    }
                }
                
                if (chartDriverPoints.ChartAreas.Count > 0)
                {
                    chartDriverPoints.ChartAreas[0].Area3DStyle.Enable3D = true;
                    chartDriverPoints.ChartAreas[0].Area3DStyle.Rotation = 10;
                    chartDriverPoints.ChartAreas[0].Area3DStyle.Inclination = 15;
                    chartDriverPoints.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;
                    chartDriverPoints.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                    chartDriverPoints.ChartAreas[0].BackColor = Color.FromArgb(245, 245, 245);
                    chartDriverPoints.ChartAreas[0].BackSecondaryColor = Color.White;
                    
                    chartDriverPoints.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(200, 200, 200);
                    chartDriverPoints.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(200, 200, 200);
                    chartDriverPoints.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                }

                if (chartDriverPoints.Legends.Count == 0)
                {
                    Legend legend = new Legend
                    {
                        Name = "Legend",
                        Docking = Docking.Bottom,
                        Alignment = StringAlignment.Center,
                        BackColor = Color.Transparent,
                        Font = new Font("Arial", 8)
                    };
                    chartDriverPoints.Legends.Add(legend);
                }
                
                chartDriverPoints.Click -= ChartDriverPoints_Click;
                chartDriverPoints.Click += ChartDriverPoints_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading driver points chart: {ex.Message}", "Chart Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void ChartDriverPoints_Click(object? sender, EventArgs e)
        {
            if (sender is Chart chart && e is MouseEventArgs mouseEvent)
            {
                HitTestResult result = chart.HitTest(mouseEvent.X, mouseEvent.Y);
                
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    DataPoint point = result.Series.Points[result.PointIndex];
                    string driver = point.AxisLabel;
                    double points = point.YValues[0];
                    
                    MessageBox.Show(
                        $"Driver: {driver}\nPoints: {points}\n\nClick 'View Results' to see detailed race results.",
                        "Driver Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
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

        // Team Selector Dialog
        public class TeamSelectorDialog : Form
        {
            public int SelectedTeamId { get; private set; }
            private ComboBox cmbTeams;

            public TeamSelectorDialog(System.Collections.Generic.List<(int Id, string Name)> teams)
            {
                this.Text = "Select Team";
                this.Size = new Size(400, 180);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                Label lblPrompt = new Label
                {
                    Text = "Select a team:",
                    Location = new Point(20, 20),
                    Size = new Size(350, 20),
                    Font = new Font("Arial", 10)
                };
                this.Controls.Add(lblPrompt);

                cmbTeams = new ComboBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(340, 25),
                    Font = new Font("Arial", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                
                foreach (var team in teams)
                {
                    cmbTeams.Items.Add(new TeamItem { Id = team.Id, Name = team.Name });
                }
                cmbTeams.DisplayMember = "Name";
                cmbTeams.ValueMember = "Id";
                
                if (cmbTeams.Items.Count > 0)
                    cmbTeams.SelectedIndex = 0;
                
                this.Controls.Add(cmbTeams);

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
                    if (cmbTeams.SelectedItem is TeamItem selected)
                    {
                        SelectedTeamId = selected.Id;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
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

            public class TeamItem
            {
                public int Id { get; set; }
                public string Name { get; set; } = "";
            }
        }

        private void btnViewResults_Click(object? sender, EventArgs e)
        {
            if (dgvDrivers != null && dgvDrivers.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedRow = dgvDrivers.SelectedRows[0];
                    
                    // Check if columns exist
                    if (!dgvDrivers.Columns.Contains("ID_Piloto") || !dgvDrivers.Columns.Contains("DriverName"))
                    {
                        MessageBox.Show("Required columns not found. Please refresh the data.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Get driver ID and name
                    if (selectedRow.Cells["ID_Piloto"].Value != null && 
                        selectedRow.Cells["DriverName"].Value != null)
                    {
                        int driverId = Convert.ToInt32(selectedRow.Cells["ID_Piloto"].Value);
                        string? driverName = selectedRow.Cells["DriverName"].Value?.ToString();

                        ResultsForm resultsForm = new ResultsForm(userRole, driverId, driverName ?? "Unknown");
                        NavigationHelper.NavigateTo(resultsForm, "RESULTS - " + driverName);
                    }
                    else
                    {
                        MessageBox.Show("Please select a valid driver.", "No Selection",
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
                MessageBox.Show("Please select a driver first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtSearch == null || driverDataComplete == null || dgvDrivers == null)
                return;

            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                // Mostrar todos os dados
                dgvDrivers.DataSource = driverDataComplete;
            }
            else
            {
                // Filtrar dados
                try
                {
                    DataView dv = new DataView(driverDataComplete);
                    string escapedSearch = searchText.Replace("'", "''");
                    
                    // Filtro que procura em DriverName, NumeroPermanente, Abreviação e TeamName
                    dv.RowFilter = $"DriverName LIKE '%{escapedSearch}%' OR " +
                                  $"CONVERT(NumeroPermanente, 'System.String') LIKE '%{escapedSearch}%' OR " +
                                  $"Abreviação LIKE '%{escapedSearch}%' OR " +
                                  $"TeamName LIKE '%{escapedSearch}%'";
                    
                    dgvDrivers.DataSource = dv.ToTable();
                }
                catch (Exception ex)
                {
                    // Se houver erro no filtro, mostrar todos os dados
                    Console.WriteLine($"Filter error: {ex.Message}");
                    dgvDrivers.DataSource = driverDataComplete;
                }
            }
        }
    }
}
