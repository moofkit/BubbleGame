using UnityEngine;
using System.Collections;

public class BubbleController : MonoBehaviour
{

    BubbleMatrix matrixScript;								//BubbleMatrix script
    public bool isMoving;									//Status of moving the bubble
    public bool isScaling;									//Status of scaling bubble
    public Vector3 targetPos;								//Target position of moving bubble
    public float speedOfMoving;								//Speed of velocity
    public float targetRadius;								//Target radius off bubble for scaling
    public float speedOfScaling;							//Speed of scaling bubble



    void Start()
    {
        matrixScript = GameObject.Find("Controller").GetComponent<BubbleMatrix>();
        isMoving = false;
        targetPos = gameObject.transform.localPosition;
        isScaling = false;

    }

    void Update()
    {
        if (isMoving && gameObject.transform.localPosition != targetPos)
        {														                                                                //Checking status
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, speedOfMoving * Time.deltaTime);
        }
        else if (gameObject.transform.localPosition == targetPos)
        {															                                                            //The moving is done
            isMoving = false;																									//It's important for stoping the move
        }

        float currentRadius = transform.localScale.x * 0.5f;																	//Current scale (0.5f is default radius of prefab bubble)
        if (isScaling && currentRadius != targetRadius)
        {																		                                                //Checking status 
            float targetScale = targetRadius / 0.5f;																		    //Calculating target scale
            float currentScale = transform.localScale.x;																		//Get current scale
            currentScale = Mathf.MoveTowards(currentScale, targetScale, speedOfScaling * Time.deltaTime);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
        else if (currentRadius == targetRadius)
        {
            isScaling = false;
        }
    }

    void OnMouseDown()
    {
        matrixScript.OnBubbleClick(gameObject);																					//Give controll for the matrixController
    }

    public void Hide(float sec)
    {
        StartCoroutine(FadeTimer(sec));
    }

    IEnumerator FadeTimer(float sec)
    {

        gameObject.renderer.enabled = false;
        yield return new WaitForSeconds(sec);
        gameObject.renderer.enabled = true;

    }


}
