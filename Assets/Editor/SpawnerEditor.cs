using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{

    void OnSceneGUI()
    {
        Spawner spawner = (Spawner)target;
        Handles.DrawSolidRectangleWithOutline(new Rect(
                    new Vector2(spawner.transform.position.x - spawner.width / 2,
                    spawner.transform.position.y - spawner.height / 2),
                    new Vector2(spawner.width, spawner.height)), spawner.spawnColor, Color.black);
    }
}
