using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {

    private const int MOVE_STRAIGHT_COST = 10;//Cài đặt mặc định độ dài chiều ngang khoảng cách 2 ô
    private const int MOVE_DIAGONAL_COST = 14;//Cài đặt mặc định độ dài đường chéo khoảng cách 2 ô

    private bool isWalkDiagonally = true;//Cái này để phân loại 2 kiểu di chuyển có thể chéo và ko thể chéo(tính năng này đang ẩn)

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> grid; 
    private List<PathNode> openList;
    private List<PathNode> closedList;

    private List<PathNode> closedListEnemy;

    public Pathfinding(int width, int height) {//Hàm khởi tạo Pathfinding
        Instance = this;
        grid = new Grid<PathNode>(width, height, 10f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));//Khởi tạo lưới
    }

    public Grid<PathNode> GetGrid() { //Hàm get grid
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) { //Hàm này trả về 1 list vector 3 toạ độ thực
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null) {
            return null;
        } else {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path) {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    // Hàm này trả về 1 list node trong lưới và là đường đi đến đích
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY) { 
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null) { //Đảm bảo có điểm đầu và có điểm cuối, tránh trường hợp bị lỗi
            return null;
        }

        openList = new List<PathNode> { startNode }; //Khởi tạo openList và thêm điểm xuất phát vào openList
        closedList = new List<PathNode>(); //Khởi tạo closedList
        //Vòng for này khởi tạo các giá trị mặc định trong lưới
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        // Tính chi phí đường đi cho startNode
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        
        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList); //Gán node hiện tại bằng node có fCost nhỏ nhất trong openList
            //Kiểm tra nếu node hiện tại là node đích thì trả về đường đi
            if (currentNode == endNode) {
                return CalculatePath(endNode);
            }
            //Chuyển node hiện tại từ openList sang closedList
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //duyệt qua các node trong List node hàng xóm của node hiện tại
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                //Nếu node đó chứa trong closedList hoặc đó là chướng ngại vật thì bỏ qua
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }
                //Tính chi phí đường đi
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    //Thêm vào openList nếu node đó chưa có trong openList
                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        //Nếu openList trống mà chưa tìm được đường đi thì trả về null(ko tìm thấy đường)
        return null;
    }
    //Hàm này trả về 1 đường đi cho enemy
    //
    public List<PathNode> FindPathEnemy(PathNode currentNode)
    {
        List<PathNode> duongDi = new List<PathNode> ();
        closedListEnemy = new List<PathNode>();
        int i = 0;
        while (i < 25)
        {
            if (GetNeighbourEnemy(currentNode) == null) return duongDi;
            
            foreach (PathNode neighbourNode in GetNeighbourEnemy(currentNode))
            {
                if (closedListEnemy.Contains(neighbourNode)) 
                    continue;
                if (!neighbourNode.isWalkable)
                {
                    closedListEnemy.Add(neighbourNode);
                    continue;
                }
                duongDi.Add(neighbourNode);
                closedListEnemy.Add (neighbourNode);
                currentNode = neighbourNode;
                break;
            }
            i++;

        }
        return duongDi;
    }
    //Hàm này tìm hàng xóm của Enemy
    private List<PathNode> GetNeighbourEnemy(PathNode currentNode)
    {
        List<PathNode> neighbourListt = new List<PathNode>();

        // Tạo danh sách chứa tất cả các điều kiện cần chọn ngẫu nhiên
        List<System.Action> randomConditions = new List<System.Action>
    {
        () =>
        {
            if (currentNode.x - 1 >= 0)
            {
                neighbourListt.Add(GetNode(currentNode.x - 1, currentNode.y));
            }
        },
        () =>
        {
            if (currentNode.x + 1 < grid.GetWidth())
            {
                neighbourListt.Add(GetNode(currentNode.x + 1, currentNode.y));
            }
        },
        () =>
        {
            if (currentNode.y - 1 >= 0)
            {
                neighbourListt.Add(GetNode(currentNode.x, currentNode.y - 1));
            }
        },
        () =>
        {
            if (currentNode.y + 1 < grid.GetHeight())
            {
                neighbourListt.Add(GetNode(currentNode.x, currentNode.y + 1));
            }
        }
    };

        // Sử dụng Random để chọn ngẫu nhiên một điều kiện và thực thi nó
        System.Random random = new System.Random();
        int randomIndex = random.Next(randomConditions.Count);
        randomConditions[randomIndex].Invoke();

        return neighbourListt;
    }
    //Hàm này tìm 1 hàng xóm của node hiện tại
    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0 && isWalkDiagonally)
            {
                if (GetNode(currentNode.x - 1, currentNode.y).isWalkable || GetNode(currentNode.x, currentNode.y - 1).isWalkable)
                {
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                }
            }
                
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight() && isWalkDiagonally)
            {
                if (GetNode(currentNode.x - 1, currentNode.y).isWalkable || GetNode(currentNode.x, currentNode.y + 1).isWalkable)
                {
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
                }
            }
                
        }
        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0 && isWalkDiagonally)
            {
                if (GetNode(currentNode.x + 1, currentNode.y).isWalkable || GetNode(currentNode.x, currentNode.y - 1).isWalkable)
                {
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                }
            }
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight() && isWalkDiagonally)
            {
                if (GetNode(currentNode.x + 1, currentNode.y).isWalkable || GetNode(currentNode.x, currentNode.y + 1).isWalkable)
                {
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
                }
            }
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }
    //Cái này chuyển đổi 2 kiểu đi là chéo được và không chéo được
    public void ReDiagonally()
    {
        isWalkDiagonally = !isWalkDiagonally;
    }
    //Cái này lấy toạ độ thực của node trong lưới
    public PathNode GetNode(int x, int y) {
        return grid.GetGridObject(x, y);
    }
    //Trả về đường đi khi chuyền vào node đích tìm được
    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
    //Tính heuristic
    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    //Lấy ra node có fCost thấp nhất
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

}
