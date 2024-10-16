using PolygonEditor.GUI.Models;
using PolygonEditor.GUI.Models.Enums;

namespace PolygonEditor.GUI.Algorithms;

public static class Algorithm
{
    public static IEnumerable<Point> BresenhamPoints(int x0, int y0, int x1, int y1)
    {
        var m = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (m)
        {
            (x0, y0) = (y0, x0);
            (x1, y1) = (y1, x1);
        }
        if (x0 > x1)
        {
            (x0, x1) = (x1, x0);
            (y0, y1) = (y1, y0);
        }

        var dx = x1 - x0;
        var dy = Math.Abs(y1 - y0);

        var d = 2 * dy - dx;
        var de = 2 * dy;
        var dne = 2 * (dy - dx);

        var ystep = y0 < y1 ? 1 : -1;

        var y = y0;
        for (var x = x0; x <= x1; x++)
        {
            yield return new Point(m ? y : x, m ? x : y);

            if (d < 0)
            {
                d += de;
            }
            else
            {
                y += ystep;
                d += dne;
            }
        }

        yield break;
    }
    public static bool MoveVertexWithConstraints(Vertex vertex, int x, int y, ref Polygon? polygon)
    {
        if (vertex.Parent == null) return false;

        var copy = new PolygonPosition(vertex.Parent);

        vertex.X = x;
        vertex.Y = y;

        if (!RestoreConstraints(vertex))
        {
            copy.RestorePosition(vertex.Parent);
            return false;
        }

        return true;
    }
    public static bool MoveControlVertexWithConstraints(ControlVertex controlVertex, int x, int y, ref Polygon? polygon)
    {
        if (controlVertex.Parent == null) return false;

        var (vertex, _, otherVertex, edge) = controlVertex.GetControlVertexConfiguration();

        if (vertex == null || otherVertex == null || edge == null) return false;

        var copy = new PolygonPosition(controlVertex.Parent);

        if (vertex.ConstraintType == VertexConstraintType.G0 ||
            vertex.ConstraintType == VertexConstraintType.None)
        {
            controlVertex.X = x;
            controlVertex.Y = y;
            return true;
        }
        else if (vertex.ConstraintType == VertexConstraintType.G1)
        {
            if (edge.ConstraintType == EdgeConstraintType.Vertical ||
               edge.ConstraintType == EdgeConstraintType.Horizontal)
            {
                controlVertex.Point = Geometry.ProjectPointOnLine(new Point(x, y), otherVertex.Point, vertex.Point);
                return true;
            }
            else
            {
                controlVertex.X = x;
                controlVertex.Y = y;
                otherVertex.Point = Geometry.PreserveG1(vertex.Point, otherVertex.Point, controlVertex.Point);

                if (!RestoreLoop(otherVertex, v => edge == otherVertex.FirstEdge ? v.SecondEdge : v.FirstEdge))
                {
                    copy.RestorePosition(controlVertex.Parent);
                    return false;
                }

                return true;
            }
        }
        else if (vertex.ConstraintType == VertexConstraintType.C1)
        {
            if (edge.ConstraintType == EdgeConstraintType.Vertical ||
                edge.ConstraintType == EdgeConstraintType.Horizontal)
            {
                controlVertex.Point = Geometry.ProjectPointOnLine(new Point(x, y), otherVertex.Point, vertex.Point);
                otherVertex.Point = Geometry.PreserveC1(vertex.Point, controlVertex.Point, k: 1 / 3.0f);

                if (!RestoreLoop(otherVertex, v => edge == otherVertex.FirstEdge ? v.SecondEdge : v.FirstEdge))
                {
                    copy.RestorePosition(controlVertex.Parent);
                    return false;
                }

                return true;
            }
            else if (edge.ConstraintType == EdgeConstraintType.None)
            {
                controlVertex.X = x;
                controlVertex.Y = y;
                otherVertex.Point = Geometry.PreserveC1(vertex.Point, controlVertex.Point, k: 1 / 3.0f);

                if (!RestoreLoop(otherVertex, v => edge == otherVertex.FirstEdge ? v.SecondEdge : v.FirstEdge))
                {
                    copy.RestorePosition(controlVertex.Parent);
                    return false;
                }

                return true;
            }
            else if (edge.ConstraintType == EdgeConstraintType.FixedLength)
            {
                controlVertex.Point = Geometry.CircleLineIntersection(new Point(x, y), vertex.Point, edge.FixedLength / 3.0f);
                otherVertex.Point = Geometry.PreserveC1(vertex.Point, controlVertex.Point, k: 1 / 3.0f);

                if (!RestoreLoop(otherVertex, v => edge == otherVertex.FirstEdge ? v.SecondEdge : v.FirstEdge))
                {
                    copy.RestorePosition(controlVertex.Parent);
                    return false;
                }

                return true;
            }
        }

        return true;
    }
    private static bool RestoreLoop(Vertex vertex, Func<Vertex, Edge?> edgeDirection)
    {
        Vertex current = vertex;

        while (true)
        {
            var edge = edgeDirection(current);

            if (edge == null) return false;

            var other = edge.OtherVertex(current);

            if (edge.IsBezier)
            {
                if (current.ConstraintType != VertexConstraintType.None &&
                    current.ConstraintType != VertexConstraintType.G0 &&
                    !current.CheckConstraint())
                {
                    current.PreserveConstraint();
                }

                break;
            }
            else // !edge.IsBezier
            {
                if (!edge.CheckConstraint())
                {
                    if (other == vertex) return false;

                    edge.PreserveConstraint(current);
                    current = other;
                }
                else if (edge.IsBezierNeighbour(other))
                {
                    current = other;
                }
                else
                {
                    break;
                }
            }
        }

        return true;
    }
    public static bool RestoreConstraints(Vertex vertex)
    {
        if (!RestoreLoop(vertex, v => v.FirstEdge))
        {
            return false;
        }

        if (!RestoreLoop(vertex, v => v.SecondEdge))
        {
            return false;
        }

        return true; // temporary solution
    }

    public static bool CheckConstraint(this Edge edge)
    {
        return edge.ConstraintType switch
        {
            EdgeConstraintType.Horizontal => edge.CheckHorizontal(),
            EdgeConstraintType.Vertical => edge.CheckVertical(),
            EdgeConstraintType.FixedLength => edge.CheckFixedLength(),
            _ => true
        };
    }
    public static bool CheckHorizontal(this Edge edge)
    {
        return !edge.IsBezier && edge.Start.Y == edge.End.Y;
    }
    public static bool CheckVertical(this Edge edge)
    {
        return !edge.IsBezier && edge.Start.X == edge.End.X;
    }
    public static bool CheckFixedLength(this Edge edge)
    {
        return Math.Abs(edge.FixedLength - edge.Length) < PolygonEditorConstants.Epsilon;
    }
    public static void PreserveConstraint(this Edge edge, Vertex current)
    {
        switch (edge.ConstraintType)
        {
            case EdgeConstraintType.Horizontal:
                edge.PreserveHorizontal(current);
                break;
            case EdgeConstraintType.Vertical:
                edge.PreserveVertical(current);
                break;
            case EdgeConstraintType.FixedLength:
                edge.PreserveFixedLength(current);
                break;
        };
    }
    public static void PreserveHorizontal(this Edge edge, Vertex current)
    {
        var other = edge.OtherVertex(current);

        if (other == null) return;

        other.Y = current.Y;
    }
    public static void PreserveVertical(this Edge edge, Vertex current)
    {
        var other = edge.OtherVertex(current);

        if (other == null) return;

        other.X = current.X;
    }
    public static void PreserveFixedLength(this Edge edge, Vertex current)
    {
        var other = edge.OtherVertex(current);

        if (other == null) return;

        other.Point = Geometry
            .CircleLineIntersection(other.Point, current.Point, edge.FixedLength);
    }

    public static bool CheckG1(this Vertex vertex)
    {
        var (_, controlVertex, otherVertex) = vertex.GetVertexConfiguration();

        if (controlVertex == null || otherVertex == null) return true;

        return Geometry.CheckG1(vertex.Point, controlVertex.Point, otherVertex.Point);
    }
    public static bool CheckC1(this Vertex vertex)
    {
        var (_, controlVertex, otherVertex) = vertex.GetVertexConfiguration();

        if (controlVertex == null || otherVertex == null) return true;

        return Geometry.CheckC1(vertex.Point, controlVertex.Point, otherVertex.Point);
    }
    public static void PreserveG1(this Vertex vertex)
    {
        var (_, controlVertex, otherVertex) = vertex.GetVertexConfiguration();

        if (controlVertex == null || otherVertex == null) return;

        controlVertex.Point = Geometry.PreserveG1(vertex.Point, controlVertex.Point, otherVertex.Point);
    }
    public static void PreserveC1(this Vertex vertex)
    {
        var (_, controlVertex, otherVertex) = vertex.GetVertexConfiguration();

        if (controlVertex == null || otherVertex == null) return;

        controlVertex.Point = Geometry.PreserveC1(vertex.Point, otherVertex.Point);
    }
    public static bool CheckConstraint(this Vertex vertex)
    {
        return vertex.ConstraintType switch
        {
            VertexConstraintType.G1 => vertex.CheckG1(),
            VertexConstraintType.C1 => vertex.CheckC1(),
            _ => true
        };
    }
    public static void PreserveConstraint(this Vertex vertex)
    {
        switch (vertex.ConstraintType)
        {
            case VertexConstraintType.G1:
                vertex.PreserveG1();
                break;
            case VertexConstraintType.C1:
                vertex.PreserveC1();
                break;
        }
    }
    private static (Vertex? vertex, ControlVertex? controlVertex, Vertex? otherVertex) GetVertexConfiguration(this Vertex vertex)
    {
        Vertex? otherVertex = null;
        ControlVertex? controlVertex = null;

        var edge = vertex.FirstEdge != null &&
            !vertex.FirstEdge.IsBezier
            ? vertex.FirstEdge
            : vertex.SecondEdge;

        var bezier = edge == vertex.FirstEdge
            ? vertex.SecondEdge
            : vertex.FirstEdge;

        if (edge == null || bezier == null)
            return (vertex, controlVertex, otherVertex);

        otherVertex = vertex == edge.Start
            ? edge.End
            : edge.Start;

        controlVertex = vertex == bezier.Start
            ? bezier.FirstControlVertex
            : bezier.SecondControlVertex;

        return (vertex, controlVertex, otherVertex);
    }
    private static (Vertex? vertex, ControlVertex? controlVertex, Vertex? otherVertex, Edge? edge) GetControlVertexConfiguration(this ControlVertex controlVertex)
    {
        Vertex? vertex = null;
        Vertex? otherVertex = null;
        Edge? edge = null;

        var bezier = controlVertex.Edge;

        if (bezier == null)
            return (vertex, controlVertex, otherVertex, edge);

        vertex = controlVertex == bezier.FirstControlVertex
            ? bezier.Start
            : bezier.End;

        edge = vertex.FirstEdge == bezier
            ? vertex.SecondEdge
            : vertex.FirstEdge;

        otherVertex = edge?.OtherVertex(vertex);

        return (vertex, controlVertex, otherVertex, edge);
    }

    public static bool CheckAllConstraints(Polygon? polygon)
    {
        return polygon != null &&
            polygon.Edges.All(edge => edge.CheckConstraint()) &&
            polygon.Vertices.All(vertex => vertex.CheckConstraint());
    }

    private static bool IsBezierNeighbour(this Edge edge, Vertex other)
    {
        var otherEdge = other
            .Edges()
            .FirstOrDefault(e => e != edge);

        if (otherEdge == null) return false;

        return otherEdge.IsBezier;
    }
    
    [Obsolete("Works for polygons with no bezier edges only.")]
    public static bool RestoreConstraintsObsolete(Vertex vertex)
    {
        var stack = new Stack<Vertex>();
        var visited = new HashSet<Vertex>();

        stack.Push(vertex);
        visited.Add(vertex);

        var iterations = 0;
        var maxIterations = vertex.Parent?.Vertices.Count * 10;

        while (stack.Count > 0 && iterations < maxIterations)
        {
            var current = stack.Pop();

            foreach (var edge in current.Edges())
            {
                if (edge == null) continue;

                var changed = false;
                var other = edge.OtherVertex(current);

                if (!edge.IsBezier && !edge.CheckConstraint())
                {
                    changed = true;
                    edge.PreserveConstraint(current);
                }

                if (!visited.Contains(other) && changed && edge.ConstraintType != EdgeConstraintType.None)
                {
                    stack.Push(other);
                    visited.Add(other);
                }
            }

            iterations++;
        }

        return CheckAllConstraints(vertex.Parent);
    }
}