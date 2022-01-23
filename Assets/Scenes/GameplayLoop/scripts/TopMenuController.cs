using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TopMenuController : MonoBehaviour
{
    public TMPro.TMP_Text nameContainer;
    public TMPro.TMP_Text moneyContainer;
    public Sprite[] sprites;
    public SpriteRenderer pawprintRenderer;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
            nameContainer.text = gm.currentCreature.name;
            pawprintRenderer.sprite = sprites[gm.currentCreature.pawId - 1];
        }
        catch(System.Exception e)
        {

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        moneyContainer.text = System.Math.Min(gm.currentCreature.money,9999).ToString("");
        

    }
}
