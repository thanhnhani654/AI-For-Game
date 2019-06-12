using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    int gCost;
    int hCost;
    int fCost;

    // Tọa độ của node trong mảng
    public int posX;
    public int posY;

    // Tọa độ của node.
    public Vector2 worldPos;

    // Nhớ node trước đó dẫn tới nó
    public Node Parent;

    public bool IsWall;

    public Node (int x, int y, bool _IsWall)
    {
        posX = x;
        posY = y;
        IsWall = _IsWall;
    }
    public Node()
    { }

    CircleCollider2D collider;

    void Start()
    {
        
    }
}