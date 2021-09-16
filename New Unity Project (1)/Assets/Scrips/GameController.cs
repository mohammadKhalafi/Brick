using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;

public class GameController : MonoBehaviour
{
    public AudioClip clip;
    public float volume=0.5f;
    public AudioClip clip1;
    public float volume1=0.5f;
    public Canvas gameOverConvasPrefab;
    public GameObject newBallPrefab;
    public GameObject arrowPrefab;
    public GameObject arrow;
    public Button hohoButton;
    public GameObject ballPrefab;
    public GameObject mainBall;
    public GameObject brickPrefab;
    public TMP_Text scoreText;
    public TMP_Text recordText;
    public int numberOfReturnedBalls = 0;
    public int numballs = 1;
    private float angle = 0;
    public bool isGameRunning = false;
    public int score = 1;
    public int record = 1;
    public List<GameObject> shelllicedBalls = new List<GameObject>();
    public int force = 300;
    public Button backButton;
    public static GameController instance;
    private List<List<GameObject>> bricks = new List<List<GameObject>>();
    List <int> easy = new List<int>();
    List <int> medium = new List<int>();
    List <int> hard = new List<int>();
    List<int> indexes = new List<int>();
    int currentIndex = 0;
    System.Random rng = new System.Random();    
    int numberOfBallsForshoot = 0;
    GameObject tempMainBall;
    float tempMainBallX = 0;
    
    void Start()
    {
        instance = this;
        
        for (int i = 0; i < 8; i++)
        {
            var tempArray = new List<GameObject>();
            for (int j = 0; j < 6; j++)
            {
                tempArray.Add(null);
            }
            bricks.Add(tempArray);
        }
        
        hohoButton.interactable = false;
        BallController.manager = gameObject;

        easy.Add(1);
        easy.Add(1);
        easy.Add(2);
        easy.Add(2);
        easy.Add(3);

        hard.Add(2);
        hard.Add(3);
        hard.Add(5);
        hard.Add(4);
        hard.Add(5);

        medium.Add(1);
        medium.Add(2);
        medium.Add(3);
        medium.Add(4);
        medium.Add(3);

        indexes.Add(0);
        indexes.Add(1);
        indexes.Add(2);
        indexes.Add(3);
        indexes.Add(4);
        indexes.Add(5);

        scoreText.text = score + "";
        record = PlayerPrefs.GetInt ("record");
        recordText.text = PlayerPrefs.GetInt ("record") + "";
        creatNewBricks();
    }
    
    float tempTime;
    
    void FixedUpdate() {

        if(numberOfBallsForshoot != 0 && Time.time > 0.07f + tempTime){
            numberOfBallsForshoot--;
            tempTime = Time.time;
            ThrowTheBall();
        }
    }
      
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            if(canShelllic()){
                shelllic();
            }
        }
    }
    
    void OnDestroy()
    {
        PlayerPrefs.SetInt("record", record);
        PlayerPrefs.Save();
    }

    public void stop(){

        if(!isGameRunning) return;

        numberOfBallsForshoot = 0;

        if(mainBall == null){
            mainBall = Instantiate(ballPrefab, new Vector3(0, -4.2f, 0), new Quaternion(0, 0, 0, 0));
        } else {
            shelllicedBalls.Remove(mainBall);
        }

        foreach(var ball in shelllicedBalls){
            Destroy(ball);
        }

        HandleNextScore();
    }

    private void gameover(){
        Instantiate(gameOverConvasPrefab);
        backButton.interactable = false;
    }
    
    
    public void HandleNextScore()
    {

        bool gameover = false;
        foreach (GameObject gameObject in bricks[1]) 
        {
            if (gameObject != null && gameObject.GetComponent<NewBallController>() == null)
            {
                gameover = true;
                break;
            }
        }

        if (gameover)
        {
            Debug.Log("game over");
            PlayerPrefs.SetInt("record", record);
            PlayerPrefs.Save();
            foreach(var gameobject in bricks[1]){
                if(gameObject != null){
                   Destroy(gameObject.GetComponent<BoxCollider2D>());
                   Destroy(gameObject.GetComponent<CircleCollider2D>());
                }
            }
            shiftBricks();
            isGameRunning = false;
            Invoke("gameover", 0.3f);
        }
        else
        {
            score++;

            foreach(GameObject newBall in NewBallController.balls){
                newBall.GetComponent<NewBallController>().shouldCheck = true;
                try
                {
                    newBall.GetComponent<NewBallController>().GoToMainBall();
                }
                catch (System.Exception ex)
                {
                    Debug.Log("ex");   
                }
                
            }

            foreach(GameObject newBall in bricks[1]){
                if(newBall != null && newBall.GetComponent<NewBallController>() != null){
                    newBall.GetComponent<NewBallController>().shouldCheck = true;
                    try
                    {
                        Destroy(newBall.GetComponent<ThrowDown>());
                        newBall.GetComponent<NewBallController>().GoToMainBall();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.Log("ex");  
                    }
                }
            }
            

            creatNewBricks();
            
            scoreText.text = score + "";

            if(score > record){
                record = score;
                recordText.text = record + "";
            }

            isGameRunning = false;
            NewBallController.balls.Clear();
            shelllicedBalls.Clear();
        }
    }

    private void handleColors(){
        foreach (var list in bricks)
        {
            foreach (GameObject brick in list)
            {
                if(brick != null && brick.GetComponent<BrickNumberController>() != null){
                    handleColor(brick);
                }
                
            }
        }
        
    }

    public void handleColor(GameObject brick){
        int number = int.Parse(brick.GetComponentInChildren<TMP_Text>().text);

        brick.GetComponent<Renderer>().material.color = new Color(236f/255f, getColor(score, number), 61f/255f, 1);

    }

    private float getColor(int max, int num){

        if(max == 1){
            return 0;
        }
        float ans = 255 - ((num - 1) * 255 / (max - 1));
        return ans / 255f;
    }
    
    public List <int> Shuffle (List<int> list)  
    {  
        List<int> ans = new List<int>();

        foreach(int a in list){
            ans.Add(a);
        }

        int n = list.Count;

        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            int value = ans[k];  
            ans[k] = ans[n];  
            ans[n] = value;  
        }

        return ans;
    }

    private void creatNewBricks(){

        var tempArray = bricks[0];
        bricks.RemoveAt(0);
        bricks.Add(tempArray);

        var myList = easy;

        if(score <= 10){
            
        } else if (score <= 20){
            myList = medium;
        } else {
            myList = hard;
        }

        var shuffledIndexes = Shuffle(indexes);

        for(int i = 0; i < myList[currentIndex]; i++){
            GameObject newBrick = Instantiate
            (brickPrefab, GetNewBrickPosition(shuffledIndexes[i]), Quaternion.identity);
            newBrick.GetComponent<BrickNumberController>().number = score;
            newBrick.GetComponentInChildren<TMP_Text>().text = score + "";
            tempArray.Insert(shuffledIndexes[i], newBrick);
        }

        tempArray.Insert(shuffledIndexes[myList[currentIndex]], Instantiate
            (newBallPrefab, GetNewBrickPosition(shuffledIndexes[myList[currentIndex]]),
             Quaternion.identity));

        currentIndex = (currentIndex + 1) % 5;

        shiftBricks();
        handleColors();
    }

    private void shiftBricks(){
        foreach (var list in bricks)
        {
            foreach (GameObject brick in list)
            {
                if(brick != null){
                    if(NewBallController.balls.Contains(brick)){

                    } else {
                        brick.GetComponent<ThrowDown>().MoveDown();
                    }
                }
                
            }
        }
    }

    private Vector3 GetNewBrickPosition(int index)
    {
        return new Vector3(-3.75f + index * 1.5f, 4, 0);
    }
    
    public bool canShelllic(){
        if(isGameRunning) return false;
        if(arrow == null) return false;
        return true;
    }
    
    public void ChangeArrowDirection(float angle)
    {
        if(isGameRunning) return;

        this.angle = angle;
        
        if (arrow == null)
        {
            hohoButton.interactable = true;
            
            arrow = Instantiate(arrowPrefab, new Vector3(mainBall.transform.position.x,-4.15f, 0), 
                new Quaternion(0,0,0,0));
        }

        arrow.GetComponent<Transform>().rotation = Quaternion.Euler(0,0,-angle);
    }


    void shelllic()
    {
        Destroy(arrow);
        tempMainBallX = mainBall.transform.position.x;
        tempMainBall = mainBall;
        mainBall = null;
        tempTime = Time.time;
        numberOfBallsForshoot = numballs;
        isGameRunning = true;
        hohoButton.interactable = false;
    }


    public void ThrowTheBall(){
        GameObject tempBall;
        
        if(tempMainBall == null){
            tempBall = Instantiate(ballPrefab, new Vector3(tempMainBallX, -4.2f, 0), 
                new Quaternion(0, 0, 0, 0));
        } else {
            tempBall = tempMainBall;
            tempMainBall = null;
        }
        
        tempBall.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), 
                Mathf.Cos(angle * Mathf.Deg2Rad)) * force, ForceMode2D.Force);

        shelllicedBalls.Add(tempBall);
    }

    


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void HandleCollectingBalls(Collision2D other, GameObject ball){
        
        if(mainBall == null){
            mainBall = ball;
            ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            ball.transform.position = new Vector3(ball.transform.position.x, -4.2f, 0);
            numberOfReturnedBalls = 1;
        } 
        else {
            ball.GetComponent<BallController>().shouldCheck = true;
            ball.GetComponent<Rigidbody2D>().velocity = 
            new Vector2(-(ball.transform.position.x - mainBall.transform.position.x) * 5, 0);
            numberOfReturnedBalls++;
        }

        if(numberOfReturnedBalls == numballs){
            HandleNextScore();
        }
    }


    public void PlayAudio(){//brick and ball collision
        Invoke("play", 0.3f);
    }

    private void play(){
        GetComponent<AudioSource>().PlayOneShot(clip, volume);
    }


    public void PlayAudio1(){ // ball and new ball collision
        Invoke("play1", 0.3f);
    }

    private void play1(){
        GetComponent<AudioSource>().PlayOneShot(clip1, volume1);
    }
}
