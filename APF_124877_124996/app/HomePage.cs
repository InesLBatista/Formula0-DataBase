    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Reflection;
    using System.IO;
    using System.Linq; // Necessário para usar FirstOrDefault

    namespace ProjetoFBD
    {
        // A palavra-chave 'partial' é crucial para ligar ao HomePage.Designer.cs
        public partial class HomePage : Form
        {
            private string userRole;
            private Panel pnlContent = null!;
            private Panel pnlTopBar = null!;
            private Button btnBack = null!;
            private Label lblCurrentView = null!;
            private Stack<Control> navigationStack;
            
            // As declarações dos Painéis foram movidas para o Designer.cs
            // private Panel pnlGrandPrix;
            // private Panel pnlSeasons;
            // private Panel pnlGrid;
            
            // -------------------------------------------------------------------------
            // CONSTRUTOR
            // -------------------------------------------------------------------------
            
            public HomePage(string role) 
            {
                // CRÍTICO: InitializeComponent() deve ser a primeira chamada
                InitializeComponent(); 
                this.DoubleBuffered = true;
                
                // Initialize navigation stack
                navigationStack = new Stack<Control>();
                
                // Initialize global navigation helper
                NavigationHelper.Initialize(this);
                
                // 1. Configurações Iniciais e Background
                SetupBackgroundImage();
                this.userRole = role; 
                
                this.Text = "Home Page - " + role;
                this.WindowState = FormWindowState.Maximized;
                
                // 2. Criação do Layout com Content Panel
                SetupLayout(); 
            }

            // -------------------------------------------------------------------------
            // BACKGROUND IMAGE (Recurso Embutido com Fallback)
            // -------------------------------------------------------------------------
            
            private void SetupBackgroundImage()
            {
                var assembly = Assembly.GetExecutingAssembly();
                
                // Tenta encontrar o recurso que termine em "background.png"
                string? resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("background.png", StringComparison.OrdinalIgnoreCase));

                Image? backgroundImg = null;

                if (!string.IsNullOrEmpty(resourceName))
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            backgroundImg = Image.FromStream(stream);
                        }
                    }
                }

                // Fallback: tenta carregar a partir da pasta de execução
                if (backgroundImg == null)
                {
                    string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "background.png");
                    if (File.Exists(fallbackPath))
                    {
                        backgroundImg = Image.FromFile(fallbackPath);
                    }
                }

                if (backgroundImg != null)
                {
                    // Criar imagem com 50% de opacidade
                    Bitmap dimmedImage = new Bitmap(backgroundImg.Width, backgroundImg.Height);
                    using (Graphics g = Graphics.FromImage(dimmedImage))
                    {
                        // Desenha a imagem original
                        g.DrawImage(backgroundImg, 0, 0);
                        
                        // Desenha overlay preto com 50% de opacidade por cima
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                        {
                            g.FillRectangle(brush, 0, 0, dimmedImage.Width, dimmedImage.Height);
                        }
                    }

                    // Define a imagem como background do Form
                    this.BackgroundImage = dimmedImage;
                    this.BackgroundImageLayout = ImageLayout.Stretch;

                    // Mantém o PictureBox como fallback (já está dock-fill)
                    pbFundo.Image = dimmedImage;
                    pbFundo.SendToBack();
                }
                else
                {
                    // Se nada funcionar, define uma cor de fundo sólida
                    this.BackgroundImage = null;
                    this.BackColor = Color.FromArgb(20, 20, 20); 
                }
            }

            // -------------------------------------------------------------------------
            // LAYOUT E MENU (Com Estilo F1)
            // -------------------------------------------------------------------------

            private void SetupLayout()
            {
                // --- 1. Criar e Ancorar o Painel de Menu Principal no Topo ---
                Panel pnlMainMenu = new Panel
                {
                    Size = new Size(this.ClientSize.Width, 70),
                    Location = new Point(0, 0),
                    BackColor = Color.FromArgb(200, 15, 15, 15), // Mais opaco para destaque
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Name = "pnlMainMenu"
                };
                this.Controls.Add(pnlMainMenu);
                pnlMainMenu.BringToFront();
                
                // --- 1.5 Create Top Navigation Bar (Below Main Menu) ---
                pnlTopBar = new Panel
                {
                    Size = new Size(this.ClientSize.Width, 50),
                    Location = new Point(0, 70),
                    BackColor = Color.FromArgb(200, 30, 30, 30),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Name = "pnlTopBar",
                    Visible = false // Hidden until content is loaded
                };
                this.Controls.Add(pnlTopBar);
                
                btnBack = new Button
                {
                    Text = "← BACK",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    Size = new Size(120, 35),
                    Location = new Point(15, 7),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnBack.FlatAppearance.BorderSize = 0;
                btnBack.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 30, 30);
                btnBack.Click += BtnBack_Click;
                pnlTopBar.Controls.Add(btnBack);
                
                lblCurrentView = new Label
                {
                    Text = "",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(this.ClientSize.Width - 450, 35),
                    Location = new Point(150, 7),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                pnlTopBar.Controls.Add(lblCurrentView);
                
                Button btnHome = new Button
                {
                    Text = "🏠 HOME",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    BackColor = Color.FromArgb(0, 102, 204),
                    ForeColor = Color.White,
                    Size = new Size(120, 35),
                    Location = new Point(this.ClientSize.Width - 155, 7),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnHome.FlatAppearance.BorderSize = 0;
                btnHome.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 224);
                btnHome.Click += BtnHome_Click;
                pnlTopBar.Controls.Add(btnHome);
                
                // --- 1.6 Create Content Panel (Full Screen Below Navigation) ---
                pnlContent = new Panel
                {
                    Location = new Point(0, 120), // Below menu + nav bar
                    Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 120),
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Name = "pnlContent"
                };
                this.Controls.Add(pnlContent);
                pnlContent.BringToFront();
                
                // --- 2. Criação dos Botões do Cabeçalho (Estilo F1 Moderno) ---
                
                Font menuFont = new Font("Arial", 14, FontStyle.Bold);
                Color headerColor = Color.FromArgb(220, 20, 20); // Vermelho F1
                Color textColor = Color.White;
                
                Button btnGrandPrix = CreateMenuHeaderButton("GRAND PRIX", menuFont, headerColor, textColor, new Point(30, 15));
                Button btnSeasons = CreateMenuHeaderButton("SEASONS", menuFont, headerColor, textColor, new Point(230, 15));
                Button btnGrid = CreateMenuHeaderButton("GRID", menuFont, headerColor, textColor, new Point(430, 15));

                pnlMainMenu.Controls.Add(btnGrandPrix);
                pnlMainMenu.Controls.Add(btnSeasons);
                pnlMainMenu.Controls.Add(btnGrid);
                
                // --- 3. Criação e Configuração dos Painéis Dropdown ---
                
                // Os painéis globais são inicializados no Designer, mas reconfigurados e reposicionados aqui
                this.pnlGrandPrix = CreateDropdownPanel(new Point(30, 70)); 
                this.pnlSeasons = CreateDropdownPanel(new Point(230, 70)); 
                this.pnlGrid = CreateDropdownPanel(new Point(430, 70)); 
                
                // Adicionar Itens e Eventos Toggle... (Resto da lógica de layout)
                AddDropdownItem(pnlGrandPrix, "GP", 0);
                AddDropdownItem(pnlGrandPrix, "Circuits", 1);

                AddDropdownItem(pnlSeasons, "All Seasons", 0);
                
                AddDropdownItem(pnlGrid, "Drivers", 0);
                AddDropdownItem(pnlGrid, "Teams", 1);
                
                // Team Members - Only for Staff
                if (userRole == "Staff")
                {
                    AddDropdownItem(pnlGrid, "Team Members", 2);
                    AddDropdownItem(pnlGrid, "Staff Management", 3);
                    // Adjust panel height for 4 items
                    pnlGrid.Size = new Size(180, 200);
                }

                // Adicionar os painéis ao formulário (só se não os adicionou no Designer)
                // Se o Designer já os adicionou, remova as próximas 3 linhas
                this.Controls.Add(pnlGrandPrix);
                this.Controls.Add(pnlSeasons);
                this.Controls.Add(pnlGrid);

                // CRÍTICO: Trazer os painéis para frente para ficarem acima do background
                pnlGrandPrix.BringToFront();
                pnlSeasons.BringToFront();
                pnlGrid.BringToFront();

                pnlGrandPrix.Visible = false;
                pnlSeasons.Visible = false;
                pnlGrid.Visible = false;
                
                // Lógica de Clique (Toggle)
                btnGrandPrix.Click += (s, e) => ToggleDropdown(pnlGrandPrix);
                btnSeasons.Click += (s, e) => ToggleDropdown(pnlSeasons);
                btnGrid.Click += (s, e) => ToggleDropdown(pnlGrid);
                
                // Mensagem de Boas-Vindas e Logout
                DisplayWelcomeMessage();
                
                Button btnLogout = new Button
                {
                    Text = "LOGOUT",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    BackColor = Color.FromArgb(150, 150, 150),
                    ForeColor = Color.White,
                    Size = new Size(120, 40),
                    Location = new Point(this.ClientSize.Width - 270, this.ClientSize.Height - 60),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };
                btnLogout.FlatAppearance.BorderSize = 0;
                btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(180, 180, 180);
                btnLogout.Click += new EventHandler(this.btnLogout_Click);
                this.Controls.Add(btnLogout);
                btnLogout.BringToFront();
                
                Button btnQuit = new Button
                {
                    Text = "QUIT",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    BackColor = Color.FromArgb(220, 20, 20),
                    ForeColor = Color.White,
                    Size = new Size(120, 40),
                    Location = new Point(this.ClientSize.Width - 140, this.ClientSize.Height - 60),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };
                btnQuit.FlatAppearance.BorderSize = 0;
                btnQuit.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 30, 30);
                btnQuit.Click += new EventHandler(this.btnQuit_Click);
                this.Controls.Add(btnQuit);
                btnQuit.BringToFront();
            }

            // -------------------------------------------------------------------------
            // MÉTODOS AUXILIARES E EVENTOS
            // -------------------------------------------------------------------------
            
            private void DisplayWelcomeMessage()
            {
                // Mensagem de boas-vindas simples sem painel de fundo
                Label lblWelcome = new Label
                {
                    Text = $"BEM-VINDO, {userRole.ToUpper()}!",
                    Font = new Font("Arial", 28, FontStyle.Bold),
                    AutoSize = false,
                    Size = new Size(500, 60),
                    Location = new Point(this.ClientSize.Width / 2 - 250, this.ClientSize.Height / 2 - 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White
                };

                this.Controls.Add(lblWelcome);
                lblWelcome.SendToBack();
            }

            private void btnLogout_Click(object? sender, EventArgs e)
            {
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.FormClosed += (s, args) =>
                {
                    // Se o LoginForm fechar, fecha também a HomePage
                    this.Close();
                };
                loginForm.Show();
            }

            private void btnQuit_Click(object? sender, EventArgs e)
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to quit the application?",
                    "Quit Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }

            private Button CreateMenuHeaderButton(string text, Font font, Color backColor, Color foreColor, Point location)
            {
                Button btn = new Button
                {
                    Text = text,
                    Font = font,
                    BackColor = backColor,
                    ForeColor = foreColor,
                    Size = new Size(180, 40),
                    Location = location,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 10, 10);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 30, 30);
                return btn;
            }

            private Panel CreateDropdownPanel(Point location)
            {
                Panel panel = new Panel
                {
                    Size = new Size(180, 200),
                    Location = location,
                    BackColor = Color.FromArgb(240, 240, 240),
                    BorderStyle = BorderStyle.None,
                    AutoScroll = true
                };
                
                panel.Paint += (sender, e) => {
                    // Borda mais elegante
                    using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                    }
                };
                
                panel.BringToFront();
                return panel;
            }

            private void AddDropdownItem(Panel parentPanel, string text, int index)
            {
                Button btnItem = new Button
                {
                    Text = text,
                    Size = new Size(parentPanel.Width, 30),
                    Location = new Point(0, index * 30),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { 
                        BorderSize = 0, 
                        MouseDownBackColor = Color.LightGray, 
                        MouseOverBackColor = Color.Gainsboro 
                    }, 
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 0, 0, 0), 
                    BackColor = Color.White,
                    ForeColor = Color.Black 
                };
                
                btnItem.Paint += (sender, e) => {
                    e.Graphics.DrawLine(Pens.LightGray, 0, btnItem.Height - 1, btnItem.Width, btnItem.Height - 1);
                };
                
                btnItem.MouseEnter += (s, e) => btnItem.BackColor = Color.Gainsboro;
                btnItem.MouseLeave += (s, e) => btnItem.BackColor = Color.White;

                parentPanel.Controls.Add(btnItem);
        if (parentPanel == pnlGrandPrix && text == "GP")
    {
        btnItem.Click += (s, e) => OpenGPForm();
    }
    else if (parentPanel == pnlGrandPrix && text == "Circuits")
    {
        btnItem.Click += (s, e) => OpenCircuitForm(); 
    }
    else if (parentPanel == pnlSeasons && text == "All Seasons")
    {
        btnItem.Click += (s, e) => OpenSeasonForm();
    }
    else if (parentPanel == pnlGrid && text == "Drivers")
    {
        btnItem.Click += (s, e) => OpenDriverForm();
    }
    else if (parentPanel == pnlGrid && text == "Teams")
    {
        btnItem.Click += (s, e) => OpenTeamForm();
    }
    else if (parentPanel == pnlGrid && text == "Team Members")
    {
        btnItem.Click += (s, e) => OpenTeamMemberForm();
    }
    else if (parentPanel == pnlGrid && text == "Staff Management")
    {
        btnItem.Click += (s, e) => OpenStaffManagementForm();
    }

            }
            

            private void ToggleDropdown(Panel targetPanel)
            {
                pnlGrandPrix.Visible = false;
                pnlSeasons.Visible = false;
                pnlGrid.Visible = false;
                
                targetPanel.Visible = !targetPanel.Visible;
            }


            // Ficheiro: HomePage.cs

    private void OpenGPForm()
    {
        pnlGrandPrix.Visible = false;
        GPForm gpForm = new GPForm(this.userRole);
        LoadFormIntoContent(gpForm, "GRAND PRIX");
    }

    private void OpenCircuitForm()
    {
        pnlGrandPrix.Visible = false;
        CircuitForm circuitForm = new CircuitForm(this.userRole);
        LoadFormIntoContent(circuitForm, "CIRCUITS");
    }
    
    private void OpenSeasonForm()
    {
        pnlSeasons.Visible = false;
        SeasonForm seasonForm = new SeasonForm(this.userRole);
        LoadFormIntoContent(seasonForm, "SEASONS");
    }
    
    private void OpenDriverForm()
    {
        pnlGrid.Visible = false;
        DriverForm driverForm = new DriverForm(this.userRole);
        LoadFormIntoContent(driverForm, "DRIVERS");
    }
    
    private void OpenTeamForm()
    {
        pnlGrid.Visible = false;
        TeamForm teamForm = new TeamForm(this.userRole);
        LoadFormIntoContent(teamForm, "TEAMS");
    }
    
    private void OpenTeamMemberForm()
    {
        pnlGrid.Visible = false;
        TeamMemberForm memberForm = new TeamMemberForm(this.userRole);
        LoadFormIntoContent(memberForm, "TEAM MEMBERS");
    }
    
    private void OpenStaffManagementForm()
    {
        pnlGrid.Visible = false;
        StaffManagementForm staffForm = new StaffManagementForm(this.userRole);
        LoadFormIntoContent(staffForm, "STAFF MANAGEMENT");
    }
    
    // Single-window navigation system
    private void LoadFormIntoContent(Form form, string title)
    {
        // Save current content to history if exists
        if (pnlContent.Controls.Count > 0 && pnlContent.Controls[0] != null)
        {
            Control? currentControl = pnlContent.Controls[0];
            if (currentControl != null)
            {
                navigationStack.Push(currentControl);
            }
        }
        
        // Clear content panel
        pnlContent.Controls.Clear();
        
        // Configure form as control
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        
        // Add to content panel
        pnlContent.Controls.Add(form);
        form.Show();
        
        // Update navigation UI
        lblCurrentView.Text = title;
        pnlTopBar.Visible = true;
        pnlTopBar.BringToFront();
    }
    
    private void BtnBack_Click(object? sender, EventArgs e)
    {
        if (navigationStack.Count > 0)
        {
            // Get previous content
            Control previousControl = navigationStack.Pop();
            
            // Clear current content
            if (pnlContent.Controls.Count > 0)
            {
                Control currentControl = pnlContent.Controls[0];
                if (currentControl is Form currentForm)
                {
                    currentForm.Close();
                }
                pnlContent.Controls.Clear();
            }
            
            // Restore previous content
            pnlContent.Controls.Add(previousControl);
            
            // Update title based on control type
            if (previousControl is GPForm)
                lblCurrentView.Text = "GRAND PRIX";
            else if (previousControl is CircuitForm)
                lblCurrentView.Text = "CIRCUITS";
            else if (previousControl is SeasonForm)
                lblCurrentView.Text = "SEASONS";
            else if (previousControl is DriverForm)
                lblCurrentView.Text = "DRIVERS";
            else if (previousControl is TeamForm)
                lblCurrentView.Text = "TEAMS";
            else if (previousControl is TeamMemberForm)
                lblCurrentView.Text = "TEAM MEMBERS";
            else if (previousControl is ResultsForm)
                lblCurrentView.Text = "RESULTS";
        }
        else
        {
            // No more history - go back to home
            pnlContent.Controls.Clear();
            pnlTopBar.Visible = false;
            DisplayWelcomeMessage();
        }
    }
    
    private void BtnHome_Click(object? sender, EventArgs e)
    {
        // Clear navigation stack and return to home
        while (navigationStack.Count > 0)
        {
            navigationStack.Pop();
        }
        
        // Clear current content
        if (pnlContent.Controls.Count > 0)
        {
            Control currentControl = pnlContent.Controls[0];
            if (currentControl is Form currentForm)
            {
                currentForm.Close();
            }
            pnlContent.Controls.Clear();
        }
        
        pnlTopBar.Visible = false;
        DisplayWelcomeMessage();
    }
    
    // Public methods for NavigationHelper to call
    public void NavigateToForm(Form form, string title)
    {
        LoadFormIntoContent(form, title);
    }
    
    public void NavigateBack()
    {
        BtnBack_Click(null, EventArgs.Empty);
    }


    // Nota: Você pode precisar de ajustar os nomes dos seus métodos (OpenGPForm, OpenCircuitForm) 
    // para corresponder exatamente aos nomes que está a usar no seu projeto.
        }
    }
