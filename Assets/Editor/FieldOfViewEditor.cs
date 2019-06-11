using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.right, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);
        
        for (int i = 0; i < fow.visibleTarget.Count; i++)
        {
            if (fow.GetComponent<Attribute>().Team != fow.visibleTarget[i].GetComponent<Attribute>().Team)
            {
                Handles.color = Color.red;
                Handles.DrawLine(fow.transform.position, fow.visibleTarget[i].position);
            }
            else
            {
                Handles.color = Color.blue;
                Handles.DrawLine(fow.transform.position, fow.visibleTarget[i].position);
            }
        }

    }
}
