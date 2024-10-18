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

        var t = sV + Vector2.Normalize(direction) * r;

        t.CheckVector2Overflow();

        return new Point((int)t.X, (int)t.Y);
    }
    public static Point ProjectPointOnLine(Point p, Point s, Point r)
    {
        var pV = new Vector2(p.X, p.Y);
        var sV = new Vector2(s.X, s.Y);
        var rV = new Vector2(r.X, r.Y);

        var lineDirectionNormalized = Vector2.Normalize(rV - sV);
        var dot = Vector2.Dot(pV - sV, lineDirectionNormalized);
        var t = sV + lineDirectionNormalized * dot;

        t.CheckVector2Overflow();

        return new Point((int)t.X, (int)t.Y);
    }
    public static Point OffsetPreserveLength(Point p, Point s, float length)
    {
        var pV = new Vector2(p.X, p.Y);
        var sV = new Vector2(s.X, s.Y);

        var sp = pV - sV;
        var direction = Vector2.Normalize(sp);
        var t = sV + direction * length;
        
        t.CheckVector2Overflow();

        return new Point((int)t.X, (int)t.Y);
    }
    public static bool CheckG1(Point s, Point r, Point p)
    {
        var sV = new Vector2(s.X, s.Y);
        var rV = new Vector2(r.X, r.Y);
        var pV = new Vector2(p.X, p.Y);

        var sr = rV - sV;
        var sp = pV - sV;

        var dot = Vector2.Dot(sr, sp);
        var cross = sr.X * sp.Y - sr.Y * sp.X;
        return dot < 0 && Math.Abs(cross) < PolygonEditorConstants.Epsilon;
    }
    public static bool CheckC1(Point s, Point r, Point p)
    {
        var sV = new Vector2(s.X, s.Y);
        var rV = new Vector2(r.X, r.Y);
        var pV = new Vector2(p.X, p.Y);
        var t = pV - sV - 3 * (sV - rV);

        var epsilon = PolygonEditorConstants.Epsilon;

        return Math.Abs(t.X) < epsilon && Math.Abs(t.Y) < epsilon;
    }
    public static Point PreserveG1(Point s, Point r, Point p)
    {
        var sV = new Vector2(s.X, s.Y);
        var rV = new Vector2(r.X, r.Y);
        var pV = new Vector2(p.X, p.Y);

        var direction = sV - pV;
        var length = (rV - sV).Length();
        var t = sV + Vector2.Normalize(direction) * length;

        if (Math.Abs(t.X) > int.MaxValue / 2 ||
            Math.Abs(t.Y) > int.MaxValue / 2)
        {
            t = rV;
        }

        return new Point((int)t.X, (int)t.Y);
    }
    public static Point PreserveC1(Point s, Point p, float k = 3.0f)
    {
        var sV = new Vector2(s.X, s.Y);
        var pV = new Vector2(p.X, p.Y);

        var t = sV + (sV - pV) / k;

        if (Math.Abs(t.X) > int.MaxValue / 2 ||
            Math.Abs(t.Y) > int.MaxValue / 2)
        {
            t = sV;
        }

        return new Point((int)t.X, (int)t.Y);
    }

    public static void CheckVector2Overflow(this Vector2 vector)
    {
        if (Math.Abs(vector.X) > int.MaxValue / 2 ||
            Math.Abs(vector.Y) > int.MaxValue / 2)
        {
            throw new Exception();
        }
    }
}