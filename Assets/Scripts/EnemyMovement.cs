using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private const float speed = 10f;
    private List<Vector3> pathVectorList;
    private List<PathNode> pathNodeList;
    Vector3 moveDir;

    private int currentPathIndex;

    [SerializeField] Testing testing;

    private void Start()
    {
        if(testing == null)
        {
            testing = FindObjectOfType<Testing>();
        }
        UpdateGrid();
    }
    //Tìm đường đi mới cho enemy 
    void UpdateGrid()
    {
        currentPathIndex = 0;
        pathVectorList = new List<Vector3>();
        pathNodeList = new List<PathNode>();
        testing.Pathfinding.GetGrid().GetXY(transform.position, out int x, out int y);
        PathNode startNode = testing.Pathfinding.GetGrid().GetGridObject(x, y);
        pathNodeList = testing.Pathfinding.FindPathEnemy(startNode);
        for(int i = 0; i < pathNodeList.Count; i++)
        {
            convertPathNodeToVt3();
        }
    }
    private void Update()
    {
        //Nếu pathVectorList khác null thì enemy di chuyển
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                moveDir = Vector3.zero;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                    UpdateGrid();
                }
            }
        }
    }
    //Chuyển đổi đường đi từ lưới sang toạ độ thực
    void convertPathNodeToVt3()
    {
        if (pathNodeList[0] == null) return;
        PathNode pt = pathNodeList[0];
        Debug.Log(pt.ToString());
        Vector3 test = testing.Pathfinding.GetGrid().GetWorldPosition(pt.x, pt.y);
        Vector3 vt3 = new Vector3(test.x + 5, test.y + 5);
        pathNodeList.Remove(pt);
        pathVectorList.Add(vt3);
    }
    //dừng lại
    private void StopMoving()
    {
        Debug.Log("abcdef");
        pathVectorList = null;
    }
}
