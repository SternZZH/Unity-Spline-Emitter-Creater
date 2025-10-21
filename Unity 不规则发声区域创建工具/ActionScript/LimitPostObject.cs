using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�����ɵ�Post���������������
public class LimitPostObject : MonoBehaviour
{
    public MeshCollider boundaryCollider;  // ���������MeshCollider
    public Transform postEventGameobject;         // ��Ҫ�����ƶ�������
    public Transform cameraTransform;      // �����Transform

    void Update()
    {
        if (boundaryCollider == null || postEventGameobject == null || cameraTransform == null)
        {
            return;
        }

        // ��ȡ�����λ�ò�����ΪĿ��λ��
        Vector3 targetPosition = cameraTransform.position;

        // ���Ŀ��λ���Ƿ���MeshCollider�İ�Χ����
        if (IsPositionInsideCollider(targetPosition))
        {
            postEventGameobject.position = targetPosition;
        }
        else
        {
            // ����Post����λ�õ�MeshCollider��
            postEventGameobject.position = GetCorrectedPosition(targetPosition);
        }
    }

    bool IsPositionInsideCollider(Vector3 position)
    {
        // ʹ��MeshCollider.bounds.Contains���а�Χ�м��
        return boundaryCollider.bounds.Contains(position);
    }

    Vector3 GetCorrectedPosition(Vector3 position)
    {
        // ʹ��ClosestPoint�����ҵ�MeshCollider���������Чλ��
        Vector3 closestPoint = boundaryCollider.ClosestPoint(position);
        return closestPoint;
    }
}
