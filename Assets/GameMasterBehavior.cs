using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// assets
// https://greebles.itch.io/freebie-factory?download
public class GameMasterBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBoard(int mode)
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        GameObject newBall = Instantiate(balls[0]);
        newBall.transform.position = new Vector2(-4.5f, -3f);
        if (mode == 2)
        {
            newBall.GetComponent<BallBehavior>().velocity = balls[0].GetComponent<BallBehavior>().velocity + 0.1f;
        }
        foreach (var ball in balls)
        {
            Destroy(ball);
        }
        Destroy(GameObject.Find("LineRenderer"));

        // reposition robot
        var robot = GameObject.Find("Robot");
        int myInt = (new System.Random()).Next(1, 600);
        float myFloat = (myInt / 100) + -3f;
        robot.transform.position = new Vector2(robot.transform.position.x, myFloat);
    }
}
