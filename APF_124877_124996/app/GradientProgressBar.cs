using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class GradientProgressBar : ProgressBar
{
    private Color startColor = Color.DarkRed;
    private Color endColor = Color.Red;
    private int borderRadius = 10;

    public GradientProgressBar()
    {
        this.SetStyle(ControlStyles.UserPaint, true);
    }

    [Category("Appearance")]
    [Description("Cor inicial do degradê.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] 
    [TypeConverter(typeof(System.Drawing.ColorConverter))]

    public Color StartColor
    {
        get => startColor;
        set { startColor = value; this.Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Cor final do degradê.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [TypeConverter(typeof(System.Drawing.ColorConverter))]
    public Color EndColor
    {
        get => endColor;
        set { endColor = value; this.Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Raio dos cantos arredondados.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int BorderRadius
    {
        get => borderRadius;
        set { borderRadius = value; this.Invalidate(); }
    }

    public bool ShouldSerializeStartColor() => startColor != Color.DarkRed;
    public void ResetStartColor() => StartColor = Color.DarkRed;

    public bool ShouldSerializeEndColor() => endColor != Color.Red;
    public void ResetEndColor() => EndColor = Color.Red;

    public bool ShouldSerializeBorderRadius() => borderRadius != 10;
    public void ResetBorderRadius() => BorderRadius = 10;

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle rect = this.ClientRectangle;

        using (GraphicsPath backgroundPath = GetRoundRectPath(rect, borderRadius))

        using (SolidBrush backgroundBrush = new SolidBrush(this.BackColor)) 
        {
            e.Graphics.FillPath(backgroundBrush, backgroundPath);
        }

        rect.Width = (int)(rect.Width * ((double)this.Value / this.Maximum));
        if (rect.Width > 0)
        {
        Rectangle fillBounds = this.ClientRectangle; 
    
        using (GraphicsPath fillPath = GetRoundRectPath(rect, borderRadius))
        using (LinearGradientBrush brush = new LinearGradientBrush(
            fillBounds, StartColor, EndColor, LinearGradientMode.Horizontal)) 
        {
            e.Graphics.FillPath(brush, fillPath);
        }
    }

    }

    private GraphicsPath GetRoundRectPath(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}