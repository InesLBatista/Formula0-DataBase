using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjetoFBD
{
    public partial class TeamMemberForm : Form
    {
        private DataGridView? dgvMembers;
        private Panel? pnlStaffActions;
        
        private string userRole;
        private SqlDataAdapter? dataAdapter;
        private DataTable? memberTable;

        public TeamMemberForm(string role)
        {
            InitializeComponent();
            
            this.userRole = role;
            
            this.Text = "Team Members Management";
            this.Size = new Size(1400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadMemberData();
        }

        private void SetupLayout()
        {
            Label lblTitle = new Label
            {
                Text = "Team Members Management",
                Location = new Point(20, 20),
                Size = new Size(500, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            dgvMembers = new DataGridView
            {
                Name = "dgvMembers",
                Location = new Point(20, 70),
                Size = new Size(1340, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            this.Controls.Add(dgvMembers);

            pnlStaffActions = new Panel
            {
                Location = new Point(20, 570),
                Size = new Size(840, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add Member", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(280, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(400, 5));

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
                dgvMembers.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvMembers.ReadOnly = true;
                pnlStaffActions.Visible = false;
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

        private void LoadMemberData()
        {
            string connectionString = DbConfig.ConnectionString;
            
            string query = @"
                SELECT 
                    m.ID_Membro,
                    m.Nome,
                    m.Nacionalidade,
                    m.DataNascimento,
                    m.Género,
                    m.Função,
                    m.ID_Equipa,
                    e.Nome AS TeamName
                FROM Membros_da_Equipa m
                LEFT JOIN Equipa e ON m.ID_Equipa = e.ID_Equipa
                ORDER BY m.Nome ASC";

            try
            {
                dataAdapter = new SqlDataAdapter(query, connectionString);
                memberTable = new DataTable();
                dataAdapter.Fill(memberTable);
                
                if (dgvMembers != null)
                {
                    dgvMembers.DataSource = memberTable;

                    if (dgvMembers.Columns.Contains("ID_Membro"))
                    {
                        dgvMembers.Columns["ID_Membro"]!.HeaderText = "Member ID";
                        dgvMembers.Columns["ID_Membro"]!.ReadOnly = true;
                        dgvMembers.Columns["ID_Membro"]!.Width = 80;
                    }
                    
                    if (dgvMembers.Columns.Contains("Nome"))
                        dgvMembers.Columns["Nome"]!.HeaderText = "Name";
                    
                    if (dgvMembers.Columns.Contains("Nacionalidade"))
                        dgvMembers.Columns["Nacionalidade"]!.HeaderText = "Nationality";
                    
                    if (dgvMembers.Columns.Contains("DataNascimento"))
                        dgvMembers.Columns["DataNascimento"]!.HeaderText = "Birth Date";
                    
                    if (dgvMembers.Columns.Contains("Género"))
                        dgvMembers.Columns["Género"]!.HeaderText = "Gender";
                    
                    if (dgvMembers.Columns.Contains("Função"))
                        dgvMembers.Columns["Função"]!.HeaderText = "Role";
                    
                    if (dgvMembers.Columns.Contains("ID_Equipa"))
                        dgvMembers.Columns["ID_Equipa"]!.Visible = false;
                    
                    if (dgvMembers.Columns.Contains("TeamName"))
                    {
                        dgvMembers.Columns["TeamName"]!.HeaderText = "Team";
                        dgvMembers.Columns["TeamName"]!.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading member data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && memberTable != null && userRole == "Staff")
            {
                string connectionString = DbConfig.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        if (dgvMembers != null)
                        {
                            dgvMembers.EndEdit();
                        }

                        connection.Open();
                        
                        dataAdapter.UpdateCommand = new SqlCommand(
                            @"UPDATE Membros_da_Equipa 
                              SET Nome = @Nome, 
                                  Nacionalidade = @Nacionalidade, 
                                  DataNascimento = @DataNascimento,
                                  Género = @Género,
                                  Função = @Função,
                                  ID_Equipa = @ID_Equipa
                              WHERE ID_Membro = @ID_Membro", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 100, "Nome");
                        dataAdapter.UpdateCommand.Parameters.Add("@Nacionalidade", SqlDbType.NVarChar, 100, "Nacionalidade");
                        dataAdapter.UpdateCommand.Parameters.Add("@DataNascimento", SqlDbType.Date, 0, "DataNascimento");
                        dataAdapter.UpdateCommand.Parameters.Add("@Género", SqlDbType.Char, 1, "Género");
                        dataAdapter.UpdateCommand.Parameters.Add("@Função", SqlDbType.NVarChar, 100, "Função");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        dataAdapter.UpdateCommand.Parameters.Add("@ID_Membro", SqlDbType.Int, 0, "ID_Membro");
                        
                        dataAdapter.InsertCommand = new SqlCommand(
                            @"INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa) 
                              VALUES (@Nome, @Nacionalidade, @DataNascimento, @Género, @Função, @ID_Equipa)", connection);
                        dataAdapter.InsertCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 100, "Nome");
                        dataAdapter.InsertCommand.Parameters.Add("@Nacionalidade", SqlDbType.NVarChar, 100, "Nacionalidade");
                        dataAdapter.InsertCommand.Parameters.Add("@DataNascimento", SqlDbType.Date, 0, "DataNascimento");
                        dataAdapter.InsertCommand.Parameters.Add("@Género", SqlDbType.Char, 1, "Género");
                        dataAdapter.InsertCommand.Parameters.Add("@Função", SqlDbType.NVarChar, 100, "Função");
                        dataAdapter.InsertCommand.Parameters.Add("@ID_Equipa", SqlDbType.Int, 0, "ID_Equipa");
                        
                        dataAdapter.DeleteCommand = new SqlCommand(
                            @"DELETE FROM Membros_da_Equipa WHERE ID_Membro = @ID_Membro", connection);
                        dataAdapter.DeleteCommand.Parameters.Add("@ID_Membro", SqlDbType.Int, 0, "ID_Membro");

                        int rowsAffected = dataAdapter.Update(memberTable);
                        MessageBox.Show($"{rowsAffected} row(s) updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        memberTable.AcceptChanges();
                        LoadMemberData();
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
            if (userRole == "Staff" && memberTable != null)
            {
                try
                {
                    DataRow newRow = memberTable.NewRow();
                    
                    // Nome
                    using (var inputForm = new InputDialog("Add Team Member", "Name:"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue))
                        {
                            newRow["Nome"] = inputForm.InputValue.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Name is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Nacionalidade
                    using (var inputForm = new InputDialog("Add Team Member", "Nationality:"))
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
                    
                    // Data Nascimento
                    using (var inputForm = new InputDialog("Add Team Member", "Birth Date (YYYY-MM-DD):"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            DateTime.TryParse(inputForm.InputValue, out DateTime birthDate))
                        {
                            newRow["DataNascimento"] = birthDate;
                        }
                        else
                        {
                            MessageBox.Show("Valid birth date is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Género
                    using (var inputForm = new InputDialog("Add Team Member", "Gender (M/F):"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue))
                        {
                            string gender = inputForm.InputValue.Trim().ToUpper();
                            if (gender == "M" || gender == "F")
                            {
                                newRow["Género"] = gender;
                            }
                            else
                            {
                                MessageBox.Show("Gender must be M or F.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Gender is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Função
                    using (var inputForm = new InputDialog("Add Team Member", "Role/Function:"))
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK &&
                            !string.IsNullOrWhiteSpace(inputForm.InputValue))
                        {
                            newRow["Função"] = inputForm.InputValue.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Role is required.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Team selection
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
                    newRow["TeamName"] = DBNull.Value;
                    
                    memberTable.Rows.Add(newRow);

                    if (dgvMembers != null && dgvMembers.Rows.Count > 0)
                    {
                        dgvMembers.CurrentCell = dgvMembers.Rows[dgvMembers.Rows.Count - 1].Cells[1];
                    }
                    
                    MessageBox.Show("Team member added. Click 'Save Changes' to commit to database.",
                        "Member Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding member: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff" && dgvMembers != null && dgvMembers.SelectedRows.Count > 0 && memberTable != null)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete the selected member(s)?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvMembers.SelectedRows)
                        {
                            if (row.Index >= 0 && row.Index < memberTable.Rows.Count)
                            {
                                memberTable.Rows[row.Index].Delete();
                            }
                        }
                        MessageBox.Show("Selected row(s) marked for deletion. Click 'Save Changes' to commit.",
                            "Deletion Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting member: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            if (memberTable != null && memberTable.GetChanges() != null)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved changes. Do you want to discard them and refresh?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    memberTable.RejectChanges();
                    LoadMemberData();
                }
            }
            else
            {
                LoadMemberData();
            }
        }

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
    }
}
