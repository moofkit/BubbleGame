using UnityEngine;
using System.Collections;

public class BubbleMatrix : MonoBehaviour
{
    //Private
    GameObject[,] bubbleMatrix;				//The matrix of the bubbles
    bool isScaning;							//Is scaning statement
    GameController gameControllerScript;    //Game controller script
    bool isFirstTurn = true;                //Indicates that its first player turn
    bool isScanFound = false;               //Scan found smth

    //Public
    public int rows = 3;					//Number of the rows
    public int columns = 3;					//Number of the columns
    public GameObject bubblePrefab;			//Bubble prefab
    public GameObject socket;				//The start point (0,0) of the bubble matrix
    public GameObject bubbleBlast;          //Partice system prefab
    public GameObject shinePrefab;          //Shine effect prefab
    public float bubbleRadius = 0.5f;		//The radius of the bubbles
    public float bubbleInterval = 2f;		//Interval between the bubbles
    public float bubbleFadeTime = 1.0f;		//The time of fading bubbles
    public float bubbleMovingSpeed = 1.0f;	//Speed of moving bubbles
    public float bubbleScalingSpeed = 1.0f; //Speed of scaling bubbles
    public float scaningDelay = 1.0f;		//Time in sec
    
    public Color[] colorStack;				//The pool of the random colors for bubbles



    void Start()
    {
        GenMatrix();

        StartCoroutine(FirstStartScan());
        isScaning = true;
        gameControllerScript = GetComponent<GameController>();
    }

    void Update()                                                                                                                       //TODO: THink about not Update method
    {


        if (!isScaning && !BubblesBusy())
        {
            isScanFound = Scan();
        }

        if (isFirstTurn && !isScaning && !BubblesBusy() && !isScanFound)                                                                //Need to optimaze that
        {
            Debug.Log("First turn");
            isFirstTurn = false;
            FirstTurnScan();

        }
    }

    IEnumerator FirstStartScan()                                                                                                        //Need some delay before start scaning
    {						                                                                                                            //Delay after load level and begin scaning
        yield return new WaitForSeconds(scaningDelay);
        if (!BubblesBusy())
        {
            Scan();
            isScaning = false;
        }
    }

    /// <summary>
    /// Returns true if found cluster of same color
    /// </summary>
    /// <returns></returns>
    bool Scan()
    {										                                                                                            //Just a scan method
        isScaning = true;

        bool clusterFoundColumns = false;
        bool clusterFoundRows = false;

        clusterFoundColumns = ScanColumns();
        if (!clusterFoundColumns)
        {
            clusterFoundRows = ScanRows();

        }
        isScaning = false;

        if (clusterFoundColumns || clusterFoundRows) return true;

        return false;
    }

    /// <summary>
    /// Bubbleses  busy.
    /// </summary>
    /// <returns>
    /// True if bubbles are busy (moving or scaling)
    /// </returns>
    bool BubblesBusy()                                                                                                                  //Check bubbles. Moving or scaling 
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                BubbleController bubbleScript = bubbleMatrix[i, j].GetComponent<BubbleController>();
                if (bubbleScript.isMoving || bubbleScript.isScaling)
                {
                    return true;
                }
            }
        }
        return false;
    }

   

    /// <summary>
    /// Gens the matrix.
    /// </summary>
    void GenMatrix()
    {
        bubbleMatrix = new GameObject[columns, rows];
                                                                                                                                        //Filling the matrix
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject someBubble = (GameObject)Instantiate(bubblePrefab, socket.transform.position, socket.transform.rotation);	//Important for right position in the global space
                float scale = bubbleRadius / 0.5f; 																						//0.5f is radius of collider by default
                someBubble.transform.localScale = new Vector3(scale, scale, scale);
                someBubble.transform.parent = socket.transform;
                Vector3 pos = new Vector3(i * (bubbleRadius * 2 + bubbleInterval), j * (bubbleRadius * 2 + bubbleInterval));			//Position in the local space
                someBubble.transform.localPosition = pos;
                bubbleMatrix[i, j] = someBubble;

            }
        }
        //Generates the random color
        foreach (GameObject bub in bubbleMatrix)
        {
            bub.renderer.material.color = RandColor();
        }

    }

    /// <summary>
    /// Rands the color.
    /// </summary>
    /// <returns>
    /// The color.
    /// </returns>
    Color RandColor()
    {
        int len = colorStack.Length;
        return colorStack[Random.Range(0, len)];
    }

    /// <summary>
    /// Raises the bubble click event.
    /// </summary>
    public void OnBubbleClick(GameObject bubble)
    {
        //Debug.Log(bubble.transform.localPosition);
        //Debug.Log(position(bubble));
        if (!isScaning && !BubblesBusy())
        {
            int row = Mathf.RoundToInt(position(bubble).y);
            int column = Mathf.RoundToInt(position(bubble).x);
            DelColumnCluster(row, row, column, bubbleMatrix);
        }

    }

    /// <summary>
    /// Return the position of the bubble
    /// </summary>
    /// <param name='bubble'>
    /// Bubble.
    /// </param>
    Vector2 position(GameObject bubble)
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (bubble == bubbleMatrix[i, j])
                {
                    return new Vector2(i, j);
                }
            }
        }
        return new Vector2(-1, -1);							                                                                            //if smth wrong return -1 -1
    }

    /// <summary>
    /// Scans the columns.
    /// </summary>
    /// <returns>
    /// True if there are some samecolor cluster
    /// </returns>
    bool ScanColumns()
    {
        for (int i = 0; i < columns; i++)
        {
            Color currentColor = bubbleMatrix[i, rows - 1].renderer.material.color;
            int count = 0;

            for (int j = rows - 1; j >= 0; j--)
            {
                if (bubbleMatrix[i, j].renderer.material.color == currentColor)
                {
                    count++;
                }
                else
                {
                    if (count > 2)
                    {
                        Debug.Log("rows from " + (j + 1) + " to" + (j + count) + " in column " + i);
                        bubbleMatrix = DelColumnCluster(j + 1, j + count, i, bubbleMatrix);
                        
                        return true;
                    }

                    count = 1;
                    currentColor = bubbleMatrix[i, j].renderer.material.color;
                }

            }
            if (count > 2)
            {
                Debug.Log("rows from " + 0 + " to" + count + " in column " + i);
                bubbleMatrix = DelColumnCluster(0, count-1, i, bubbleMatrix);
                return true;
            }
     
        }

        return false;
    }

    /// <summary>
    /// Scan the rows for same color clusters. 
    /// return true if there are samecolor clusters 
    /// </summary>
    bool ScanRows()
    {
        for (int i = rows - 1; i >= 0; i--)
        {
            Color currentColor = bubbleMatrix[0, i].renderer.material.color;
            int count = 0;

            for (int j = 0; j < columns; j++)
            {

                if (bubbleMatrix[j, i].renderer.material.color == currentColor)
                {		                                                                                                                //serching same color clusters
                    count++;

                }
                else
                {																                                                        //this bubble is not same color that previous one
                    if (count > 2)  
                    {														                                                            //checking length of the cluster

                        bubbleMatrix = DelRowCluster(j - count, j - 1, i, bubbleMatrix);	                                            //Delete the row cluster, shift the bubbles under the cluster down, generate new cluster on the top of the matrix
                        //count = -1;														                                            //From this point we should start checking from begin
                        return true;

                    }
                    count = 1;
                    currentColor = bubbleMatrix[j, i].renderer.material.color;
                }


            }
            if (count > 2)
            {
                bubbleMatrix = DelRowCluster(columns - count, columns - 1, i, bubbleMatrix);
                return true;
            }


        }
        return false;
    }

    /// <summary>
    /// Dels the row cluster.
    /// </summary>
    /// <returns>
    /// The same size bubble matrix
    /// </returns>
    /// <param name='_from'>
    /// Number from column
    /// </param>
    /// <param name='_to'>
    /// Number till column
    /// </param>
    /// <param name='row'>
    /// The row number
    /// </param>
    /// <param name='_matrix'>
    /// The bubble matrix
    /// </param>
    GameObject[,] DelRowCluster(int _from, int _to, int row, GameObject[,] _matrix)
    {

        int count = _to - _from + 1;				                                                                                        //lenght of the cluster

        gameControllerScript.BubblesDestroyed(count);

        GameObject[] buffer = new GameObject[count];

        Debug.Log("columns from " + _from + " to " + _to + " row " + row);

        int a = 0;																				                                            //buffer counter
        for (int i = _from; i <= _to; i++)
        {														                                                                            //Fill the buffer
            buffer[a] = _matrix[i, row];

            GameObject someBlast = (GameObject)Instantiate(bubbleBlast, buffer[a].transform.position, buffer[a].transform.rotation);        //TODO: Make colors of blast same as bubbles
            //someBlast.particleSystem.renderer.material.color = buffer[a].renderer.material.color;                                                      //Don't work http://answers.unity3d.com/questions/347675/how-to-change-particle-systems-color-over-lifetime.html
            //omeBlast.particleSystem.Play();
            //buffer[a].renderer.enabled = false;												                                            //Hide cluster of bubbles
            BubbleController bubbleScript = buffer[a].GetComponent<BubbleController>();
            bubbleScript.Hide(bubbleFadeTime);

           
            a++;
        }

        //IMPORTANT i > 0 exclusive 0;
        for (int i = row; i > 0; i--)
        {
            for (int j = _from; j <= _to; j++)
            {
                _matrix[j, i] = _matrix[j, i - 1];

                Vector3 pos = new Vector3(j * (bubbleRadius * 2 + bubbleInterval), i * (bubbleRadius * 2 + bubbleInterval));
                BubbleController bubbleScript = _matrix[j, i].GetComponent<BubbleController>();				                                //relocate the bubbles																		
                bubbleScript.targetPos = pos;																			
                bubbleScript.isMoving = true;
                bubbleScript.speedOfMoving = bubbleMovingSpeed;
                //_matrix[j,i].transform.localPosition = pos;
            }
        }

        a = 0;
        for (int i = _from; i <= _to; i++)
        {														                                                                            //realase the buffer

            _matrix[i, 0] = buffer[a];
            Vector3 pos = new Vector3(i * (bubbleRadius * 2 + bubbleInterval), 0.0f);				                                        //Position in the local space
            _matrix[i, 0].transform.localPosition = pos;
            _matrix[i, 0].renderer.material.color = RandColor();

            BubbleController bubbleScript = _matrix[i, 0].GetComponent<BubbleController>();
            _matrix[i, 0].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            bubbleScript.speedOfScaling = bubbleScalingSpeed;
            bubbleScript.targetRadius = bubbleRadius;
            bubbleScript.isScaling = true;
            //_matrix[i,0].renderer.enabled = true;
            a++;
        }

        return _matrix;
    }

    /// <summary>
    /// Dels the column cluster.
    /// </summary>
    /// <returns>
    /// The column cluster.
    /// </returns>
    /// <param name='_from'>
    /// _from.
    /// </param>
    /// <param name='_to'>
    /// _to.
    /// </param>
    /// <param name='column'>
    /// Column.
    /// </param>
    /// <param name='_matrix'>
    /// _matrix.
    /// </param>
    GameObject[,] DelColumnCluster(int _from, int _to, int column, GameObject[,] _matrix)
    {

        int count = _to - _from + 1;				                                                                                        //lenght of the cluster

        gameControllerScript.BubblesDestroyed(count);
        GameObject[] buffer = new GameObject[count];

        int a = 0;
        for (int i = _from; i <= _to; i++)
        {
            buffer[a] = _matrix[column, i];

            GameObject someBlast = (GameObject)Instantiate(bubbleBlast, buffer[a].transform.position, buffer[a].transform.rotation);        //TODO: Make colors of blast same as bubbles
            //someBlast.particleSystem.renderer.material.color = buffer[a].renderer.material.color;                                                      //Don't work http://answers.unity3d.com/questions/347675/how-to-change-particle-systems-color-over-lifetime.html
            //someBlast.particleSystem.Play();
                                                            											                                    //Hide cluster of bubbles
            BubbleController bubbleScript = buffer[a].GetComponent<BubbleController>();
            bubbleScript.Hide(bubbleFadeTime);

            
            a++;
        }



        for (int i = _to; i - count >= 0; i--)
        {
            _matrix[column, i] = _matrix[column, i - count];

            Vector3 pos = new Vector3(column * (bubbleRadius * 2 + bubbleInterval), i * (bubbleRadius * 2 + bubbleInterval));
            BubbleController bubbleScript = _matrix[column, i].GetComponent<BubbleController>();							                //relocate the bubbles																		
            bubbleScript.targetPos = pos;																			
            bubbleScript.isMoving = true;
            bubbleScript.speedOfMoving = bubbleMovingSpeed;
        }

        for (int i = 0; i < count; i++)
        {
            _matrix[column, i] = buffer[i];
            Vector3 pos = new Vector3(column * (bubbleRadius * 2 + bubbleInterval), i * (bubbleRadius * 2 + bubbleInterval));				//Position in the local space
            _matrix[column, i].transform.localPosition = pos;
            _matrix[column, i].renderer.material.color = RandColor();

            BubbleController bubbleScript = _matrix[column, i].GetComponent<BubbleController>();
            _matrix[column, i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            bubbleScript.speedOfScaling = bubbleScalingSpeed;
            bubbleScript.targetRadius = bubbleRadius;
            bubbleScript.isScaling = true;

            a++;
        }

        return _matrix;
    }


    /// <summary>
    /// Scan matrix for first turn advice
    /// </summary>
    bool FirstTurnScan()
    {

        
       for (int j = 1; j < rows; j++)                                                                                                      // Searching potential clusters in the rows
        {
            Color currentColor = bubbleMatrix[0, j].renderer.material.color;
            int count = 0;

            for (int i = 0; i < columns - 1; i++)
            {
                if (bubbleMatrix[i, j].renderer.material.color == currentColor)
                {
                    count++;
                }
                else
                {
                    count = 1;
                    currentColor = bubbleMatrix[i, j].renderer.material.color;
                }

                if (count == 2 && bubbleMatrix[i + 1, j - 1].renderer.material.color == currentColor)                                       //If upper right bubble is same color then pop next bubble
                {
                    Debug.Log("Pop bubble in " + (i + 1) + " column " + j + " row");
                    Vector3 pos = bubbleMatrix[i + 1, j].transform.position;
                    Quaternion rot = bubbleMatrix[i + 1, j].transform.rotation;
                    Instantiate(shinePrefab, pos, rot);
                    return true;
                }

                if (i>=2 && count == 2 && bubbleMatrix[i-2, j - 1].renderer.material.color == currentColor)                                 //upper left -2 row
                {
                    Debug.Log("Pop bubble in " + (i - 2) + " column " + j + " row");
                    Vector3 pos = bubbleMatrix[i - 2, j].transform.position;
                    Quaternion rot = bubbleMatrix[i - 2, j].transform.rotation;
                    Instantiate(shinePrefab, pos, rot);
                    return true;
                }

            }
        }

        for (int j = 0; j < columns ; j++)                                                                                                  // seraching potential clusters in the columns
        {
            Color currentColor = bubbleMatrix[j, rows - 1].renderer.material.color;
            int count = 0;

            for (int i = rows - 1; i >= 2; i--)
            {
                if (bubbleMatrix[j, i].renderer.material.color == currentColor)
                {
                    count++;
                }
                else
                {
                    count = 1;
                    currentColor = bubbleMatrix[j, i].renderer.material.color;
                }

                if (count == 2 && bubbleMatrix[j, i - 2].renderer.material.color == currentColor)
                {
                    Debug.Log("Pop bubble in " + j + " column " + (i - 1) + " row");
                    Vector3 pos = bubbleMatrix[j, i - 1].transform.position;
                    Quaternion rot = bubbleMatrix[j, i - 1].transform.rotation;
                    Instantiate(shinePrefab, pos, rot);
                    return true;
                }
            }
        }

        return false;

    }


}

