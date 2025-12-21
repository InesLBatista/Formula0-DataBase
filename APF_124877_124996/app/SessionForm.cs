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
            this.Size = new Size(1400, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadSessionData();
        }

        // UI SETUP
        
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
                Size = new Size(1340, 350),
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
            Button btnAdd = CreateActionButton("Add Session", new Point(0, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(0, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(0, 5));
            Button btnViewResults = CreateActionButton("View Results", new Point(0, 5), Color.FromArgb(0, 102, 204));
            Button btnAddPenalty = CreateActionButton("Add Penalty", new Point(0, 5), Color.FromArgb(255, 140, 0));
            Button btnViewPenalties = CreateActionButton("View Penalties", new Point(0, 5), Color.FromArgb(178, 34, 34));
            
            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewResults.Click += btnViewResults_Click;
            btnAddPenalty.Click += btnAddPenalty_Click;
            btnViewPenalties.Click += btnViewPenalties_Click;

            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewResults);
            pnlStaffActions.Controls.Add(btnAddPenalty);
            pnlStaffActions.Controls.Add(btnViewPenalties);
            
            pnlStaffActions.Size = new Size(1200, 50);

            // Distribuir botões de forma equidistante
            DistributeButtons(pnlStaffActions);
            pnlStaffActions.Resize += (s, e) => DistributeButtons(pnlStaffActions);

            // Role-Based Access Control
            if (this.userRole == "Staff")
            {
                dgvSessions.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                // Guest pode ver mas não pode editar
                dgvSessions.ReadOnly = true;
                pnlStaffActions.Visible = true;
                // Esconder botões de edição
                btnSave.Visible = false;
                btnAdd.Visible = false;
                btnDelete.Visible = false;
                btnAddPenalty.Visible = false;
                // Guest pode ver resultados e penalties mas não pode adicionar
            }
        }

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
                btn.Anchor = AnchorStyles.Top;
                x += btn.Width + spacing;
            }
        }

        private void LoadSessionData()
        {
            string connectionString = DbConfig.ConnectionString;
            string query = "SELECT NomeSessão, Estado, CondiçõesPista, NomeGP FROM vw_Sessions_ByGP WHERE NomeGP = @GPName ORDER BY NomeSessão ASC";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GPName", gpName);
                    dataAdapter = new SqlDataAdapter(command);
                    sessionTable = new DataTable();
                    dataAdapter.Fill(sessionTable);
                }

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
                if (cmbSessionType.Items.Count > 0)
                    cmbSessionType.SelectedIndex = 0;
                dialog.Controls.Add(cmbSessionType);

                Button btnOk = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(230, 110),
                    Size = new Size(90, 30),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOk.FlatAppearance.BorderSize = 0;
                dialog.Controls.Add(btnOk);

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(330, 110),
                    Size = new Size(90, 30),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
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

        private void btnViewPitstops_Click(object? sender, EventArgs e)
        {
            if (dgvSessions == null || !IsRowSelected(dgvSessions, "session"))
                return;

            var selectedRow = dgvSessions.SelectedRows[0];
            string? sessionName = selectedRow.Cells["NomeSessão"].Value?.ToString();

            if (!string.IsNullOrEmpty(sessionName))
            {
                PitstopViewerDialog pitstopDialog = new PitstopViewerDialog(sessionName, gpName, this.userRole);
                pitstopDialog.ShowDialog();
            }
            else
            {
                ShowWarning("Please select a valid session.");
            }
        }

        private void btnViewPenalties_Click(object? sender, EventArgs e)
        {
            if (dgvSessions == null || !IsRowSelected(dgvSessions, "session"))
                return;

            var selectedRow = dgvSessions.SelectedRows[0];
            string? sessionName = selectedRow.Cells["NomeSessão"].Value?.ToString();

            if (!string.IsNullOrEmpty(sessionName))
            {
                PenaltyViewerDialog penaltyDialog = new PenaltyViewerDialog(sessionName, gpName);
                penaltyDialog.ShowDialog();
            }
            else
            {
                ShowWarning("Please select a valid session.");
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

    // ============================================================================
    // PITSTOP VIEWER DIALOG
    // ============================================================================
    public class PitstopViewerDialog : Form
    {
        private DataGridView? dgvPitstops;
        private ComboBox? cmbDriver;
        private ComboBox? cmbTeam;
        private Button? btnClearFilters;
        private DataTable? pitstopTable;
        private string sessionName;
        private string gpName;
        private string userRole;

        public PitstopViewerDialog(string sessionName, string gpName, string userRole)
        {
            this.sessionName = sessionName;
            this.gpName = gpName;
            this.userRole = userRole;

            this.Text = $"Pitstops - {sessionName}";
            this.Size = new Size(1200, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            LoadDriversAndTeams();
            LoadPitstops();
        }

        private void SetupUI()
        {
            Label lblTitle = new Label
            {
                Text = $"Pitstops - {sessionName}",
                Location = new Point(20, 20),
                Size = new Size(600, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // Filter Panel
            Panel pnlFilters = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1140, 50),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlFilters);

            Label lblDriver = new Label
            {
                Text = "Driver:",
                Location = new Point(10, 15),
                Size = new Size(60, 20)
            };
            pnlFilters.Controls.Add(lblDriver);

            cmbDriver = new ComboBox
            {
                Location = new Point(70, 12),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDriver.SelectedIndexChanged += Filter_Changed;
            pnlFilters.Controls.Add(cmbDriver);

            Label lblTeam = new Label
            {
                Text = "Team:",
                Location = new Point(390, 15),
                Size = new Size(50, 20)
            };
            pnlFilters.Controls.Add(lblTeam);

            cmbTeam = new ComboBox
            {
                Location = new Point(445, 12),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTeam.SelectedIndexChanged += Filter_Changed;
            pnlFilters.Controls.Add(cmbTeam);

            btnClearFilters = new Button
            {
                Text = "Clear Filters",
                Location = new Point(720, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearFilters.FlatAppearance.BorderSize = 0;
            btnClearFilters.Click += BtnClearFilters_Click;
            pnlFilters.Controls.Add(btnClearFilters);

            // DataGridView
            dgvPitstops = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(1140, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            this.Controls.Add(dgvPitstops);

            Button btnAddPitstop = new Button
            {
                Text = "Add Pitstop",
                Location = new Point(900, 530),
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = (this.userRole == "Staff") // Apenas visível para Staff
            };
            btnAddPitstop.FlatAppearance.BorderSize = 0;
            btnAddPitstop.Click += BtnAddPitstop_Click;
            this.Controls.Add(btnAddPitstop);

            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(1040, 530),
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnClose);
        }

        private void LoadDriversAndTeams()
        {
            if (cmbDriver == null || cmbTeam == null) return;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Load Drivers
                    cmbDriver.Items.Add("-- All Drivers --");
                    string driverQuery = @"
                        SELECT DISTINCT m.Nome, p.Abreviação
                        FROM Pitstop ps
                        INNER JOIN Piloto p ON ps.ID_Piloto = p.ID_Piloto
                        LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        WHERE ps.NomeSessão = @SessionName AND ps.NomeGP = @GPName
                        ORDER BY m.Nome";
                    
                    SqlCommand cmdDriver = new SqlCommand(driverQuery, conn);
                    cmdDriver.Parameters.AddWithValue("@SessionName", sessionName);
                    cmdDriver.Parameters.AddWithValue("@GPName", gpName);
                    SqlDataReader readerDriver = cmdDriver.ExecuteReader();
                    while (readerDriver.Read())
                    {
                        string nome = readerDriver.IsDBNull(0) ? "Unknown" : readerDriver.GetString(0);
                        string code = readerDriver.IsDBNull(1) ? "???" : readerDriver.GetString(1);
                        cmbDriver.Items.Add($"{code} - {nome}");
                    }
                    readerDriver.Close();
                    cmbDriver.SelectedIndex = 0;

                    // Load Teams
                    cmbTeam.Items.Add("-- All Teams --");
                    string teamQuery = @"
                        SELECT DISTINCT e.Nome
                        FROM Pitstop ps
                        INNER JOIN Piloto p ON ps.ID_Piloto = p.ID_Piloto
                        INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                        WHERE ps.NomeSessão = @SessionName AND ps.NomeGP = @GPName
                        ORDER BY e.Nome";
                    
                    SqlCommand cmdTeam = new SqlCommand(teamQuery, conn);
                    cmdTeam.Parameters.AddWithValue("@SessionName", sessionName);
                    cmdTeam.Parameters.AddWithValue("@GPName", gpName);
                    SqlDataReader readerTeam = cmdTeam.ExecuteReader();
                    while (readerTeam.Read())
                    {
                        cmbTeam.Items.Add(readerTeam["Nome"].ToString() ?? "");
                    }
                    readerTeam.Close();
                    cmbTeam.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading filters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPitstops(string? driverFilter = null, string? teamFilter = null)
        {
            if (dgvPitstops == null) return;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            ps.ID_Pitstop,
                            ps.NumeroVolta,
                            ps.DuraçãoParagem,
                            ps.DuraçãoPitlane,
                            p.Abreviação AS DriverCode,
                            m.Nome AS DriverName,
                            e.Nome AS TeamName,
                            r.PosiçãoFinal AS Position
                        FROM Pitstop ps
                        INNER JOIN Piloto p ON ps.ID_Piloto = p.ID_Piloto
                        LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                        LEFT JOIN Resultados r ON r.ID_Piloto = ps.ID_Piloto AND r.NomeSessão = ps.NomeSessão AND r.NomeGP = ps.NomeGP
                        WHERE ps.NomeSessão = @SessionName AND ps.NomeGP = @GPName";

                    if (!string.IsNullOrEmpty(driverFilter))
                    {
                        query += " AND p.Abreviação = @DriverCode";
                    }

                    if (!string.IsNullOrEmpty(teamFilter))
                    {
                        query += " AND e.Nome = @TeamName";
                    }

                    query += " ORDER BY ps.NumeroVolta ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SessionName", sessionName);
                    cmd.Parameters.AddWithValue("@GPName", gpName);

                    if (!string.IsNullOrEmpty(driverFilter))
                    {
                        cmd.Parameters.AddWithValue("@DriverCode", driverFilter);
                    }

                    if (!string.IsNullOrEmpty(teamFilter))
                    {
                        cmd.Parameters.AddWithValue("@TeamName", teamFilter);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    pitstopTable = new DataTable();
                    adapter.Fill(pitstopTable);
                    
                    dgvPitstops.DataSource = pitstopTable;

                    // Configure columns
                    dgvPitstops.Refresh();
                    Application.DoEvents();

                    if (dgvPitstops.Columns != null && dgvPitstops.Columns.Count > 0)
                    {
                        // Centralizar todas as colunas
                        foreach (DataGridViewColumn col in dgvPitstops.Columns)
                        {
                            if (col != null)
                            {
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                        }

                        // Esconder ID
                        if (dgvPitstops.Columns.Contains("ID_Pitstop") && dgvPitstops.Columns["ID_Pitstop"] != null)
                        {
                            dgvPitstops.Columns["ID_Pitstop"]!.Visible = false;
                        }
                        
                        if (dgvPitstops.Columns.Contains("NumeroVolta") && dgvPitstops.Columns["NumeroVolta"] != null)
                            dgvPitstops.Columns["NumeroVolta"]!.HeaderText = "Lap";
                        if (dgvPitstops.Columns.Contains("DuraçãoParagem") && dgvPitstops.Columns["DuraçãoParagem"] != null)
                            dgvPitstops.Columns["DuraçãoParagem"]!.HeaderText = "Stop Duration";
                        if (dgvPitstops.Columns.Contains("DuraçãoPitlane") && dgvPitstops.Columns["DuraçãoPitlane"] != null)
                            dgvPitstops.Columns["DuraçãoPitlane"]!.HeaderText = "Pitlane Duration";
                        if (dgvPitstops.Columns.Contains("DriverCode") && dgvPitstops.Columns["DriverCode"] != null)
                            dgvPitstops.Columns["DriverCode"]!.HeaderText = "Driver Code";
                        if (dgvPitstops.Columns.Contains("DriverName") && dgvPitstops.Columns["DriverName"] != null)
                            dgvPitstops.Columns["DriverName"]!.HeaderText = "Driver";
                        if (dgvPitstops.Columns.Contains("TeamName") && dgvPitstops.Columns["TeamName"] != null)
                            dgvPitstops.Columns["TeamName"]!.HeaderText = "Team";
                        if (dgvPitstops.Columns.Contains("Position") && dgvPitstops.Columns["Position"] != null)
                            dgvPitstops.Columns["Position"]!.HeaderText = "Final Position";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading pitstops: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Filter_Changed(object? sender, EventArgs e)
        {
            string? driverFilter = null;
            string? teamFilter = null;

            if (cmbDriver != null && cmbDriver.SelectedIndex > 0 && cmbDriver.SelectedItem != null)
            {
                string? selected = cmbDriver.SelectedItem.ToString();
                if (selected != null && selected.Contains(" - "))
                {
                    driverFilter = selected.Split(new[] { " - " }, StringSplitOptions.None)[0];
                }
            }

            if (cmbTeam != null && cmbTeam.SelectedIndex > 0 && cmbTeam.SelectedItem != null)
            {
                teamFilter = cmbTeam.SelectedItem.ToString();
            }

            LoadPitstops(driverFilter, teamFilter);
        }

        private void BtnClearFilters_Click(object? sender, EventArgs e)
        {
            if (cmbDriver != null)
                cmbDriver.SelectedIndex = 0;
            if (cmbTeam != null)
                cmbTeam.SelectedIndex = 0;
        }

        private void BtnAddPitstop_Click(object? sender, EventArgs e)
        {
            using (var dialog = new AddPitstopDialog(sessionName, gpName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadPitstops(); // Refresh the list
                }
            }
        }
    }

    // ============================================================================
    // ADD PITSTOP DIALOG
    // ============================================================================
    public class AddPitstopDialog : Form
    {
        private ComboBox? cmbTeam;
        private ComboBox? cmbDriver;
        private NumericUpDown? nudLap;
        private MaskedTextBox? txtStopDuration;
        private MaskedTextBox? txtPitlaneDuration;
        
        private string sessionName;
        private string gpName;
        private bool isUpdatingFilters = false;

        public AddPitstopDialog(string sessionName, string gpName)
        {
            this.sessionName = sessionName;
            this.gpName = gpName;
            
            InitializeUI();
            LoadTeams();
            LoadDrivers();
        }

        private void InitializeUI()
        {
            this.Text = $"Add Pitstop - {sessionName}";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitle = new Label
            {
                Text = $"Add Pitstop - {sessionName}",
                Location = new Point(20, 20),
                Size = new Size(400, 25),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // Team
            Label lblTeam = new Label
            {
                Text = "Team:",
                Location = new Point(20, 60),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblTeam);

            cmbTeam = new ComboBox
            {
                Location = new Point(150, 57),
                Size = new Size(260, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTeam.SelectedIndexChanged += CmbTeam_SelectedIndexChanged;
            this.Controls.Add(cmbTeam);

            // Driver
            Label lblDriver = new Label
            {
                Text = "Driver:",
                Location = new Point(20, 100),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblDriver);

            cmbDriver = new ComboBox
            {
                Location = new Point(150, 97),
                Size = new Size(260, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDriver.SelectedIndexChanged += CmbDriver_SelectedIndexChanged;
            this.Controls.Add(cmbDriver);

            // Lap Number
            Label lblLap = new Label
            {
                Text = "Lap Number:",
                Location = new Point(20, 140),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblLap);

            nudLap = new NumericUpDown
            {
                Location = new Point(150, 137),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 999,
                Value = 1
            };
            this.Controls.Add(nudLap);

            // Stop Duration
            Label lblStopDuration = new Label
            {
                Text = "Stop Duration:",
                Location = new Point(20, 180),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblStopDuration);

            txtStopDuration = new MaskedTextBox
            {
                Location = new Point(150, 177),
                Size = new Size(100, 25),
                Mask = "00:00:00.000",
                Text = "00:00:00.000"
            };
            this.Controls.Add(txtStopDuration);

            Label lblStopFormat = new Label
            {
                Text = "(HH:MM:SS.mmm)",
                Location = new Point(260, 180),
                Size = new Size(150, 20),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblStopFormat);

            // Pitlane Duration
            Label lblPitlaneDuration = new Label
            {
                Text = "Pitlane Duration:",
                Location = new Point(20, 220),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblPitlaneDuration);

            txtPitlaneDuration = new MaskedTextBox
            {
                Location = new Point(150, 217),
                Size = new Size(100, 25),
                Mask = "00:00:00.000",
                Text = "00:00:00.000"
            };
            this.Controls.Add(txtPitlaneDuration);

            Label lblPitlaneFormat = new Label
            {
                Text = "(HH:MM:SS.mmm)",
                Location = new Point(260, 220),
                Size = new Size(150, 20),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblPitlaneFormat);

            // Buttons - Equidistantes
            int buttonWidth = 100;
            int buttonSpacing = (this.ClientSize.Width - (2 * buttonWidth)) / 3;
            
            Button btnOK = new Button
            {
                Text = "Add Pitstop",
                Location = new Point(buttonSpacing, 300),
                Size = new Size(buttonWidth, 35),
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
                Location = new Point(2 * buttonSpacing + buttonWidth, 300),
                Size = new Size(buttonWidth, 35),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadTeams()
        {
            if (cmbTeam == null) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ID_Equipa, Nome FROM Equipa ORDER BY Nome";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        
                        cmbTeam.Items.Add(new TeamItem { ID = id, Name = name });
                    }
                    
                    if (cmbTeam.Items.Count > 0)
                        cmbTeam.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDrivers(int? teamId = null)
        {
            if (cmbDriver == null) return;

            try
            {
                isUpdatingFilters = true;
                cmbDriver.Items.Clear();
                
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT p.ID_Piloto, p.NumeroPermanente, p.Abreviação, m.Nome, e.ID_Equipa, e.Nome AS Team
                        FROM Piloto p
                        LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa";
                    
                    if (teamId.HasValue)
                    {
                        query += " WHERE p.ID_Equipa = @TeamId";
                    }
                    
                    query += " ORDER BY m.Nome";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (teamId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@TeamId", teamId.Value);
                    }
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string num = reader.IsDBNull(1) ? "?" : reader.GetInt32(1).ToString();
                        string code = reader.IsDBNull(2) ? "???" : reader.GetString(2);
                        string name = reader.IsDBNull(3) ? "Unknown" : reader.GetString(3);
                        int teamIdValue = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                        string team = reader.IsDBNull(5) ? "No Team" : reader.GetString(5);
                        
                        cmbDriver.Items.Add(new DriverItem 
                        { 
                            ID = id, 
                            Number = num, 
                            Code = code, 
                            Name = name,
                            TeamID = teamIdValue,
                            Team = team 
                        });
                    }
                    
                    if (cmbDriver.Items.Count > 0)
                        cmbDriver.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading drivers: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isUpdatingFilters = false;
            }
        }

        private void CmbTeam_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isUpdatingFilters || cmbTeam == null || cmbTeam.SelectedItem == null) return;

            var selectedTeam = (TeamItem)cmbTeam.SelectedItem;
            LoadDrivers(selectedTeam.ID);
        }

        private void CmbDriver_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isUpdatingFilters || cmbDriver == null || cmbDriver.SelectedItem == null || cmbTeam == null) return;

            var selectedDriver = (DriverItem)cmbDriver.SelectedItem;
            
            // Atualizar a equipa para a equipa do piloto selecionado
            isUpdatingFilters = true;
            for (int i = 0; i < cmbTeam.Items.Count; i++)
            {
                var team = (TeamItem)cmbTeam.Items[i]!;
                if (team.ID == selectedDriver.TeamID)
                {
                    cmbTeam.SelectedIndex = i;
                    break;
                }
            }
            isUpdatingFilters = false;
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            if (cmbDriver == null || cmbDriver.SelectedItem == null)
            {
                MessageBox.Show("Please select a driver.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nudLap == null || nudLap.Value < 1)
            {
                MessageBox.Show("Please enter a valid lap number.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var driver = (DriverItem)cmbDriver.SelectedItem;
                int lap = (int)nudLap.Value;
                TimeSpan stopDuration = TimeSpan.Parse(txtStopDuration?.Text ?? "00:00:00");
                TimeSpan pitlaneDuration = TimeSpan.Parse(txtPitlaneDuration?.Text ?? "00:00:00");

                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string insertQuery = @"
                        INSERT INTO Pitstop (NumeroVolta, DuraçãoParagem, DuraçãoPitlane, NomeSessão, NomeGP, ID_Piloto)
                        VALUES (@Lap, @StopDuration, @PitlaneDuration, @Session, @GP, @Driver)";
                    
                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Lap", lap);
                    cmd.Parameters.AddWithValue("@StopDuration", stopDuration);
                    cmd.Parameters.AddWithValue("@PitlaneDuration", pitlaneDuration);
                    cmd.Parameters.AddWithValue("@Session", sessionName);
                    cmd.Parameters.AddWithValue("@GP", gpName);
                    cmd.Parameters.AddWithValue("@Driver", driver.ID);
                    
                    cmd.ExecuteNonQuery();
                    
                    MessageBox.Show($"Pitstop added successfully!\n\nDriver: {driver.Name}\nLap: {lap}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid time format. Please use HH:MM:SS.mmm format.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}\n\nError Number: {sqlEx.Number}", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding pitstop: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class TeamItem
        {
            public int ID { get; set; }
            public string Name { get; set; } = "";

            public override string ToString()
            {
                return Name;
            }
        }

        private class DriverItem
        {
            public int ID { get; set; }
            public string Number { get; set; } = "";
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public int TeamID { get; set; }
            public string Team { get; set; } = "";

            public override string ToString()
            {
                return $"#{Number} {Code} - {Name}";
            }
        }
    }

    // ============================================================================
    // PENALTY VIEWER DIALOG
    // ============================================================================
    public class PenaltyViewerDialog : Form
    {
        private DataGridView? dgvPenalties;
        private ComboBox? cmbDriver;
        private ComboBox? cmbTeam;
        private Button? btnClearFilters;
        private DataTable? penaltyTable;
        private string sessionName;
        private string gpName;

        public PenaltyViewerDialog(string sessionName, string gpName)
        {
            this.sessionName = sessionName;
            this.gpName = gpName;

            this.Text = $"Penalties - {sessionName}";
            this.Size = new Size(1200, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            LoadDriversAndTeams();
            LoadPenalties();
        }

        private void SetupUI()
        {
            Label lblTitle = new Label
            {
                Text = $"Penalties - {sessionName}",
                Location = new Point(20, 20),
                Size = new Size(600, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // Filter Panel
            Panel pnlFilters = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1140, 50),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlFilters);

            Label lblDriver = new Label
            {
                Text = "Driver:",
                Location = new Point(10, 15),
                Size = new Size(60, 20)
            };
            pnlFilters.Controls.Add(lblDriver);

            cmbDriver = new ComboBox
            {
                Location = new Point(70, 12),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDriver.SelectedIndexChanged += Filter_Changed;
            pnlFilters.Controls.Add(cmbDriver);

            Label lblTeam = new Label
            {
                Text = "Team:",
                Location = new Point(390, 15),
                Size = new Size(50, 20)
            };
            pnlFilters.Controls.Add(lblTeam);

            cmbTeam = new ComboBox
            {
                Location = new Point(445, 12),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTeam.SelectedIndexChanged += Filter_Changed;
            pnlFilters.Controls.Add(cmbTeam);

            btnClearFilters = new Button
            {
                Text = "Clear Filters",
                Location = new Point(720, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(220, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearFilters.FlatAppearance.BorderSize = 0;
            btnClearFilters.Click += BtnClearFilters_Click;
            pnlFilters.Controls.Add(btnClearFilters);

            // DataGridView
            dgvPenalties = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(1140, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            this.Controls.Add(dgvPenalties);

            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(1040, 530),
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnClose);
        }

        private void LoadDriversAndTeams()
        {
            if (cmbDriver == null || cmbTeam == null) return;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    // Load Drivers
                    cmbDriver.Items.Add("-- All Drivers --");
                    string driverQuery = @"
                        SELECT DISTINCT p.Abreviação, m.Nome
                        FROM Penalizações pen
                        INNER JOIN Piloto p ON pen.ID_Piloto = p.ID_Piloto
                        LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        WHERE pen.NomeSessão = @SessionName AND pen.NomeGP = @GPName
                        ORDER BY p.Abreviação";
                    
                    using (SqlCommand cmdDriver = new SqlCommand(driverQuery, conn))
                    {
                        cmdDriver.Parameters.AddWithValue("@SessionName", sessionName);
                        cmdDriver.Parameters.AddWithValue("@GPName", gpName);
                        using (SqlDataReader readerDriver = cmdDriver.ExecuteReader())
                        {
                            while (readerDriver.Read())
                            {
                                string code = readerDriver.IsDBNull(0) ? "???" : readerDriver.GetString(0);
                                string nome = readerDriver.IsDBNull(1) ? "Unknown" : readerDriver.GetString(1);
                                cmbDriver.Items.Add($"{code} - {nome}");
                            }
                        }
                    }
                    cmbDriver.SelectedIndex = 0;

                    // Load Teams
                    cmbTeam.Items.Add("-- All Teams --");
                    string teamQuery = @"
                        SELECT DISTINCT e.Nome
                        FROM Penalizações pen
                        INNER JOIN Piloto p ON pen.ID_Piloto = p.ID_Piloto
                        INNER JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                        WHERE pen.NomeSessão = @SessionName AND pen.NomeGP = @GPName
                        ORDER BY e.Nome";
                    
                    using (SqlCommand cmdTeam = new SqlCommand(teamQuery, conn))
                    {
                        cmdTeam.Parameters.AddWithValue("@SessionName", sessionName);
                        cmdTeam.Parameters.AddWithValue("@GPName", gpName);
                        using (SqlDataReader readerTeam = cmdTeam.ExecuteReader())
                        {
                            while (readerTeam.Read())
                            {
                                string teamName = readerTeam.IsDBNull(0) ? "" : readerTeam.GetString(0);
                                if (!string.IsNullOrEmpty(teamName))
                                    cmbTeam.Items.Add(teamName);
                            }
                        }
                    }
                    cmbTeam.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading filters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPenalties(string? driverFilter = null, string? teamFilter = null)
        {
            if (dgvPenalties == null)
            {
                throw new InvalidOperationException("dgvPenalties not initialized before loading penalties.");
            }
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            pen.ID_Penalização,
                            pen.TipoPenalização,
                            pen.Motivo,
                            p.Abreviação AS DriverCode,
                            m.Nome AS DriverName,
                            e.Nome AS TeamName
                        FROM Penalizações pen
                        LEFT JOIN Piloto p ON pen.ID_Piloto = p.ID_Piloto
                        LEFT JOIN Membros_da_Equipa m ON p.ID_Membro = m.ID_Membro
                        LEFT JOIN Equipa e ON p.ID_Equipa = e.ID_Equipa
                        WHERE pen.NomeSessão = @SessionName AND pen.NomeGP = @GPName";

                    if (!string.IsNullOrEmpty(driverFilter))
                    {
                        query += " AND p.Abreviação = @DriverCode";
                    }

                    if (!string.IsNullOrEmpty(teamFilter))
                    {
                        query += " AND e.Nome = @TeamName";
                    }

                    query += " ORDER BY pen.ID_Penalização ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SessionName", sessionName);
                    cmd.Parameters.AddWithValue("@GPName", gpName);

                    if (!string.IsNullOrEmpty(driverFilter))
                    {
                        cmd.Parameters.AddWithValue("@DriverCode", driverFilter);
                    }

                    if (!string.IsNullOrEmpty(teamFilter))
                    {
                        cmd.Parameters.AddWithValue("@TeamName", teamFilter);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    penaltyTable = new DataTable();
                    adapter.Fill(penaltyTable);
                    
                    dgvPenalties.DataSource = penaltyTable;

                    // Configure columns safely
                    dgvPenalties.Refresh();
                    Application.DoEvents();

                    if (dgvPenalties.Columns != null)
                    {
                        foreach (DataGridViewColumn col in dgvPenalties.Columns)
                        {
                            if (col == null) continue;
                            // Apply simple headers; avoid width changes to prevent band thickness null refs
                            if (string.Equals(col.Name, "ID_Penalização", StringComparison.OrdinalIgnoreCase))
                                col.HeaderText = "ID";
                            else if (string.Equals(col.Name, "TipoPenalização", StringComparison.OrdinalIgnoreCase))
                                col.HeaderText = "Penalty Type";
                            else if (string.Equals(col.Name, "Motivo", StringComparison.OrdinalIgnoreCase))
                                col.HeaderText = "Reason";
                            else if (string.Equals(col.Name, "DriverCode", StringComparison.OrdinalIgnoreCase))
                                col.HeaderText = "Driver Code";
                            else if (string.Equals(col.Name, "DriverName", StringComparison.OrdinalIgnoreCase))
                                col.HeaderText = "Driver";
                            else if (string.Equals(col.Name, "TeamName", StringComparison.OrdinalIgnoreCase))
                                col.HeaderText = "Team";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading penalties: {ex}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Filter_Changed(object? sender, EventArgs e)
        {
            string? driverFilter = null;
            string? teamFilter = null;

            if (cmbDriver != null && cmbDriver.SelectedIndex > 0 && cmbDriver.SelectedItem != null)
            {
                string? selected = cmbDriver.SelectedItem.ToString();
                if (selected != null && selected.Contains(" - "))
                {
                    driverFilter = selected.Split(new[] { " - " }, StringSplitOptions.None)[0];
                }
            }

            if (cmbTeam != null && cmbTeam.SelectedIndex > 0 && cmbTeam.SelectedItem != null)
            {
                teamFilter = cmbTeam.SelectedItem.ToString();
            }

            LoadPenalties(driverFilter, teamFilter);
        }

        private void BtnClearFilters_Click(object? sender, EventArgs e)
        {
            if (cmbDriver != null)
                cmbDriver.SelectedIndex = 0;
            if (cmbTeam != null)
                cmbTeam.SelectedIndex = 0;
        }
    }
}
