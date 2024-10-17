using PolygonEditor.GUI.Models.Enums;

namespace PolygonEditor.GUI.Models;
public sealed class PolygonPosition
{
    public PolygonPosition(Polygon polygon)
    {
        Position = polygon.Edges
            .Select(edge => (edge.Start.Point, edge?.FirstControlVertex?.Point, edge?.SecondControlVertex?.Point))
            .ToList();
        EdgeConstraints = polygon.Edges
            .Select(edge => edge.ConstraintType)
            .ToList();
        VertexConstraints = polygon.Vertices
            .Select(vertex => vertex.ConstraintType)
            .ToList();
    }

    public PolygonPosition()
    {

    }

    public List<(Point start, Point? firstControlVertex, Point? secondControlVertex)> Position { get; } = [];
    public List<EdgeConstraintType> EdgeConstraints { get; } = [];
    public List<VertexConstraintType> VertexConstraints { get; } = [];

    public void RestorePosition(Polygon polygon)
    {
        for (var i = 0; i < Position.Count; i++)
        {
            polygon.Edges[i].Start.Point = Position[i].start;
            polygon.Edges[i].Start.ConstraintType = VertexConstraints[i];
            polygon.Edges[i].ConstraintType = EdgeConstraints[i];

            if (polygon.Edges[i].FirstControlVertex != null && Position[i].firstControlVertex.HasValue)
            {
                polygon.Edges[i].FirstControlVertex!.Point = Position[i].firstControlVertex!.Value;
            }

            if (polygon.Edges[i].SecondControlVertex != null && Position[i].secondControlVertex.HasValue)
            {
                polygon.Edges[i].SecondControlVertex!.Point = Position[i].secondControlVertex!.Value;
            }
        }
    }

    public Polygon BuildPolygon()
    {
        var vertices = new List<Vertex>();
        var edges = new List<Edge>();

        var polygon = new Polygon(vertices, edges);

        Vertex? prevEnd = null;

        for (var i = 0; i < Position.Count; i++)
        {
            var start = i == 0
                ? new Vertex(Position[i].start) { ConstraintType = VertexConstraints[i] }
                : prevEnd;

            var end = i == Position.Count - 1
                ? vertices[0]
                : new Vertex(Position[(i + 1) % Position.Count].start) { ConstraintType = VertexConstraints[(i + 1) % Position.Count] };

            prevEnd = end;

            Edge edge;

            if (Position[i].firstControlVertex.HasValue && Position[i].secondControlVertex.HasValue)
            {
                var firstControlVertex = new ControlVertex(Position[i].firstControlVertex!.Value, polygon);
                var secondControlVertex = new ControlVertex(Position[i].secondControlVertex!.Value, polygon);

                edge = new Edge(start!, end, firstControlVertex: firstControlVertex, secondControlVertex: secondControlVertex)
                {
                    ConstraintType = EdgeConstraints[i]
                };
            }
            else
            {
                edge = new Edge(start!, end)
                {
                    ConstraintType = EdgeConstraints[i]
                };
            }

            if(edge.ConstraintType == EdgeConstraintType.FixedLength)
            {
                edge.FixedLength = edge.Length;
            }

            vertices.Add(start!);
            edges.Add(edge);
        }

        polygon.SetSelfAsParent();

        return polygon;
    }

    public static bool ValidatePolygonPosition(PolygonPosition polygonPosition)
    {
        var n = polygonPosition.Position.Count;

        if (n < 3) return false;

        return n == polygonPosition.VertexConstraints.Count &&
            n == polygonPosition.EdgeConstraints.Count;
    }
}