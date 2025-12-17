using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Configuration; 
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ProjetoFBD
{
    public partial class CircuitForm : Form
    {
        private string userRole;
        private SqlDataAdapter? dataAdapter; // Tornar anulável
        private DataTable? circuitoTable;
        // Fields must be declared in the Designer file (CircuitForm.Designer.cs)
        // We'll assume they are declared there, so we remove the '?' to simplify usage.
        // Data management fields

        // Mapeamento de países para códigos ISO e emojis de bandeiras
        private static readonly Dictionary<string, string> CountryFlags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Portugal", "🇵🇹" },
            { "Spain", "🇪🇸" }, { "Espanha", "🇪🇸" },
            { "Italy", "🇮🇹" }, { "Itália", "🇮🇹" },
            { "France", "🇫🇷" }, { "França", "🇫🇷" },
            { "UK", "🇬🇧" }, { "United Kingdom", "🇬🇧" }, { "Reino Unido", "🇬🇧" },
            { "Belgium", "🇧🇪" }, { "Bélgica", "🇧🇪" },
            { "Netherlands", "🇳🇱" }, { "Holanda", "🇳🇱" },
            { "Germany", "🇩🇪" }, { "Alemanha", "🇩🇪" },
            { "Austria", "🇦🇹" }, { "Áustria", "🇦🇹" },
            { "Monaco", "🇲🇨" }, { "Mónaco", "🇲🇨" },
            { "Hungary", "🇭🇺" }, { "Hungria", "🇭🇺" },
            { "Brazil", "🇧🇷" }, { "Brasil", "🇧🇷" },
            { "USA", "🇺🇸" }, { "United States", "🇺🇸" }, { "Estados Unidos", "🇺🇸" },
            { "Mexico", "🇲🇽" }, { "México", "🇲🇽" },
            { "Canada", "🇨🇦" }, { "Canadá", "🇨🇦" },
            { "Japan", "🇯🇵" }, { "Japão", "🇯🇵" },
            { "Singapore", "🇸🇬" }, { "Singapura", "🇸🇬" },
            { "Australia", "🇦🇺" }, { "Austrália", "🇦🇺" },
            { "UAE", "🇦🇪" }, { "United Arab Emirates", "🇦🇪" }, { "Emirados Árabes", "🇦🇪" },
            { "Bahrain", "🇧🇭" }, { "Bahrein", "🇧🇭" },
            { "Saudi Arabia", "🇸🇦" }, { "Arábia Saudita", "🇸🇦" },
            { "Qatar", "🇶🇦" }, { "Catar", "🇶🇦" },
            { "China", "🇨🇳" },
            { "South Korea", "🇰🇷" }, { "Coreia do Sul", "🇰🇷" },
            { "Russia", "🇷🇺" }, { "Rússia", "🇷🇺" },
            { "Azerbaijan", "🇦🇿" }, { "Azerbaijão", "🇦🇿" },
            { "Turkey", "🇹🇷" }, { "Turquia", "🇹🇷" }
        };

        private static string GetCountryFlag(string? country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return "";
            
            if (CountryFlags.TryGetValue(country.Trim(), out string? flag))
                return flag + " ";
            
            return "";
        }

        // Parameterless constructor required by the Designer
        public CircuitForm() : this("Guest") { } 

        public CircuitForm(string role)
        {
            // CRITICAL: InitializeComponent must be available from the Designer file
            InitializeComponent(); 
            this.userRole = role;
            
            // UI Text in English
            this.Text = "Circuits Management";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupCircuitsLayout();
            LoadCircuitoData();
        }

        private void SetupCircuitsLayout()
        {
            // --- 1. DataGridView for Listing ---
            dgvCircuitos = new DataGridView
            {
                Name = "dgvCircuits",
                Location = new Point(10, 10),
                Size = new Size(1160, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                // Default to ReadOnly. Staff access handled below.
                ReadOnly = true 
            };
            this.Controls.Add(dgvCircuitos);

            // --- Adicionar Coluna de Botão de Mapa ---
            DataGridViewButtonColumn mapButtonColumn = new DataGridViewButtonColumn
            {
                Name = "MapColumn",
                HeaderText = "Mapa",
                Text = "Ver Mapa",
                UseColumnTextForButtonValue = true
            };
            dgvCircuitos.Columns.Add(mapButtonColumn);
            dgvCircuitos.CellContentClick += dgvCircuitos_CellContentClick; // Ligar o evento

            // --- 2. Staff Actions Panel ---
            pnlStaffActions = new Panel
            {
                Location = new Point(10, 580),
                Size = new Size(980, 50),  // 6 botões x 140 + espaços
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(pnlStaffActions);
            

            // UI Text in English
            Button btnSave = CreateActionButton("Save Changes", new Point(0, 5));
            Button btnAdd = CreateActionButton("Add New", new Point(140, 5));
            Button btnDelete = CreateActionButton("Delete Selected", new Point(280, 5));
            Button btnEdit = CreateActionButton("Edit Selected", new Point(420, 5));
            Button btnRefresh = CreateActionButton("Refresh", new Point(560, 5));
            Button btnUploadMap = CreateActionButton("Upload Map", new Point(700, 5));

            btnSave.Click += btnSave_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnEdit.Click += btnEdit_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnUploadMap.Click += btnUploadMap_Click;

            // Adiciona os botões ao painel (cada botão apenas uma vez)
            pnlStaffActions.Controls.Add(btnSave);
            pnlStaffActions.Controls.Add(btnAdd);
            pnlStaffActions.Controls.Add(btnDelete);
            pnlStaffActions.Controls.Add(btnEdit);
            pnlStaffActions.Controls.Add(btnRefresh);
            pnlStaffActions.Controls.Add(btnUploadMap);
            
            // --- 3. Role-Based Access Control (RBAC) ---
            if (this.userRole == "Staff")
            {
                dgvCircuitos.ReadOnly = false; // Allow inline editing for Staff
                pnlStaffActions.Visible = true; // Show action buttons
            }
            else
            {
                // Guest access: Read-only and hide action buttons
                dgvCircuitos.ReadOnly = true; 
                pnlStaffActions.Visible = false;
            }
        }
        
        // Helper method for action buttons
        private Button CreateActionButton(string text, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(204, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            // configure FlatAppearance after construction (safer em object initializer)
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        // -------------------------------------------------------------------------
        // UI event handlers
        // -------------------------------------------------------------------------

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (userRole != "Staff") return;
            if (dgvCircuitos == null || dgvCircuitos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to edit.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
    return;
}

// Select first selected row and begin editing the first editable cell (excluding PK)
var row = dgvCircuitos.SelectedRows[0];
            int editColIndex = -1;
            for (int i = 0; i < dgvCircuitos.Columns.Count; i++)
            {
                var col = dgvCircuitos.Columns[i];
                if (!col.ReadOnly && col.Visible)
                {
                    editColIndex = i; break;
                }
            }
            if (editColIndex >= 0)
            {
                dgvCircuitos.CurrentCell = row.Cells[editColIndex];
                dgvCircuitos.BeginEdit(true);
            }
            else
            {
                MessageBox.Show("No editable column found.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // -------------------------------------------------------------------------
        // DATA ACCESS METHODS
        // -------------------------------------------------------------------------

// Ficheiro: CircuitForm.cs

private void LoadCircuitoData()
{
    string connectionString = DbConfig.ConnectionString;
    string query = "SELECT ID_Circuito, Nome, Cidade, Pais, Comprimento_km, NumCurvas FROM Circuito";

    try
    {
        dataAdapter = new SqlDataAdapter(query, connectionString);
        circuitoTable = new DataTable();
        
        dataAdapter.Fill(circuitoTable!);
        
        // Adicionar bandeiras aos países
        if (circuitoTable.Columns.Contains("Pais"))
        {
            foreach (DataRow row in circuitoTable.Rows)
            {
                if (row["Pais"] != DBNull.Value)
                {
                    string country = row["Pais"].ToString() ?? "";
                    string flag = GetCountryFlag(country);
                    if (!string.IsNullOrEmpty(flag))
                    {
                        row["Pais"] = flag + country;
                    }
                }
            }
        }
        
        dgvCircuitos.DataSource = circuitoTable;
        
        // --- CRÍTICO: Configurar os comandos de salvamento AQUI e APENAS AQUI ---
        
        SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
        
        // Atribui os comandos gerados (o erro NullRef vinha desta reatribuição no Save)
        dataAdapter.InsertCommand = commandBuilder.GetInsertCommand(true);
        dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand(true);
        dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand(true);

        // Garante que o ID gerado pelo SQL volta para o DataGridView
        dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.Both; 
        
                    
                    // ID_Circuito (Primary Key) should be read-only even for Staff
                    if (dgvCircuitos.Columns.Contains("ID_Circuito"))
                    {
                        var column = dgvCircuitos.Columns["ID_Circuito"];
                        if (column != null)
                        {
                            column.ReadOnly = true;
                        }
                    }

                    // Translate column headers to English for the UI
                    var nomeColumn = dgvCircuitos.Columns["Nome"];
                    if (nomeColumn != null) nomeColumn.HeaderText = "Name";
                    var cidadeColumn = dgvCircuitos.Columns["Cidade"];
                    if (cidadeColumn != null) cidadeColumn.HeaderText = "City";
                    var paisColumn = dgvCircuitos.Columns["Pais"];
                    if (paisColumn != null) paisColumn.HeaderText = "Country";
                    var comprimentoColumn = dgvCircuitos.Columns["Comprimento_km"];
                    if (comprimentoColumn != null) comprimentoColumn.HeaderText = "Length (km)";
                    var numCurvasColumn = dgvCircuitos.Columns["NumCurvas"];
                    if (numCurvasColumn != null) numCurvasColumn.HeaderText = "Num Corners";

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Circuit data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        private static string RemoveCountryFlag(string? countryWithFlag)
        {
            if (string.IsNullOrWhiteSpace(countryWithFlag))
                return "";
            
            // Remove o emoji de bandeira (primeiros caracteres se existirem)
            string cleaned = countryWithFlag.Trim();
            foreach (var flag in CountryFlags.Values)
            {
                if (cleaned.StartsWith(flag))
                {
                    cleaned = cleaned.Substring(flag.Length).Trim();
                    break;
                }
            }
            return cleaned;
        }
        
private void btnSave_Click(object? sender, EventArgs e)
{
    if (dataAdapter != null && circuitoTable != null && userRole == "Staff")
    {
        string connectionString = DbConfig.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            MessageBox.Show("Connection string is not configured.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Utilizamos 'using' para garantir que a conexão fecha automaticamente
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                dgvCircuitos.EndEdit(); 
                
                // Remover bandeiras antes de salvar
                if (circuitoTable.Columns.Contains("Pais"))
                {
                    foreach (DataRow row in circuitoTable.Rows)
                    {
                        if (row["Pais"] != DBNull.Value && row["Pais"] != null)
                        {
                            row["Pais"] = RemoveCountryFlag(row["Pais"].ToString());
                        }
                    }
                }
                
                // CRÍTICO: Ligar os comandos gerados à NOVA conexão ativa
                if (dataAdapter.InsertCommand != null) dataAdapter.InsertCommand.Connection = connection;
                if (dataAdapter.UpdateCommand != null) dataAdapter.UpdateCommand.Connection = connection;
                if (dataAdapter.DeleteCommand != null) dataAdapter.DeleteCommand.Connection = connection;

                connection.Open();
                
                // Salvar as alterações
                int rowsAffected = dataAdapter.Update(circuitoTable); 
                
                MessageBox.Show($"{rowsAffected} rows saved successfully!", "Success");
                circuitoTable.AcceptChanges();
                
                // Recarregar dados para mostrar as bandeiras novamente
                LoadCircuitoData();
            }
            catch (Exception ex)
            {
                // Este catch irá capturar erros de violação de chave primária, NULL, ou dados inválidos.
                MessageBox.Show($"Error saving data: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                circuitoTable.RejectChanges(); 
            }
        } 
    }
}
        // Ficheiro: CircuitForm.cs

private void btnAdd_Click(object? sender, EventArgs e)
{
    
    if (circuitoTable != null && userRole == "Staff")
    {
        // 1. Adiciona uma nova linha vazia no topo do DataTable
        DataRow newRow = circuitoTable.NewRow();
        circuitoTable.Rows.InsertAt(newRow, 0);
        
        // --- CRÍTICO: Forçar o foco e a edição na nova linha ---
        
        // 2. Seleciona a primeira linha (onde a nova linha foi inserida)
        if (dgvCircuitos.Rows.Count > 0 && dgvCircuitos.Columns.Contains("Nome"))
        {
            dgvCircuitos.CurrentCell = dgvCircuitos.Rows[0].Cells["Nome"]; // Assumimos que 'Nome' é a primeira célula editável
            // 3. Força o início do modo de edição
            dgvCircuitos.BeginEdit(true);
        }
    }
}

// Ficheiro: CircuitForm.cs

private void btnDelete_Click(object? sender, EventArgs e)
{
    // Verifica se o utilizador tem permissão e se há alguma linha selecionada
    if (userRole == "Staff" && dgvCircuitos.SelectedRows.Count > 0 && circuitoTable != null)
    {
        // Confirmação de segurança antes de eliminar
        DialogResult dialogResult = MessageBox.Show(
            "Are you sure you want to delete the selected row(s)? This action cannot be undone.", 
            "Confirm Deletion", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Warning);

        if (dialogResult == DialogResult.Yes)
        {
            try
            {
                // Percorre as linhas selecionadas (em ordem inversa para evitar problemas de índice)
                foreach (DataGridViewRow row in dgvCircuitos.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(r => r.Index))
                {
                    // Obtém a DataRow correspondente no DataTable
                    DataRow? dataRow = (row.DataBoundItem as DataRowView)?.Row;
                    
                    if (dataRow != null)
                    {
                        // Marca a linha para eliminação
                        dataRow.Delete();
                    }
                }
                
                // Salva as alterações na BD imediatamente após a eliminação
                btnSave_Click(sender, e); 
                
                // Recarrega os dados (opcional, mas garante que a visualização é atualizada)
                // LoadCircuitoData(); 
            }
            catch (SqlException sqlEx)
            {
                // Tratamento específico para erros de constraint de foreign key
                if (sqlEx.Message.Contains("REFERENCE constraint") || sqlEx.Message.Contains("FK_"))
                {
                    MessageBox.Show(
                        "Cannot delete this circuit because it is being used by one or more Grand Prix races.\n\n" +
                        "Please remove or reassign the affected Grand Prix races first, then try deleting the circuit again.",
                        "Circuit In Use",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Database error during deletion: {sqlEx.Message}", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                circuitoTable.RejectChanges(); // Reverte a eliminação se houver erro
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during deletion: {ex.Message}", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                circuitoTable.RejectChanges(); // Reverte a eliminação se houver erro
            }
        }
    }
}

// No CircuitForm.cs (Adicione junto dos outros métodos de evento, ex: btnSave_Click)

private void btnRefresh_Click(object? sender, EventArgs e)
{
    // Confirma as alterações pendentes para evitar perda de dados
    if (circuitoTable != null && circuitoTable.GetChanges() != null)
    {
        DialogResult result = MessageBox.Show(
            "You have unsaved changes. Do you want to discard them and refresh the data?",
            "Unsaved Changes",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            circuitoTable.RejectChanges();
            LoadCircuitoData(); // Recarrega os dados do banco de dados
        }
        // Se o utilizador clicar em 'No', não faz nada.
    }
    else
    {
        LoadCircuitoData(); // Não há alterações pendentes, carrega diretamente.
    }
}
// Ficheiro: CircuitForm.cs

        // You may need to add the btnDelete_Click method here (implementing logic to delete the selected row)

        private void dgvCircuitos_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Verifica se o clique foi na coluna do botão e numa linha válida
            var mapColumn = dgvCircuitos.Columns["MapColumn"];
            if (e.RowIndex >= 0 && mapColumn != null && e.ColumnIndex == mapColumn.Index)
            {
                // Obtém o nome do circuito a partir da célula 'Nome' da linha clicada
                string circuitName = dgvCircuitos.Rows[e.RowIndex].Cells["Nome"].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(circuitName))
                {
                    ShowMapImage(circuitName);
                }
            }
        }

        private void btnUploadMap_Click(object? sender, EventArgs e)
        {
            if (userRole != "Staff")
            {
                MessageBox.Show("Only Staff members can upload circuit maps.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvCircuitos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a circuit first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string circuitName = dgvCircuitos.SelectedRows[0].Cells["Nome"].Value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(circuitName))
            {
                MessageBox.Show("The selected circuit has no name. Please add a name first.", "Invalid Circuit",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abrir diálogo para selecionar imagem
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Circuit Map Image";
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|PNG Files|*.png|JPEG Files|*.jpg;*.jpeg|All Files|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Criar pasta mapasCircuitos se não existir
                        string mapsFolderPath = Path.Combine(Application.StartupPath, @"..\..\..\mapasCircuitos");
                        string fullMapsFolderPath = Path.GetFullPath(mapsFolderPath);

                        if (!Directory.Exists(fullMapsFolderPath))
                        {
                            Directory.CreateDirectory(fullMapsFolderPath);
                        }

                        // Nome do ficheiro: nome do circuito com underscores + extensão .png
                        string fileName = circuitName.Replace(' ', '_') + ".png";
                        string destinationPath = Path.Combine(fullMapsFolderPath, fileName);

                        // Se já existe um mapa, perguntar se quer substituir
                        if (File.Exists(destinationPath))
                        {
                            DialogResult result = MessageBox.Show(
                                $"A map already exists for '{circuitName}'.\n\nDo you want to replace it?",
                                "Replace Existing Map",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (result != DialogResult.Yes)
                            {
                                return;
                            }

                            // Eliminar o ficheiro antigo
                            File.Delete(destinationPath);
                        }

                        // Copiar a imagem selecionada para a pasta de mapas
                        File.Copy(openFileDialog.FileName, destinationPath, true);

                        MessageBox.Show($"Map uploaded successfully for '{circuitName}'!\n\nLocation: {destinationPath}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error uploading map: {ex.Message}", "Upload Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ShowMapImage(string circuitName)
        {
            try
            {
                // Converte o nome do circuito para o formato do nome do ficheiro (substitui espaços por underscores)
                string imageName = circuitName.Replace(' ', '_') + ".png";

                // Constrói o caminho para a imagem. Assume que a pasta 'mapasCircuitos' está na raiz do projeto.
                // O executável corre em 'IMPLEMENTAÇÃO/bin/Debug/netX.X-windows', por isso subimos 3 níveis para a pasta 'IMPLEMENTAÇÃO'.
                string relativePath = @"..\..\..\mapasCircuitos\" + imageName;
                string fullPath = Path.GetFullPath(Path.Combine(Application.StartupPath, relativePath));


                if (File.Exists(fullPath))
                {
                    // Cria um novo formulário para mostrar o mapa
                    using (Form mapForm = new Form())
                    {
                        mapForm.Text = "Mapa: " + circuitName;
                        mapForm.Size = new Size(800, 600);
                        mapForm.StartPosition = FormStartPosition.CenterParent;

                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Dock = DockStyle.Fill;
                        pictureBox.Load(fullPath); // Usar Load é mais seguro que Image.FromFile
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                        mapForm.Controls.Add(pictureBox);
                        mapForm.ShowDialog(); // Mostra o formulário como uma janela de diálogo
                    }
                }
                else
                {
                    MessageBox.Show($"Mapa não encontrado para o circuito: {circuitName}\n\nCaminho procurado:\n{fullPath}", "Mapa não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao tentar mostrar o mapa:\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}