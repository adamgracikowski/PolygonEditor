using System.Drawing;

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
}