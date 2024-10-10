namespace PolygonEditor.GUI.Models;

public sealed class Polygon
{
    public Polygon(List<Vertex> vertices, List<Edge> edges)
    {
        Vertices = vertices;
        Edges = edges;

        Vertices.ForEach(v => v.Parent = this);
        Edges.ForEach(e => e.Parent = this);
    }

    public List<Vertex> Vertices { get; set; } = [];
    public List<Edge> Edges { get; set; } = [];
    public int VertexCount => Vertices.Count;
    public int EdgeCount => Edges.Count;

    public bool DeleteVertex(Vertex vertex)
    {
        if (vertex == null ||
           Vertices.Count <= 3)
        {
            return false;
        }

        var vertexIndex = Vertices.IndexOf(vertex);

        if (vertexIndex < 0 ||
            vertex.FirstEdge == null ||
            vertex.SecondEdge == null)
        {
            return false;
        }

        var edgeIndex = vertexIndex - 1 >= 0 ? vertexIndex - 1 : Edges.Count - 2;
        var edge = new Edge(vertex.FirstEdge.Start, vertex.SecondEdge.End, this);

        Vertices.RemoveAt(vertexIndex);
        Edges.Remove(vertex.FirstEdge);
        Edges.Remove(vertex.SecondEdge);
        Edges.Insert(edgeIndex, edge);

        return true;
    }

    public void AddVertex(Edge edge)
    {
        if (edge == null ||
           edge.IsBezier ||
           Edges.Count == 0)
        {
            return;
        }

        var edgeIndex = Edges.IndexOf(edge);

        if (edgeIndex < 0)
        {
            return;
        }

        Edges.RemoveAt(edgeIndex);

        var x = (edge.Start.X + edge.End.X) / 2;
        var y = (edge.Start.Y + edge.End.Y) / 2;
        var vertex = new Vertex(x, y, parent: this);

        Vertices.Insert(edgeIndex + 1, vertex);

        var firstEdge = new Edge(edge.Start, vertex, this);
        var secondEdge = new Edge(vertex, edge.End, this);

        Edges.Insert(edgeIndex, firstEdge);
        Edges.Insert(edgeIndex + 1, secondEdge);
    }
    public void Move(double x, double y)
    {
        Vertices.ForEach(vertex =>
        {
            vertex.X += x;
            vertex.Y += y;
        });
    }
}

public interface IVertex
{
    double X { get; set; }
    double Y { get; set; }
    Polygon? Parent { get; set; }
    
    bool IsWithinHitArea(double x, double y)
    {
        var r = GraphicsConstants.HitDistance;
        return new Rectangle((int)(X - r), (int)(Y - r), (int)(2 * r), (int)(2 * r))
            .Contains((int)x, (int)y);
    }
}

public abstract class VertexBase
{
}

public sealed class Vertex : VertexBase, IVertex
{
    private double _x;
    private double _y;

    public Vertex(double x, double y, 
        Edge? firstEdge = null, Edge? secondEdge = null, 
        Polygon? parent = null)
    {
        _x = x;
        _y = y;
        Parent = parent;
        FirstEdge = firstEdge;
        SecondEdge = secondEdge;
    }

    public double X
    {
        get
        {
            return _x;
        }

        set
        {
            _x = value;
        }
    }

    public double Y
    {
        get
        {
            return _y;
        }

        set
        {
            _y = value;
        }
    }

    public Polygon? Parent { get; set; }
    public Edge? FirstEdge { get; set; }
    public Edge? SecondEdge { get; set; }
}

public sealed class ControlVertex : VertexBase, IVertex
{
    private double _x;
    private double _y;

    public double X
    {
        get
        {
            return _x;
        }

        set
        {
            _x = value;
        }
    }
    public double Y
    {
        get
        {
            return _y;
        }

        set
        {
            _y = value;
        }
    }
    public Polygon? Parent { get; set; }
    public Edge? Edge { get; set; }
}

public sealed class Edge
{
    public Edge(Vertex start, Vertex end, Polygon? parent = null,
        ControlVertex? firstControlVertex = null, ControlVertex? secondControlVertex = null)
    {
        Start = start;
        End = end;
        Parent = parent;
        FirstControlVertex = firstControlVertex;
        SecondControlVertex = secondControlVertex;
        IsBezier = firstControlVertex != null && secondControlVertex != null;

        // set references

        Start.SecondEdge = this;
        End.FirstEdge = this;

        if (FirstControlVertex != null)
            FirstControlVertex.Edge = this;

        if (SecondControlVertex != null)
            SecondControlVertex.Edge = this;
    }

    public Vertex Start { get; set; }
    public Vertex End { get; set; }
    public Polygon? Parent { get; set; }
    public ControlVertex? FirstControlVertex { get; set; }
    public ControlVertex? SecondControlVertex { get; set; }
    public bool IsBezier { get; set; }
}

public static class GraphicsExtensions
{
    public static void DrawVertex(this Graphics g, Vertex vertex, SolidBrush brush)
    {
        var radius = GraphicsConstants.HitDistance;
        g.FillEllipse(brush,
            (int)(vertex.X - radius),
            (int)(vertex.Y - radius),
            (int)(2 * radius),
            (int)(2 * radius)
        );
    }
}

public static class GraphicsConstants
{
    public const double HitDistance = 4.0;
    public const int EdgeDefaultThickness = 1;

    public static readonly SolidBrush VertexDefaultBrush = new(Color.Black);
    public static readonly SolidBrush ControlVertexDefaultBrush = new(Color.Black);

    public static void Dispose()
    {
        VertexDefaultBrush.Dispose();
        ControlVertexDefaultBrush.Dispose();
    }
}