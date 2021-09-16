using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class BrickNumberController : MonoBehaviour
{
    public int number = 1;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            number--;
            if(number == 0){
                Destroy(gameObject);
            }
            GetComponentInChildren<TMP_Text>().text = number + "";
            GameController.instance.handleColor(gameObject);
            GameController.instance.PlayAudio();
        }
    }
}
