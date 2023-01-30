using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ballTrajectory;
    [SerializeField] private GameObject club;
    [SerializeField] private GameObject cam;
    [SerializeField] private Rigidbody ballRigidBody;

    [Header("Ball")]
    [SerializeField] private bool shooted = false;
    [SerializeField] private float ballRotationSpeed = 5f;
    [SerializeField] private float ballForce;
    [SerializeField] private float forceMultiplier = 1f;
    [SerializeField] private float minBallForce = 0.5f;
    [SerializeField] private float maxBallForce = 5f;
    [SerializeField] private float minBallRotation = 0f;
    [SerializeField] private float maxBallRotation = 75f;
    private float ballRotation;


    [Header("Club")]
    [SerializeField] private float clubRotationSpeed = 5f;
    private float clubRotation;

    [Header("Camera")]
    [SerializeField] private float camOffset = 5f;
    private float cameraRotation;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayBallTrajectory();
        this.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        if(shooted == false)
        {
            RotateTheBall();
            RotateTheClub();
            CameraTransform();
            SetBallForce();
        }
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (shooted == false)
            {
                Shot();
            } 
        }
        if(shooted && ballRigidBody.velocity.magnitude == 0)
        {
            shooted = false;
        }
       

    }

    void DisplayBallTrajectory()
    {
        if (ballRigidBody.velocity.magnitude == 0)
        {
            ballTrajectory.SetActive(true);
            club.SetActive(true);
        }    
        else
        {
            ballTrajectory.SetActive(false);
            club.SetActive(false);
        }
            
    }
    void RotateTheBall()
    {
        ballRotation += Input.GetAxisRaw("Horizontal") * Time.deltaTime * ballRotationSpeed;
        transform.rotation = Quaternion.Euler(0, ballRotation, 0);
    }
    void RotateTheClub()
    {
        clubRotation -= Input.GetAxisRaw("Vertical") * Time.deltaTime * clubRotationSpeed;
        clubRotation = Mathf.Clamp(clubRotation, 0, 75f);
        club.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, clubRotation);
    }
    void CameraTransform()
    {
        //cam.transform.rotation = Quaternion.Euler(0, 0, ball);
        //cam.transform.position = 
    }
    void ResetClubRotation()
    {
        clubRotation = 0;
    }
    void SetBallForce()
    {
        ballForce = Map(clubRotation, minBallRotation, maxBallRotation, minBallForce, maxBallForce);
    }
    float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    void Shot()
    {
        float force = ballForce * forceMultiplier;
        ballRigidBody.velocity += force*transform.forward;
        shooted = true;
        ResetClubRotation();
    }
}
