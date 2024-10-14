using PolygonEditor.GUI.Models;
using PolygonEditor.GUI.Models.Enums;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Channels;

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
    public static bool MoveVertexWithConstraints(Vertex vertex, int x, int y)
    {
        if (vertex.Parent == null) return false;

        var polygon = vertex.Parent;
        var copy = polygon.DeepCopy();

        vertex.X = x;
        vertex.Y = y;

        if (!RestoreConstraints(vertex))
        {
            vertex.Parent = copy;
            return false;
        }

        return true;
    }
    public static void MoveControlVertexWithConstraints(ControlVertex controlVertex, int x, int y)
    {
        if (controlVertex.Edge == null || controlVertex.Parent == null) return;

        var edge = controlVertex.Edge;
        var polygon = controlVertex.Parent;
        var copy = polygon.DeepCopy();

        controlVertex.X = x;
        controlVertex.Y = y;

        var vertex = controlVertex == edge.FirstControlVertex
            ? edge.Start.FirstEdge?.Start 
            : edge.End.SecondEdge?.End;

        if (vertex == null) return;

        if (!MoveVertexWithConstraints(vertex, vertex.X, vertex.Y)) // to trzeba zmienić
        {
            vertex.Parent = copy;
        }
    }

    public static bool RestoreConstraints(Vertex vertex)
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

                if (changed && edge.ConstraintType != EdgeConstraintType.None && !visited.Contains(other))
                {
                    stack.Push(other);
                    visited.Add(other);
                }
            }

            iterations++;
        }

        return CheckAllConstraints(vertex.Parent);
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
    public static bool CheckAllConstraints(Polygon? polygon)
    {
        return polygon != null && polygon.Edges.All(edge => edge.CheckConstraint());
    }
}