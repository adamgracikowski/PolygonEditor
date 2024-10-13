using PolygonEditor.GUI.Models.Interfaces;
using System.Drawing.Drawing2D;

namespace PolygonEditor.GUI.Models;

public sealed class Polygon : ISelectable
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
        var vertex = new Vertex(new Point(x, y), this);

        Vertices.Insert(edgeIndex + 1, vertex);

        var firstEdge = new Edge(edge.Start, vertex, this);
        var secondEdge = new Edge(vertex, edge.End, this);

        Edges.Insert(edgeIndex, firstEdge);
        Edges.Insert(edgeIndex + 1, secondEdge);
    }
    public bool IsWithinSelection(int x, int y)
    {
        using var graphicsPath = new GraphicsPath();
        graphicsPath.AddPolygon(Vertices.Select(v => v.Point).ToArray());
        return graphicsPath.IsVisible(x, y);
    }
}
