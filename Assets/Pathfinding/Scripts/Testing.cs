using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Testing : MonoBehaviour {
    [SerializeField] private CharacterMove characterMove;
    private Pathfinding pathfinding;
    public Pathfinding Pathfinding { get { return pathfinding; } }
    Vector3 endPoint;
    List<PathNode> path;
    bool isDraw = true;
    //2 giá trị độ dài của lưới
    [SerializeField] int heightGrid = 10;
    [SerializeField] int widthGrid = 20;
    public LineRenderer lineRenderer;

    private void Start() {
        pathfinding = new Pathfinding(widthGrid, heightGrid);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (Time.timeScale < 0.1f) return;
            endPoint = UtilsClass.GetMouseWorldPosition();
            characterMove.SetTargetPosition(endPoint);
        }
        drawLine();
        if (Input.GetKeyDown(KeyCode.C))
        {
            isDraw = !isDraw;
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            pathfinding.ReDiagonally();
        }
        
    }
    public void closedPosWall(Vector3 pos)
    {
        pathfinding.GetGrid().GetXY(pos, out int x, out int y);
        pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        if(characterMove!=null && endPoint!= null)
        characterMove.SetTargetPosition(endPoint);

    }
    //Vẽ đường đi
    void drawLine()
    {
        if (!isDraw || characterMove.MoveDir == Vector3.zero)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        Vector3 posCharacterDraw = characterMove.GetPositionCharacter() / pathfinding.GetGrid().GetCellSize();
        Vector3 endPointDraw = endPoint / pathfinding.GetGrid().GetCellSize();
        path = pathfinding.FindPath((int)posCharacterDraw.x, (int)posCharacterDraw.y, (int)endPointDraw.x, (int)endPointDraw.y);

        if (path != null)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f);
            }
        }
    }
    

}
