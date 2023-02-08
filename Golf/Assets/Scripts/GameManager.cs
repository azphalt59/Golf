using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;


public class GameManager : MonoBehaviour
{
    public enum GameState { Play, Pause, Menu }
    public GameState State = GameState.Menu;

    bool ballCanRotate = false;
    [SerializeField] private float ballMaxDistance;
    [SerializeField] private float ballDistance;
    [SerializeField] private Vector3 worldMousePos;

    [SerializeField] private Ball ball;
    [SerializeField] private Cam cam;
    [SerializeField] private Camera camGame;

    [Header("Server stuff")]
    public Connection pioconnection;
    private List<Message> msgList = new List<Message>(); //  Messsage queue implementation
    private bool joinedroom = false;
    public GameObject target;
    public GameObject PlayerPrefab;
    private string infomsg = "";

    [Header("Others Players")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private List<GameObject> playersBall;

    

    private void Start()
    {
        Application.runInBackground = true;
        System.Random random = new System.Random();
        string userid = "Guest" + random.Next(0, 10000);
        PlayerIO.Authenticate(
            "multigolf-ooaobu5ukusjnr6qaraaw",            //Your game id
            "public",                               //Your connection id
            new Dictionary<string, string> {        //Authentication arguments
				{ "userId", userid },
            },
            null,                                   //PlayerInsight segments
            delegate (Client client) {
                Debug.Log("Successfully connected to Player.IO");
                infomsg = "Successfully connected to Player.IO";

                target.transform.name = userid;

                Debug.Log("Create ServerEndpoint");
                // Comment out the line below to use the live servers instead of your development server
                client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

                Debug.Log("CreateJoinRoom");
                //Create or join the room 
                client.Multiplayer.CreateJoinRoom(
                    "Golf",                    //Room id. If set to null a random roomid is used
                    "Golf",                   //The room type started on the server
                    true,                               //Should the room be visible in the lobby?
                    null,
                    null,
                    delegate (Connection connection) {
                        Debug.Log("Joined Room");
                        infomsg = "Joined Room.";
                        // We successfully joined a room so set up the message handler
                        pioconnection = connection;
                        pioconnection.OnMessage += handlemessage;
                        joinedroom = true;
                    },
                    delegate (PlayerIOError error) {
                        Debug.Log("Error Joining Room: " + error.ToString());
                        infomsg = error.ToString();
                    }
                );
            },
            delegate (PlayerIOError error) {
                Debug.Log("Error connecting: " + error.ToString());
                infomsg = error.ToString();
            }
        );

    }
    public void FixedUpdate()
    {

        foreach (Message m in msgList)
        {

            switch (m.Type)
            {
                case "BallMove":
                    break;
                case "PlayerJoined":
                    GameObject playerBall = Instantiate(ballPrefab, ball.gameObject.transform.position, Quaternion.identity);
                    playersBall.Add(playerBall);
                    break;

                   
            }

        }
        msgList.Clear();
    }
   
    void handlemessage(object sender, Message m)
    {
        msgList.Add(m);
    }
    private void Update()
    {
        BallShot();
        worldMousePos = camGame.ScreenToWorldPoint(Input.mousePosition);
    }
    
    public void BallShot()
    {
        if (State != GameState.Play) return;
  
        ClickOnBall();
        DrawShotDirection();
        Shot();
         
    }
    public void ClickOnBall()
    {
        if (Input.GetMouseButtonDown(0) && ballCanRotate == false)
        {
            GetDistanceMouseBall();
            ballCanRotate = true;

            if (ballDistance <= ballMaxDistance)
            {
                ball.StartDirectionShot();
            }
        }
    }
    public void DrawShotDirection()
    {
        if (ballCanRotate && Input.GetMouseButton(0))
        {
            if (ballDistance <= ballMaxDistance)
            {
                ball.DrawDirectionShot();
            }
            else
            {
                cam.Rotate(Input.GetAxis("Mouse X"));
            }
        }
    }
    public void Shot()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ballCanRotate = false;
            if (ballDistance <= ballMaxDistance)
            {
                ball.Shot();
            }
        }
    }
    private void GetDistanceMouseBall()
    {
        ballDistance = Vector3.Distance(worldMousePos, ball.transform.position);
    }
}
