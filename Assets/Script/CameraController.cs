﻿using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : Singleton<CameraController>
{
    public float moveDuration;

    [Header("数据")]
    public SceneData sceneData;

    private float max_X = 9;
    private float max_Y = 5;
    private float min_X = 0;
    private float min_Y = 0;


    public bool canFreeMove;

    public float dragSpeed = 0.01f;
    private Vector3 dragOrigin;
    private bool isDragging = false;


    private void Start()
    {
        if (sceneData.sceneInfoList.Count != 0)
        {
            foreach (var item in sceneData.sceneInfoList)
            {
                UpdateMaxMin(item.X, item.Y);
            }
        }
    }

    void Update()
    {
        if (!canFreeMove) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentPos = Input.mousePosition;
            Vector3 difference = dragOrigin - currentPos;

            // 计算新位置
            Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0) * dragSpeed;

            // 应用阈值限制
            newPosition.x = Mathf.Clamp(newPosition.x, min_X, max_X);
            newPosition.y = Mathf.Clamp(newPosition.y, min_Y, max_Y);

            // 移动摄像机到限制后的位置
            transform.position = newPosition;

            // 更新起始拖动位置
            dragOrigin = currentPos;
        }
    }

    public void MoveCamera(string targetName)
    {
        var targetPos = new Vector2(0, 0);
        foreach (var sceneInfo in sceneData.sceneInfoList)
        {
            if (sceneInfo.Name == targetName)
            {
                targetPos = new Vector2(sceneInfo.X, sceneInfo.Y);
                break;
            }
            else if (targetName == string.Empty)
            {
                break;
            }
        }

        transform.position = new Vector3(targetPos.x, targetPos.y, -10);

        //tartCoroutine(Move(new Vector3(targetPos.x, targetPos.y, -10)));
    }

    //private IEnumerator Move(Vector3 targetPos)
    //{

    //    float result = Vector3.Distance(transform.position, targetPos);
    //    float speed = Mathf.Abs(result) / moveDuration;

    //    // 当距离目标位置还有一定距离时继续移动
    //    while (Vector3.Distance(transform.position, targetPos) > 0.01f)
    //    {
    //        // 使用Vector3.MoveTowards进行平滑移动
    //        transform.position = Vector3.MoveTowards(
    //            transform.position,
    //            targetPos,
    //            speed * Time.deltaTime
    //        );

    //        yield return null;
    //    }

    //    // 确保最终位置精确
    //    transform.position = targetPos;

    //}


    public void UpdateMaxMin(float x, float y)
    {

        max_X = Mathf.Max(max_X, x);
        max_Y = Mathf.Max(max_Y, y);
        min_X = Mathf.Min(min_X, x);
        min_Y = Mathf.Min(min_Y, y);

    }

    
}
