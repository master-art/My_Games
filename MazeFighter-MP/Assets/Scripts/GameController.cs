using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Text timeLabel;
    [SerializeField] private Text scoreLabel;

    PhotonView PV;
    private MazeConstructor generator;

    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;

    private int score;
    private bool goalReached;

    // Use this for initialization

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        //if (!PV.IsMine)
        //{
        //    return;
        //}
            generator = GetComponent<MazeConstructor>();

        // PV.RPC("StartNewGame", RpcTarget.All);
        StartNewGame();


    }

    [PunRPC]
    private void StartNewGame()
    {
        timeLimit = 80;
        reduceLimitBy = 5;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
        //if(PV.IsMine)
        //    PV.RPC("StartNewMaze", RpcTarget.All);
    }

    [PunRPC]
    private void StartNewMaze()
    {
        generator.GenerateNewMaze(13, 15, OnStartTrigger, OnGoalTrigger);

        float x = generator.startCol * generator.hallWidth;
        float y = 1;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);

        goalReached = false;

        player.enabled = true;

        // restart timer
        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {

        if (!player.enabled)
        {
            return;
        }

        int timeUsed = (int)(DateTime.Now - startTime).TotalSeconds;
        int timeLeft = timeLimit - timeUsed;

        if (timeLeft > 0)
        {
            timeLabel.text = timeLeft.ToString();
        }
        else
        {
            timeLabel.text = "TIME UP";
            player.enabled = false;

            photonView.Invoke("StartNewGame", 4);
        }
    }

    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;
        
            score += 1;
            scoreLabel.text = score.ToString();
        
        Destroy(trigger);
    }

    
    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            Debug.Log("Finish!");
            player.enabled = false;

            Invoke("StartNewMaze", 4);
        }
    }
}
