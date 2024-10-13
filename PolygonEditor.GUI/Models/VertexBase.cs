using PolygonEditor.GUI.Algorithms;
using PolygonEditor.GUI.Models.Interfaces;

namespace PolygonEditor.GUI.Models;

public abstract class VertexBase : ISelectable
{
    protected VertexBase(Point point, Polygon? parent = null)
    {
        Point = point;
        Parent = parent;
    }

    public virtual int X
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
    public virtual int Y
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
    public Point Point { get; set; }
    public Polygon? Parent { get; set; }

    public bool IsWithinSelection(int x, int y)
    {
        var r = PolygonEditorConstants.HitRadius;
        return Geometry.IsNear(X, Y, x, y, r);
    }
    public void MoveWithoutConstraint(int dx, int dy)
    {
        Point = new Point(Point.X + dx, Point.Y + dy);
    }
}