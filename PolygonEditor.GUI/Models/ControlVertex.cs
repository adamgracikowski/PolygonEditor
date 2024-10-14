using Newtonsoft.Json;

namespace PolygonEditor.GUI.Models;

public sealed class ControlVertex : VertexBase
{
    public ControlVertex(Point point, Polygon? parent = null) 
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

    public Edge? Edge { get; set; }

    public ControlVertex DeepCopy()
    {
        return new ControlVertex(Point);
    }
}
