using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TopMenuController : MonoBehaviour
{
    public TMPro.TMP_Text nameContainer;
    public Sprite[] sprites;
    public SpriteRenderer pawprintRenderer;

    // Start is called before the first frame update
    void Start()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        nameContainer.text = gm.currentCreature.name;
        pawprintRenderer.sprite = sprites[gm.currentCreature.pawId - 1];
    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
