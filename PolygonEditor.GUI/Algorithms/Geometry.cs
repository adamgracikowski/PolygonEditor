using System.Numerics;

namespace PolygonEditor.GUI.Algorithms;
public static class Geometry
{
    public static bool IsNear(int x0, int y0, int x, int y, int distance)
    {
        return ((x0 - x) * (x0 - x) + (y0 - y) * (y0 - y)) <= distance * distance;
    }

    public static Point CircleLineIntersection(Point p, Point s, float r)
    {
        var pV = new Vector2(p.X, p.Y);
        var sV = new Vector2(s.X, s.Y);
        var direction = pV - sV;
        var q = sV + Vector2.Normalize(direction) * r;
        return new Point((int)q.X, (int)q.Y);
    }
}
