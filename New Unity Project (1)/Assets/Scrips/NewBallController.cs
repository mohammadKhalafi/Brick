using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewBallController : MonoBehaviour
{

    public static List<GameObject> balls = new List<GameObject>();
    public bool shouldCheck = false;
    bool collected = false;
    bool stopped = false;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Ball") && !collected)
        {
            collected = true;
            GameController.instance.PlayAudio1();
            Destroy(GetComponent<ThrowDown>());
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -7);
            balls.Add(gameObject);
        } else if (other.gameObject.CompareTag("BottomWall") && !stopped){
            stopped = true;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.position = new Vector3(transform.position.x, -4.15f, 0);
        }

    }

    void FixedUpdate()
    {
        if(shouldCheck){
            if(Math.Abs(gameObject.transform.position.x - GameController.instance.mainBall
            .transform.position.x) <= 0.2){
                GameController.instance.numballs++;
                Destroy(gameObject);
            }
        }
    }

    public void GoToMainBall(){
        GetComponent<Rigidbody2D>().velocity = new Vector2 
            (-(transform.position.x - GameController.instance.mainBall.transform.position.x) * 5,
            -(transform.position.y - (-4f)) * 5);
    }

}
