using PolygonEditor.GUI.Models.Enums;

namespace PolygonEditor.GUI.Models;

public sealed class Vertex : VertexBase
{
    public Edge? FirstEdge { get; set; }
    public Edge? SecondEdge { get; set; }
    public VertexConstraintType ConstraintType { get; set; } = VertexConstraintType.None;

    public Vertex(Point point = default, Polygon? parent = null)
        : base(point, parent)
    {
    }

    public bool CanApplyConstraint
    {
        get
        {
            return FirstEdge != null && SecondEdge != null
                && (FirstEdge.IsBezier || SecondEdge.IsBezier);
        }
    }

    public void ApplyVertexConstraint(VertexConstraintType constraintType)
    {
        ConstraintType = constraintType;
    }
    public IEnumerable<Edge?> Edges()
    {
        yield return FirstEdge;
        yield return SecondEdge;
    }
}
