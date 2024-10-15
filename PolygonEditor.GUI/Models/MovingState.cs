namespace PolygonEditor.GUI.Models;

public sealed class MovingState<T>
    where T : class
{
    public MovingState(T? selectedElement = null, Point hitPoint = default)
    {
        SelectedElement = selectedElement;
        HitPoint = hitPoint;
    }

    public T? SelectedElement { get; set; }
    public Point HitPoint { get; set; }

    public void Clear()
    {
        SelectedElement = null;
    }

    public (int x, int y) UpdateHitPoint(Point point)
    {
        var dx = point.X - HitPoint.X;
        var dy = point.Y - HitPoint.Y;

        HitPoint = point;

        return (dx, dy);
    }
}