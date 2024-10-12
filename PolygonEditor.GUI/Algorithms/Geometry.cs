namespace PolygonEditor.GUI.Algorithms;
public static class Geometry
{
    public static bool IsNear(int x0, int y0, int x, int y, int distance)
    {
        return ((x0 - x) * (x0 - x) + (y0 - y) * (y0 - y)) <= distance * distance;
    }
}
