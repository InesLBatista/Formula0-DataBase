using System.ComponentModel;

public class InputDialog : Form
{
    private TextBox txtInput = null!;
    private Button btnOk = null!;
    private Button btnCancel = null!;
    private Label lblPrompt = null!;
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? InputValue { get; set; }
    
    public InputDialog(string title, string prompt, string initialValue = "")
    {
        InitializeComponents(title, prompt, initialValue);
    }
    
    private void InitializeComponents(string title, string prompt, string initialValue)
    {
        this.Text = title;
        this.Size = new Size(500, 250);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        
        lblPrompt = new Label
        {
            Text = prompt,
            Location = new Point(20, 20),
            Size = new Size(450, 80),
            Font = new Font("Arial", 10),
            AutoSize = false
        };
        
        txtInput = new TextBox
        {
            Location = new Point(20, 110),
            Size = new Size(450, 30),
            Font = new Font("Arial", 10),
            Text = initialValue
        };
        
        btnOk = new Button
        {
            Text = "OK",
            Location = new Point(300, 160),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };
        
        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(390, 160),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };
        
        btnOk.Click += (s, e) => { InputValue = txtInput.Text; };
        
        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
        
        this.Controls.Add(lblPrompt);
        this.Controls.Add(txtInput);
        this.Controls.Add(btnOk);
        this.Controls.Add(btnCancel);
        
        // Focus and select text for easy editing
        this.Shown += (s, e) => { txtInput.Focus(); txtInput.SelectAll(); };
    }
}