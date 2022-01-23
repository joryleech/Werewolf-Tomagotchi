using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    public string id;
    public Sprite workingSprite;
    public Sprite brokenSprite;
    public SpriteRenderer sr;

    public bool debug_toggleBroken = false;
    // Start is called before the first frame update
    void Start()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.updateFurnitureActions += updateSprite;
        updateSprite();

    }

    void onDestroy()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.updateFurnitureActions -= updateSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(debug_toggleBroken)
        {
            Furniture f = this.GetFurniture();
            f.broken = !f.broken;
            debug_toggleBroken = false;
            WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
            gm.updateFurniture();
        }
    }

    public void updateSprite()
    {
        Furniture f = this.GetFurniture();
        if(f != null && f.owned)
        {
            sr.enabled = true;
            if(f.broken)
            {
                sr.sprite = brokenSprite;
            }
            else
            {
                sr.sprite = workingSprite;
            }
        }else
        {
            sr.enabled = false;
        }
    }

    Furniture GetFurniture()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Room cr = gm.currentRoom;
        Furniture f = cr.getFurnitureItem(id);
        return f;
    }
}
