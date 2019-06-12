using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    // Tầm nhìn
    public float viewRadius;
    [Range(0,360)]
    // Góc nhìn
    public float viewAngle;

    // Layer dùng để phân biệt các object nhanh hơn
    // Chứa Layer của mục tiêu (Nhân vật và các NPC) 
    public LayerMask targetMark;
    // Chứa Layer của vật cản
    public LayerMask obstacleMask;

    // Danh sách chứa các mục tiêu trong tầm nhìn
    public List<Transform> visibleTarget;

    //void Start()
    //{
    //    StartCoroutine("FindTargetWithDelay", 0.2f);
    //}

    void OnEnable()
    {
        // Chạy hàm FindTargetWithDelay(0.2f);
        StartCoroutine("FindTargetWithDelay", 0.2f);
    }

    // Đợi delay time rồi chạy hàm FindVisibleTarget. Dùng để giảm số lượng hàm phải chạy. kiểu 0.2s chạy 1 lần.
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
        // Xóa danh sách mục tiêu trước đó.
        visibleTarget.Clear();

        // Kiểm tra tất cả mục tiêu trong bán kính tầm nhìn
        Collider2D[] targetInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMark);
        
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            // Lấy vector chỉ hướng đến mục tiêu
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Kiểm tra mục tiêu có nằm trong góc nhìn và nếu không có vật cản chắn ngang thì thêm mục tiêu vào danh sách
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

    // Chuyển từ vector chỉ hướng sang góc
    public Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees -= transform.eulerAngles.z;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

}
