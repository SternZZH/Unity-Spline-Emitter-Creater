using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//将生成的Post物体进行区域限制
public class LimitPostObject : MonoBehaviour
{
    public MeshCollider boundaryCollider;  // 区域声域的MeshCollider
    public Transform postEventGameobject;         // 需要限制移动的物体
    public Transform cameraTransform;      // 相机的Transform

    void Update()
    {
        if (boundaryCollider == null || postEventGameobject == null || cameraTransform == null)
        {
            return;
        }

        // 获取相机的位置并设置为目标位置
        Vector3 targetPosition = cameraTransform.position;

        // 检查目标位置是否在MeshCollider的包围盒内
        if (IsPositionInsideCollider(targetPosition))
        {
            postEventGameobject.position = targetPosition;
        }
        else
        {
            // 调整Post物体位置到MeshCollider内
            postEventGameobject.position = GetCorrectedPosition(targetPosition);
        }
    }

    bool IsPositionInsideCollider(Vector3 position)
    {
        // 使用MeshCollider.bounds.Contains进行包围盒检查
        return boundaryCollider.bounds.Contains(position);
    }

    Vector3 GetCorrectedPosition(Vector3 position)
    {
        // 使用ClosestPoint方法找到MeshCollider内最近的有效位置
        Vector3 closestPoint = boundaryCollider.ClosestPoint(position);
        return closestPoint;
    }
}
