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

    // Danh sách các khu vực mà NPC có thể đi tới
    public List<SAreaMoveTo> listArea;
    // Vị trí khu vực giữa 2 team ở trong mảng.
    public int centerAreaIndex;
    // Lưu trữ riêng khu vực chính giữa 2 team
    SAreaMoveTo centerArea;

    // Dùng để Debug kiểm tra các Node trên Editor
    public bool SeeNodeOnEditor = true;

    // Lấy Prefap Node mẫu
    public GameObject NodePrefap;

    [Range(0,100)]
    public int columns = 10;    // Số lượng Node theo hàng ngang
    [Range(0,100)]
    public int rows = 10;       // Số lượng Node theo hàng dọc
    [Range(0,2)]
    public float alignment = 2; // Khoảng cách giữa các Node
    [Range(0,2)]
    public float nodeRadius = 0.5f;     // Bán kính của Node

    private Node[,] nodeArrays;     // Mảng Lưu trữ Node
    private List<Node> path;        // Danh sách các Node của đường đi ngắn nhất

    public LayerMask ObstacleMask;  // Lưu trữ Layer của vật cản để xét các node nằm trong vật cản mà tránh

    public bool debugFindPath = false;  // Test Tìm đường
    public GameObject debugStartPoint;  // Điểm bắt đầu test
    public GameObject debugEndPoint;    // Điểm kết thúc

    void Start()
    {
        nodeArrays = new Node[rows, columns];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
            {
                nodeArrays[i, j] = new Node(j,i, false);

                nodeArrays[i, j].worldPos = new Vector2(this.transform.position.x + j * alignment, this.transform.position.y - i * alignment);
                
                // Node nào dính vào vật cản thì coi như node đó là tường
                if (Physics2D.OverlapCircle(nodeArrays[i, j].worldPos,nodeRadius, ObstacleMask) != null)
                {
                    nodeArrays[i, j].IsWall = true;
                }
            }

        // Lưu trữ riêng khu vực giữa 2 team nơi cả 2 team đều đi tới
        centerArea = listArea[centerAreaIndex];
    }

    void Update()
    {
        path = FindPath(debugStartPoint.transform.position, debugEndPoint.transform.position);

        

    }

    // Tìm đường đi ngắn nhất với đầu vào là điểm bắt đầu và điểm kết thúc
    public List<Node> FindPath(Vector2 startPos, Vector2 endPos)
    {
        // Khởi tạo danh sách các node của đường đi ngắn nhất (mới tạo dánh sách, chưa có gì hết)
        List<Node> finalPath = new List<Node>();

        // Chuyển tọa độ của điểm bắt đầu và điểm kết thúc về 2 node gần nhất
        Node startNode = WorldPointToGrid(startPos);
        Node endNode = WorldPointToGrid(endPos);

        // Khởi tạo danh sách chứa các node sẽ được xem xét tới để tìm đường đi ngắn nhất
        List<Node> OpenList = new List<Node>();
        // Khởi tạo danh sách chứa các node không cần xem xét nữa
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(startNode);

        int Count = 0;

        // Từ đây làm lâu quá nên quên rồi :V
        // Mà cũng vì vậy nên các hàm liên quan trong đây cũng cố xem tự hiểu tác dụng nhe :V
        // Cố xem ở đây để hiểu: https://en.wikipedia.org/wiki/A*_search_algorithm
        while (OpenList.Count > 0)
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

    // Chọn một khu vực ngẫu nhiên trong danh sách
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

    // Kiểm tra vị trí đó có nằm bên trong khu vực đó hay không
    bool IsInArea(Vector3 pos, SAreaMoveTo area)
    {
        if (pos.x < area.center.transform.position.x - area.width / 2 ||
            pos.x > area.center.transform.position.x + area.width / 2 ||
            pos.y < area.center.transform.position.y - area.height / 2 ||
            pos.y < area.center.transform.position.y - area.height / 2)
            return false;
        return true;
    }

    // Lấy vị trí ngẫu nhiên trong khu vực
    public Vector2 GetRandomPositionInArea(SAreaMoveTo area)
    {
        float x = Random.Range(area.center.transform.position.x - area.width / 2, area.center.transform.position.x + area.width / 2);
        float y = Random.Range(area.center.transform.position.y - area.height / 2, area.center.transform.position.y + area.height / 2);

        return new Vector2(x, y);
    }

    // Lấy vị trí ngẫu nhiên trong khu vực ngẫu nhiên
    public Vector2 GetRandomPostion(Vector3 pos)
    {
        return GetRandomPositionInArea(GetRandomArea(pos));
    }

    // Lấy vị trí ngẫy nhiên trong khu vực chính giữa
    public Vector2 GetRandomPositionInCenterArea()
    {
        float x = Random.Range(centerArea.center.transform.position.x - centerArea.width / 2, centerArea.center.transform.position.x + centerArea.width / 2);
        float y = Random.Range(centerArea.center.transform.position.y - centerArea.height / 2, centerArea.center.transform.position.y + centerArea.height / 2);

        return new Vector2(x, y);
    }
}
