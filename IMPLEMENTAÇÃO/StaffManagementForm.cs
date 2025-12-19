using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjetoFBD
{
    public partial class StaffManagementForm : BaseForm
    {
        private DataGridView? dgvStaff;
        private Panel? pnlStaffActions;
        private DataTable? staffTable;
        private SqlDataAdapter? dataAdapter;

        public StaffManagementForm(string role) : base(role)
        {
            InitializeComponent();
            
            this.Text = "Staff Management";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            SetupLayout();
            LoadStaffData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "StaffManagementForm";
            this.ResumeLayout(false);
        }

        private void SetupLayout()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "Staff Management",
                Location = new Point(20, 20),
                Size = new Size(400, 35),
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // DataGridView for Staff
            dgvStaff = new DataGridView
            {
                Name = "dgvStaff",
                Location = new Point(20, 70),
                Size = new Size(1140, 470),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = false,
                AutoGenerateColumns = true
            };
            ConfigureDataGridView(dgvStaff);
            this.Controls.Add(dgvStaff);

            // Staff Actions Panel
            pnlStaffActions = new Panel
            {
                Location = new Point(20, 560),
                Size = new Size(725, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);

            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add Staff", new Point(145, 5));
            Button btnDelete = CreateActionButton("Delete", new Point(290, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(435, 5));
            Button btnViewContract = CreateActionButton("View Contract", new Point(580, 5), Color.FromArgb(0, 102, 204));

            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnViewContract.Click += btnViewContract_Click;

            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnViewContract);

            if (userRole == "Staff")
            {
                dgvStaff.ReadOnly = false;
                pnlStaffActions.Visible = true;
            }
            else
            {
                dgvStaff.ReadOnly = true;
                pnlStaffActions.Visible = false;
            }
        }

        private void LoadStaffData()
        {
            try
            {
                string connectionString = DbConfig.ConnectionString;
                
                string query = @"
                    SELECT 
                        s.StaffID,
                        s.Username,
                        s.Password,
                        s.NomeCompleto,
                        s.Role,
                        c.ID_Contrato,
                        c.AnoInicio,
                        c.AnoFim,
                        c.Função,
                        c.Salário,
                        c.Género,
                        c.ID_Membro
                    FROM Staff s
                    LEFT JOIN Membros_da_Equipa m ON s.NomeCompleto = m.Nome
                    LEFT JOIN Contrato c ON m.ID_Membro = c.ID_Membro
                    ORDER BY s.StaffID";

                dataAdapter = new SqlDataAdapter(query, connectionString);
                
                staffTable = new DataTable();
                dataAdapter.Fill(staffTable);
                
                if (dgvStaff != null)
                {
                    dgvStaff.DataSource = staffTable;
                    
                    // Wait for columns to be auto-generated
                    dgvStaff.Refresh();
                    Application.DoEvents();

                    try
                    {
                        // Configure columns
                        MakeColumnReadOnly(dgvStaff, "StaffID");
                        SetColumnHeader(dgvStaff, "StaffID", "ID");
                        if (dgvStaff.Columns.Contains("StaffID"))
                            dgvStaff.Columns["StaffID"]!.Width = 60;

                        SetColumnHeader(dgvStaff, "Username", "Username");
                        SetColumnHeader(dgvStaff, "Password", "Password");
                        SetColumnHeader(dgvStaff, "NomeCompleto", "Full Name");
                        SetColumnHeader(dgvStaff, "Role", "Role");
                        
                        MakeColumnReadOnly(dgvStaff, "ID_Contrato");
                        SetColumnHeader(dgvStaff, "ID_Contrato", "Contract ID");
                        if (dgvStaff.Columns.Contains("ID_Contrato"))
                            dgvStaff.Columns["ID_Contrato"]!.Width = 80;
                        
                        SetColumnHeader(dgvStaff, "AnoInicio", "Start Year");
                        SetColumnHeader(dgvStaff, "AnoFim", "End Year");
                        SetColumnHeader(dgvStaff, "Função", "Function");
                        SetColumnHeader(dgvStaff, "Salário", "Salary");
                        SetColumnHeader(dgvStaff, "Género", "Gender");
                        
                        MakeColumnReadOnly(dgvStaff, "ID_Membro");
                        HideColumn(dgvStaff, "ID_Membro");
                        
                        // Hide password column for security
                        HideColumn(dgvStaff, "Password");
                    }
                    catch (Exception colEx)
                    {
                        Console.WriteLine($"Error configuring columns: {colEx.Message}");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(sqlEx, "loading staff data");
            }
            catch (Exception ex)
            {
                ShowError($"Error loading staff: {ex.Message}");
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (dataAdapter != null && staffTable != null && userRole == "Staff")
            {
                try
                {
                    // Note: This will only update Staff table, not Contrato
                    // For Contrato updates, use View Contract button
                    string connectionString = DbConfig.ConnectionString;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        
                        dataAdapter.UpdateCommand = new SqlCommand(
                            @"UPDATE Staff 
                              SET Username = @Username, 
                                  Password = @Password, 
                                  NomeCompleto = @NomeCompleto,
                                  Role = @Role
                              WHERE StaffID = @StaffID", connection);
                        dataAdapter.UpdateCommand.Parameters.Add("@Username", SqlDbType.NVarChar, 50, "Username");
                        dataAdapter.UpdateCommand.Parameters.Add("@Password", SqlDbType.NVarChar, 50, "Password");
                        dataAdapter.UpdateCommand.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar, 100, "NomeCompleto");
                        dataAdapter.UpdateCommand.Parameters.Add("@Role", SqlDbType.NVarChar, 20, "Role");
                        dataAdapter.UpdateCommand.Parameters.Add("@StaffID", SqlDbType.Int, 0, "StaffID");

                        int rowsAffected = dataAdapter.Update(staffTable);
                        ShowSuccess($"{rowsAffected} row(s) updated successfully!");
                        
                        staffTable.AcceptChanges();
                        LoadStaffData();
                    }
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

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (userRole == "Staff")
            {
                using (var dialog = new AddStaffDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                            {
                                conn.Open();
                                
                                // First, insert or get ID_Membro from Membros_da_Equipa
                                string insertMember = @"
                                    IF NOT EXISTS (SELECT 1 FROM Membros_da_Equipa WHERE Nome = @Nome)
                                    BEGIN
                                        INSERT INTO Membros_da_Equipa (Nome, Nacionalidade, DataNascimento, Género, Função, ID_Equipa)
                                        VALUES (@Nome, @Nacionalidade, @DataNascimento, @Genero, @Funcao, @ID_Equipa)
                                    END
                                    SELECT ID_Membro FROM Membros_da_Equipa WHERE Nome = @Nome";
                                
                                SqlCommand cmdMember = new SqlCommand(insertMember, conn);
                                cmdMember.Parameters.AddWithValue("@Nome", dialog.FullName);
                                cmdMember.Parameters.AddWithValue("@Nacionalidade", dialog.Nationality ?? (object)DBNull.Value);
                                cmdMember.Parameters.AddWithValue("@DataNascimento", dialog.BirthDate.HasValue ? (object)dialog.BirthDate.Value : DBNull.Value);
                                cmdMember.Parameters.AddWithValue("@Genero", dialog.Gender ?? (object)DBNull.Value);
                                cmdMember.Parameters.AddWithValue("@Funcao", dialog.Function ?? (object)DBNull.Value);
                                cmdMember.Parameters.AddWithValue("@ID_Equipa", dialog.TeamId.HasValue ? (object)dialog.TeamId.Value : DBNull.Value);
                                
                                int memberId = (int)cmdMember.ExecuteScalar();
                                
                                // Insert Staff
                                string insertStaff = @"
                                    INSERT INTO Staff (Username, Password, NomeCompleto, Role)
                                    VALUES (@Username, @Password, @NomeCompleto, @Role)";
                                
                                SqlCommand cmdStaff = new SqlCommand(insertStaff, conn);
                                cmdStaff.Parameters.AddWithValue("@Username", dialog.Username);
                                cmdStaff.Parameters.AddWithValue("@Password", dialog.Password);
                                cmdStaff.Parameters.AddWithValue("@NomeCompleto", dialog.FullName);
                                cmdStaff.Parameters.AddWithValue("@Role", dialog.Role);
                                cmdStaff.ExecuteNonQuery();
                                
                                // Insert Contract (MANDATORY for all staff)
                                string insertContract = @"
                                    INSERT INTO Contrato (AnoInicio, AnoFim, Função, Salário, Género, ID_Membro)
                                    VALUES (@AnoInicio, @AnoFim, @Funcao, @Salario, @Genero, @ID_Membro)";
                                
                                SqlCommand cmdContract = new SqlCommand(insertContract, conn);
                                cmdContract.Parameters.AddWithValue("@AnoInicio", dialog.StartYear);
                                cmdContract.Parameters.AddWithValue("@AnoFim", dialog.EndYear.HasValue ? (object)dialog.EndYear.Value : DBNull.Value);
                                cmdContract.Parameters.AddWithValue("@Funcao", dialog.Function ?? (object)DBNull.Value);
                                cmdContract.Parameters.AddWithValue("@Salario", dialog.Salary);
                                cmdContract.Parameters.AddWithValue("@Genero", dialog.Gender ?? (object)DBNull.Value);
                                cmdContract.Parameters.AddWithValue("@ID_Membro", memberId);
                                cmdContract.ExecuteNonQuery();
                                
                                ShowSuccess("Staff member added successfully!");
                                LoadStaffData();
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            HandleSqlException(sqlEx, "adding staff member");
                        }
                        catch (Exception ex)
                        {
                            ShowError($"Error adding staff: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (!IsRowSelected(dgvStaff!, "staff member"))
                return;

            if (userRole == "Staff" && ShowConfirmation("Are you sure you want to delete this staff member?"))
            {
                try
                {
                    var selectedRow = dgvStaff!.SelectedRows[0];
                    int staffId = Convert.ToInt32(selectedRow.Cells["StaffID"].Value);

                    using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                    {
                        conn.Open();
                        string deleteQuery = "DELETE FROM Staff WHERE StaffID = @StaffID";
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@StaffID", staffId);
                        cmd.ExecuteNonQuery();
                    }

                    ShowSuccess("Staff member deleted successfully!");
                    LoadStaffData();
                }
                catch (SqlException sqlEx)
                {
                    HandleSqlException(sqlEx, "deleting staff member");
                }
                catch (Exception ex)
                {
                    ShowError($"Error deleting staff: {ex.Message}");
                }
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            LoadStaffData();
        }

        private void btnViewContract_Click(object? sender, EventArgs e)
        {
            if (!IsRowSelected(dgvStaff!, "staff member"))
                return;

            var selectedRow = dgvStaff!.SelectedRows[0];
            object? contractIdObj = selectedRow.Cells["ID_Contrato"].Value;
            
            if (contractIdObj == null || contractIdObj == DBNull.Value)
            {
                ShowWarning("This staff member does not have a contract.");
                return;
            }

            int contractId = Convert.ToInt32(contractIdObj);
            string fullName = selectedRow.Cells["NomeCompleto"].Value?.ToString() ?? "Unknown";

            // Show contract details dialog
            ShowContractDetails(contractId, fullName);
        }

        private void ShowContractDetails(int contractId, string staffName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT c.*, m.Nome, m.Nacionalidade
                        FROM Contrato c
                        INNER JOIN Membros_da_Equipa m ON c.ID_Membro = m.ID_Membro
                        WHERE c.ID_Contrato = @ContractID";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ContractID", contractId);
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Criar um form customizado para mostrar os detalhes
                        Form contractForm = new Form
                        {
                            Text = "Contract Details",
                            Size = new Size(500, 500),
                            StartPosition = FormStartPosition.CenterParent,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            MaximizeBox = false,
                            MinimizeBox = false,
                            BackColor = Color.White
                        };

                        // Título
                        Label lblTitle = new Label
                        {
                            Text = $"CONTRACT DETAILS",
                            Location = new Point(20, 20),
                            Size = new Size(440, 35),
                            Font = new Font("Arial", 16, FontStyle.Bold),
                            ForeColor = Color.FromArgb(220, 20, 20),
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        contractForm.Controls.Add(lblTitle);

                        // Nome do Staff
                        Label lblStaffName = new Label
                        {
                            Text = staffName,
                            Location = new Point(20, 60),
                            Size = new Size(440, 25),
                            Font = new Font("Arial", 12, FontStyle.Bold),
                            ForeColor = Color.FromArgb(60, 60, 60),
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        contractForm.Controls.Add(lblStaffName);

                        // Painel com informações
                        Panel infoPanel = new Panel
                        {
                            Location = new Point(20, 100),
                            Size = new Size(440, 300),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.FromArgb(250, 250, 250)
                        };
                        contractForm.Controls.Add(infoPanel);

                        int yPos = 15;
                        void AddInfoRow(string label, string value)
                        {
                            Label lbl = new Label
                            {
                                Text = label + ":",
                                Location = new Point(20, yPos),
                                Size = new Size(180, 22),
                                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                                ForeColor = Color.FromArgb(80, 80, 80)
                            };
                            infoPanel.Controls.Add(lbl);

                            Label val = new Label
                            {
                                Text = value,
                                Location = new Point(210, yPos),
                                Size = new Size(210, 22),
                                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                                ForeColor = Color.FromArgb(40, 40, 40)
                            };
                            infoPanel.Controls.Add(val);

                            yPos += 30;
                        }

                        // Guardar dados do contrato para edição
                        int currentContractId = Convert.ToInt32(reader["ID_Contrato"]);
                        int startYear = Convert.ToInt32(reader["AnoInicio"]);
                        int? endYear = reader["AnoFim"] == DBNull.Value ? null : Convert.ToInt32(reader["AnoFim"]);
                        string function = reader["Função"]?.ToString() ?? "";
                        decimal salary = Convert.ToDecimal(reader["Salário"]);
                        string gender = reader["Género"]?.ToString() ?? "";
                        int memberId = Convert.ToInt32(reader["ID_Membro"]);

                        AddInfoRow("Contract ID", reader["ID_Contrato"].ToString() ?? "");
                        AddInfoRow("Start Year", reader["AnoInicio"].ToString() ?? "");
                        AddInfoRow("End Year", reader["AnoFim"] == DBNull.Value ? "Ongoing" : reader["AnoFim"].ToString() ?? "");
                        AddInfoRow("Function", reader["Função"]?.ToString() ?? "N/A");
                        AddInfoRow("Salary", $"€{Convert.ToDecimal(reader["Salário"]):N2}");
                        AddInfoRow("Gender", reader["Género"]?.ToString() ?? "N/A");
                        AddInfoRow("Member Name", reader["Nome"].ToString() ?? "");
                        AddInfoRow("Nationality", reader["Nacionalidade"].ToString() ?? "");

                        reader.Close();

                        // Botão Edit
                        Button btnEdit = new Button
                        {
                            Text = "Edit",
                            Location = new Point(130, 420),
                            Size = new Size(100, 35),
                            BackColor = Color.FromArgb(0, 153, 76),
                            ForeColor = Color.White,
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold)
                        };
                        btnEdit.FlatAppearance.BorderSize = 0;
                        btnEdit.Click += (s, e) =>
                        {
                            contractForm.Hide();
                            var editDialog = new EditContractDialog(currentContractId, startYear, endYear, function, salary, gender, memberId);
                            if (editDialog.ShowDialog() == DialogResult.OK)
                            {
                                // Atualizar contrato na base de dados
                                try
                                {
                                    using (SqlConnection updateConn = new SqlConnection(DbConfig.ConnectionString))
                                    {
                                        updateConn.Open();
                                        string updateQuery = @"
                                            UPDATE Contrato 
                                            SET AnoInicio = @AnoInicio, 
                                                AnoFim = @AnoFim, 
                                                Função = @Funcao, 
                                                Salário = @Salario, 
                                                Género = @Genero
                                            WHERE ID_Contrato = @ContractID";
                                        
                                        SqlCommand updateCmd = new SqlCommand(updateQuery, updateConn);
                                        updateCmd.Parameters.AddWithValue("@AnoInicio", editDialog.StartYear);
                                        updateCmd.Parameters.AddWithValue("@AnoFim", editDialog.EndYear.HasValue ? (object)editDialog.EndYear.Value : DBNull.Value);
                                        updateCmd.Parameters.AddWithValue("@Funcao", editDialog.Function ?? (object)DBNull.Value);
                                        updateCmd.Parameters.AddWithValue("@Salario", editDialog.Salary);
                                        updateCmd.Parameters.AddWithValue("@Genero", editDialog.Gender ?? (object)DBNull.Value);
                                        updateCmd.Parameters.AddWithValue("@ContractID", currentContractId);
                                        
                                        updateCmd.ExecuteNonQuery();
                                        
                                        MessageBox.Show("Contract updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        contractForm.Close();
                                        LoadStaffData();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error updating contract: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                contractForm.Show();
                            }
                        };
                        contractForm.Controls.Add(btnEdit);

                        // Botão Close
                        Button btnClose = new Button
                        {
                            Text = "Close",
                            Location = new Point(250, 420),
                            Size = new Size(100, 35),
                            BackColor = Color.FromArgb(220, 20, 20),
                            ForeColor = Color.White,
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            DialogResult = DialogResult.OK
                        };
                        btnClose.FlatAppearance.BorderSize = 0;
                        contractForm.Controls.Add(btnClose);

                        contractForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading contract details: {ex.Message}");
            }
        }
    }

    // Dialog for adding new staff
    public class AddStaffDialog : Form
    {
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private TextBox txtFullName = null!;
        private ComboBox cmbRole = null!;
        private TextBox txtNationality = null!;
        private DateTimePicker dtpBirthDate = null!;
        private ComboBox cmbTeam = null!;
        private NumericUpDown nudStartYear = null!;
        private NumericUpDown nudEndYear = null!;
        private TextBox txtFunction = null!;
        private NumericUpDown nudSalary = null!;
        private ComboBox cmbGender = null!;

        public string Username { get; private set; } = "";
        public string Password { get; private set; } = "";
        public string FullName { get; private set; } = "";
        public string Role { get; private set; } = "";
        public string? Nationality { get; private set; }
        public DateTime? BirthDate { get; private set; }
        public int? TeamId { get; private set; }
        public int StartYear { get; private set; }
        public int? EndYear { get; private set; }
        public string? Function { get; private set; }
        public decimal Salary { get; private set; }
        public string? Gender { get; private set; }

        public AddStaffDialog()
        {
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Add New Staff Member";
            this.Size = new Size(550, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 20;

            // Title
            Label lblTitle = new Label
            {
                Text = "Add New Staff Member",
                Location = new Point(20, y),
                Size = new Size(500, 25),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);
            y += 40;

            // Username
            AddLabel("Username:", y);
            txtUsername = AddTextBox(y);
            y += 35;

            // Password
            AddLabel("Password:", y);
            txtPassword = AddTextBox(y);
            txtPassword.PasswordChar = '*';
            y += 35;

            // Full Name
            AddLabel("Full Name:", y);
            txtFullName = AddTextBox(y);
            y += 35;

            // Role
            AddLabel("Role:", y);
            cmbRole = new ComboBox
            {
                Location = new Point(150, y),
                Size = new Size(360, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new string[] { "Staff", "Guest" });
            cmbRole.SelectedIndex = 0;
            this.Controls.Add(cmbRole);
            y += 35;

            // Nationality
            AddLabel("Nationality:", y);
            txtNationality = AddTextBox(y);
            y += 35;

            // Birth Date
            AddLabel("Birth Date:", y);
            dtpBirthDate = new DateTimePicker
            {
                Location = new Point(150, y),
                Size = new Size(360, 25),
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtpBirthDate);
            y += 35;

            // Team
            AddLabel("Team:", y);
            cmbTeam = new ComboBox
            {
                Location = new Point(150, y),
                Size = new Size(360, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbTeam);
            LoadTeams();
            y += 45;

            // Add Contract Section (MANDATORY)
            Label lblContractTitle = new Label
            {
                Text = "Contract Information (Required)",
                Location = new Point(20, y),
                Size = new Size(300, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblContractTitle);
            y += 35;

            // Contract fields
            AddLabel("Start Year:", y);
            nudStartYear = new NumericUpDown
            {
                Location = new Point(150, y),
                Size = new Size(120, 25),
                Minimum = 1950,
                Maximum = 2100,
                Value = DateTime.Now.Year
            };
            this.Controls.Add(nudStartYear);
            y += 35;

            AddLabel("End Year:", y);
            nudEndYear = new NumericUpDown
            {
                Location = new Point(150, y),
                Size = new Size(120, 25),
                Minimum = 1950,
                Maximum = 2100,
                Value = DateTime.Now.Year + 1
            };
            this.Controls.Add(nudEndYear);
            y += 35;

            AddLabel("Function:", y);
            txtFunction = AddTextBox(y);
            y += 35;

            AddLabel("Salary:", y);
            nudSalary = new NumericUpDown
            {
                Location = new Point(150, y),
                Size = new Size(150, 25),
                Minimum = 0,
                Maximum = 10000000,
                DecimalPlaces = 2,
                ThousandsSeparator = true
            };
            this.Controls.Add(nudSalary);
            y += 35;

            AddLabel("Gender:", y);
            cmbGender = new ComboBox
            {
                Location = new Point(150, y),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new string[] { "M", "F" });
            this.Controls.Add(cmbGender);
            y += 45;

            // Buttons
            Button btnOK = new Button
            {
                Text = "Add Staff",
                Location = new Point(300, y),
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
                Location = new Point(410, y),
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

        private void LoadTeams()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ID_Equipa, Nome FROM Equipa ORDER BY Nome";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    cmbTeam.Items.Add(new TeamItem { ID = null, Name = "-- No Team --" });
                    
                    while (reader.Read())
                    {
                        cmbTeam.Items.Add(new TeamItem 
                        { 
                            ID = reader.GetInt32(0), 
                            Name = reader.GetString(1) 
                        });
                    }
                    
                    cmbTeam.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddLabel(string text, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(20, y + 3),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lbl);
        }

        private TextBox AddTextBox(int y)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(150, y),
                Size = new Size(360, 25)
            };
            this.Controls.Add(txt);
            return txt;
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter a full name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Contract validation (MANDATORY)
            if (nudEndYear.Value < nudStartYear.Value)
            {
                MessageBox.Show("End year must be greater than or equal to start year.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbGender.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a gender for the contract.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get values
            Username = txtUsername.Text.Trim();
            Password = txtPassword.Text.Trim();
            FullName = txtFullName.Text.Trim();
            Role = cmbRole.SelectedItem?.ToString() ?? "Guest";
            Nationality = string.IsNullOrWhiteSpace(txtNationality.Text) ? null : txtNationality.Text.Trim();
            BirthDate = dtpBirthDate.Value;

            // Team
            if (cmbTeam.SelectedItem is TeamItem team && team.ID.HasValue)
            {
                TeamId = team.ID;
            }

            // Contract data (MANDATORY)
            StartYear = (int)nudStartYear.Value;
            EndYear = (int)nudEndYear.Value;
            Function = string.IsNullOrWhiteSpace(txtFunction.Text) ? null : txtFunction.Text.Trim();
            Salary = nudSalary.Value;
            Gender = cmbGender.SelectedItem?.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private class TeamItem
        {
            public int? ID { get; set; }
            public string Name { get; set; } = "";

            public override string ToString()
            {
                return Name;
            }
        }
    }

    // Dialog for editing contract
    public class EditContractDialog : Form
    {
        private NumericUpDown nudStartYear = null!;
        private NumericUpDown nudEndYear = null!;
        private CheckBox chkOngoing = null!;
        private TextBox txtFunction = null!;
        private NumericUpDown nudSalary = null!;
        private ComboBox cmbGender = null!;

        public int StartYear { get; private set; }
        public int? EndYear { get; private set; }
        public string? Function { get; private set; }
        public decimal Salary { get; private set; }
        public string? Gender { get; private set; }

        public EditContractDialog(int contractId, int startYear, int? endYear, string function, decimal salary, string gender, int memberId)
        {
            this.Text = "Edit Contract";
            this.Size = new Size(450, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "EDIT CONTRACT",
                Location = new Point(20, 20),
                Size = new Size(390, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            int yPos = 70;

            // Start Year
            AddLabel("Start Year:", yPos);
            nudStartYear = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(260, 25),
                Minimum = 1950,
                Maximum = 2100,
                Value = startYear
            };
            this.Controls.Add(nudStartYear);
            yPos += 40;

            // End Year
            AddLabel("End Year:", yPos);
            nudEndYear = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(180, 25),
                Minimum = 1950,
                Maximum = 2100,
                Value = endYear ?? DateTime.Now.Year,
                Enabled = endYear.HasValue
            };
            this.Controls.Add(nudEndYear);

            chkOngoing = new CheckBox
            {
                Text = "Ongoing",
                Location = new Point(340, yPos),
                Size = new Size(80, 25),
                Checked = !endYear.HasValue
            };
            chkOngoing.CheckedChanged += (s, e) =>
            {
                nudEndYear.Enabled = !chkOngoing.Checked;
            };
            this.Controls.Add(chkOngoing);
            yPos += 40;

            // Function
            AddLabel("Function:", yPos);
            txtFunction = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(260, 25),
                Text = function
            };
            this.Controls.Add(txtFunction);
            yPos += 40;

            // Salary
            AddLabel("Salary (€):", yPos);
            nudSalary = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(260, 25),
                Minimum = 0,
                Maximum = 100000000,
                DecimalPlaces = 2,
                Value = salary,
                ThousandsSeparator = true
            };
            this.Controls.Add(nudSalary);
            yPos += 40;

            // Gender
            AddLabel("Gender:", yPos);
            cmbGender = new ComboBox
            {
                Location = new Point(150, yPos),
                Size = new Size(260, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new object[] { "M", "F" });
            if (!string.IsNullOrEmpty(gender))
                cmbGender.SelectedItem = gender;
            this.Controls.Add(cmbGender);
            yPos += 50;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Save",
                Location = new Point(130, yPos),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 153, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                StartYear = (int)nudStartYear.Value;
                EndYear = chkOngoing.Checked ? null : (int?)nudEndYear.Value;
                Function = txtFunction.Text;
                Salary = nudSalary.Value;
                Gender = cmbGender.SelectedItem?.ToString();
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnSave);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(240, yPos),
                Size = new Size(100, 35),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(20, y + 3),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(lbl);
        }
    }
}
