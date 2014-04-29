using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public int pointsPerBubble = 1;                                     //Earn points of each bubble
    public int score = 0;                                               //Total score
    public int turns = 3;                                               //Number of turns
    

    string playerName = "Enter Your Name Please";                       //Player name
    bool isPause = false;                                               //State of pausing game
    bool isBadEnd = false;                                              //State if player dont have enough points for high score table
    bool isGoodEnd = false;                                             //State if player have enough points for high score table
    HighScoreController highScoreScript;                                //High score script


    public void BubblesDestroyed(int bubbles)                           //Earn points for destroyed bubbless
    {
        if (bubbles == 1)
        {
            turns--;
            score += pointsPerBubble;
        }
        if (bubbles >= 3)
        {
            turns += bubbles - 1;
            score += pointsPerBubble * bubbles;
        }
    }

    void Start()
    {
        highScoreScript = GetComponent<HighScoreController>();
    }

    void Update()
    {
        if (turns <= 0)
        {
            
            if (!isGoodEnd && !isBadEnd)
            {
                if (highScoreScript.CheckCurrentScore(score))            //Check points 
                {
                    isGoodEnd = true;
                }
                else isBadEnd = true;
                GamePause();
            }
        }

        
    }

    void GamePause()                                                    //Pausing the game
    {
        if (Time.timeScale != 0.0f)
        {
           
            Time.timeScale = 0.0f;
        }
        else
           
            Time.timeScale = 1.0f;
    }

    
    
    void OnGUI()
    {

        
        

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height * 0.15f), "");

        GUI.Label(new Rect(10, 10, 50, 25), "Score:");
        GUI.Label(new Rect(53, 10, 50, 25), "" + score);

        GUI.Label(new Rect(110, 10, 50, 25), "Turns:");
        GUI.Label(new Rect(160, 10, 50, 25), "" + turns);
        
        if (GUI.Button(new Rect(Screen.width - 50, 10, 50, 25), "Menu"))
        {
            Debug.Log("Pause");
            GamePause();
            isPause = true;
        }

        GUI.EndGroup();
        if (isPause)
        {
            Rect window0 = new Rect(Screen.width/2 - 100, Screen.height/2 - 50, 250, 100);
            window0 = GUI.ModalWindow(0, window0, DoPauseWindow, "Menu");
            
        }

        if (isGoodEnd)
        {
            Rect window0 = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 300, 100);
            window0 = GUI.ModalWindow(0, window0, DoGoodEndWindow, "CONGRATULATIONS!");
        }

        if (isBadEnd)
        {
            Rect window0 = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 300, 100);
            window0 = GUI.ModalWindow(0, window0, DoBadEndWindow, "So sad...");
        }
    }

    void DoPauseWindow(int windowID)
    {
        GUI.Label(new Rect(15, 30, 230, 20), "Do You Want exit to the Main Menu?");
        if (GUI.Button(new Rect(10, 70, 80, 20), "Main Menu"))
        {
            Application.LoadLevel("MainMenu");
        }
        if (GUI.Button(new Rect(160, 70, 80, 20), "Continue"))
        {
            GamePause();
            isPause = false;
        }
    }

    void DoGoodEndWindow(int windowID)
    {
        GUI.Label(new Rect(79, 19, 160, 20), "You have earn " + score + " points!");
        
        playerName = GUI.TextField(new Rect(75, 40, 155, 20), playerName, 25);
        if (GUI.Button(new Rect(122, 68.5f, 62, 20), "Submit"))
        {
            Debug.Log("LOAD HIGH SCORE");
            highScoreScript.SetCurrentScore(score, playerName);
            Application.LoadLevel("HighScore");
        }
    }

    void DoBadEndWindow(int windowID)
    {
        GUI.Label(new Rect(34, 30, 250, 20), "You need a little more to beat a record!");
        if(GUI.Button(new Rect(122, 68.5f, 62, 20), "OK"))
        {
            Debug.Log("Load Main Menu");
            Application.LoadLevel("MainMenu");
        }
    }
}
