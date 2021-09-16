using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static void LoadHomePage(){
        SceneManager.LoadScene("home");
    }

    public static void LoadGamePage(){
        SceneManager.LoadScene("game");
    }
}
