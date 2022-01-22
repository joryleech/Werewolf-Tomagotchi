using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureStatus
{
    sleeping,
    idle
}
public class CreatureController : MonoBehaviour
{
    // Start is called before the first frame update
    private float timeSinceUpdated = 0.0f;
    public float timeBetweenUpdates = 0.5f;



    public float stat_hunger_loss_per_second = 1.0f;
    public float stat_werewolf_hunger_loss_per_second = 1.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        if(gm.canUpdateCreature())
        {
            timeSinceUpdated += Time.deltaTime;
            if(timeSinceUpdated > timeBetweenUpdates)
            {
                updateNeeds();
            }


        }
        
    }

    private void updateNeeds()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature currentCreature = gm.currentCreature;
        // Update Sleep
        // Update Hunger
        if (gm.currentRoom.isLightOn())
        {
            currentCreature.stat_hungry = Mathf.Max(0, currentCreature.stat_hungry - (stat_hunger_loss_per_second*timeSinceUpdated));
        }else
        {
            currentCreature.stat_hungry = Mathf.Max(0, currentCreature.stat_hungry - (stat_werewolf_hunger_loss_per_second * timeSinceUpdated));
        }
        timeSinceUpdated = 0;
    }
}
