﻿using PolygonEditor.GUI.Algorithms;
using PolygonEditor.GUI.Models.Interfaces;

namespace PolygonEditor.GUI.Models;

public sealed class Edge : ISelectable
{
    public Edge(Vertex start, Vertex end, Polygon? parent = null,
        ControlVertex? firstControlVertex = null, ControlVertex? secondControlVertex = null)
    {
        Start = start;
        End = end;
        Parent = parent;
        FirstControlVertex = firstControlVertex;
        SecondControlVertex = secondControlVertex;
        IsBezier = firstControlVertex != null && secondControlVertex != null;

        // set references

        Start.SecondEdge = this;
        End.FirstEdge = this;

        if (FirstControlVertex != null)
            FirstControlVertex.Edge = this;

        if (SecondControlVertex != null)
            SecondControlVertex.Edge = this;
    }

    public Vertex Start { get; set; }
    public Vertex End { get; set; }
    public Polygon? Parent { get; set; }
    public ControlVertex? FirstControlVertex { get; set; }
    public ControlVertex? SecondControlVertex { get; set; }
    public bool IsBezier { get; set; }

    public double Length
    {
        get
        {
            return Math.Sqrt(
                (Start.X - End.X) * (Start.X - End.X) +
                (Start.Y - End.Y) * (Start.Y - End.Y)
            );
        }
    }

    public bool IsWithinSelection(int x, int y)
    {
        if (IsBezier && FirstControlVertex != null && SecondControlVertex != null)
        {
            return FirstControlVertex.IsWithinSelection(x, y) || SecondControlVertex.IsWithinSelection(x, y);
        }
        else
        {
            return Algorithm.BresenhamPoints(Start.X, Start.Y, End.X, End.Y)
                .Any(p => Geometry.IsNear(p.X, p.Y, x, y, PolygonEditorConstants.HitRadius));
        }
    }

    public void ToggleBezier()
    {
        if (IsBezier)
        {
            FirstControlVertex = null;
            SecondControlVertex = null;
        }
        else
        {
            SetDefaultControlPoints();
        }

        IsBezier = !IsBezier;
    }

    private void SetDefaultControlPoints(float k = 0.5f)
    {
        float mx = (Start.X + End.X) / 2;
        float my = (Start.Y + End.Y) / 2;

        var dx = End.X - Start.X;
        var dy = End.Y - Start.Y;

        var length = (float)Math.Sqrt(dx * dx + dy * dy);

        var ux = dx / length;
        var uy = dy / length;

        var x1 = (int)(mx - uy * k * length / 2);
        var y1 = (int)(my + ux * k * length / 2);

        FirstControlVertex = new(new Point(x1, y1))
        {
            Parent = this.Parent,
            Edge = this
        };

        var x2 = (int)(mx + uy * k * length / 2);
        var y2 = (int)(my - ux * k * length / 2);

        SecondControlVertex = new(new Point(x2, y2))
        {
            Parent = this.Parent,
            Edge = this
        };
    }
}