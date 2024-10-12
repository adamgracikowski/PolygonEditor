using PolygonEditor.GUI.Drawing;
using PolygonEditor.GUI.Models.Enums;
using System.Windows.Forms;

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
        Buffer = new Bitmap(PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);
        var bitmap = new Bitmap(PictureBox.ClientSize.Width, PictureBox.ClientSize.Height);

        using var graphicsBitmap = Graphics.FromImage(bitmap);
        graphicsBitmap.Clear(DrawingStyles.BackgroundColor);

        PictureBox.Image = bitmap;
        PictureBox.Refresh();

        using var graphicsBuffer = Graphics.FromImage(Buffer);
        graphicsBuffer.Clear(DrawingStyles.BackgroundColor);
    }

    public void Dispose()
    {
        Buffer.Dispose();
        PictureBox.Image?.Dispose();
    }

    public void DrawPartialPolygon(Point point, AlgorithmType algorithmType)
    {
        using var graphics = Graphics.FromImage(Buffer);
        graphics.DrawVertex([..CachedVertices]);
        graphics.DrawEdge(algorithmType, [.. CachedEdges]);
        graphics.DrawEdge(algorithmType, CachedVertices[^1], point);

        SwapBitmaps();
        ClearBuffer();
        PictureBox.Refresh();
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

    public void DrawPolygon(AlgorithmType algorithmType)
    {
        if (Polygon == null)
            return;

        using var graphics = Graphics.FromImage(Buffer);
        graphics.DrawVertex([.. Polygon.Vertices]);
        graphics.DrawEdge(algorithmType, [.. Polygon.Edges]);
        graphics.FillPolygon(
            DrawingStyles.PolygonFillBrush,
            Polygon.Vertices
                .Select(v => new PointF(v.X, v.Y))
                .ToArray()
        );

        SwapBitmaps();
        ClearBuffer();
        PictureBox.Refresh();
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

    private void ClearBuffer()
    {
        using var graphicsBuffer = Graphics.FromImage(Buffer);
        graphicsBuffer.Clear(DrawingStyles.BackgroundColor);
    }

    private void SwapBitmaps()
    {
        (Buffer, PictureBox.Image) = (PictureBox.Image as Bitmap, Buffer);
    }
}