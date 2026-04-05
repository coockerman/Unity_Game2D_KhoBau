using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    private Grid<PathNode> grid;
    //Toạ độ lưới
    public int x;
    public int y;

    //Chi phí đường đi
    public int gCost;
    public int hCost;
    public int fCost;

    //Có phải là tường hay không
    public bool isWalkable;

    //node trước đó
    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }
    //Tính FCost
    public void CalculateFCost() {
        fCost = gCost + hCost;
    }
    //Set tường
    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }
    //Trả về vị trí là 1 string
    public override string ToString() {
        return x + "," + y;
    }

}
