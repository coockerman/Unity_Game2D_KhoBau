using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private const float speed = 40f;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    Vector3 moveDir;
    public Vector3 MoveDir { get { return moveDir; } }
    private void Start()
    {

    }

    private void Update()
    {
        HandleMovement();

        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition(UtilsClass.GetMouseWorldPosition());
        }
    }
    public Vector2 GetPositionCharacter()
    {
        return transform.position;
    }
    //Xử lý di chuyển
    private void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                moveDir = (targetPosition - transform.position).normalized;

                Debug.Log("Toạ độ di chuyển: " + moveDir);
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
                }
            }
        }
        else
        {
        }
    }
    //Dừng lại
    private void StopMoving()
    {
        pathVectorList = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    //Tìm đường đi cho nhân vật và gán đường đi đó vào pathVectorList
    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
