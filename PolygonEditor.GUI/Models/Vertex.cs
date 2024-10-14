using PolygonEditor.GUI.Models.Enums;

namespace PolygonEditor.GUI.Models;

public sealed class Vertex : VertexBase
{
    public Vertex(Point point, Polygon? parent = null)
        : base(point, parent)
    {
    }

    public override int X
    {
        get
        {
            return Point.X;
        }

        set
        {
            Point = new Point(value, Point.Y);

        }
    }

    public override int Y
    {
        get
        {
            return Point.Y;
        }

        set
        {
            Point = new Point(Point.X, value);

        }
    }

    public Edge? FirstEdge { get; set; }
    public Edge? SecondEdge { get; set; }
    public VertexConstraintType ConstraintType { get; set; } = VertexConstraintType.None;

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
        // TODO: add logic
    }

    public Vertex DeepCopy()
    {
        return new Vertex(Point);
    }
}
