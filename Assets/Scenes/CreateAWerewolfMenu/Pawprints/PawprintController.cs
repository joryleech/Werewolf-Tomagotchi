using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PawprintController : MonoBehaviour
{
    public Animator anim;
    public int pawprintId;
    public static System.Random RNGSingleton;
    public bool raised = false;
    public Vector3 startingPosition;
    public float raisedAmmount = 0.05f;
    public Vector3 raisedPosition;

    // Start is called before the first frame update
    void Start()
    {
        RNGSingleton = new System.Random(pawprintId*3);
        startingPosition = anim.gameObject.transform.position;
        raisedPosition = startingPosition + new Vector3(0, raisedAmmount, 0);
    }

    public float timeSinceWiggleAttempt = 0;
    // Update is called once per frame
    void Update()
    {
        timeSinceWiggleAttempt += Time.deltaTime;
        if (timeSinceWiggleAttempt > 0.8f)
        {
            timeSinceWiggleAttempt = 0;
            
            if (RNGSingleton.Next(1, 9) <= 1)
            {
                anim.SetTrigger("wiggle");
            }
        }

        Vector3 targetPosition = raised ? raisedPosition : startingPosition;
        Vector3 spritePosition = anim.gameObject.transform.position;
        if(Vector3.Distance(spritePosition, targetPosition) > 0.01f)
        {
            anim.gameObject.transform.position = Vector3.Lerp(spritePosition, targetPosition, 1 - (Mathf.Pow(0.005f,Time.deltaTime)));
        }

        if(Input.GetMouseButtonDown(0) && raised)
        {
            ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current).currentCreature.pawId = pawprintId;
            ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current).ChangeScene("Scenes/CreateName");
        }
    }

    public void setCreatureFootprint()
    {

    }

    void OnMouseOver()
    {
        raised = true;
    }

    void OnMouseExit()
    {
        raised = false;
    }
}
