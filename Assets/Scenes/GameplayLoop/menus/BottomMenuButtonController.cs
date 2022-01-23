using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BottomMenuButtonType
{
    food,
    furniture,
    light,
    clean,
    stats
}
public class BottomMenuButtonController : MonoBehaviour
{
    public BottomMenuButtonType type;
    public GameObject icon;
    private bool raised;
    private Vector3 startingPosition;
    private Vector3 raisedPosition;
    public float raisedAmmount = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = icon.transform.position;
        raisedPosition = startingPosition + new Vector3(0, raisedAmmount, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && raised)
        {
            WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
            switch (type)
            {
                case BottomMenuButtonType.food:
                    gm.changeMenu("food");
                    break;
                case BottomMenuButtonType.furniture:
                    gm.changeMenu("furniture");
                    break;
                case BottomMenuButtonType.light:
                    gm.toggleLight();
                    break;
                case BottomMenuButtonType.clean:
                    gm.Clean();
                    break;
                case BottomMenuButtonType.stats:
                    gm.changeMenu("stats");
                    break;
            }
        }

        Vector3 targetPosition = raised ? raisedPosition : startingPosition;
        Vector3 spritePosition = icon.transform.position;
        if (Vector3.Distance(spritePosition, targetPosition) > 0.01f)
        {
            icon.transform.position = Vector3.Lerp(spritePosition, targetPosition, 1 - (Mathf.Pow(0.005f, Time.deltaTime)));
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
