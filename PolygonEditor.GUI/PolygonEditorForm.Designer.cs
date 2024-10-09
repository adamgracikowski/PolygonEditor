namespace PolygonEditor.GUI;

partial class PolygonEditorForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(PolygonEditorForm));
        UpperMenuStrip = new MenuStrip();
        FileToolStripMenuItem = new ToolStripMenuItem();
        LoadToolStripMenuItem = new ToolStripMenuItem();
        SaveToolStripMenuItem = new ToolStripMenuItem();
        ClearToolStripMenuItem = new ToolStripMenuItem();
        ExitToolStripMenuItem = new ToolStripMenuItem();
        HelpToolStripMenuItem = new ToolStripMenuItem();
        AboutToolStripMenuItem = new ToolStripMenuItem();
        UserGuideToolStripMenuItem = new ToolStripMenuItem();
        PictureBox = new PictureBox();
        UpperMenuStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)PictureBox).BeginInit();
        SuspendLayout();
        // 
        // UpperMenuStrip
        // 
        UpperMenuStrip.ImageScalingSize = new Size(20, 20);
        UpperMenuStrip.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem, HelpToolStripMenuItem });
        UpperMenuStrip.Location = new Point(0, 0);
        UpperMenuStrip.Name = "UpperMenuStrip";
        UpperMenuStrip.Size = new Size(782, 28);
        UpperMenuStrip.TabIndex = 0;
        UpperMenuStrip.Text = "menuStrip1";
        // 
        // FileToolStripMenuItem
        // 
        FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { LoadToolStripMenuItem, SaveToolStripMenuItem, ClearToolStripMenuItem, ExitToolStripMenuItem });
        FileToolStripMenuItem.Name = "FileToolStripMenuItem";
        FileToolStripMenuItem.Size = new Size(46, 24);
        FileToolStripMenuItem.Text = "&File";
        // 
        // LoadToolStripMenuItem
        // 
        LoadToolStripMenuItem.Image = (Image)resources.GetObject("LoadToolStripMenuItem.Image");
        LoadToolStripMenuItem.Name = "LoadToolStripMenuItem";
        LoadToolStripMenuItem.Size = new Size(224, 26);
        LoadToolStripMenuItem.Text = "&Load";
        // 
        // SaveToolStripMenuItem
        // 
        SaveToolStripMenuItem.Image = (Image)resources.GetObject("SaveToolStripMenuItem.Image");
        SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
        SaveToolStripMenuItem.Size = new Size(224, 26);
        SaveToolStripMenuItem.Text = "&Save";
        // 
        // ClearToolStripMenuItem
        // 
        ClearToolStripMenuItem.Image = (Image)resources.GetObject("ClearToolStripMenuItem.Image");
        ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
        ClearToolStripMenuItem.Size = new Size(224, 26);
        ClearToolStripMenuItem.Text = "&Clear";
        // 
        // ExitToolStripMenuItem
        // 
        ExitToolStripMenuItem.Image = (Image)resources.GetObject("ExitToolStripMenuItem.Image");
        ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
        ExitToolStripMenuItem.Size = new Size(224, 26);
        ExitToolStripMenuItem.Text = "&Exit";
        // 
        // HelpToolStripMenuItem
        // 
        HelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { AboutToolStripMenuItem, UserGuideToolStripMenuItem });
        HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
        HelpToolStripMenuItem.Size = new Size(55, 24);
        HelpToolStripMenuItem.Text = "&Help";
        // 
        // AboutToolStripMenuItem
        // 
        AboutToolStripMenuItem.Image = (Image)resources.GetObject("AboutToolStripMenuItem.Image");
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
        AboutToolStripMenuItem.Size = new Size(224, 26);
        AboutToolStripMenuItem.Text = "&About";
        // 
        // UserGuideToolStripMenuItem
        // 
        UserGuideToolStripMenuItem.Image = (Image)resources.GetObject("UserGuideToolStripMenuItem.Image");
        UserGuideToolStripMenuItem.Name = "UserGuideToolStripMenuItem";
        UserGuideToolStripMenuItem.Size = new Size(224, 26);
        UserGuideToolStripMenuItem.Text = "&User Guide";
        // 
        // PictureBox
        // 
        PictureBox.Dock = DockStyle.Fill;
        PictureBox.Location = new Point(0, 28);
        PictureBox.Name = "PictureBox";
        PictureBox.Size = new Size(782, 525);
        PictureBox.TabIndex = 1;
        PictureBox.TabStop = false;
        // 
        // PolygonEditorForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(782, 553);
        Controls.Add(PictureBox);
        Controls.Add(UpperMenuStrip);
        MainMenuStrip = UpperMenuStrip;
        Name = "PolygonEditorForm";
        Text = "Polygon Editor";
        UpperMenuStrip.ResumeLayout(false);
        UpperMenuStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)PictureBox).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip UpperMenuStrip;
    private ToolStripMenuItem FileToolStripMenuItem;
    private ToolStripMenuItem LoadToolStripMenuItem;
    private ToolStripMenuItem SaveToolStripMenuItem;
    private ToolStripMenuItem ClearToolStripMenuItem;
    private ToolStripMenuItem ExitToolStripMenuItem;
    private ToolStripMenuItem HelpToolStripMenuItem;
    private ToolStripMenuItem AboutToolStripMenuItem;
    private ToolStripMenuItem UserGuideToolStripMenuItem;
    private PictureBox PictureBox;
}