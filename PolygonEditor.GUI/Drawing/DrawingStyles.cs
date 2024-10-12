namespace PolygonEditor.GUI.Drawing;

public static class DrawingStyles
{
    public static readonly int PolygonFillOpacity = 50;
    public static readonly double ControlVertexRadius = 4.0;

    // Colors:
    public static readonly Color BackgroundColor = Color.White;
    public static readonly Color PolygonFillColor = Color.Blue;

    // Brushes:
    public static readonly SolidBrush VertexBrush = new(Color.Black);
    public static readonly SolidBrush ControlVertexBrush = new(Color.Gray);
    public static readonly SolidBrush EdgeBrush = new(Color.Black);
    public static readonly SolidBrush BezierCurveBrush = new(Color.Red);
    public static readonly SolidBrush BezierEdgeBrush = new(Color.LightGray);
    public static readonly SolidBrush BackgroundBrush = new(Color.White);
    public static readonly SolidBrush PolygonFillBrush 
        = new(Color.FromArgb(PolygonFillOpacity, PolygonFillColor));

    // Pens:
    public static readonly Pen EdgePen = new(EdgeBrush);
    public static readonly Pen BezierEdgePen = new(BezierEdgeBrush);
    public static readonly Pen BezierCurvePen = new(BezierCurveBrush);

    public static void Dispose()
    {
        // Brushes:
        VertexBrush.Dispose();
        ControlVertexBrush.Dispose();
        EdgeBrush.Dispose();
        BezierCurveBrush.Dispose();
        BezierEdgeBrush.Dispose();
        BackgroundBrush.Dispose();
        PolygonFillBrush.Dispose();

        // Pens:
        EdgePen.Dispose();
        BezierEdgePen.Dispose();
        BezierCurvePen.Dispose();
    }
}