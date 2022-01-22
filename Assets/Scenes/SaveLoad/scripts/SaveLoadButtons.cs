using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SaveLoadButtonType
{
    new_game_button,
    continue_button
}
public class SaveLoadButtons : MonoBehaviour
{
    private Vector3 startingPosition;
    public float raisedAmmount = 0.05f;
    private Vector3 raisedPosition;
    public GameObject sprite;
    private bool raised = false;
    public GameObject error;

    public SaveLoadButtonType type;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = sprite.transform.position;
        raisedPosition = startingPosition + new Vector3(0, raisedAmmount, 0);
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
            switch(type)
            {
                case SaveLoadButtonType.new_game_button:
                    this.NewGame();
                    break;
                case SaveLoadButtonType.continue_button:
                    this.Continue();
                    break;
            }
        }
    }

    void NewGame()
    {
        ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current).currentCreature = new Creature();
        ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current).ChangeScene("Scenes/CreateAWerewolfMenu");
    }

    void Continue()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        SaveManager s = gm.getSaveManager();
        if(s.exists())
        {
            gm.Load();
            if(gm.currentCreature.isValid())
            {
                gm.ChangeScene("Scenes/GameplayLoop");
            }
            else
            {
                error.active = true;
                Debug.LogError("<CONTINUE>: Load failed current creature invalid.");
            }

        }
        else
        {
            error.active = true;
            Debug.LogError("<CONTINUE>: Save file not found.");
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
