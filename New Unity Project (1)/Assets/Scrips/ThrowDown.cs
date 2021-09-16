using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDown : MonoBehaviour
{
    public float speed = 1;
    public Vector3 targetPosition;

    void Awake()
    {
        targetPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
    }

    public void MoveDown()
    {
        targetPosition = new Vector3(targetPosition.x, targetPosition.y - 1, 0);
    }
}
