using PolygonEditor.GUI.Drawing;
using PolygonEditor.GUI.Models.Enums;

namespace PolygonEditor.GUI.Models;

public sealed class PolygonContainer : IDisposable
{
    public Polygon? Polygon { get; set; }
    public List<Vertex> CachedVertices = [];
    public List<Edge> CachedEdges = [];
    public Bitmap Buffer { get; set; }
    public PictureBox PictureBox { get; set; }

    public PolygonContainer(PictureBox pictureBox)
    {
        PictureBox = pictureBox;
        //Buffer = new Bitmap(PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);
        Buffer = new Bitmap(PolygonEditorConstants.EditorMaxWidth, PolygonEditorConstants.EditorMaxHeight);
        ClearBuffer();

        //var bitmap = new Bitmap(PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);
        var bitmap = new Bitmap(PolygonEditorConstants.EditorMaxWidth, PolygonEditorConstants.EditorMaxHeight);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingStyles.BackgroundColor);

        PictureBox.Image = bitmap;
        PictureBox.Refresh();
    }

    public void Resize(AlgorithmType algorithmType)
    {
        var oldBuffer = Buffer;
        var oldBitmap = PictureBox.Image;

        Buffer = new Bitmap(PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);

        using var graphicsBuffer = Graphics.FromImage(Buffer);
        graphicsBuffer.Clear(DrawingStyles.BackgroundColor);

        if (Polygon != null)
        {
            DrawPolygonUsingGraphics(graphicsBuffer, algorithmType);
        }
        else if (CachedVertices.Count > 0)
        {
            DrawPartialPolygonUsingGraphics(graphicsBuffer, CachedVertices[^1].Point, algorithmType);
        }

        SwapBitmaps();
        PictureBox.Refresh();

        Buffer = new Bitmap(PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);
        ClearBuffer();

        oldBuffer?.Dispose();
        oldBitmap?.Dispose();
    }
    public void Dispose()
    {
        Buffer.Dispose();
        PictureBox.Image?.Dispose();
    }
    public void ClearContainer()
    {
        Polygon = null;
        CachedEdges = [];
        CachedVertices = [];

        SwapBitmaps();
        ClearBuffer();
        PictureBox.Refresh();
    }

    public bool AddVertexToPartialPolygon(Point point, AlgorithmType algorithmType)
    {
        var vertex = new Vertex(point);

        if (CachedVertices.Count == 0)
        {
            CachedVertices.Add(vertex);
            DrawPartialPolygon(point, algorithmType);
            return false;
        }

        var first = CachedVertices[0];
        var last = CachedVertices[^1];
        Edge edge;

        if (CachedVertices.Count > 2 &&
            first.IsWithinSelection(vertex.X, vertex.Y))
        {
            edge = new Edge(last, first);
            CachedEdges.Add(edge);
            Polygon = new Polygon(CachedVertices, CachedEdges);
            CachedEdges = [];
            CachedVertices = [];
            DrawPolygon(algorithmType);
            return true;
        }

        edge = new Edge(last, vertex);
        CachedEdges.Add(edge);
        CachedVertices.Add(vertex);
        DrawPartialPolygon(point, algorithmType);
        return false;
    }
    public bool IsVertexHit(Point point, out Vertex? vertex)
    {
        vertex = Polygon?.Vertices.FirstOrDefault(v => v.IsWithinSelection(point.X, point.Y));
        return vertex != null;
    }
    public bool IsEdgeHit(Point point, out Edge? edge)
    {
        edge = Polygon?.Edges.FirstOrDefault(e => e.IsWithinSelection(point.X, point.Y));
        return edge != null;
    }
    public bool IsPolygonHit(Point point, out Polygon? polygon)
    {
        polygon = Polygon;

        if (Polygon == null)
            return false;

        return Polygon.IsWithinSelection(point.X, point.Y);
    }
    private void SwapBitmaps()
    {
        (Buffer, PictureBox.Image) = (PictureBox.Image as Bitmap, Buffer);
    }
    public void DrawPolygon(AlgorithmType algorithmType)
    {
        using var graphics = Graphics.FromImage(Buffer);
        DrawPolygonUsingGraphics(graphics, algorithmType);
        SwapBitmaps();
        ClearBuffer();
        PictureBox.Refresh();
    }
    public void DrawPartialPolygon(Point point, AlgorithmType algorithmType)
    {
        using var graphics = Graphics.FromImage(Buffer);
        DrawPartialPolygonUsingGraphics(graphics, point, algorithmType);
        SwapBitmaps();
        ClearBuffer();
        PictureBox.Refresh();
    }
    private void ClearBuffer()
    {
        using var graphicsBuffer = Graphics.FromImage(Buffer);
        graphicsBuffer.Clear(DrawingStyles.BackgroundColor);
    }
    private void DrawPartialPolygonUsingGraphics(Graphics graphics, Point point, AlgorithmType algorithmType)
    {
        graphics.DrawEdge(algorithmType, [.. CachedEdges]);
        graphics.DrawEdge(algorithmType, CachedVertices[^1], point);
        graphics.DrawVertex([.. CachedVertices]);
    }
    private void DrawPolygonUsingGraphics(Graphics graphics, AlgorithmType algorithmType)
    {
        if (Polygon == null)
            return;

        graphics.DrawEdge(algorithmType, [.. Polygon.Edges]);
        graphics.FillPolygon(
            DrawingStyles.PolygonFillBrush,
            Polygon.Vertices
                .Select(v => new PointF(v.X, v.Y))
                .ToArray()
        );
        graphics.DrawVertex([.. Polygon.Vertices]);
        graphics.DrawEdgeConstraints([.. Polygon.Edges]);
        graphics.DrawVertexConstraint([.. Polygon.Vertices]);
    }
}