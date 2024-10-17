namespace PolygonEditor.GUI;
public partial class FixedLengthForm : Form
{
    public float Length { get; set; }
    public bool SetConstraint { get; set; }
    public FixedLengthForm(float length)
    {
        InitializeComponent();
        Length = length;
        LengthTextBox.Text = length.ToString();
    }

    private void CheckButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        SetConstraint = true;
        Close();
    }

    private void LengthTextBox_TextChanged(object sender, EventArgs e)
    {
        if (sender != LengthTextBox) return;

        if (!float.TryParse(LengthTextBox.Text, out var length) || length <= 0)
        {
            CheckButton.Enabled = false;
            return;
        }

        Length = length;
        CheckButton.Enabled = true;
    }

    private void FixedLengthForm_FormClosing(object sender, FormClosingEventArgs e)
    {

    }
}
