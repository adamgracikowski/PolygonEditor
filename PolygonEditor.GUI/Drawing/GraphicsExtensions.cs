using PolygonEditor.GUI.Algorithms;
using PolygonEditor.GUI.Models;
using PolygonEditor.GUI.Models.Enums;
using PolygonEditor.GUI.Properties;

namespace PolygonEditor.GUI.Drawing;

public static class GraphicsExtensions
{
    private static void DrawVertexBase(this Graphics g, SolidBrush brush, params VertexBase[] vertices)
    {
        if (vertices == null || vertices.Length == 0) return;

        var r = PolygonEditorConstants.VertexRadius;
        var d = 2 * r;
        foreach (var vertex in vertices)
        {
            g.FillEllipse(brush,
                (int)(vertex.X - r),
                (int)(vertex.Y - r),
                (int)d,
                (int)d
            );

        }
    }
    public static void DrawVertex(this Graphics graphics, params Vertex[] vertices)
    {
        graphics.DrawVertexBase(DrawingStyles.VertexBrush, vertices);
    }
    public static void DrawControlVertex(this Graphics graphics, params ControlVertex[] vertices)
    {
        graphics.DrawVertexBase(DrawingStyles.ControlVertexBrush, vertices);
    }
    public static void DrawEdge(this Graphics graphics, AlgorithmType algorithmType, params Edge[] edges)
    {
        foreach (var edge in edges)
        {
            var startPoint = edge.Start.Point;
            var endPoint = edge.End.Point;

            if (edge.IsBezier)
            {
                var firstControlPoint = edge.FirstControlVertex!.Point;
                var secondControlPoint = edge.SecondControlVertex!.Point;

                if (algorithmType == AlgorithmType.Library)
                {
                    graphics.DrawLine(
                        DrawingStyles.BezierEdgePen,
                        startPoint,
                        firstControlPoint
                    );
                    graphics.DrawLine(
                        DrawingStyles.BezierEdgePen,
                        firstControlPoint,
                        secondControlPoint
                    );
                    graphics.DrawLine(
                        DrawingStyles.BezierEdgePen,
                        secondControlPoint,
                        endPoint
                    );
                    graphics.DrawLine(
                        DrawingStyles.BezierEdgePen,
                        startPoint,
                        endPoint
                    );
                    graphics.DrawBezier(
                        DrawingStyles.BezierCurvePen,
                        startPoint,
                        firstControlPoint,
                        secondControlPoint,
                        endPoint
                    );
                }
                else if (algorithmType == AlgorithmType.Custom)
                {
                    graphics.DrawBresenhamLine(
                        DrawingStyles.BezierEdgePen,
                        startPoint,
                        firstControlPoint
                    );
                    graphics.DrawBresenhamLine(
                        DrawingStyles.BezierEdgePen,
                        firstControlPoint,
                        secondControlPoint
                    );
                    graphics.DrawBresenhamLine(
                        DrawingStyles.BezierEdgePen,
                        secondControlPoint,
                        endPoint
                    );
                    graphics.DrawBresenhamLine(
                        DrawingStyles.BezierEdgePen,
                        startPoint,
                        endPoint
                    );
                    graphics.DrawBezierLine(
                        DrawingStyles.BezierCurvePen,
                        startPoint,
                        firstControlPoint,
                        secondControlPoint,
                        endPoint
                    );
                }

                graphics.DrawControlVertex(edge.FirstControlVertex, edge.SecondControlVertex);
            }
            else
            {
                if (algorithmType == AlgorithmType.Library)
                {
                    graphics.DrawLine(
                        DrawingStyles.EdgePen,
                        startPoint,
                        endPoint
                    );
                }
                else if (algorithmType == AlgorithmType.Custom)
                {
                    graphics.DrawBresenhamLine(
                        DrawingStyles.EdgePen,
                        startPoint,
                        endPoint
                    );
                }
            }
        }
    }
    public static void DrawEdge(this Graphics graphics, AlgorithmType algorithmType, VertexBase vertex, Point point)
    {
        if (algorithmType == AlgorithmType.Custom)
        {
            graphics.DrawBresenhamLine(
                DrawingStyles.EdgePen,
                vertex.Point,
                point
            );
        }
        else if (algorithmType == AlgorithmType.Library)
        {
            graphics.DrawLine(
                DrawingStyles.EdgePen,
                vertex.Point,
                point
            );
        }
    }
    public static void DrawBresenhamLine(this Graphics graphics, Pen pen, Point pt1, Point pt2)
    {
        foreach (var point in Algorithm.BresenhamPoints(pt1.X, pt1.Y, pt2.X, pt2.Y))
        {
            graphics.DrawRectangle(pen, point.X, point.Y, 1, 1);
        }
    }
    public static void DrawBezierLine(this Graphics graphics, Pen pen, Point p1, Point p2, Point p3, Point p4)
    {
        float t = 0;
        int steps = PolygonEditorConstants.BezierStep;
        float dt = 1f / steps;

        Point prevPoint = p1;

        for (var i = 1; i <= steps; i++)
        {
            t += dt;

            var a = Math.Pow(1 - t, 3);
            var b = 3 * t * Math.Pow(1 - t, 2);
            var c = Math.Pow(t, 3);
            var d = 3 * (1 - t) * Math.Pow(t, 2);

            int x = (int)Math.Round(a * p1.X + b * p2.X + d * p3.X + c * p4.X);
            int y = (int)Math.Round(a * p1.Y + b * p2.Y + d * p3.Y + c * p4.Y);

            var newPoint = new Point(x, y);

            graphics.DrawBresenhamLine(pen, prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    public static void DrawEdgeConstraints(this Graphics graphics, params Edge[] edges)
    {
        foreach (var edge in edges)
        {
            if (edge.IsBezier || edge.ConstraintType == EdgeConstraintType.None) continue;

            var x = (edge.Start.X + edge.End.X) / 2;
            var y = (edge.Start.Y + edge.End.Y) / 2;

            graphics.DrawIcon(edge.ConstraintType, x, y);
        }
    }
    private static void DrawIcon(this Graphics graphics, EdgeConstraintType constraintType, int x, int y)
    {
        Image? icon = constraintType switch
        {
            EdgeConstraintType.Horizontal => Resources.horizontal,
            EdgeConstraintType.Vertical => Resources.vertical,
            EdgeConstraintType.FixedLength => Resources.ruler,
            _ => null
        };

        if (icon == null) return;

        graphics.DrawImage(icon, x, y, icon.Width, icon.Height);
    }
    public static void DrawVertexConstraint(this Graphics graphics, params Vertex[] vertices)
    {
        foreach (var vertex in vertices)
        {
            if (vertex.ConstraintType == VertexConstraintType.None) continue;

            graphics.DrawText(
                vertex.ConstraintType.ToString(),
                vertex.Point.X + 4,
                vertex.Point.Y + 4
            );
        }
    }
    private static void DrawText(this Graphics graphics, string text, int x, int y)
    {
        var font = DrawingStyles.ConstraintFont;
        var brush = DrawingStyles.ConstraintBrush;
        var textSize = TextRenderer.MeasureText(text, font);
        graphics.DrawString(text, font, brush, new Rectangle(x, y, textSize.Width, textSize.Height));
    }
}