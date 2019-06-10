using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    [System.Serializable]
    public struct SAreaMoveTo
    {
        public Color color;
        public GameObject center;
        public float width;
        public float height;
    }

    public List<SAreaMoveTo> listArea;
    public int centerAreaIndex;
    SAreaMoveTo centerArea;

    public bool SeeNodeOnEditor = true;

    public GameObject NodePrefap;
    [Range(0,100)]
    public int columns = 10;
    [Range(0,100)]
    public int rows = 10;
    [Range(0,2)]
    public float alignment = 2;
    [Range(0,2)]
    public float nodeRadius = 0.5f;

    private Node[,] nodeArrays;
    private List<Node> path;

    public LayerMask ObstacleMask;

    public bool debugFindPath = false;
    public GameObject debugStartPoint;
    public GameObject debugEndPoint;



    void Start()
    {
        nodeArrays = new Node[rows, columns];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
            {
                nodeArrays[i, j] = new Node(j,i, false);

                nodeArrays[i, j].worldPos = new Vector2(this.transform.position.x + j * alignment, this.transform.position.y - i * alignment);
                
                if (Physics2D.OverlapCircle(nodeArrays[i, j].worldPos,nodeRadius, ObstacleMask) != null)
                {
                    nodeArrays[i, j].IsWall = true;
                }
            }

        centerArea = listArea[centerAreaIndex];
    }

    void Update()
    {
        path = FindPath(debugStartPoint.transform.position, debugEndPoint.transform.position);

        

    }

    public List<Node> FindPath(Vector2 startPos, Vector2 endPos)
    {
        //Node startNode = NodeFromWorldPos(startPos);
        //Node endNode = NodeFromWorldPos(endPos);
        List<Node> finalPath = new List<Node>();

        Node startNode = WorldPointToGrid(startPos);
        Node endNode = WorldPointToGrid(endPos);

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(startNode);

        int Count = 0;

        while(OpenList.Count > 0)
        {
            
            Count++;
            Node currentNode = OpenList[0];
            
            for (int i = 0; i < OpenList.Count; i++)
            {
                if (GetFCost(startNode,endNode,currentNode) > GetFCost(startNode, endNode, OpenList[i]))
                {
                    currentNode = OpenList[i];
                }
            }

            if (currentNode.worldPos == endNode.worldPos)
            {
                //Debug.Log("EndNode: " + endNode.Parent.Parent.Parent.posX + " + " + endNode.Parent.Parent.Parent.posY);
                finalPath = GetFinalPath(startNode,endNode);
                return finalPath;
            }
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);
         
            foreach (Node nearNode in GetNearNode(currentNode))
            {
                if (nearNode.IsWall)
                    continue;
                if (ClosedList.Contains(nearNode))
                {
                    continue;
                }

                if (!OpenList.Contains(nearNode))
                {
                    nearNode.Parent = currentNode;
                    OpenList.Add(nearNode);
                }
            }

        }
        return null;
    }

    int GetCost(Node start, Node end)
    {
        int x = Mathf.Abs(end.posX - start.posX);
        int y = Mathf.Abs(end.posY - start.posY);
        
        return x + y;
    }

    int GetFCost(Node start, Node end, Node pos)
    {
        return GetCost(start, pos) + GetCost(end, pos);
    }

    List<Node> GetFinalPath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        int t = 0;
        while (currentNode != startNode)
        {
            t++;
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();

        return path;
    }

    List<Node> GetNearNode(Node node)
    {
        List<Node> nodes = new List<Node>();

        int x;
        int y;

        //top
        x = node.posX;
        y = node.posY - 1;                  //Goc Grid nam o goc tren ben trai
        if (x >= 0 && x < columns)
            if (y >= 0 && y < rows)
            {          
                nodes.Add(nodeArrays[y, x]);
            }

        //bottom
        x = node.posX;
        y = node.posY + 1;
        if (x >= 0 && x < columns)
            if (y >= 0 && y < rows)
            {
                nodes.Add(nodeArrays[y, x]);
            }

        //left
        x = node.posX - 1;
        y = node.posY;
        if (x >= 0 && x < columns)
            if (y >= 0 && y < rows)
            {
                nodes.Add(nodeArrays[y, x]);
            }

        //right
        x = node.posX + 1;
        y = node.posY;
        if (x >= 0 && x < columns)
            if (y >= 0 && y < rows)
            {
                nodes.Add(nodeArrays[y, x]);
            }
        return nodes;
    }
   
    Node WorldPointToGrid(Vector2 point)
    {
        int x = Mathf.RoundToInt((point.x - this.transform.position.x) / ( alignment));
        int y = Mathf.RoundToInt(Mathf.Abs(point.y - this.transform.position.y) / (alignment));

        if (x >= 0 && x < columns)
            if (y >= 0 && y < rows)
            {
                return nodeArrays[y,x];
            }
        
        return nodeArrays[0, 0];
    }

    SAreaMoveTo GetRandomArea(Vector3 pos)
    {
        int rand = listArea.Count;
        int i;
        for (i = 0; i < listArea.Count; i++)
        {
            if (IsInArea(pos, listArea[i]))
            {
                rand--;
                continue;
            }
            if (Random.Range(0, rand - 1) == 0)
            {
                break;
            }
            rand--;
        }
        return listArea[i];
    }

    bool IsInArea(Vector3 pos, SAreaMoveTo area)
    {
        if (pos.x < area.center.transform.position.x - area.width / 2 ||
            pos.x > area.center.transform.position.x + area.width / 2 ||
            pos.y < area.center.transform.position.y - area.height / 2 ||
            pos.y < area.center.transform.position.y - area.height / 2)
            return false;
        return true;
    }

    public Vector2 GetRandomPositionInArea(SAreaMoveTo area)
    {
        float x = Random.Range(area.center.transform.position.x - area.width / 2, area.center.transform.position.x + area.width / 2);
        float y = Random.Range(area.center.transform.position.y - area.height / 2, area.center.transform.position.y + area.height / 2);

        return new Vector2(x, y);
    }

    public Vector2 GetRandomPostion(Vector3 pos)
    {
        return GetRandomPositionInArea(GetRandomArea(pos));
    }

    public Vector2 GetRandomPositionInCenterArea()
    {
        float x = Random.Range(centerArea.center.transform.position.x - centerArea.width / 2, centerArea.center.transform.position.x + centerArea.width / 2);
        float y = Random.Range(centerArea.center.transform.position.y - centerArea.height / 2, centerArea.center.transform.position.y + centerArea.height / 2);

        return new Vector2(x, y);
    }
}
