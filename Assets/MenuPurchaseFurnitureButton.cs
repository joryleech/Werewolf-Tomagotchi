using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuPurchaseFurnitureButton : MonoBehaviour
{
    public string id;
    public int cost;
    public GameObject icon;
    private SpriteRenderer sr;
    private bool raised;
    private Vector3 startingPosition;
    private Vector3 raisedPosition;
    public float raisedAmmount = 0.05f;
    public Color disabledColor;
    public Color normalColor = new Color(1, 1, 1);

    public int happyinessGain;
    // Start is called before the first frame update

    void Start()
    {
        startingPosition = icon.transform.localPosition;
        raisedPosition = startingPosition + new Vector3(0, raisedAmmount, 0);
        sr = icon.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
     
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Furniture f = gm.currentRoom.getFurnitureItem(id);
        if (f != null && f.isPurchasable() && gm.currentCreature.money >= cost)
        {
            sr.color = normalColor;
            if (Input.GetMouseButtonDown(0) && raised)
            {
                gm.currentCreature.money -= cost;
                f.Purchase();
                gm.updateFurniture();
                gm.currentCreature.stat_happy = Mathf.Min(100, gm.currentCreature.stat_happy + happyinessGain);
            }
        }
        else
        {
            raised = false;
            sr.color = disabledColor;
        }
        Vector3 targetPosition = raised ? raisedPosition : startingPosition;
        Vector3 spritePosition = icon.transform.localPosition;
        if (Vector3.Distance(spritePosition, targetPosition) > 0.01f)
        {
            icon.transform.localPosition = Vector3.Lerp(spritePosition, targetPosition, 1 - (Mathf.Pow(0.005f, Time.deltaTime)));
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