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

    private List<(Point start, Point? firstControlVertex, Point? secondControlVertex)> Position { get; }
    private List<EdgeConstraintType> EdgeConstraints { get; }
    private List<VertexConstraintType> VertexConstraints { get; }

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
}