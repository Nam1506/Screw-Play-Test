using DG.Tweening;
using System;
using UnityEngine;

public class ActionMoveParabol : ActionScript
{
    public Transform startPoint; // Position A
    public Transform endPoint;   // Position B
    public float pathHeight = -3f;    // Height of the curve
    public float moveDuration = 0.6f;  // Duration of the movement
    public float distanceFromEnd = 0.8f;
    public Ease moveEase = Ease.OutExpo;

    private Vector3 prePos;

    public void StartAction(Action callback)
    {
        prePos = transform.position;

        float fixedHeight = transform.position.x > endPoint.position.x ? pathHeight : -pathHeight;

        MoveAlongCurvedPath(transform.position, endPoint.position, fixedHeight, moveDuration, callback);
    }

    void OnMoveUpdate()
    {
        Vector3 moveDirection = prePos - transform.position;

        if (moveDirection.magnitude > 0)
        {
            if (moveDirection.x == 0 || moveDirection.y == 0) return;

            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        prePos = transform.position;
    }

    void MoveAlongCurvedPath(Vector3 startPos, Vector3 endPos, float pathHeight, float duration, Action callback)
    {
        Vector3[] controlPoint = GetQuadraticBezierPoints(startPos, endPos, pathHeight);

        transform.DOPath(controlPoint, duration, PathType.CatmullRom)
                 .SetEase(moveEase)
                 .OnUpdate(OnMoveUpdate)
                 .OnComplete(() => callback?.Invoke());
    }

    public Vector3[] GetQuadraticBezierPoints(Vector3 startpoint, Vector3 endTarget, float curveHeight)
    {
        // Tính toán hướng của hai vector
        Vector3 direction = (endTarget - startpoint).normalized;

        // Tính toán một vector phụ hợp dựa trên hướng của hai vector
        Vector3 perpendicularVector = new Vector3(-direction.y, direction.x, 0f); // Vector nằm vuông góc với hướng di chuyển

        // Tính toán điểm đỉnh của đường cong Bézier
        Vector3 heighPoint = startpoint + (endTarget - startpoint) / 2f + perpendicularVector * curveHeight;

        Vector3 midDirection = (endTarget - heighPoint).normalized;

        // Tính toán điểm mới cách endTarget một khoảng distanceFromEnd theo hướng di chuyển ban đầu
        Vector3 newEndTarget = endTarget - midDirection * distanceFromEnd;

        Vector3[] res = new Vector3[100];
        int maxT = 100;
        int index = 0;

        for (float t = 0; t <= maxT; t += 0.01f)
        {
            Vector3 newPoint = (Mathf.Pow(1 - t, 2) * startpoint) + (2 * (1 - t) * t * heighPoint) + (t * t * newEndTarget);
            try
            {
                res[index++] = newPoint;
            }
            catch
            {
                break;
            }
        }

        return res;
    }

    public void ResetMove()
    {
        Vector3 prePos = startPoint.transform.position;
        transform.position = prePos;
        transform.localPosition = new Vector3(transform.localPosition.x, -0.7f, transform.localPosition.z);
        transform.localRotation = Quaternion.identity;
    }

    public void SetEndPoint(Transform endPoint)
    {
        this.endPoint = endPoint;
    }
}