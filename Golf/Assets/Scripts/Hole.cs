using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Transform NewBallTransform;
    [SerializeField] private Ball ball;

    public void SetBallPosition()
    {
        ball.gameObject.transform.position = NewBallTransform.position;
    }
}
