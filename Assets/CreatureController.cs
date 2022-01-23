using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureStatus
{
    sleeping,
    idle,
    wandering,
    complain_dirty,
    complain_unhappy,
    complain_sleep,
    complain_hunger,
    manual_override_1,
    transform,
    gratitude_happy,
    poop, 
    destroy_furniture,
}

public enum CreatureAnimation
{
    idle = 0,
    walking = 1,
    sleep = 2,
    speak = 3,
    pooping = 4,
    transforming = 5
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

    public float stat_dirty_lost_from_poop = 1.0f;
    public float stat_dirty_lost_from_broken_furniture = 0.2f;
    public float stat_happy_lost_from_broken_furniture = 0.2f;
    public float stat_happy_gain_from_working_furniture = 0.05f;

    public float stat_sleeping_happy_loss_per_second = 1.0f;

    public float clenliness_loss_from_food;

    public Collider2D walkingArea;
    public Transform centerPoint;

    public float walk_speed = 1.0f;
    private Rigidbody2D rb;

    public EmotionBubble[] speechBubbles;
    public SpriteRenderer currentSpeechBubble;

    public FurnitureController[] sceneFurniture;

    public Animator spriteAnimator;


    private void changeAnimation(CreatureAnimation c)
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        if(gm.currentRoom.isLightOn())
        {
            spriteAnimator.SetLayerWeight(0, 100);
            spriteAnimator.SetLayerWeight(1, 0);
        }
        else
        {
            spriteAnimator.SetLayerWeight(1, 100);
            spriteAnimator.SetLayerWeight(0, 0);
        }
        spriteAnimator.SetInteger("AnimationState", (int)c);
    }

    void Start()
    {
        sceneFurniture = UnityEngine.Object.FindObjectsOfType<FurnitureController>();
        status = CreatureStatus.idle;
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.cleanActions += onClean;
        gm.updateLightActions += onLightChange;
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.cleanActions -= onClean;
        gm.updateLightActions -= onLightChange;
    }

    void onLightChange()
    {
        spriteAnimator.SetTrigger("forceTransform");
        this.changeStatus(CreatureStatus.transform);
    }

    void onClean()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature creature = gm.currentCreature;
        Room room = gm.currentRoom;
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
        creature.stat_dirty = 100;
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
        timeSpentInCurrentStatus += Time.deltaTime;
        switch(this.status)
        {
            case CreatureStatus.idle:
                actionIdle();
                break;
            case CreatureStatus.wandering:
                actionWandering();
                break;
            case CreatureStatus.sleeping:
                actionSleep();
                break;
            case CreatureStatus.complain_sleep:
                actionExpressEmotion("tired");
                break;
            case CreatureStatus.gratitude_happy:
                actionExpressEmotion("happy");
                break;
            case CreatureStatus.complain_unhappy:
                actionExpressEmotion("unhappy");
                break;
            case CreatureStatus.complain_dirty:
                actionExpressEmotion("dirty");
                break;
            case CreatureStatus.poop:
                actionPoop();
                break;
            case CreatureStatus.complain_hunger:
                actionExpressEmotion("hungry");
                break;
            case CreatureStatus.manual_override_1:
                actionManualOverride1();
                break;
            case CreatureStatus.transform:
                actionTransform();
                break;
            default:
                Debug.LogError($"Status not set or finished: {status}");
                this.changeStatus(CreatureStatus.idle);
                break;
        }
    }

    private float maxActionTransformTime = 2.0f;
    private void actionTransform()
    {
        rb.velocity = Vector2.zero;
        changeAnimation(CreatureAnimation.transforming);
        if (timeSpentInCurrentStatus > maxActionTransformTime)
        {
            changeStatus(CreatureStatus.idle);
            actionWanderingDestination = Vector2.negativeInfinity;
        }
    }

    private Vector2 actionWanderingDestination = Vector2.negativeInfinity;
    public float maxActionWanderingTime = 0;
    private Vector3 actionWanderingLastFramePosition;
    private int actionWanderingStuckCount = 0;
    private void actionWandering()
    {
        changeAnimation(CreatureAnimation.walking);
        if (actionWanderingDestination.magnitude > 10000 || actionWanderingStuckCount > 20)
        {
            actionWanderingStuckCount = 0;
            actionWanderingDestination = RandomPointInBounds(walkingArea.bounds);
        }
        if (MoveTowardDestination(actionWanderingDestination))
        {
            actionWanderingDestination = Vector2.negativeInfinity;
        }
        if(timeSpentInCurrentStatus > maxActionWanderingTime)
        {
            changeStatus(CreatureStatus.idle);
            actionWanderingDestination = Vector2.negativeInfinity;
        }

        //Anti Stuck code
        if(Vector2.Distance(this.transform.position, actionWanderingLastFramePosition) <= 0.001)
        {
            actionWanderingStuckCount++;
        }
        else
        {
            actionWanderingStuckCount = 0;
        }
        actionWanderingLastFramePosition = this.transform.position;
    }

    public float timeSpentInCurrentStatus;
    public void changeStatus(CreatureStatus newStatus)
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.Save();
        this.setEmotionBubble("");
        timeSpentInCurrentStatus = 0;
        this.status = newStatus;
    }



    private bool MoveTowardDestination(Vector2 destination)
    {
        Vector2 pos = this.transform.position;
        if (Vector2.Distance(destination, pos) >= 0.2f)
        {
            rb.velocity = ((destination - pos).normalized) * walk_speed;
            return false;
        }
        rb.velocity = Vector2.zero;
        return true;
    }

    private Vector3 manualOverride1_destination;
    private void actionManualOverride1()
    {
        changeAnimation(CreatureAnimation.speak);
        this.MoveTowardDestination(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void updateNeeds()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature currentCreature = gm.currentCreature;
        Room currentRoom = gm.currentRoom;
        float stat_tired_offset = 0.0f;
        float stat_hungry_offset = 0.0f;
        float stat_happy_offset = 0.0f;
        float stat_dirty_offset = 0.0f;
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

        //Update Dirty
        if(currentRoom.getFurnitureItem("poop").owned)
        {
            stat_dirty_offset -= stat_dirty_lost_from_poop;
        }
        foreach(Furniture f in currentRoom.furniture)
        {
            if(f.broken)
            {
                stat_dirty_offset -= stat_dirty_lost_from_broken_furniture;
            }
        }

        //Update happy
        //Reduce happy for broken things

        foreach (Furniture f in currentRoom.furniture)
        {
            if (f.needsCleaned() && f.owned)
            {
                stat_happy_offset -= stat_happy_lost_from_broken_furniture;
            }else if(f.owned)
            {
                stat_happy_offset -= stat_happy_gain_from_working_furniture;
            }
        }


        currentCreature.stat_tired = getStatValue(currentCreature.stat_tired + (stat_tired_offset * timeSinceUpdated));
        currentCreature.stat_hungry = getStatValue(currentCreature.stat_hungry + (stat_hungry_offset * timeSinceUpdated));
        currentCreature.stat_happy = getStatValue(currentCreature.stat_happy + (stat_happy_offset * timeSinceUpdated));
        currentCreature.stat_dirty = getStatValue(currentCreature.stat_dirty + (stat_dirty_offset * timeSinceUpdated));
        timeSinceUpdated = 0;
    }



    public float getStatValue(float value)
    {
        return Mathf.Max(0, Mathf.Min(100, value));
    }

    public float maxActionExpressEmotionTime = 20;
    public void actionExpressEmotion(string emotion)
    {
        if(this.MoveTowardDestination(this.centerPoint.position))
        {
            changeAnimation(CreatureAnimation.speak);
            setEmotionBubble(emotion);
        }else
        {
            changeAnimation(CreatureAnimation.walking);
            timeSpentInCurrentStatus = 0;
        }
        if (timeSpentInCurrentStatus > maxActionExpressEmotionTime)
        {
            changeStatus(CreatureStatus.idle);
        }
    }
    public float maxActionPoopTime = 3.0f;
    public float statHappinessFromPooping = 30.0f;
    public float statBoredomFromPooping = 30.0f;
    public float statHungryLostFromPooping = 10.0f;
    public void actionPoop()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Furniture poop = gm.currentRoom.getFurnitureItem("poop");

        if (!isWarewolf())
        {
            changeStatus(CreatureStatus.idle);
            return;
        }
        if (this.MoveTowardDestination(this.centerPoint.position))
        {
            changeAnimation(CreatureAnimation.pooping);
            if(!poop.owned)
            {
                poop.Purchase();
                gm.updateFurniture();
                gm.currentCreature.stat_hungry = this.getStatValue(gm.currentCreature.stat_boredom - statHungryLostFromPooping);
                gm.currentCreature.stat_boredom = this.getStatValue(gm.currentCreature.stat_boredom + statBoredomFromPooping);
                gm.currentCreature.stat_happy = this.getStatValue(gm.currentCreature.stat_happy + statHappinessFromPooping);
            }
        }
        else
        {
            changeAnimation(CreatureAnimation.walking);
            timeSpentInCurrentStatus = 0;
        }
        if (timeSpentInCurrentStatus > maxActionPoopTime)
        {
            changeStatus(CreatureStatus.idle);
        }
    }

    public bool isWarewolf()
    {
        return !((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current).currentRoom.isLightOn();
    }

    public void setEmotionBubble(string emotion)
    {
        try
        {
            EmotionBubble bubble;
            bubble = Array.Find(speechBubbles, b => b.id == emotion);
            if(bubble != null)
            {
                this.currentSpeechBubble.gameObject.SetActive(true);
                this.currentSpeechBubble.sprite = bubble.sprite;
            }else
            {
                this.currentSpeechBubble.gameObject.SetActive(false);
            }
        }
        catch (ArgumentNullException e)
        {
            this.currentSpeechBubble.gameObject.SetActive(false);
        }
        
    }


    public void actionIdle()
    {
        changeAnimation(CreatureAnimation.idle);
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature c = gm.currentCreature;
        rb.velocity = Vector2.zero;
        //Create a list of all available actions and their weights
        List<ActiveOption> options = new List<ActiveOption>();
        //Always leave an idle
        options.Add(new ActiveOption(CreatureStatus.idle, 1));
        options.Add(new ActiveOption(CreatureStatus.wandering, 5));
        if (gm.currentRoom.isLightOn())
        {
            if (c.stat_tired <= 10)
            {
                options.Add(new ActiveOption(CreatureStatus.sleeping, 1000));
            }
            else if (c.stat_tired <= 30)
            {
                options.Add(new ActiveOption(CreatureStatus.sleeping, 30));
                options.Add(new ActiveOption(CreatureStatus.complain_sleep, 10));
            }

            if(c.stat_happy < 20)
            {
                options.Add(new ActiveOption(CreatureStatus.complain_unhappy, 10));
            }
            if (c.stat_happy > 70)
            {
                options.Add(new ActiveOption(CreatureStatus.gratitude_happy, 10));
            }
            if (c.stat_hungry < 30)
            {
                options.Add(new ActiveOption(CreatureStatus.complain_hunger, 10));
            }
            
            if (c.stat_dirty < needs_cleaned_threshold/2)
            {
                options.Add(new ActiveOption(CreatureStatus.complain_dirty, 10));
            }
        }else
        {
            //Warewolf only options
            if(c.stat_hungry > 60 && !gm.currentRoom.getFurnitureItem("poop").owned)
            {
                options.Add(new ActiveOption(CreatureStatus.poop, 20));
            }
        }
       
        //Then pick a random one from the list
        int totalWeight = 0;
        foreach(ActiveOption o in options)
        {
            totalWeight += o.weight;
        }
        
        int randomWeight = gm.rng.Next(0, totalWeight);
        int currentWeight = 0;
        foreach (ActiveOption o in options)
        {
            currentWeight += o.weight;
            if(randomWeight <= currentWeight)
            {
                this.changeStatus(o.option);
                return;
            }
        }
    }

    public void actionSleep()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        Creature currentCreature = gm.currentCreature;
        changeAnimation(CreatureAnimation.sleep);
        if (currentCreature.stat_tired < 50)
        {
            //Rest
        }else
        {
            this.changeStatus(CreatureStatus.idle);
        }
    }

    public static Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
        );
    }

    [System.Serializable]
    public class EmotionBubble
    {
        public string id;
        public Sprite sprite;
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
