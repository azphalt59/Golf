using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float RotationSpeed = 1f;
    [SerializeField] private GameObject ballTarget;
    private Vector3 offset;
    private Vector3 newPos;
    
    public void Rotate(float xRot)
    {
        transform.Rotate(Vector3.down, -xRot * RotationSpeed);
    }
    public void SetTarget(GameObject ball)
    {
        ballTarget = ball;
        offset = ball.transform.position - transform.position;
        newPos = transform.position;
    }

    private void LateUpdate()
    {
        newPos.x = ballTarget.transform.position.x - offset.x;
        newPos.z = ballTarget.transform.position.z - offset.z;
        transform.position = newPos;
    }
}
