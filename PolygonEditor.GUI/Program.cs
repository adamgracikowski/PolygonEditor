namespace PolygonEditor.GUI;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new PolygonEditorForm());
    }
}