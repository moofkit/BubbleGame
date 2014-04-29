using UnityEngine;
using System.Collections;

public class HighScoreController : MonoBehaviour
{

    public TextAsset highScoreCSV;					//CSV File (.txt extension)
    public Score[] highScore;						//Array of Score
    public bool isScoreScene;                       //Switch Gameplay/HighScore scene Logic in the inspector. (if it's Gameplay scene then must be false)
    
    int tableLength = 5;							//Default high score table length. (if change then change CSV file first)
	int higlightRow;
    
    public class Score								//Score class
    {

        public int score;							//Score value
        public string name;							//Name of the player
        public string date;							//Date of the record score


    }

    void OnGUI()
    {
        if (isScoreScene)                                                                       //Checking is it high score scene
        {
			GUI.BeginGroup(new Rect(Screen.width/2 - 250, Screen.height/2 - 300/2, 500, tableLength*25 + 85), "");
			GUI.Box(new Rect(0, 0, 500, tableLength*25 + 85), "High Score");
			GUI.Label(new Rect(10, 30, 35, 20), "Place");
			GUI.Label(new Rect(120, 30, 35, 20), "Name");
			GUI.Label(new Rect(320, 30, 35, 20), "Score");
			GUI.Label(new Rect(400, 30, 35, 20), "Date");

			for(int i = 0; i < tableLength; i++)
			{
				Color guiColor = GUI.color;                                                     //Highlight the record
				if(i == higlightRow){
					GUI.color = Color.red;
				}

				GUI.Label(new Rect(23, 55 + i*25, 35, 20), "" + (i+1));

				GUI.Label(new Rect(120, 55 + i*25, 155, 20), highScore[i].name);

				GUI.Label(new Rect(320, 55 + i*25, 155, 20), "" + highScore[i].score);

				GUI.Label(new Rect(400, 55 + i*25, 155, 20), highScore[i].date);

				if(i == higlightRow){
					GUI.color = guiColor;
				}
			}

			if(GUI.Button(new Rect(500 / 2 - 44, 55 + tableLength*25, 82, 20), "Main Menu")){
				
				PlayerPrefs.DeleteKey("HiglightRow");
				Application.LoadLevel("MainMenu");
			}
			GUI.EndGroup();
        }
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();                                                        //Delete Prefs for testing
        GetHighScore();
		if(PlayerPrefs.HasKey("HiglightRow")){
			higlightRow= PlayerPrefs.GetInt("HiglightRow");
		} 
		else 
		{
			higlightRow = tableLength + 1;
		}
        													      
    }

    public bool CheckCurrentScore(int playerScore)                                        //Does current score better then high score table
    {

        for (int i = 0; i < tableLength; i++)
        {
            if (playerScore >= highScore[i].score)
            {
                Debug.Log(playerScore + " more then " + highScore[i].score);
                return true;  
            }
        }
        return false;
    }

    public void SetCurrentScore(int playerScore, string playerName)
    {
        Score currentScore = new Score();										//Init current score								
        currentScore.name = playerName;
        currentScore.score = playerScore;
        currentScore.date = System.DateTime.Now.Date.ToString("dd.MM.yyyy");

		bool isFirst = true;
        for (int i = 0; i < tableLength; i++)
        {


            if (currentScore.score >= highScore[i].score)
            {
				if(isFirst) {
					PlayerPrefs.SetInt("HiglightRow", i);
					isFirst = false;
				}

                Score tempScore = new Score();
                tempScore = highScore[i];											//Swap score
                highScore[i] = currentScore;
                currentScore = tempScore;
            }

          }

        for (int i = 0; i < tableLength; i++)                                  //Set prefs
        {														

            PlayerPrefs.SetString("Player Name" + i, highScore[i].name);

            PlayerPrefs.SetInt("Score" + i, highScore[i].score);

            PlayerPrefs.SetString("Date" + i, highScore[i].date);
        }
    }

    void GetHighScore()
    {
        if (!PlayerPrefsX.GetBool("HighScore")) 									//Check the flag
        {
            GetHighScoreCSV();													//Fill the Score array with default table from CSV
        }
        else
        {
            GetHighScorePrefs();												//Fill the Score array with prefs values
        }

       
    }

    /// <summary>
    /// Gets the high score from  CSV file.
    /// </summary>
    void GetHighScoreCSV()
    {
        string[,] highScoreStr = CSVReader.SplitCsvGrid(highScoreCSV.text);		//String array buffer 
        int columns = highScoreStr.GetLength(0) - 2; 								//Some parsing error
        highScore = new Score[columns];

        PlayerPrefsX.SetBool("HighScore", true);									//Put flag that there are a high score in prefs

        for (int i = 0; i < columns; i++)
        {											//Fill the Score array			
            highScore[i] = new Score();
            highScore[i].name = highScoreStr[i, 0];
            PlayerPrefs.SetString("Player Name" + i, highScore[i].name);
            

            highScore[i].score = int.Parse(highScoreStr[i, 1]);
            PlayerPrefs.SetInt("Score" + i, highScore[i].score);
            

            highScore[i].date = highScoreStr[i, 2];
            
            PlayerPrefs.SetString("Date" + i, highScore[i].date);
        }


    }

    /// <summary>
    /// Gets the high score from prefs.
    /// </summary>
    void GetHighScorePrefs()
    {
        Debug.Log("Read From prefs");

        highScore = new Score[tableLength];
        for (int i = 0; i < tableLength; i++)
        {
            highScore[i] = new Score();

            highScore[i].name = PlayerPrefs.GetString("Player Name" + i);
            

            highScore[i].score = PlayerPrefs.GetInt("Score" + i);
            

            highScore[i].date = PlayerPrefs.GetString("Date" + i);
           

        }
    }

}



