namespace PolygonEditor.GUI.Models;

public sealed class ControlVertex : VertexBase
{
    public Edge? Edge { get; set; }

    public ControlVertex(Point point, Polygon? parent = null) 
        : base(point, parent)
    {
    }
}