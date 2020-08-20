using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float velocity = 0.1f;
    public float maxY = 3f;
    public float minY = -3f;
    public GameObject ball;
    public AudioClip laserHitAudioClip;
    public AudioClip laserMissAudioClip;
    public AudioClip bounceAudioClip;

    private string direction = "up";
    private AudioSource ac;
    private int mode = 0; // 0 - continue, 1 - miss, 2 - score

    // Start is called before the first frame update
    void Start()
    {
        ac = GetComponent<AudioSource>();
        Debug.Log("created ball");
    }

    // Update is called once per frame
    void Update()
    {
        // make this better! (also improve some other things, performance-wise)
        // https://gamedev.stackexchange.com/questions/134002/how-can-i-do-something-after-an-audio-has-finished
        if (mode > 0 && !ac.isPlaying)
        {
            GameObject.Find("GameMaster").GetComponent<GameMasterBehavior>().ResetBoard(mode);
        }

        // handle stop movement
        if (Input.GetKeyDown("space") && direction != "") {
            direction = "";
            
            var balls = GameObject.FindGameObjectsWithTag("Ball");
            int numBalls = balls.Length;
            if (numBalls < 10) {
                GameObject newBall = Instantiate(ball);
                newBall.transform.position = new Vector2(transform.position.x + 1, transform.position.y);
            } else {
                var go = new GameObject();
                var lr = go.AddComponent<LineRenderer>();
                lr.name = "LineRenderer";
                lr.startWidth = 0.5f;
                lr.endWidth = 0.5f;
                lr.material = new Material (Shader.Find ("Sprites/Default"));
                lr.material.color = Color.white;
                var firstBall = balls[0];
                var robot = GameObject.Find("Robot");

                // do linear regression
                // https://www.robosoup.com/linear-regression-c/
                float xAvg = 0;
                float yAvg = 0;
                foreach (var ball in balls)
                {
                    xAvg += ball.transform.position.x;
                    yAvg += ball.transform.position.y;
                }
                xAvg = xAvg / numBalls;
                yAvg = yAvg / numBalls;
                float v1 = 0;
                float v2 = 0;
                foreach (var ball in balls)
                {
                    v1 += (ball.transform.position.x - xAvg) * (ball.transform.position.y - yAvg);
                    v2 += (float) Math.Pow(ball.transform.position.x - xAvg, 2);
                }
                float m = v1 / v2;
                float b = yAvg - m * xAvg;

                float startY = m * -10 + b;
                float endY = m * 10 + b;

                lr.SetPosition(0, new Vector2(-10, startY));
                lr.SetPosition(1, new Vector2(10, endY));

                // hit or miss?
                float robotY = robot.transform.position.y;
                float laserHitCheckY = m * 7 + b;
                if (laserHitCheckY > robotY - 1 && laserHitCheckY < robotY + 1) {
                    ac.PlayOneShot(laserHitAudioClip, 0.7f);
                    GameObject.Find("Score").GetComponent<ScoreBehavior>().AddOneToScore();
                    mode = 2;
                } else {
                    ac.PlayOneShot(laserMissAudioClip, 0.7f);
                    mode = 1;
                }
            }
        }

        // handle movement
        float currentY = transform.position.y;
        if (direction == "up" && currentY >= maxY)
        {
            direction = "down";
            ac.PlayOneShot(bounceAudioClip, 0.7f);
        }
        else if (direction == "down" && currentY <= minY)
        {
            direction = "up";
            ac.PlayOneShot(bounceAudioClip, 0.7f);
        }

        if (direction != "")
        {
            float newY = currentY + (direction == "up" ? velocity : (-velocity));
            transform.position = new Vector2(transform.position.x, newY);
        }
    }
}
