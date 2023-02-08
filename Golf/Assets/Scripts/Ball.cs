using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float forceMultiplier = 1f;
    [SerializeField] private float forceMax = 100f;
    private float force;
    private Rigidbody rb;

    private Vector3 dir;
    private Vector3 mouseStartPos;
    private Vector3 mouseEndPos;
    private Vector3 lastBallPosition;
    [SerializeField] private LineRenderer shotDir;

    private bool shootable = false;
    private bool isMoving = false;
    private bool inHole = false;
    [SerializeField] private Camera camGame;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        //gameManager.pioconnection.Send("BallMove", transform.position.x, transform.position.y, transform.position.z);
    }
    private void Update()
    {
        if (rb.velocity == Vector3.zero && isMoving == true)
        {
            isMoving = false;
            SaveBallPosition(transform.position);
        }
        if(isMoving == true)
        {
            gameManager.pioconnection.Send("BallMove", transform.position.x, transform.position.y, transform.position.z);
        }
    }
    
    private void FixedUpdate()
    {
        if(shootable)
        {
            shootable = false;
            isMoving = true;
            dir = mouseStartPos - mouseEndPos;
            rb.AddForce(dir * force, ForceMode.Impulse);
            Reset();
            
        }
        
    }

    private void Reset()
    {
        force = 0;
        mouseEndPos = Vector3.zero;
        mouseStartPos = Vector3.zero;
    }

    private void SaveBallPosition(Vector3 position)
    {
        lastBallPosition = position;
    }

    private Vector3 MousePos()
    {
        Ray ray = camGame.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        Vector3 mousePos = Vector3.zero;
        if(Physics.Raycast(ray, out hit, float.MaxValue))
        {
            mousePos = hit.point;
        }
        return mousePos;
    }

    public void StartDirectionShot()
    {
        if (isMoving == true)
        {
            return;
        }
        mouseStartPos = MousePos();
        shotDir.gameObject.SetActive(true);
        shotDir.SetPosition(0, shotDir.transform.localPosition +new Vector3(0, 0.01f, 0));
    }
    public void DrawDirectionShot()
    {
        if (isMoving == true)
        {
            return;
        }
        mouseEndPos = MousePos();
        mouseStartPos.y = transform.position.y;
        mouseEndPos.y = transform.position.y;
        float forceValue = Vector3.Distance(mouseEndPos, mouseStartPos);
        force = Mathf.Clamp(forceValue * forceMultiplier, 0, forceMax);
        shotDir.SetPosition(1, transform.InverseTransformPoint(mouseEndPos) + new Vector3(0,0.01f,0));
    }
    public void Shot()
    {
        if(isMoving == true)
        {
            return;
        }
        shootable = true;
        shotDir.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Hole>() != null)
        {
            Debug.Log("Win");
            if(other.GetComponent<Hole>().NewBallTransform != null)
            {
                rb.velocity = Vector3.zero;
                inHole = true;
                other.GetComponent<Hole>().SetBallPosition();
            }
            
        }
        if (other.GetComponent<OutMap>() != null)
        {
            rb.velocity = Vector3.zero;
            transform.position = lastBallPosition;
        }
    }
}
