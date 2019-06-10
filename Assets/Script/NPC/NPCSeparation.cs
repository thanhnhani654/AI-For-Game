using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSeparation : MonoBehaviour
{
    public float radius = 3.0f;
    public LayerMask mask;
    private void Update()
    {
        foreach (GameObject npc in NPCManager.instance.listNPCs)
        {           
            Collider2D obstacle = Physics2D.OverlapCircle(npc.transform.position, radius, mask);
            //Lấy hướng ngược lại
            Vector3 direction = npc.transform.position - obstacle.transform.position;
            float ratio = 1 - Vector3.Distance(npc.transform.position, obstacle.transform.position) / radius;

            Vector3 currentPos = npc.GetComponent<NPCController>().nextPosition;
            Vector3 nextPos = Vector3.MoveTowards(currentPos, direction, npc.GetComponent<Attribute>().speed * ratio * 115.0f);
            npc.GetComponent<NPCController>().nextPosition = nextPos;
        }
    }
}
