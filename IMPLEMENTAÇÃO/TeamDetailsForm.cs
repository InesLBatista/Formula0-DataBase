using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjetoFBD
{
    public partial class TeamDetailsForm : Form
    {
        private int teamId;
        private string userRole;
        private DataTable? teamData;
        
        // Controles de UI
        private TextBox? txtName;
        private TextBox? txtNationality;
        private TextBox? txtBase;
        private TextBox? txtTeamPrincipal;
        private TextBox? txtTechnicalDirector;
        private TextBox? txtDebutYear;
        private TextBox? txtChassisModel;
        private TextBox? txtPowerUnit;
        private TextBox? txtReserveDrivers;
        
        public TeamDetailsForm(int teamId, string role)
        {
            InitializeComponent();
            
            this.teamId = teamId;
            this.userRole = role;
            
            this.Text = $"Team Details - ID {teamId}";
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            SetupDetailsLayout();
            LoadTeamDetails();
        }
        
        private void SetupDetailsLayout()
        {
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };
            this.Controls.Add(mainPanel);
            
            int yPos = 20;
            int labelWidth = 180;
            int textBoxWidth = 320;
            int rowHeight = 60;
            
            // Title
            Label lblTitle = new Label
            {
                Text = "Team Details",
                Location = new Point(20, yPos),
                Size = new Size(500, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            mainPanel.Controls.Add(lblTitle);
            yPos += 50;
            
            // Name
            AddField(mainPanel, "Name:", ref txtName, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Nationality
            AddField(mainPanel, "Nationality:", ref txtNationality, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Base
            AddField(mainPanel, "Base:", ref txtBase, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Team Principal
            AddField(mainPanel, "Team Principal:", ref txtTeamPrincipal, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Technical Director
            AddField(mainPanel, "Technical Director:", ref txtTechnicalDirector, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Debut Year
            AddField(mainPanel, "Debut Year:", ref txtDebutYear, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Chassis Model
            AddField(mainPanel, "Chassis Model:", ref txtChassisModel, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Power Unit
            AddField(mainPanel, "Power Unit:", ref txtPowerUnit, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            // Reserve Drivers
            AddField(mainPanel, "Reserve Drivers:", ref txtReserveDrivers, ref yPos, labelWidth, textBoxWidth, rowHeight);
            
            yPos += 20;
            
            // Buttons
            if (userRole == "Staff")
            {
                Button btnSave = new Button
                {
                    Text = "Save Changes",
                    Location = new Point(200, yPos),
                    Size = new Size(130, 40),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Click += btnSave_Click;
                mainPanel.Controls.Add(btnSave);
            }
        }
        
        private void AddField(Panel panel, string labelText, ref TextBox? textBox, ref int yPos, int labelWidth, int textBoxWidth, int rowHeight)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            panel.Controls.Add(lbl);
            
            textBox = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(textBoxWidth, 25),
                Font = new Font("Arial", 10),
                ReadOnly = (userRole != "Staff")
            };
            panel.Controls.Add(textBox);
            
            yPos += rowHeight;
        }
        
        private void LoadTeamDetails()
        {
            string connectionString = DbConfig.ConnectionString;
            
            string query = @"
                SELECT 
                    ID_Equipa,
                    Nome,
                    Nacionalidade,
                    Base,
                    ChefeEquipa,
                    ChefeTécnico,
                    AnoEstreia,
                    ModeloChassis,
                    Power_Unit,
                    PilotosReserva
                FROM Equipa
                WHERE ID_Equipa = @TeamId";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TeamId", teamId);
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    teamData = new DataTable();
                    adapter.Fill(teamData);
                    
                    if (teamData.Rows.Count > 0)
                    {
                        DataRow row = teamData.Rows[0];
                        
                        if (txtName != null) txtName.Text = row["Nome"]?.ToString() ?? "";
                        if (txtNationality != null) txtNationality.Text = row["Nacionalidade"]?.ToString() ?? "";
                        if (txtBase != null) txtBase.Text = row["Base"]?.ToString() ?? "";
                        if (txtTeamPrincipal != null) txtTeamPrincipal.Text = row["ChefeEquipa"]?.ToString() ?? "";
                        if (txtTechnicalDirector != null) txtTechnicalDirector.Text = row["ChefeTécnico"]?.ToString() ?? "";
                        if (txtDebutYear != null) txtDebutYear.Text = row["AnoEstreia"]?.ToString() ?? "";
                        if (txtChassisModel != null) txtChassisModel.Text = row["ModeloChassis"]?.ToString() ?? "";
                        if (txtPowerUnit != null) txtPowerUnit.Text = row["Power_Unit"]?.ToString() ?? "";
                        if (txtReserveDrivers != null) txtReserveDrivers.Text = row["PilotosReserva"]?.ToString() ?? "";
                    }
                    else
                    {
                        MessageBox.Show("Team not found.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (userRole != "Staff")
            {
                MessageBox.Show("Only Staff can edit team details.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string connectionString = DbConfig.ConnectionString;
            
            string updateQuery = @"
                UPDATE Equipa SET
                    Nome = @Nome,
                    Nacionalidade = @Nacionalidade,
                    Base = @Base,
                    ChefeEquipa = @ChefeEquipa,
                    ChefeTécnico = @ChefeTécnico,
                    AnoEstreia = @AnoEstreia,
                    ModeloChassis = @ModeloChassis,
                    Power_Unit = @PowerUnit,
                    PilotosReserva = @PilotosReserva
                WHERE ID_Equipa = @TeamId";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(updateQuery, connection);
                    
                    command.Parameters.AddWithValue("@Nome", txtName?.Text ?? "");
                    command.Parameters.AddWithValue("@Nacionalidade", string.IsNullOrWhiteSpace(txtNationality?.Text) ? DBNull.Value : txtNationality.Text);
                    command.Parameters.AddWithValue("@Base", string.IsNullOrWhiteSpace(txtBase?.Text) ? DBNull.Value : txtBase.Text);
                    command.Parameters.AddWithValue("@ChefeEquipa", string.IsNullOrWhiteSpace(txtTeamPrincipal?.Text) ? DBNull.Value : txtTeamPrincipal.Text);
                    command.Parameters.AddWithValue("@ChefeTécnico", string.IsNullOrWhiteSpace(txtTechnicalDirector?.Text) ? DBNull.Value : txtTechnicalDirector.Text);
                    
                    if (int.TryParse(txtDebutYear?.Text, out int year))
                        command.Parameters.AddWithValue("@AnoEstreia", year);
                    else
                        command.Parameters.AddWithValue("@AnoEstreia", DBNull.Value);
                    
                    command.Parameters.AddWithValue("@ModeloChassis", string.IsNullOrWhiteSpace(txtChassisModel?.Text) ? DBNull.Value : txtChassisModel.Text);
                    command.Parameters.AddWithValue("@PowerUnit", string.IsNullOrWhiteSpace(txtPowerUnit?.Text) ? DBNull.Value : txtPowerUnit.Text);
                    command.Parameters.AddWithValue("@PilotosReserva", string.IsNullOrWhiteSpace(txtReserveDrivers?.Text) ? DBNull.Value : txtReserveDrivers.Text);
                    command.Parameters.AddWithValue("@TeamId", teamId);
                    
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Team details updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No changes were made.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving team details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
