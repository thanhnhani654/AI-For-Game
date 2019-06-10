using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

    void OnSceneGUI()
    {
        Grid grid = (Grid)target;
        Handles.color = Color.white;
        if (grid.SeeNodeOnEditor)
        for (int i = 0; i < grid.rows; i++)
            for (int j = 0; j < grid.columns; j++)
            {
                Handles.DrawSolidArc(grid.transform.position + new Vector3(j * grid.alignment,- i * grid.alignment, 0), Vector3.forward, Vector3.right, 360, grid.nodeRadius);
            }
        Handles.color = Color.green;
        Handles.DrawSolidArc(grid.debugStartPoint.transform.position, Vector3.forward, Vector3.right, 360, grid.nodeRadius);
        Handles.color = Color.red;
        Handles.DrawSolidArc(grid.debugEndPoint.transform.position, Vector3.forward, Vector3.right, 360, grid.nodeRadius);

        //Event.current.M

        RenderRandomPosArea(grid);
    }

    void RenderRandomPosArea(Grid grid)
    {
        for (int i = 0; i < grid.listArea.Count; i++)
        {
            if (grid.listArea[i].center != null)
            {
                Handles.color = grid.listArea[i].color;
                Handles.DrawSolidRectangleWithOutline(new Rect(
                    new Vector2(grid.listArea[i].center.transform.position.x - grid.listArea[i].width/2, 
                    grid.listArea[i].center.transform.position.y - grid.listArea[i].height/2), 
                    new Vector2(grid.listArea[i].width, grid.listArea[i].height)), grid.listArea[i].color, Color.black);
            }
        }
    }

}
