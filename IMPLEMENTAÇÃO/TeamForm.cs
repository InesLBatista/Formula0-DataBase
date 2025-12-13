using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace ProjetoFBD
{
    public partial class TeamForm : Form
    {
        private DataGridView? dgvTeams;
        private Panel? pnlStaffActions;
        
        private string userRole;
        private SqlDataAdapter? dataAdapter;
        private DataTable? teamTable;

        public TeamForm(string role)
        {
            InitializeComponent();
            
            this.userRole = role;
            
            this.Text = "Teams Management";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadTeamData();
        }

        // -------------------------------------------------------------------------
        // UI SETUP
        // -------------------------------------------------------------------------

        private void SetupLayout()
        {
            // Título
            Label lblTitle = new Label
            {
                Text = "Teams Management",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // DataGridView para listar equipas
            dgvTeams = new DataGridView
            {
                Name = "dgvTeams",
                Location = new Point(20, 70),
                Size = new Size(1140, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            this.Controls.Add(dgvTeams);

            // Painel de ações
            pnlStaffActions = new Panel
            {
                Location = new Point(20, 570),
                Size = new Size(840, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            // Create Buttons
            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add Team", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(260, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(360, 5));
            Button btnViewResults = CreateActionButton("View Results", new Point(480, 5));
            Button btnViewDetails = CreateActionButton("View Details", new Point(620, 5));
            
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
            
            pnlStaffActions.Size = new Size(760, 50);

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
            
            string query = @"
                SELECT 
                    ID_Equipa,
                    Nome,
                    Nacionalidade,
                    AnoEstreia
                FROM Equipa
                ORDER BY Nome ASC";

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                
                teamTable = new DataTable();
                
                dataAdapter.Fill(teamTable);
                
                if (dgvTeams != null)
                {
                    dgvTeams.DataSource = teamTable;
                    
                    // Configurar colunas principais - verificar se existem antes de acessar
                    try
                    {
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
                    }
                    catch (Exception colEx)
                    {
                        // Ignorar erros de configuração de colunas - não é crítico
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
                        
                        // Comandos SQL explícitos
                        dataAdapter.UpdateCommand = new SqlCommand(
                            @"UPDATE Equipa 
                              SET Nome = @Nome, 
                                  Nacionalidade = @Nacionalidade, 
                                  AnoEstreia = @AnoEstreia 
                              WHERE ID_Equipa = @ID_Equipa", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 100, "Nome");
                        dataAdapter.UpdateCommand.Parameters.Add("@Nacionalidade", SqlDbType.NVarChar, 50, "Nacionalidade");
                        dataAdapter.UpdateCommand.Parameters.Add("@AnoEstreia", SqlDbType.Int, 0, "AnoEstreia");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        
                        dataAdapter.InsertCommand = new SqlCommand(
                            @"INSERT INTO Equipa (Nome, Nacionalidade, Base, ChefeEquipa, ChefeTécnico, AnoEstreia, ModeloChassis, Power_Unit) 
                              VALUES (@Nome, 
                                      @Nacionalidade, 
                                      'TBD', 
                                      'TBD', 
                                      'TBD', 
                                      @AnoEstreia, 
                                      'TBD', 
                                      'TBD')", connection);
                        dataAdapter.InsertCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 100, "Nome");
                        dataAdapter.InsertCommand.Parameters.Add("@Nacionalidade", SqlDbType.NVarChar, 100, "Nacionalidade");
                        dataAdapter.InsertCommand.Parameters.Add("@AnoEstreia", SqlDbType.Int, 0, "AnoEstreia");
                        
                        dataAdapter.DeleteCommand = new SqlCommand(
                            @"DELETE FROM Equipa WHERE ID_Equipa = @ID_Equipa", connection);
                        dataAdapter.DeleteCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");

                        int rowsAffected = dataAdapter.Update(teamTable);
                        MessageBox.Show($"{rowsAffected} row(s) updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        teamTable.AcceptChanges();
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
    }
}
