using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCPathFinder {

    // Lưu trữ Object chứa Grid dùng để tìm đường. Do không lấy Grid trực tiếp được nên phải trung gian qua Object
    // Thật ra là quên dùng SingleTon cho grid nên không lấy trực tiếp được
    public GameObject pathFinder;

    // Lưu trữ Gỉd
    Grid grid;
    // Danh sách Node chỉ đường cho NPC.
    public List<Node> path;
    // Vị trí mà NPC cần đi tìm đường đi tới
    Vector2 targetPoint;

    // Debug
    public GameObject debugEndPoint;
    public bool debug = true;
    public bool debugDrawPath = true;
    public bool debugDrawPredictLine = true;

    // Kiểm tra NPC đã đi đến điểm cần đi tới chưa để tìm điểm mới để đi
    public bool bReachPoint;
    // Kiểm tra NPC đã đi đến Node cần phải đi trong quá trình tìm đường chưa
    public bool bReachNode = false;

    public NPCPathFinder(GameObject grid, GameObject debugPoint)
    {
        pathFinder = grid;
        debugEndPoint = debugPoint;
        Start();
    }
    
    void Start()
    {
        grid = pathFinder.GetComponent<Grid>();
    }

    // Cho vào tọa độ hiện tại NPC sẽ tìm đường đi đến 1 vị trí ngẫu nhiên ở khu vực giữa 2 team
    public Vector3 FollowPath(Vector3 currentPos, float speed, LayerMask obstacleMask)
    {
        // newPos dùng để trả về tọa độ trong frame tiếp theo của NPC
        Vector3 newPos = currentPos;

        if (debugEndPoint != null && debug)
        {
            path = grid.FindPath(new Vector2(currentPos.x, currentPos.y), new Vector2(debugEndPoint.transform.position.x, debugEndPoint.transform.position.y));
        }
        else if (targetPoint == null)
            FindNewPath(currentPos);
        else if (targetPoint != null && bReachPoint)
            // Lấy danh sách các node cần để đi đến mục tiêu
            path = grid.FindPath(new Vector2(currentPos.x, currentPos.y), targetPoint);

        if (path != null && path.Count > 0)
        {
            int i = 0;
            Vector2 dir = new Vector2(0, 0);
            // Tìm Node gần Node cuối nhất mà ở giữa NPC và Node đó không có vật cản
            for (i = path.Count - 1; i > 0; i--)
            {
                dir = new Vector2(path[i].worldPos.x - currentPos.x, path[i].worldPos.y - currentPos.y);
                RaycastHit2D hit = Physics2D.Raycast(currentPos, new Vector2(path[i].worldPos.x - currentPos.x, path[i].worldPos.y - currentPos.y), Vector2.Distance(path[i].worldPos, new Vector2(currentPos.x, currentPos.y)), obstacleMask);
                if (hit.collider == null)
                {
                    break;
                }
            }

            // Debug
            if (debugDrawPredictLine)
                Debug.DrawRay(currentPos, new Vector3(dir.x, dir.y, 0), Color.yellow);

            // Di chuyển thẳng đến node tìm được
            newPos = Vector3.MoveTowards(currentPos, path[i].worldPos, speed);
            
            // Xóa các Node đã đi qua rồi
            if (path.Count > 1)
            {
                if (path[1].worldPos.x - path[0].worldPos.x == 0)
                {
                    if ((newPos.y - path[0].worldPos.y) * (newPos.y - path[1].worldPos.y) <= 0 ||
                        path[1].worldPos.y - path[0].worldPos.y > 0 && newPos.y >= path[1].worldPos.y ||
                        path[1].worldPos.y - path[0].worldPos.y < 0 && newPos.y <= path[1].worldPos.y)
                    {
                        path.RemoveAt(0);
                    }
                }
                else
                {
                    if ((newPos.x - path[0].worldPos.x) * (newPos.x - path[1].worldPos.x) <= 0 ||
                        path[1].worldPos.x - path[0].worldPos.x > 0 && newPos.x >= path[1].worldPos.x ||
                        path[1].worldPos.x - path[0].worldPos.x < 0 && newPos.x <= path[1].worldPos.x)
                    {
                        path.RemoveAt(0);
                    }
                }

            }

            if (Vector2.Distance(newPos, path[path.Count - 1].worldPos) < 3)
            //if (newPos.x == path[0].worldPos.x && newPos.y == path[0].worldPos.y)
            {
                for (int p = 0; p < path.Count; p++)
                    path.RemoveAt(0);
            }


            bReachPoint = false;
        }
        else
            bReachPoint = true;

        if (debugDrawPath)
            if (path != null)
            {
                for (int i = 1; i < path.Count; i++)
                    Debug.DrawLine(path[i - 1].worldPos, path[i].worldPos, Color.red);
            }

        return newPos;
    }

    // Tìm mục tiêu di chuyển mới theo ngẫu nhiên
    public void FindNewPath(Vector3 currentPos)
    {
        targetPoint = grid.GetRandomPositionInCenterArea();
    }

    // Cho vào tọa độ hiện tại NPC sẽ tìm đường đi đến 1 vị trí chỉ định(target)
    public Vector3 FollowPath(Vector3 currentPos,Vector3 target, float speed, LayerMask obstacleMask)
    {
        
        Vector3 newPos = currentPos;

        if (targetPoint.x != target.x && targetPoint.y != target.y)
        {
            targetPoint = target;
            path = grid.FindPath(new Vector2(currentPos.x, currentPos.y), targetPoint);
        }    

        if (path != null && path.Count > 0)
        {
            //newPos = Vector3.MoveTowards(currentPos, path[0].worldPos, speed);

            //if (newPos.x == path[0].worldPos.x && newPos.y == path[0].worldPos.y)
            //{
            //    path.RemoveAt(0);
            //}

            int i = 0;
            Vector2 dir = new Vector2(0, 0);
            for (i = path.Count - 1; i > 0; i--)
            {
                dir = new Vector2(path[i].worldPos.x - currentPos.x, path[i].worldPos.y - currentPos.y);
                RaycastHit2D hit = Physics2D.Raycast(currentPos, new Vector2(path[i].worldPos.x - currentPos.x, path[i].worldPos.y - currentPos.y), Vector2.Distance(path[i].worldPos, new Vector2(currentPos.x, currentPos.y)), obstacleMask);
                //Debug.Log("A");
                if (hit.collider == null)
                {
                    //Debug.Log(i + " & " + path.Count);
                    break;
                }
            }
            if (debugDrawPredictLine)
                Debug.DrawRay(currentPos, new Vector3(dir.x, dir.y, 0), Color.yellow);

            newPos = Vector3.MoveTowards(currentPos, path[i].worldPos, speed);

            if (path.Count > 1)
            {
                if (path[1].worldPos.x - path[0].worldPos.x == 0)
                {
                    if ((newPos.y - path[0].worldPos.y) * (newPos.y - path[1].worldPos.y) <= 0 ||
                        path[1].worldPos.y - path[0].worldPos.y > 0 && newPos.y >= path[1].worldPos.y ||
                        path[1].worldPos.y - path[0].worldPos.y < 0 && newPos.y <= path[1].worldPos.y)
                    {
                        path.RemoveAt(0);
                    }
                }
                else
                {
                    if ((newPos.x - path[0].worldPos.x) * (newPos.x - path[1].worldPos.x) <= 0 ||
                        path[1].worldPos.x - path[0].worldPos.x > 0 && newPos.x >= path[1].worldPos.x ||
                        path[1].worldPos.x - path[0].worldPos.x < 0 && newPos.x <= path[1].worldPos.x)
                    {
                        path.RemoveAt(0);
                    }
                }

            }

            if (Vector2.Distance(newPos, path[path.Count - 1].worldPos) < 3)
            //if (newPos.x == path[0].worldPos.x && newPos.y == path[0].worldPos.y)
            {
                for (int p = 0; p < path.Count; p++)
                    path.RemoveAt(0);
            }

            bReachPoint = false;
        }
        else
            bReachPoint = true;

        return newPos;
    }
}
