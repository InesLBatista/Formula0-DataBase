using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace ProjetoFBD
{
    public partial class SessionForm : BaseForm
    {
        private DataGridView? dgvSessions;
        private Panel? pnlStaffActions;
        
        private string gpName;
        private SqlDataAdapter? dataAdapter;
        private DataTable? sessionTable;

        public SessionForm(string role, string grandPrixName) : base(role)
        {
            InitializeComponent();
            
            this.gpName = grandPrixName;
            
            this.Text = $"Sessions - {gpName}";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadSessionData();
        }

        // -------------------------------------------------------------------------
        // UI SETUP
        // -------------------------------------------------------------------------

        private void SetupLayout()
        {
            Label lblTitle = new Label
            {
                Text = $"Sessions - {gpName}",
                Location = new Point(20, 20),
                Size = new Size(600, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            dgvSessions = new DataGridView
            {
                Name = "dgvSessions",
                Location = new Point(20, 70),
                Size = new Size(940, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = false
            };
            ConfigureDataGridView(dgvSessions);
            this.Controls.Add(dgvSessions);

            // Painel de ações
            pnlStaffActions = new Panel
            {
                Location = new Point(20, 440),
                Size = new Size(840, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add Session", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(280, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(400, 5));
            Button btnViewResults = CreateActionButton("View Results", new Point(520, 5), Color.FromArgb(0, 102, 204));
            Button btnAddPenalty = CreateActionButton("Add Penalty", new Point(660, 5), Color.FromArgb(255, 140, 0));
            
            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewResults.Click += btnViewResults_Click;
            btnAddPenalty.Click += btnAddPenalty_Click;

            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewResults);
            pnlStaffActions.Controls.Add(btnAddPenalty);
            
            pnlStaffActions.Size = new Size(920, 50);

            // Role-Based Access Control
            if (this.userRole == "Staff")
            {
                dgvSessions.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvSessions.ReadOnly = true;
                btnSave.Visible = false;
                btnAdd.Visible = false;
                btnDelete.Visible = false;
            }
        }


        private void LoadSessionData()
        {
            string connectionString = DbConfig.ConnectionString;
            
            string query = @"
                SELECT 
                    NomeSessão,
                    Estado,
                    CondiçõesPista,
                    NomeGP
                FROM Sessões
                WHERE NomeGP = @GPName
                ORDER BY NomeSessão ASC";

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                dataAdapter.SelectCommand.Parameters.AddWithValue("@GPName", gpName);
                
                sessionTable = new DataTable();
                
                dataAdapter.Fill(sessionTable);
                
                if (dgvSessions != null)
                {
                    dgvSessions.DataSource = sessionTable;

                    SetColumnHeader(dgvSessions, "NomeSessão", "Session Name");
                    SetColumnHeader(dgvSessions, "Estado", "Status");
                    SetColumnHeader(dgvSessions, "CondiçõesPista", "Track Conditions");
                    
                    MakeColumnReadOnly(dgvSessions, "NomeGP");
                    HideColumn(dgvSessions, "NomeGP");
                }
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(sqlEx, "loading session data");
            }
            catch (Exception ex)
            {
                ShowError($"Error loading session data: {ex.Message}");
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && sessionTable != null && userRole == "Staff")
            {
                string connectionString = DbConfig.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        if (dgvSessions != null)
                        {
                            dgvSessions.EndEdit();
                        }

                        connection.Open();
                        
                        // Criar comandos SQL explícitos para chave primária composta
                        dataAdapter.InsertCommand = new SqlCommand(
                            @"INSERT INTO Sessões (NomeSessão, Estado, CondiçõesPista, NomeGP) 
                              VALUES (@NomeSessão, @Estado, @CondiçõesPista, @NomeGP)", connection);
                        dataAdapter.InsertCommand.Parameters.Add("@NomeSessão", SqlDbType.NVarChar, 100, "NomeSessão");
                        dataAdapter.InsertCommand.Parameters.Add("@Estado", SqlDbType.NVarChar, 50, "Estado");
                        dataAdapter.InsertCommand.Parameters.Add("@CondiçõesPista", SqlDbType.NVarChar, 50, "CondiçõesPista");
                        dataAdapter.InsertCommand.Parameters.Add("@NomeGP", SqlDbType.NVarChar, 100, "NomeGP");
                        
                        dataAdapter.UpdateCommand = new SqlCommand(
                            @"UPDATE Sessões 
                              SET Estado = @Estado, CondiçõesPista = @CondiçõesPista 
                              WHERE NomeSessão = @NomeSessão AND NomeGP = @NomeGP", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@Estado", SqlDbType.NVarChar, 50, "Estado");
                        dataAdapter.UpdateCommand.Parameters.Add("@CondiçõesPista", SqlDbType.NVarChar, 50, "CondiçõesPista");
                        dataAdapter.UpdateCommand.Parameters.Add("@NomeSessão", SqlDbType.NVarChar, 100, "NomeSessão");
                        dataAdapter.UpdateCommand.Parameters.Add("@NomeGP", SqlDbType.NVarChar, 100, "NomeGP");
                        
                        dataAdapter.DeleteCommand = new SqlCommand(
                            @"DELETE FROM Sessões WHERE NomeSessão = @NomeSessão AND NomeGP = @NomeGP", connection);
                        dataAdapter.DeleteCommand.Parameters.Add("@NomeSessão", SqlDbType.NVarChar, 100, "NomeSessão");
                        dataAdapter.DeleteCommand.Parameters.Add("@NomeGP", SqlDbType.NVarChar, 100, "NomeGP");

                        int rowsAffected = dataAdapter.Update(sessionTable);
                        ShowSuccess($"{rowsAffected} row(s) updated successfully!");
                        
                        sessionTable.AcceptChanges();
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
            if (sessionTable != null && userRole == "Staff")
            {
                // Create custom dialog with dropdown for session types
                Form dialog = new Form
                {
                    Text = "Add New Session",
                    Size = new Size(450, 220),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                Label lblPrompt = new Label
                {
                    Text = $"Select session type for '{gpName}':",
                    Location = new Point(20, 20),
                    Size = new Size(400, 25),
                    Font = new Font("Arial", 10)
                };
                dialog.Controls.Add(lblPrompt);

                ComboBox cmbSessionType = new ComboBox
                {
                    Location = new Point(20, 55),
                    Size = new Size(400, 30),
                    Font = new Font("Arial", 10),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                
                // Add predefined session types
                cmbSessionType.Items.AddRange(new string[]
                {
                    "Free Practice 1",
                    "Free Practice 2",
                    "Free Practice 3",
                    "Sprint Qualification",
                    "Sprint Race",
                    "Qualification",
                    "Race"
                });
                cmbSessionType.SelectedIndex = 0; // Select first item by default
                dialog.Controls.Add(cmbSessionType);

                Button btnOk = new Button
                {
                    Text = "Add",
                    Location = new Point(250, 120),
                    Size = new Size(80, 35),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };
                btnOk.FlatAppearance.BorderSize = 0;
                dialog.Controls.Add(btnOk);

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(340, 120),
                    Size = new Size(80, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.Cancel
                };
                btnCancel.FlatAppearance.BorderSize = 0;
                dialog.Controls.Add(btnCancel);

                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog() == DialogResult.OK && cmbSessionType.SelectedItem != null)
                {
                    string sessionName = cmbSessionType.SelectedItem.ToString()!;

                    // Check if session already exists in THIS GP only
                    foreach (DataRow row in sessionTable.Rows)
                    {
                        if (row["NomeSessão"].ToString() == sessionName)
                        {
                            ShowWarning($"Session '{sessionName}' already exists for this Grand Prix!");
                            return;
                        }
                    }

                    try
                    {
                        // Adicionar nova linha
                        DataRow newRow = sessionTable.NewRow();
                        newRow["NomeSessão"] = sessionName;
                        newRow["Estado"] = "Scheduled"; // Valor padrão
                        newRow["CondiçõesPista"] = "Dry"; // Valor padrão
                        newRow["NomeGP"] = gpName;
                        sessionTable.Rows.Add(newRow);

                        // Focar na nova linha
                        if (dgvSessions != null)
                        {
                            dgvSessions.CurrentCell = dgvSessions.Rows[dgvSessions.Rows.Count - 1].Cells[0];
                        }
                        
                        ShowSuccess($"Session '{sessionName}' added. Click 'Save Changes' to commit.");
                    }
                    catch (SqlException sqlEx)
                    {
                        HandleSqlException(sqlEx, "adding session");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error adding session: {ex.Message}");
                    }
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole != "Staff" || dgvSessions == null || sessionTable == null)
                return;

            if (!IsRowSelected(dgvSessions, "session"))
                return;

            if (ShowConfirmation($"Are you sure you want to delete {dgvSessions.SelectedRows.Count} session(s)?\n\nThis will also delete all results for these sessions.\nThis action cannot be undone.", "Confirm Deletion"))
            {
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvSessions.SelectedRows)
                        {
                            if (row.Index >= 0 && row.Index < sessionTable.Rows.Count)
                            {
                                sessionTable.Rows[row.Index].Delete();
                            }
                        }
                        ShowSuccess("Selected row(s) marked for deletion. Click 'Save Changes' to commit.");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error deleting session: {ex.Message}");
                    }
                }
            }
        }

        private void btnViewResults_Click(object? sender, EventArgs e)
        {
            if (dgvSessions == null || !IsRowSelected(dgvSessions, "session"))
                return;

            var selectedRow = dgvSessions.SelectedRows[0];
            string? sessionName = selectedRow.Cells["NomeSessão"].Value?.ToString();

            if (!string.IsNullOrEmpty(sessionName))
            {
                // Pass both GP name and session name to filter correctly
                ResultsForm resultsForm = new ResultsForm(userRole, gpName, sessionName);
                NavigationHelper.NavigateTo(resultsForm, "RESULTS - " + sessionName);
            }
            else
            {
                ShowWarning("Please select a valid session.");
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            if (sessionTable != null && sessionTable.GetChanges() != null)
            {
                if (ShowConfirmation("You have unsaved changes. Do you want to discard them and refresh?", "Unsaved Changes"))
                {
                    sessionTable.RejectChanges();
                    LoadSessionData();
                }
            }
            else
            {
                LoadSessionData();
            }
        }

        // InputDialog class for Add functionality
        public class InputDialog : Form
        {
            private TextBox textBox;
            [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
            public string InputValue { get; set; } = "";

            public InputDialog(string title, string prompt, string initialValue = "")
            {
                this.Text = title;
                this.Size = new Size(500, 250);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                Label lblPrompt = new Label
                {
                    Text = prompt,
                    Location = new Point(20, 20),
                    Size = new Size(450, 80),
                    Font = new Font("Arial", 10),
                    AutoSize = false
                };
                this.Controls.Add(lblPrompt);

                textBox = new TextBox
                {
                    Location = new Point(20, 110),
                    Size = new Size(450, 30),
                    Font = new Font("Arial", 10),
                    Text = initialValue
                };
                this.Controls.Add(textBox);

                Button btnOK = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(300, 160),
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
                    Location = new Point(390, 160),
                    Size = new Size(80, 30),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.FlatAppearance.BorderSize = 0;
                this.Controls.Add(btnCancel);

                this.AcceptButton = btnOK;
                this.CancelButton = btnCancel;
                
                // Focus and select text for easy editing
                this.Shown += (s, e) => { textBox.Focus(); textBox.SelectAll(); };
            }
        }

        private void btnAddPenalty_Click(object? sender, EventArgs e)
        {
            if (dgvSessions == null || !IsRowSelected(dgvSessions, "session"))
                return;

            var selectedRow = dgvSessions.SelectedRows[0];
            string? sessionName = selectedRow.Cells["NomeSessão"].Value?.ToString();

            if (string.IsNullOrEmpty(sessionName))
            {
                ShowWarning("Please select a valid session.");
                return;
            }

            // Open Add Penalty dialog
            using (var dialog = new AddPenaltyDialog(gpName, sessionName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                        {
                            conn.Open();
                            string insertQuery = @"
                                INSERT INTO Penalizações (TipoPenalização, Motivo, NomeSessão, NomeGP, ID_Piloto, ID_Resultados)
                                VALUES (@Tipo, @Motivo, @Session, @GP, @Piloto, @Resultado)";
                            
                            SqlCommand cmd = new SqlCommand(insertQuery, conn);
                            cmd.Parameters.AddWithValue("@Tipo", dialog.PenaltyType);
                            cmd.Parameters.AddWithValue("@Motivo", dialog.Reason);
                            cmd.Parameters.AddWithValue("@Session", sessionName);
                            cmd.Parameters.AddWithValue("@GP", gpName);
                            cmd.Parameters.AddWithValue("@Piloto", dialog.DriverId.HasValue ? (object)dialog.DriverId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@Resultado", dialog.ResultId.HasValue ? (object)dialog.ResultId.Value : DBNull.Value);
                            
                            cmd.ExecuteNonQuery();
                            ShowSuccess($"Penalty added successfully!\n\n{dialog.PenaltyType} - {dialog.Reason}");
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        HandleSqlException(sqlEx, "adding penalty");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error adding penalty: {ex.Message}");
                    }
                }
            }
        }
    }

    // Penalty dialog class
    public class AddPenaltyDialog : Form
    {
        private ComboBox? cmbPenaltyType;
        private TextBox? txtReason;
        private ComboBox? cmbDriver;
        private ComboBox? cmbResult;
        
        public string PenaltyType { get; private set; } = "";
        public string Reason { get; private set; } = "";
        public int? DriverId { get; private set; }
        public int? ResultId { get; private set; }
        
        private string gpName;
        private string sessionName;

        public AddPenaltyDialog(string gpName, string sessionName)
        {
            this.gpName = gpName;
            this.sessionName = sessionName;
            
            InitializeDialog();
            LoadDrivers();
            LoadResults();
        }

        private void InitializeDialog()
        {
            this.Text = $"Add Penalty - {sessionName}";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label lblTitle = new Label
            {
                Text = $"Add Penalty - {gpName} - {sessionName}",
                Location = new Point(20, 20),
                Size = new Size(450, 25),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // Penalty Type
            Label lblType = new Label
            {
                Text = "Penalty Type:",
                Location = new Point(20, 60),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblType);

            cmbPenaltyType = new ComboBox
            {
                Location = new Point(150, 57),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPenaltyType.Items.AddRange(new string[]
            {
                "Drive-through penalty",
                "Grid penalties",
                "Time penalties",
                "Disqualification",
                "Speeding in the pit lane",
                "Reprimands",
                "Suspension",
                "Corner cutting",
                "Penalty points",
                "Unsafe pit release",
                "Warnings",
                "Official warnings and reprimands"
            });
            this.Controls.Add(cmbPenaltyType);

            // Reason
            Label lblReason = new Label
            {
                Text = "Reason:",
                Location = new Point(20, 100),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblReason);

            txtReason = new TextBox
            {
                Location = new Point(150, 97),
                Size = new Size(310, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtReason);

            // Driver
            Label lblDriver = new Label
            {
                Text = "Driver (optional):",
                Location = new Point(20, 170),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblDriver);

            cmbDriver = new ComboBox
            {
                Location = new Point(150, 167),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbDriver);

            // Result
            Label lblResult = new Label
            {
                Text = "Result (optional):",
                Location = new Point(20, 210),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblResult);

            cmbResult = new ComboBox
            {
                Location = new Point(150, 207),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbResult);

            // Buttons
            Button btnOK = new Button
            {
                Text = "Add Penalty",
                Location = new Point(250, 310),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(360, 310),
                Size = new Size(100, 35),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadDrivers()
        {
            if (cmbDriver == null) return;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT p.ID_Piloto, p.NumeroPermanente, p.Abreviação, m.Nome, e.Nome AS Team
                        FROM Piloto p
                        LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                        ORDER BY m.Nome";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    cmbDriver.Items.Add("-- None --");
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string num = reader.IsDBNull(1) ? "?" : reader.GetInt32(1).ToString();
                        string code = reader.IsDBNull(2) ? "???" : reader.GetString(2);
                        string name = reader.IsDBNull(3) ? "Unknown" : reader.GetString(3);
                        string team = reader.IsDBNull(4) ? "No Team" : reader.GetString(4);
                        
                        cmbDriver.Items.Add(new DriverItem 
                        { 
                            ID = id, 
                            Number = num, 
                            Code = code, 
                            Name = name, 
                            Team = team 
                        });
                    }
                    
                    cmbDriver.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading drivers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadResults()
        {
            if (cmbResult == null) return;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT r.ID_Resultado, p.NumeroPermanente, p.Abreviação, r.PosiçãoFinal, r.Status
                        FROM Resultados r
                        INNER JOIN Piloto p ON r.ID_Piloto = p.ID_Piloto
                        WHERE r.NomeSessão = @Session AND r.NomeGP = @GP
                        ORDER BY r.PosiçãoFinal";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Session", sessionName);
                    cmd.Parameters.AddWithValue("@GP", gpName);
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    cmbResult.Items.Add("-- None --");
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string num = reader.IsDBNull(1) ? "?" : reader.GetInt32(1).ToString();
                        string code = reader.IsDBNull(2) ? "???" : reader.GetString(2);
                        int pos = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        string status = reader.IsDBNull(4) ? "?" : reader.GetString(4);
                        
                        cmbResult.Items.Add(new ResultItem 
                        { 
                            ID = id, 
                            Display = $"P{pos} - #{num} {code} ({status})" 
                        });
                    }
                    
                    cmbResult.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            // Validation
            if (cmbPenaltyType == null || cmbPenaltyType.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a penalty type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtReason == null || string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Please enter a reason for the penalty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get values
            PenaltyType = cmbPenaltyType.SelectedItem?.ToString() ?? "";
            Reason = txtReason.Text.Trim();
            
            if (cmbDriver != null && cmbDriver.SelectedIndex > 0 && cmbDriver.SelectedItem is DriverItem driver)
            {
                DriverId = driver.ID;
            }
            
            if (cmbResult != null && cmbResult.SelectedIndex > 0 && cmbResult.SelectedItem is ResultItem result)
            {
                ResultId = result.ID;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private class DriverItem
        {
            public int ID { get; set; }
            public string Number { get; set; } = "";
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public string Team { get; set; } = "";

            public override string ToString()
            {
                return $"#{Number} {Code} - {Name} ({Team})";
            }
        }

        private class ResultItem
        {
            public int ID { get; set; }
            public string Display { get; set; } = "";

            public override string ToString()
            {
                return Display;
            }
        }
    }
}
