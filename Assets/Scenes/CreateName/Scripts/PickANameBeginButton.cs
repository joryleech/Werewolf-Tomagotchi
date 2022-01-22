using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PickANameBeginButton : MonoBehaviour
{
    private Vector3 startingPosition;
    public float raisedAmmount = 0.05f;
    private Vector3 raisedPosition;
    public GameObject sprite;
    private bool raised = false;
    public TMP_InputField input;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = sprite.transform.position;
        raisedPosition = startingPosition + new Vector3(0, raisedAmmount, 0);
        input.Select();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = raised ? raisedPosition : startingPosition;
        Vector3 spritePosition = sprite.transform.position;
        if (Vector3.Distance(spritePosition, targetPosition) > 0.01f)
        {
            sprite.transform.position = Vector3.Lerp(spritePosition, targetPosition, 1 - (Mathf.Pow(0.005f, Time.deltaTime)));
        }

        if (Input.GetMouseButtonDown(0) && raised)
        {
            if(input.text.Length > 0)
            {
                WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
                gm.currentCreature.name = input.text;
                gm.Save();
                gm.ChangeScene("scenes/GameplayLoop");
            }
        }
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
