using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {

    

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300));

        GUI.Box(new Rect(0, 0, 400, 300), "Credits");
        GUI.Label(new Rect(35, 30, 310, 40), "Your goal is to remove same color cluster of bubbles from the board scoring as many points as possible.");
        GUI.Label(new Rect(35, 70, 310, 40), "You can remove balls forming groups of three or more similarly colored bubbles.");
        GUI.Label(new Rect(35, 110, 310, 40), "Game is over when ends your turns.");
        GUI.Label(new Rect(35, 135, 310, 40), "Aim with the mouse and click to pop a bubble.");
        GUI.Label(new Rect(50, 220, 180, 40), "Created by	                      		Ivliev Dmitriy");
        if (GUI.Button(new Rect(40, 265, 100, 20), "Homepage"))
        {
            Application.OpenURL("http://vk.com/dm_ivliev");
        }

        

        if (GUI.Button(new Rect(260, 265, 100, 20), "Back"))
        {
            Application.LoadLevel("MainMenu");
        }

        GUI.EndGroup();
    }
}
