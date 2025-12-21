using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProjetoFBD
{
    public partial class GPListForm : Form
    {
        private string userRole;
        private int selectedYear;
        private DataGridView? dgvGPs;
        private Panel? pnlActions;
        private TextBox? txtSearch;
        private DataTable? gpDataTable;
        private bool isCircuitFilter;
        private int? filterCircuitId;
        private string? filterCircuitName;
        private string titleText;
        
        public GPListForm(string role, int year)
        {
            InitializeComponent();
            
            this.userRole = role;
            this.selectedYear = year;
            this.isCircuitFilter = false;
            this.filterCircuitId = null;
            this.filterCircuitName = null;
            this.titleText = $"Grand Prix Events - {year} Season";
            
            // Configuração básica
            this.Text = titleText;
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Adicionar controles
            SetupLayout();
            LoadGPData();
        }

        // Constructor for filtering by circuit
        public GPListForm(string role, int circuitId, string circuitName)
        {
            InitializeComponent();

            this.userRole = role;
            this.selectedYear = 0;
            this.isCircuitFilter = true;
            this.filterCircuitId = circuitId;
            this.filterCircuitName = circuitName;
            this.titleText = $"GPs at {circuitName}";

            this.Text = titleText;
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupLayout();
            LoadGPData();
        }

        private void SetupLayout()
        {
            // Título
            Label lblTitle = new Label
            {
                Text = titleText,
                Location = new Point(20, 20),
                Size = new Size(500, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);

            // Search bar
            Label lblSearch = new Label
            {
                Text = "Search GP:",
                Location = new Point(20, 58),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(105, 55),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Type GP name or date..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            this.Controls.Add(txtSearch);

            // DataGridView para mostrar os GPs
            dgvGPs = new DataGridView
            {
                Name = "dgvGPs",
                Location = new Point(20, 90),
                Size = new Size(940, 430),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            this.Controls.Add(dgvGPs);
            // Allow double-click to open details
            dgvGPs.CellDoubleClick += DgvGPs_CellDoubleClick;

            // Painel para ações (botões)
            pnlActions = new Panel
            {
                Location = new Point(20, 540),
                Size = new Size(400, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlActions);

            // Botão para ver detalhes do GP (sessões, resultados, etc.)
            Button btnViewDetails = CreateActionButton("View GP Details", new Point(0, 5));
            btnViewDetails.Click += btnViewDetails_Click;
            
            pnlActions.Controls.Add(btnViewDetails);
            
            // Para Staff, adicionar botões de administração
            if (this.userRole == "Staff")
            {
                Button btnAddGP = CreateActionButton("Add GP", new Point(140, 5));
                btnAddGP.Click += btnAddGP_Click;
                pnlActions.Controls.Add(btnAddGP);
                
                // Botão para editar GP selecionado
                Button btnEditGP = CreateActionButton("Edit GP", new Point(280, 5));
                btnEditGP.Click += btnEditGP_Click;
                pnlActions.Controls.Add(btnEditGP);
                
                // Aumentar tamanho do painel
                pnlActions.Size = new Size(420, 50);
            }
        }
        
        private Button CreateActionButton(string text, Point location)
        {
            Button btn = new Button 
            { 
                Text = text, 
                Location = location, 
                Size = new Size(130, 40), 
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            return btn;
        }

        private void LoadGPData()
{
    try
    {
            string connectionString = DbConfig.ConnectionString;
        
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string is not configured.", "Configuration Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
    
        // Query switches depending on filter mode
        string[] queries;
        if (isCircuitFilter && filterCircuitId.HasValue)
        {
            queries = new string[]
            {
                @"
                SELECT 
                    NomeGP AS [Grand Prix Name],
                    DataCorrida AS [Race Date],
                    Ano_Temporada AS Season
                FROM Grande_Prémio
                WHERE ID_Circuito = @CircuitId
                ORDER BY DataCorrida ASC"
            };
        }
        else
        {
            queries = new string[]
            {
                @"
                SELECT 
                    NomeGP AS [Grand Prix Name],
                    DataCorrida AS [Race Date],
                    Ano_Temporada AS Season
                FROM Grande_Prémio
                WHERE Ano_Temporada = @Year
                ORDER BY DataCorrida ASC"
            };
        }
    
    DataTable gpTable = new DataTable();
    bool success = false;
    string lastError = "";
    
    for (int i = 0; i < queries.Length; i++)
    {
        try
        {
            Console.WriteLine($"Tentando query {i + 1}...");
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queries[i], connection);
                if (isCircuitFilter && filterCircuitId.HasValue)
                    command.Parameters.AddWithValue("@CircuitId", filterCircuitId.Value);
                else
                    command.Parameters.AddWithValue("@Year", selectedYear);
                
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                gpTable.Clear();
                adapter.Fill(gpTable);
                
                success = true;
                Console.WriteLine($"✓ Query {i + 1} bem sucedida! {gpTable.Rows.Count} registros encontrados.");
                
                // Mostrar colunas retornadas (apenas log)
                Console.WriteLine("Colunas retornadas:");
                foreach (DataColumn column in gpTable.Columns)
                {
                    Console.WriteLine($"  - {column.ColumnName} ({column.DataType})");
                }
                
                break;
            }
        }
        catch (SqlException ex)
        {
            lastError = ex.Message;
            Console.WriteLine($"✗ Query {i + 1} falhou: {ex.Message}");
            
            // Se não for a última query, continue tentando
            if (i < queries.Length - 1)
            {
                continue;
            }
        }
    }
    
    if (!success)
    {
        string errorMsg = $"Não foi possível carregar dados dos GPs.\n\nÚltimo erro: {lastError}\n\n";
        errorMsg += "Por favor, verifique:\n";
        errorMsg += "1. Conexão com a base de dados\n";
        errorMsg += "2. Nomes das tabelas e colunas\n";
        errorMsg += "3. Permissões de acesso\n\n";
        errorMsg += "Clique em OK para ver a estrutura das tabelas.";
        
        MessageBox.Show(errorMsg, "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }
    
    // Configurar o DataGridView
    if (dgvGPs != null && gpTable != null && gpTable.Rows.Count > 0)
    {
        dgvGPs.AutoGenerateColumns = false;
        dgvGPs.Columns.Clear();
        
        // Grand Prix Name
        DataGridViewTextBoxColumn gpColumn = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Grand Prix Name",
            HeaderText = "Grand Prix Name",
            Name = "GPName",
            Width = 320,
            ReadOnly = true
        };
        dgvGPs.Columns.Add(gpColumn);

        // Race Date
        DataGridViewTextBoxColumn dateColumn = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Race Date",
            HeaderText = "Race Date",
            Name = "RaceDate",
            Width = 200,
            ReadOnly = true
        };
        dgvGPs.Columns.Add(dateColumn);

        // Season
        DataGridViewTextBoxColumn seasonColumn = new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Season",
            HeaderText = "Season",
            Name = "Season",
            Width = 120,
            ReadOnly = true
        };
        dgvGPs.Columns.Add(seasonColumn);
        
        // Guardar dados completos para filtro
        gpDataTable = gpTable.Copy();
        
        dgvGPs.DataSource = gpTable;
    }
    else if (dgvGPs != null)
    {
        MessageBox.Show($"No Grand Prix events found for {selectedYear} season.\n\nPlease add Grand Prix events first.",
            "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    
    // Mostrar contagem de GPs
    if (dgvGPs != null && gpTable != null && gpTable.Rows.Count > 0)
    {
        // Remover label anterior se existir
        foreach (Control control in this.Controls)
        {
            if (control is Label lbl && lbl.Text != null && lbl.Text.Contains("Total GPs:"))
            {
                this.Controls.Remove(control);
                break;
            }
        }
        
        string countLabel = isCircuitFilter && filterCircuitName != null
            ? $"Total GPs for {filterCircuitName}: {gpTable.Rows.Count}"
            : $"Total GPs: {gpTable.Rows.Count}";

        Label lblCount = new Label
        {
            Text = countLabel,
            Location = new Point(500, 540),
            Size = new Size(300, 30),
            Font = new Font("Arial", 10, FontStyle.Bold),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
        this.Controls.Add(lblCount);
    }
    else if (gpTable != null && gpTable.Rows.Count == 0)
    {
        MessageBox.Show($"No Grand Prix events found for {selectedYear}.", 
            "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading GP data: {ex.Message}\n\nStack Trace: {ex.StackTrace}", 
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

        private void btnViewDetails_Click(object? sender, EventArgs e)
{
    if (dgvGPs == null || dgvGPs.SelectedRows.Count == 0)
    {
        MessageBox.Show("Please select a Grand Prix first.", "No Selection", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }
    
    DataGridViewRow selectedRow = dgvGPs.SelectedRows[0];
    
    // Get GP name from the column
    string? gpName = selectedRow.Cells["GPName"]?.Value?.ToString();
    
    if (string.IsNullOrEmpty(gpName))
    {
        MessageBox.Show("Invalid Grand Prix selection.", "Error", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }
    
    // Open SessionForm for this GP
    ShowGPDetails(gpName);
    // If this form is modal, close it so the Sessions page is visible
    try { this.DialogResult = DialogResult.OK; } catch {}
    this.Close();
}

        private void DgvGPs_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvGPs == null) return;
            dgvGPs.Rows[e.RowIndex].Selected = true;
            btnViewDetails_Click(sender, EventArgs.Empty);
        }

        private void btnAddGP_Click(object? sender, EventArgs e)
{
    if (this.userRole != "Staff")
    {
        MessageBox.Show("Only Staff members can add new Grand Prix events.", 
            "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }
    
    // Abrir o GPForm para adicionar um novo GP para esta temporada
    try
    {
        // Use o construtor que realmente existe - apenas com userRole
        GPForm gpForm = new GPForm(this.userRole);
        
        // Quando o formulário for mostrado, podemos tentar pré-selecionar o ano
        // se o GPForm tiver um controle para selecionar o ano
        gpForm.Shown += (s, args) =>
        {
            // Você pode tentar definir o ano no formulário se tiver acesso aos controles
            // Isso depende de como seu GPForm está implementado
            
            // Exemplo 1: Se houver uma propriedade pública para definir o ano
            try
            {
                var property = gpForm.GetType().GetProperty("SelectedYear");
                if (property != null && property.CanWrite)
                {
                    property.SetValue(gpForm, selectedYear);
                }
            }
            catch
            {
                // Ignorar se não funcionar
            }
            
            // Exemplo 2: Mostrar mensagem informativa
            MessageBox.Show($"Creating GP for season {selectedYear}. Please ensure you select the correct year.", 
                "Season Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        
        if (gpForm.ShowDialog() == DialogResult.OK)
        {
            // Recarregar dados após adicionar
            LoadGPData();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error opening GP form: {ex.Message}", 
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

        private void btnEditGP_Click(object? sender, EventArgs e)
        {
            if (dgvGPs == null || dgvGPs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a Grand Prix to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            DataGridViewRow selectedRow = dgvGPs.SelectedRows[0];
            string gpName = selectedRow.Cells["GPName"]?.Value?.ToString() ?? "Unknown GP";
            
            // Mensagem informativa (você precisaria implementar a edição propriamente dita)
            MessageBox.Show($"Would open edit form for: {gpName}\n\n" +
                           "You would need to pass the GP ID to the edit form.", 
                           "Edit GP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Para implementar a edição, você precisaria:
            // 1. Buscar o ID do GP selecionado
            // 2. Abrir GPForm em modo edição
            // 3. Passar o ID e os dados atuais
        }

        private void ShowGPDetails(string gpName)
        {
            try
            {
                // Abrir o SessionForm para mostrar as sessões deste GP
                SessionForm sessionForm = new SessionForm(this.userRole, gpName);
                NavigationHelper.NavigateTo(sessionForm, "SESSIONS - " + gpName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not show GP details: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para atualizar a lista
        public void RefreshGPList()
        {
            // Limpar controles existentes (exceto título e DataGridView)
            foreach (Control control in this.Controls)
            {
                if (control is Label lbl && lbl.Text != null && lbl.Text.Contains("Total GPs:"))
                {
                    this.Controls.Remove(control);
                    break;
                }
            }
            
            LoadGPData();
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtSearch == null || gpDataTable == null || dgvGPs == null)
                return;

            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                // Mostrar todos os dados
                dgvGPs.DataSource = gpDataTable;
            }
            else
            {
                // Filtrar dados por GP Name ou Race Date
                try
                {
                    DataView dv = gpDataTable.DefaultView;
                    // Escapar aspas simples para evitar erros de SQL injection
                    string escapedSearch = searchText.Replace("'", "''");
                    
                    // Filtro que procura em Grand Prix Name e Race Date (convertido para string)
                    dv.RowFilter = $"[Grand Prix Name] LIKE '%{escapedSearch}%' OR CONVERT([Race Date], 'System.String') LIKE '%{escapedSearch}%'";
                    
                    dgvGPs.DataSource = dv.ToTable();
                }
                catch
                {
                    // Se houver erro no filtro, mostrar todos os dados
                    dgvGPs.DataSource = gpDataTable;
                }
            }
        }
    }
}