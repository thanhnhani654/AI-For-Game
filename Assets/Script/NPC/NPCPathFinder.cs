using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCPathFinder {

    public GameObject pathFinder;

    Grid grid;
    public List<Node> path;
    Vector2 targetPoint;

    //public LayerMask obstacleMask;

    public GameObject debugEndPoint;
    public bool debug = true;
    public bool debugDrawPath = true;
    public bool debugDrawPredictLine = true;

    public bool bReachPoint;
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

    public Vector3 FollowPath(Vector3 currentPos, float speed, LayerMask obstacleMask)
    {
        Vector3 newPos = currentPos;
        if (debugEndPoint != null && debug)
        {
            path = grid.FindPath(new Vector2(currentPos.x, currentPos.y), new Vector2(debugEndPoint.transform.position.x, debugEndPoint.transform.position.y));
        }
        else if (targetPoint == null)
            FindNewPath(currentPos);
        else if (targetPoint != null && bReachPoint)
            path = grid.FindPath(new Vector2(currentPos.x, currentPos.y), targetPoint);

        if (path != null && path.Count > 0)
        {
            int i = 0;
            Vector2 dir = new Vector2(0, 0);
            for (i = path.Count - 1; i > 0; i--)
            {
                dir = new Vector2(path[i].worldPos.x - currentPos.x, path[i].worldPos.y - currentPos.y);
                RaycastHit2D hit = Physics2D.Raycast(currentPos, new Vector2(path[i].worldPos.x - currentPos.x, path[i].worldPos.y - currentPos.y), Vector2.Distance(path[i].worldPos, new Vector2(currentPos.x, currentPos.y)), obstacleMask);
                if (hit.collider == null)
                {
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

        if (debugDrawPath)
            if (path != null)
            {
                for (int i = 1; i < path.Count; i++)
                    Debug.DrawLine(path[i - 1].worldPos, path[i].worldPos, Color.red);
            }

        return newPos;
    }

    public void FindNewPath(Vector3 currentPos)
    {
        targetPoint = grid.GetRandomPositionInCenterArea();
    }

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
