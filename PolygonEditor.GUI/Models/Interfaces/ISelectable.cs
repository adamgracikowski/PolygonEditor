namespace PolygonEditor.GUI.Models.Interfaces;

public interface ISelectable
{
    bool IsWithinSelection(int x, int y);
}