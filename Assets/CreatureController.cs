using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureStatus
{
    sleeping,
    idle,
    wandering,
    complain_hunger,
}
public class CreatureController : MonoBehaviour
{
    // Start is called before the first frame update
    private float timeSinceUpdated = 0.0f;
    public float timeBetweenUpdates = 0.5f;
    public CreatureStatus status;

    public float stat_hunger_loss_per_second = 1.0f;
    public float stat_werewolf_hunger_loss_per_second = 1.0f;

    public float stat_sleep_loss_per_second = 1.0f;
    public float stat_sleeping_sleep_gain_per_second = 2.0f;
    public float stat_warewolf_sleep_gain_per_second = 1.0f;

    public float needs_cleaned_threshold = 30.0f;
    public float stat_happiness_from_needed_clean = 20.0f;
    public float stat_happiness_lost_from_uneeded_clean = 10.0f;

    public float stat_sleeping_happy_loss_per_second = 1.0f;

    public float walk_speed = 1.0f;

    void Start()
    {
        status = CreatureStatus.idle;
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.cleanActions += onClean;
    }

    private void OnDestroy()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.cleanActions -= onClean;
    }

    void onClean()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature creature = gm.currentCreature;
        Room room = gm.currentRoom;
        creature.stat_dirty = 100;
        if(!gm.currentRoom.isLightOn())
        {
            creature.stat_happy = getStatValue(creature.stat_happy - stat_happiness_lost_from_uneeded_clean*2);
        } else if( room.needsClean() || creature.stat_dirty <= needs_cleaned_threshold)
        {
            creature.stat_happy = getStatValue(creature.stat_happy + stat_happiness_from_needed_clean);
        }else
        {
            creature.stat_happy = getStatValue(creature.stat_happy - stat_happiness_lost_from_uneeded_clean);
        }
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

        updateCreatureActions();
    }

    private void updateCreatureActions()
    {
        switch(this.status)
        {
            case CreatureStatus.idle:
                actionIdle();
                break;
            case CreatureStatus.sleeping:
                actionSleep();
                break;
            default:
                Debug.LogError($"Status not set or finished: {status}");
                this.status = CreatureStatus.idle;
                break;
        }
    }

    private void updateNeeds()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature currentCreature = gm.currentCreature;
        float stat_tired_offset = 0.0f;
        float stat_hungry_offset = 0.0f;
        float stat_happy_offset = 0.0f;

        // Update Sleep
        if (status == CreatureStatus.sleeping)
        {
            stat_tired_offset += stat_sleeping_sleep_gain_per_second;
            //Doesnt like sleeping with light on
            stat_happy_offset -= stat_sleeping_happy_loss_per_second;
        }
        else if (!gm.currentRoom.isLightOn())
        {
            stat_tired_offset += stat_warewolf_sleep_gain_per_second;
        }
        else
        {
            stat_tired_offset -= stat_sleep_loss_per_second;
        }

        // Update Hunger
        if (gm.currentRoom.isLightOn())
        {
            stat_hungry_offset -= stat_hunger_loss_per_second;
        }else
        {
            stat_hungry_offset -= stat_werewolf_hunger_loss_per_second;
        }

        currentCreature.stat_tired = getStatValue(currentCreature.stat_tired + (stat_tired_offset * timeSinceUpdated));
        currentCreature.stat_hungry = getStatValue(currentCreature.stat_hungry + (stat_hungry_offset * timeSinceUpdated));
        currentCreature.stat_happy = getStatValue(currentCreature.stat_happy + (stat_happy_offset * timeSinceUpdated));
        timeSinceUpdated = 0;
    }



    public float getStatValue(float value)
    {
        return Mathf.Max(0, Mathf.Min(100, value));
    }



    public void actionIdle()
    {
        //Create a list of all available actions and their weights
        List<ActiveOption> options = new List<ActiveOption>();
        //Always leave an idle
        options.Add(new ActiveOption(CreatureStatus.idle, 10));
        options.Add(new ActiveOption(CreatureStatus.idle, 10));
        //Then pick a random one from the list
        int totalWeight = 0;
        foreach(ActiveOption o in options)
        {
            totalWeight += o.weight;
        }
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
    }

    public void actionSleep()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature currentCreature = gm.currentCreature;

        if(currentCreature.stat_tired < 50)
        {
            //Rest
        }else
        {
            status = CreatureStatus.idle;
        }
    }
}

public class ActiveOption
{
    public int weight = 0;
    public CreatureStatus option = CreatureStatus.idle;
    public ActiveOption( CreatureStatus o,int w)
    {
        option = o;
        weight = w;
    }
}