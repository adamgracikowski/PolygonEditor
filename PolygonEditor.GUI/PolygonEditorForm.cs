using PolygonEditor.GUI.Models;
using PolygonEditor.GUI.Models.Enums;

namespace PolygonEditor.GUI;
public partial class PolygonEditorForm : Form
{
    private ContextMenuStrip DynamicContextMenu { get; set; } = new();
    public EditorMode EditorMode { get; set; } = EditorMode.EditingPolygon;
    public AlgorithmType AlgorithmType { get; set; } = AlgorithmType.Library;
    public PolygonContainer PolygonContainer { get; set; }
    public PolygonEditorForm()
    {
        InitializeComponent();
        PolygonContainer = new(PictureBox);
    }

    private void CreatePolygonToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (EditorMode == EditorMode.CreatingPolygon)
            return;

        if (!ClearWithMessage()) return;

        EditorMode = EditorMode.CreatingPolygon;
    }

    private void PictureBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (EditorMode == EditorMode.CreatingPolygon && PolygonContainer.CachedVertices.Count > 0)
        {
            PolygonContainer.DrawPartialPolygon(e.Location, AlgorithmType);
        }
    }

    private void PictureBox_MouseDown(object sender, MouseEventArgs e)
    {

    }

    private void PictureBox_MouseClick(object sender, MouseEventArgs e)
    {
        if (EditorMode == EditorMode.CreatingPolygon && e.Button == MouseButtons.Left)
        {
            if (!PolygonContainer.AddVertexToPartialPolygon(e.Location, AlgorithmType))
                return;

            EditorMode = EditorMode.EditingPolygon;
            return;
        }

        if (EditorMode == EditorMode.EditingPolygon && e.Button == MouseButtons.Right)
        {
            if (PolygonContainer.IsVertexHit(e.Location, out var vertex) && vertex != null)
            {
                CreateVertexContextMenu(vertex, e.Location);
            }
            else if (PolygonContainer.IsEdgeHit(e.Location, out var edge) && edge != null)
            {
                CreateEdgeContextMenu(edge, e.Location);
            }
        }
    }
    private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ClearWithMessage();
    }

    private bool ClearWithMessage()
    {
        var result = MessageBox.Show(
            "This operation will override your current board.",
            "Would you like to continue?",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.No)
            return false;

        PolygonContainer.ClearContainer();

        return true;
    }

    private void CreateVertexContextMenu(Vertex vertex, Point point)
    {
        DynamicContextMenu.Items.Clear();
        var deleteItem = new ToolStripMenuItem("Delete vertex");
        deleteItem.Click += (s, e) =>
        {
            if (PolygonContainer.Polygon == null)
                return;

            if (PolygonContainer.Polygon.DeleteVertex(vertex))
            {
                PolygonContainer.DrawPolygon(AlgorithmType);
                return;
            }

            MessageBox.Show(
                "Polygon must have at least 3 vertices.",
                "Invalid operation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation
            );
        };

        DynamicContextMenu.Items.Add(deleteItem);

        if (vertex.CanApplyConstraint)
        {
            var constraintItem = new ToolStripMenuItem("Constraints");
            foreach (VertexConstraintType constraintType in Enum.GetValues(typeof(VertexConstraintType)))
            {
                var constraintTypeItem = new ToolStripMenuItem(constraintType.ToString())
                {
                    Checked = constraintType == vertex.ConstraintType
                };

                if (constraintType != vertex.ConstraintType)
                {
                    constraintTypeItem.Click += (s, e) =>
                    {
                        vertex.ApplyVertexConstraint(constraintType);
                    };
                }

                constraintItem.DropDownItems.Add(constraintTypeItem);
            }

            DynamicContextMenu.Items.Add(constraintItem);
        }

        DynamicContextMenu.Show(PictureBox, vertex.Point);
    }

    private void CreateEdgeContextMenu(Edge edge, Point point)
    {
        DynamicContextMenu.Items.Clear();

        var addVertexItem = new ToolStripMenuItem("Add vertex");
        addVertexItem.Click += (s, e) =>
        {
            if (PolygonContainer.Polygon == null)
                return;

            PolygonContainer.Polygon.AddVertex(edge);
            PolygonContainer.DrawPolygon(AlgorithmType);
        };

        DynamicContextMenu.Items.Add(addVertexItem);

        var bezierItem = new ToolStripMenuItem("Bezier")
        {
            Checked = edge.IsBezier
        };

        bezierItem.Click += (s, e) =>
        {
            edge.ToggleBezier();
            PolygonContainer.DrawPolygon(AlgorithmType);
        };

        DynamicContextMenu.Items.Add(bezierItem);

        DynamicContextMenu.Show(PictureBox, point);
    }
}
