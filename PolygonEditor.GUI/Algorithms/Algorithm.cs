using PolygonEditor.GUI.Models;
using PolygonEditor.GUI.Models.Enums;
using System.Numerics;

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

    // ciągłości pomiędzy segmentem beziera a odcinkiem:

    // funkcje zwracają położenie końca odcinka po dopasowaniu do segmentu beziera
    public static Point ContinuityG1P1(Point p1, Point p2, Point p)
    {
        var p1V = new Vector2(p1.X, p1.Y);
        var p2V = new Vector2(p2.X, p2.Y);
        var pV = new Vector2(p.X, p.Y);

        var direction = p2V - p1V;

        return G1Point(pV, p1V, direction);
    }
    public static Point ContinuityG1P4(Point p4, Point p3, Point p)
    {
        var p4V = new Vector2(p4.X, p4.Y);
        var p3V = new Vector2(p3.X, p3.Y);
        var pV = new Vector2(p.X, p.Y);

        var direction = p4V - p3V;

        return G1Point(pV, p4V, direction);
    }
    private static Point G1Point(Vector2 pV, Vector2 commonV, Vector2 direction)
    {
        var distance = Vector2.Distance(pV, commonV);
        var q = commonV + Vector2.Normalize(direction) * distance;
        return new Point((int)q.X, (int)q.Y);
    }
    public static Point ContinuityC1P1(Point p1, Point p2)
    {
        var p1V = new Vector2(p1.X, p1.Y);
        var p2V = new Vector2(p2.X, p2.Y);

        var direction = p2V - p1V;

        return C1Point(p1V, direction);
    }
    public static Point ContinuityC1P4(Point p4, Point p3)
    {
        var p4V = new Vector2(p4.X, p4.Y);
        var p3V = new Vector2(p3.X, p3.Y);

        var direction = p4V - p3V;

        return C1Point(p4V, direction);
    }
    private static Point C1Point(Vector2 commonV, Vector2 direction)
    {
        var length = 3 * direction.Length();
        Vector2 q = commonV + Vector2.Normalize(direction) * length;
        return new Point((int)q.X, (int)q.Y);
    }

    public static void MoveVertexWithConstraints(Vertex vertex, int x, int y)
    {
        if (vertex.Parent == null) return;

        var polygon = vertex.Parent;
        var copy = polygon.DeepCopy();

        vertex.X = x;
        vertex.Y = y;

        if (!RestoreConstraints(vertex))
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
        var maxIterations = 50;

        while (stack.Count > 0 && iterations < maxIterations)
        {
            var current = stack.Pop();

            foreach (var edge in current.Edges())
            {
                if (edge == null) continue;

                var other = edge.OtherVertex(current);

                if (!edge.IsBezier)
                {
                    if (edge.ConstraintType == EdgeConstraintType.Horizontal && !CheckHorizontal(edge))
                    {
                        other.Y = current.Y;
                    }
                    else if (edge.ConstraintType == EdgeConstraintType.Vertical && !CheckVertical(edge))
                    {
                        other.X = current.X;
                    }
                    else if (edge.ConstraintType == EdgeConstraintType.FixedLength && !CheckFixedLength(edge))
                    {
                        // popraw długość
                    }
                }
                else
                {
                    //if (current.ConstraintType != VertexConstraintType.None &&
                    //   current.ConstraintType != VertexConstraintType.G0)
                    //{
                    //    if (edge == current.FirstEdge)
                    //    {
                    //        switch (current.ConstraintType)
                    //        {
                    //            case VertexConstraintType.G1:

                    //                break;
                    //            case VertexConstraintType.C1:
                    //                break;
                    //            default: break;
                    //        }
                    //    }
                    //    else if (edge == current.SecondEdge)
                    //    {

                    //    }

                    //}
                }

                if (!visited.Contains(other))
                {
                    stack.Push(other);
                    visited.Add(other);
                }
            }

            iterations++;
        }

        return CheckAllConstraints(vertex.Parent);
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
        return Math.Abs(edge.FixedLength - edge.Length) < 0.001f;
    }

    public static bool CheckAllConstraints(Polygon? polygon)
    {
        return true; // TODO: dodać jakąś abstrakcję na ograniczenia typu: constraint.Check()
    }
}