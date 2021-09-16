using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

public class BallController : MonoBehaviour
{
    public static GameObject manager;
    public bool shouldCheck = false;
    
    void FixedUpdate()
    {
        if(shouldCheck){
            if(Math.Abs(gameObject.transform.position.x - getMainBall().transform.position.x) <= 0.2){
                Destroy(gameObject);
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        
        if(other.gameObject.CompareTag("BottomWall")){
            manager.GetComponent<GameController>().HandleCollectingBalls(other, gameObject);
        }
    }
    
    private GameObject getMainBall(){
        return manager.GetComponent<GameController>().mainBall;
    }
}

