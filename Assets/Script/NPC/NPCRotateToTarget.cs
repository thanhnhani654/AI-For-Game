using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRotateToTarget : MonoBehaviour {

    public Transform target;
    FieldOfView fow;

    public float rotationSpeed = 2.5f;

    void Start()
    {
        fow = GetComponent<FieldOfView>();
    }

	// Update is called once per frame
	void Update () {
        UpdateTarget();

    }

    public void UpdateTarget()
    {
        target = null;
        if (fow.visibleTarget.Count > 0)
            for (int i = 0; i < fow.visibleTarget.Count; i++)
            {
                if (fow.visibleTarget[i].GetComponent<Attribute>().Team != this.GetComponent<Attribute>().Team)
                {
                    target = fow.visibleTarget[i];
                    break;
                }
            }
    }

    public void RotateToTarget()
    {
        if (!target)
            return;

        Vector3 dir = (target.position - transform.position).normalized;

        float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    public void RotateWithDirection(Vector3 direction)
    { 
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    public void RotateToPosition(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
       
        float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion endRot = Quaternion.Euler(0f, 0f, rot_z - 90);

        if (transform.rotation != endRot)
        {
            transform.rotation = Quaternion.Slerp(this.transform.rotation, endRot, Time.deltaTime * rotationSpeed);
        }
        //transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        //transform.Rotate(Quaternion.Euler(0f, 0f, rot_z - 90),);
    }

}
