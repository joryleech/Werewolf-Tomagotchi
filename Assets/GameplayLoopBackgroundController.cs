using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayLoopBackgroundController : MonoBehaviour
{
    public Sprite lightBackground;
    public Sprite darkBackground;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        if (gm.currentRoom.isLightOn())
        {
            sr.sprite = lightBackground;
        }
        else
        {
            sr.sprite = darkBackground;
        }
    }
}
