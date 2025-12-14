using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjetoFBD
{
    public partial class TeamStandingsForm : Form
    {
        private string userRole;
        private DataGridView? dgvStandings;
        
        public TeamStandingsForm(string role)
        {
            InitializeComponent();
            
            this.userRole = role;
            this.Text = "Team Standings";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            SetupLayout();
            LoadStandings();
        }

        private void SetupLayout()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "Team Standings - Current Season",
                Location = new Point(20, 20),
                Size = new Size(500, 35),
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 20)
            };
            this.Controls.Add(lblTitle);

            // DataGridView
            dgvStandings = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(840, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            this.Controls.Add(dgvStandings);
        }

        private void LoadStandings()
        {
            try
            {
                string connectionString = DbConfig.ConnectionString;
                
                // Query to calculate team standings by summing driver points
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(r.Pontos) DESC) AS Position,
                        e.Nome AS Team,
                        e.Nacionalidade AS Nationality,
                        COUNT(DISTINCT p.ID_Piloto) AS Drivers,
                        SUM(r.Pontos) AS TotalPoints,
                        COUNT(DISTINCT CASE WHEN r.PosiçãoFinal = 1 THEN r.ID_Resultado END) AS Wins,
                        COUNT(DISTINCT CASE WHEN r.PosiçãoFinal <= 3 THEN r.ID_Resultado END) AS Podiums
                    FROM Equipa e
                    LEFT JOIN Piloto p ON e.ID_Equipa = p.ID_Equipa
                    LEFT JOIN Resultados r ON p.ID_Piloto = r.ID_Piloto
                    GROUP BY e.ID_Equipa, e.Nome, e.Nacionalidade
                    HAVING SUM(r.Pontos) IS NOT NULL
                    ORDER BY TotalPoints DESC, Wins DESC, Team ASC";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable standingsTable = new DataTable();
                    adapter.Fill(standingsTable);
                    
                    if (dgvStandings != null)
                    {
                        dgvStandings.DataSource = standingsTable;
                        
                        // Wait for columns to be auto-generated
                        dgvStandings.Refresh();
                        Application.DoEvents();
                        
                        try
                        {
                            // Configure columns
                            if (dgvStandings.Columns.Contains("Position"))
                            {
                                dgvStandings.Columns["Position"]!.HeaderText = "Pos";
                                dgvStandings.Columns["Position"]!.Width = 60;
                                dgvStandings.Columns["Position"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                dgvStandings.Columns["Position"]!.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                            }
                            
                            if (dgvStandings.Columns.Contains("Team"))
                            {
                                dgvStandings.Columns["Team"]!.HeaderText = "Team";
                            dgvStandings.Columns["Team"]!.Width = 250;
                            dgvStandings.Columns["Team"]!.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                        }
                        
                        if (dgvStandings.Columns.Contains("Nationality"))
                        {
                            dgvStandings.Columns["Nationality"]!.HeaderText = "Nationality";
                            dgvStandings.Columns["Nationality"]!.Width = 150;
                        }
                        
                        if (dgvStandings.Columns.Contains("Drivers"))
                        {
                            dgvStandings.Columns["Drivers"]!.HeaderText = "Drivers";
                            dgvStandings.Columns["Drivers"]!.Width = 80;
                            dgvStandings.Columns["Drivers"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        
                        if (dgvStandings.Columns.Contains("TotalPoints"))
                        {
                            dgvStandings.Columns["TotalPoints"]!.HeaderText = "Points";
                            dgvStandings.Columns["TotalPoints"]!.Width = 100;
                            dgvStandings.Columns["TotalPoints"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dgvStandings.Columns["TotalPoints"]!.DefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
                            dgvStandings.Columns["TotalPoints"]!.DefaultCellStyle.ForeColor = Color.FromArgb(220, 20, 20);
                        }
                        
                        if (dgvStandings.Columns.Contains("Wins"))
                        {
                            dgvStandings.Columns["Wins"]!.HeaderText = "Wins";
                            dgvStandings.Columns["Wins"]!.Width = 70;
                            dgvStandings.Columns["Wins"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        
                        if (dgvStandings.Columns.Contains("Podiums"))
                        {
                            dgvStandings.Columns["Podiums"]!.HeaderText = "Podiums";
                            dgvStandings.Columns["Podiums"]!.Width = 90;
                            dgvStandings.Columns["Podiums"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        
                        // Alternate row colors for better readability
                        dgvStandings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                        dgvStandings.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 20, 20);
                        dgvStandings.RowsDefaultCellStyle.SelectionForeColor = Color.White;
                        }
                        catch (Exception colEx)
                        {
                            Console.WriteLine($"Error configuring columns: {colEx.Message}");
                            // Continue - column configuration is not critical
                        }
                    }
                    
                    // Show count
                    Label lblCount = new Label
                    {
                        Text = $"Total Teams: {standingsTable.Rows.Count}",
                        Location = new Point(20, 530),
                        Size = new Size(200, 25),
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left
                    };
                    this.Controls.Add(lblCount);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team standings: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
