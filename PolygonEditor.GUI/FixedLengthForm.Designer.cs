namespace PolygonEditor.GUI;

partial class FixedLengthForm
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
        LayoutPanel = new TableLayoutPanel();
        LengthLabel = new Label();
        LengthTextBox = new TextBox();
        CheckButton = new Button();
        LayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // LayoutPanel
        // 
        LayoutPanel.ColumnCount = 3;
        LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
        LayoutPanel.Controls.Add(LengthLabel, 0, 0);
        LayoutPanel.Controls.Add(LengthTextBox, 1, 0);
        LayoutPanel.Controls.Add(CheckButton, 2, 0);
        LayoutPanel.Dock = DockStyle.Fill;
        LayoutPanel.Location = new Point(0, 0);
        LayoutPanel.Name = "LayoutPanel";
        LayoutPanel.RowCount = 1;
        LayoutPanel.RowStyles.Add(new RowStyle());
        LayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        LayoutPanel.Size = new Size(351, 51);
        LayoutPanel.TabIndex = 0;
        // 
        // LengthLabel
        // 
        LengthLabel.AutoSize = true;
        LengthLabel.Dock = DockStyle.Fill;
        LengthLabel.Location = new Point(3, 0);
        LengthLabel.Name = "LengthLabel";
        LengthLabel.Size = new Size(94, 51);
        LengthLabel.TabIndex = 0;
        LengthLabel.Text = "Length:";
        LengthLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LengthTextBox
        // 
        LengthTextBox.Dock = DockStyle.Fill;
        LengthTextBox.Location = new Point(103, 12);
        LengthTextBox.Margin = new Padding(3, 12, 3, 3);
        LengthTextBox.Name = "LengthTextBox";
        LengthTextBox.Size = new Size(194, 27);
        LengthTextBox.TabIndex = 1;
        LengthTextBox.TextChanged += LengthTextBox_TextChanged;
        // 
        // CheckButton
        // 
        CheckButton.Image = Properties.Resources.ruler;
        CheckButton.Location = new Point(303, 12);
        CheckButton.Margin = new Padding(3, 12, 3, 3);
        CheckButton.Name = "CheckButton";
        CheckButton.Size = new Size(45, 29);
        CheckButton.TabIndex = 2;
        CheckButton.UseVisualStyleBackColor = true;
        CheckButton.Click += CheckButton_Click;
        // 
        // FixedLengthForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(351, 51);
        Controls.Add(LayoutPanel);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FixedLengthForm";
        Text = "FixedLengthForm";
        FormClosing += FixedLengthForm_FormClosing;
        LayoutPanel.ResumeLayout(false);
        LayoutPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel LayoutPanel;
    private Label LengthLabel;
    private TextBox LengthTextBox;
    private Button CheckButton;
}