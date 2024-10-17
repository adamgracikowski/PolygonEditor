using Newtonsoft.Json;
using PolygonEditor.GUI.Algorithms;
using PolygonEditor.GUI.Drawing;
using PolygonEditor.GUI.Models;
using PolygonEditor.GUI.Models.Enums;
using PolygonEditor.GUI.Properties;
using System.Text;

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

        LoadDefaultPolygon();
    }

    private void PictureBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (EditorMode == EditorMode.CreatingPolygon && PolygonContainer.CachedVertices.Count > 0)
        {
            PolygonContainer.DrawPartialPolygon(e.Location, AlgorithmType);
        }
        else if (EditorMode == EditorMode.MovingPolygon)
        {
            PolygonContainer.MovePolygon(e.Location);
            PolygonContainer.DrawPolygon(AlgorithmType);
        }
        else if (EditorMode == EditorMode.MovingVertex)
        {
            if (!PolygonContainer.MoveSelectedVertexWithConstraints(e.Location))
            {

            }

            PolygonContainer.DrawPolygon(AlgorithmType);
        }
        else if (EditorMode == EditorMode.MovingControlVertex)
        {
            PolygonContainer.MoveSelectedControlVertexWithConstraints(e.Location);
            PolygonContainer.DrawPolygon(AlgorithmType);
        }
    }
    private void PictureBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (EditorMode == EditorMode.EditingPolygon && e.Button == MouseButtons.Left)
        {
            if (PolygonContainer.IsVertexHit(e.Location, out var vertex) && vertex != null)
            {
                EditorMode = EditorMode.MovingVertex;
                PolygonContainer.VertexMovingState.SelectedElement = vertex;
                PolygonContainer.VertexMovingState.HitPoint = e.Location;
            }
            if (PolygonContainer.IsControlVertexHit(e.Location, out var controlVertex) && controlVertex != null)
            {
                EditorMode = EditorMode.MovingControlVertex;
                PolygonContainer.ControlVertexMovingState.SelectedElement = controlVertex;
                PolygonContainer.ControlVertexMovingState.HitPoint = e.Location;
            }
            else if (PolygonContainer.IsPolygonHit(e.Location, out var polygon) && polygon != null)
            {
                EditorMode = EditorMode.MovingPolygon;
                PolygonContainer.PolygonMovingState.SelectedElement = polygon;
                PolygonContainer.PolygonMovingState.HitPoint = e.Location;
            }
        }
    }
    private void PictureBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (sender != PictureBox ||
            EditorMode == EditorMode.CreatingPolygon ||
            EditorMode == EditorMode.EditingPolygon)
        {
            return;
        }

        if (e.Button == MouseButtons.Left)
        {
            if (EditorMode == EditorMode.MovingPolygon)
            {
                PolygonContainer.PolygonMovingState.Clear();
            }
            else if (EditorMode == EditorMode.MovingVertex)
            {
                PolygonContainer.VertexMovingState.Clear();
            }
            else if (EditorMode == EditorMode.MovingControlVertex)
            {
                PolygonContainer.ControlVertexMovingState.Clear();
            }

            EditorMode = EditorMode.EditingPolygon;
        }
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
                return;
            }
            else if (PolygonContainer.IsEdgeHit(e.Location, out var edge) && edge != null)
            {
                CreateEdgeContextMenu(edge, e.Location);
                return;
            }
            else if (PolygonContainer.IsPolygonHit(e.Location, out var polygon) && polygon != null)
            {
                CreatePolygonContextMenu(polygon, e.Location);
                return;
            }
        }

        if (e.Button == MouseButtons.Right)
        {
            CreateGeneralContextMenu(e.Location);
        }
    }

    private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ClearWithMessage();
    }
    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CleanUp();
        Application.Exit();
    }
    private void CustomToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem && !CustomToolStripMenuItem.Checked)
        {
            CustomToolStripMenuItem.Checked = true;
            LibraryToolStripMenuItem.Checked = false;

            AlgorithmType = AlgorithmType.Custom;
            if (EditorMode == EditorMode.EditingPolygon)
            {
                PolygonContainer.DrawPolygon(AlgorithmType);
            }
        }
    }
    private void LibraryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem && !LibraryToolStripMenuItem.Checked)
        {
            LibraryToolStripMenuItem.Checked = true;
            CustomToolStripMenuItem.Checked = false;

            AlgorithmType = AlgorithmType.Library;
            if (EditorMode == EditorMode.EditingPolygon)
            {
                PolygonContainer.DrawPolygon(AlgorithmType);
            }
        }
    }
    private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender != LoadToolStripMenuItem) return;

        using var openFileDialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            Title = "Save polygon to JSON"
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var json = File.ReadAllText(openFileDialog.FileName);
                var polygonPosition = JsonConvert.DeserializeObject<PolygonPosition>(json)
                    ?? throw new Exception("Invalid format of the JSON file.");

                EditorMode = EditorMode.EditingPolygon;
                PolygonContainer.ClearContainer();
                PolygonContainer.Polygon = polygonPosition.BuildPolygon();
                PolygonContainer.DrawPolygon(AlgorithmType);

                MessageBox.Show(
                    "Polygon loaded successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
    private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender != SaveToolStripMenuItem ||
            PolygonContainer.Polygon == null ||
            EditorMode == EditorMode.CreatingPolygon)
        {
            return;
        }

        using var saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            Title = "Save polygon to JSON"
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var polygonPosition = new PolygonPosition(PolygonContainer.Polygon);
                var json = JsonConvert.SerializeObject(polygonPosition, Formatting.Indented);

                File.WriteAllText(saveFileDialog.FileName, json);

                MessageBox.Show(
                    "Polygon exported successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
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
                        PolygonContainer.DrawPolygon(AlgorithmType);
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

        if (!edge.IsBezier)
        {
            var addVertexItem = new ToolStripMenuItem("Add vertex");
            addVertexItem.Click += (s, e) =>
            {
                if (PolygonContainer.Polygon == null)
                    return;

                PolygonContainer.Polygon.AddVertex(edge);
                PolygonContainer.DrawPolygon(AlgorithmType);
            };

            DynamicContextMenu.Items.Add(addVertexItem);
        }

        var bezierItem = new ToolStripMenuItem("Bezier")
        {
            Checked = edge.IsBezier
        };

        bezierItem.Click += (s, e) =>
        {
            var startEdge = edge.Start.FirstEdge;
            var endEdge = edge.End.SecondEdge;

            if (startEdge == null || endEdge == null ||
               startEdge.IsBezier || endEdge.IsBezier)
            {
                MessageBox.Show(
                    "Two neighbouring edges can't be Bezier segments.",
                    "Invalid operation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            edge.ToggleBezier();
            PolygonContainer.DrawPolygon(AlgorithmType);
        };

        DynamicContextMenu.Items.Add(bezierItem);

        if (!edge.IsBezier)
        {
            var constraintItem = new ToolStripMenuItem("Constraints");
            foreach (EdgeConstraintType constraintType in Enum.GetValues(typeof(EdgeConstraintType)))
            {
                var constraintTypeItem = new ToolStripMenuItem(constraintType.ToString())
                {
                    Checked = constraintType == edge.ConstraintType
                };

                if (constraintType != edge.ConstraintType)
                {
                    constraintTypeItem.Click += (s, e) =>
                    {
                        if (constraintType == EdgeConstraintType.Horizontal &&
                           !edge.CanApplyHorizontalConstraint)
                        {
                            MessageBox.Show(
                                "Adjacent edge is already horizontal.",
                                "Invalid operation",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            return;
                        }
                        else if (constraintType == EdgeConstraintType.Vertical &&
                            !edge.CanApplyVerticalConstraint)
                        {
                            MessageBox.Show(
                                "Adjacent edge is already vertical.",
                                "Invalid operation",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            return;
                        }
                        else if (constraintType == EdgeConstraintType.FixedLength)
                        {
                            using var fixedLengthForm = new FixedLengthForm(edge.Length);

                            if (fixedLengthForm.ShowDialog() == DialogResult.OK && fixedLengthForm.SetConstraint)
                            {
                                var check = Math.Abs(fixedLengthForm.Length - edge.Length) > PolygonEditorConstants.Epsilon;
                                var copy = new PolygonPosition(PolygonContainer.Polygon);

                                edge.FixedLength = fixedLengthForm.Length;
                                edge.ConstraintType = EdgeConstraintType.FixedLength;

                                if (check)
                                {
                                    edge.End.Point = Geometry.OffsetPreserveLength(edge.End.Point, edge.Start.Point, edge.FixedLength);
                                    if (!Algorithm.RestoreConstraints(edge.End))
                                    {
                                        copy.RestorePosition(PolygonContainer.Polygon);

                                        MessageBox.Show(
                                            "This constraint can't be satisfied.",
                                            "Invalid operation",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information
                                        );
                                    }

                                }

                                PolygonContainer.DrawPolygon(AlgorithmType);
                            }
                            return;
                        }

                        edge.ConstraintType = constraintType;
                        PolygonContainer.DrawPolygon(AlgorithmType);
                    };
                }

                constraintItem.DropDownItems.Add(constraintTypeItem);
            }

            DynamicContextMenu.Items.Add(constraintItem);
        }

        DynamicContextMenu.Show(PictureBox, point);
    }
    private void CreatePolygonContextMenu(Polygon polygon, Point point)
    {
        DynamicContextMenu.Items.Clear();
        var deleteItem = new ToolStripMenuItem("Delete polygon");
        deleteItem.Click += (s, e) =>
        {
            if (PolygonContainer.Polygon == null)
                return;

            PolygonContainer.ClearContainer();
        };

        DynamicContextMenu.Items.Add(deleteItem);
        DynamicContextMenu.Show(PictureBox, point);
    }
    private void CreateGeneralContextMenu(Point point)
    {
        DynamicContextMenu.Items.Clear();
        var createPolygonItem = new ToolStripMenuItem("Create polygon")
        {
            Image = Resources.create
        };
        createPolygonItem.Click += (s, e) =>
        {
            PolygonContainer.ClearContainer();
            EditorMode = EditorMode.CreatingPolygon;
        };

        DynamicContextMenu.Items.Add(createPolygonItem);
        DynamicContextMenu.Show(PictureBox, point);
    }

    private void PolygonEditorForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && EditorMode == EditorMode.CreatingPolygon)
        {
            PolygonContainer.ClearContainer();
        }
    }
    private void PolygonEditorForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        CleanUp();
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
    private void CleanUp()
    {
        PolygonContainer.Dispose();
        DrawingStyles.Dispose();
    }
    private bool LoadDefaultPolygon()
    {
        try
        {
            var json = Encoding.UTF8.GetString(Resources.polygon);
            var polygonPosition = JsonConvert.DeserializeObject<PolygonPosition>(json)
                ?? throw new Exception("Invalid format of the JSON file.");

            PolygonContainer.Polygon = polygonPosition.BuildPolygon();
            PolygonContainer.DrawPolygon(AlgorithmType);

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            return false;
        }
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var about = Resources.about;

        MessageBox.Show(
            about,
            "About Polygon Editor",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void UserGuideToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var userguide = Resources.userguide;

        MessageBox.Show(
            userguide,
            "User Guide",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }
}