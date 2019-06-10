using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMark;
    public LayerMask obstacleMask;

    public List<Transform> visibleTarget;

    //void Start()
    //{
    //    StartCoroutine("FindTargetWithDelay", 0.2f);
    //}

    void OnEnable()
    {
        StartCoroutine("FindTargetWithDelay", 0.2f);
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTarget();
        }
    }

    IEnumerator FindTargetWithDelay2(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindVisibleTarget();
    }

    void FindVisibleTarget()
    {
        visibleTarget.Clear();
        Collider2D[] targetInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMark);
        
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask).collider == null)
                {
                    if (visibleTarget.Count == 0)
                        visibleTarget.Add(target);
                    else if (dstToTarget > Vector3.Distance(transform.position, visibleTarget[0].position))
                        visibleTarget.Add(target);
                    else
                        visibleTarget.Insert(0, target);
                }
            }
        }

    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees -= transform.eulerAngles.z;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

}
