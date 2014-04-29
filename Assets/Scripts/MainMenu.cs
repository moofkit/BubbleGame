using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    
    void OnGUI()
    {
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 75, 100, 175));

        GUI.Box(new Rect(0, 0, 100, 175), "Main Menu");

        if (GUI.Button(new Rect(10, 30, 80, 30), "Start Game!"))
        {
            Application.LoadLevel("Gameplay");
        }

        
        if (GUI.Button(new Rect(10, 65, 80, 30), "High Score"))
        {
            Application.LoadLevel("HighScore");
        }

       
        if (GUI.Button(new Rect(10, 100, 80, 30), "Credits"))
        {
            Application.LoadLevel("Credits");
        }

        if (GUI.Button(new Rect(10, 135, 80, 30), "Exit"))
        {
           Application.Quit();
        }

        GUI.EndGroup();
    }
}
