# PolygonEditor:

PolygonEditor is a simple Windows Forms application designed for creating and editing polygons.

It provides an interactive interface to draw, manipulate, and manage polygons with advanced features such as vertex constraints, edge constraints, and curve transformations.

The project was implemented as a part of the Computer Graphics course at Warsaw University of Technology during the winter semester of the 2024-2025 academic year.

## Features:

- **Drawing and Editing**: Create polygons interactively by clicking to add vertices. Add or remove vertices as needed.
- **Polygon Movement**: Move individual vertices or the entire polygon using the mouse.
- **Edge Constraints**:
  - **Vertical Edge**: Restrict an edge to remain vertical.
  - **Horizontal Edge**: Restrict an edge to remain horizontal.
  - **Fixed Length Edge**: Enforce a fixed length for an edge.
- **Vertex Constraints**:
  - **G0 Continuity**: Ensure positional continuity between connected edges.
  - **G1 Continuity**: Enforce tangential continuity between connected edges.
  - **C1 Continuity**: Enforce curvature continuity between connected edges.
- **Bézier Curve Transformation**: Convert edges into third-degree Bezier curves and manipulate their shape using interactive control points.
- **Save Scene**: Serialize the polygon data to a file.
- **Load Scene**: Deserialize polygon data from a file to continue editing.
- **Rendering Options**: Draw polygons using library-provided rendering algorithms or use a custom implementation of Bresenham's algorithm.

## Constraint Preservation Algorithm:

The algorithm starts from the activated vertex and iteratively adjusts the constraints on the edges and vertices in one direction until it encounters either the starting vertex or an edge without a set constraint (which can be modified freely).

It then corrects the constraints by traversing the edges and vertices in the opposite direction.

If the adjustment would require modifying a vertex position set by the user, the algorithm reverts the polygon to its initial state, determining that such a move is not possible.

## Controls:

1. Creating a New Polygon:

- Right-click on an empty area of the window and select _Create polygon_.
- Place subsequent vertices on the screen by clicking with the left mouse button.
- To finish creating the polygon, select the starting vertex.
- To remove a partially created polygon, press the `Escape` key.

2. Deleting a Polygon:

- Right-click and select _Delete polygon_.

3. Moving a Polygon:

- Hold down the left mouse button to move the polygon.

4. Vertex Management:

- Right-click while hovering over a vertex and select the desired option from the context menu.
- _Delete vertex_ allows you to remove a vertex and its adjacent edges.
- _Constraints_ lets you select a constraint applied to the vertex (continuity of the Bézier segment with a line segment).
- To move a vertex, hold down the left mouse button over the vertex.

5. Edge Management:

- Right-click while hovering over an edge and select the desired option from the context menu.
- _Add vertex_ adds a new vertex at the middle of the edge.
- _Bézier_ toggles the edge to a Bezier segment.
- _Constraints_ allows you to select a constraint applied to the edge.
- The constraint will be activated when you move the vertex.

6. Bezier Segment Management:

- Right-click on the control point to revert the edge back to a straight segment.
- To move the control point, hold down the left mouse button.
