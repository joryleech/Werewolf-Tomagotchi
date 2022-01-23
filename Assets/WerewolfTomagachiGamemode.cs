using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WerewolfTomagachiGamemode : GameModeBase
{
    public Creature currentCreature;
    public Room currentRoom;
    public System.Random rng;
    public bool forceLoad;


    public override void Start()
    {
        base.Start();
        if(current = this)
        {
            rng = new System.Random();
            currentCreature = new Creature();
            currentRoom = new Room();
            if (forceLoad)
            {
                this.Load();
            }
        }
        
    }

    public void Load(bool file_op = true)
    {
        Debug.Log("Loading");
        if(file_op)
        {
            this.saveManager.load();
        }
        currentCreature.Load(this.saveManager, "current_creature.");
        currentRoom.Load(this.saveManager, "current_room.");
        rng = new System.Random(currentCreature.pawId);

    }

    public void Save(bool file_op = true)
    {
        currentCreature.Save(this.saveManager, "current_creature.");
        currentRoom.Save(this.saveManager, "current_room.");
        if (file_op)
        {
            this.saveManager.save();
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public string menuOpen;
    public event Action<string> changeMenuActions;
    public void changeMenu(string key)
    {
        menuOpen = key;
        if (changeMenuActions != null)
        {
            changeMenuActions(key);
        }
    }


    public bool canUpdateCreature()
    {
        return true;
    }

    public event Action cleanActions;
    public void Clean()
    {
        if (cleanActions != null)
        {
            cleanActions();
        }
        currentRoom.Clean();
    }

    public event Action updateLightActions;
    public void toggleLight()
    {
        this.updateLight(!this.currentRoom.lightOn);
    }

    public void updateLight(bool newLightOn)
    {
        Debug.Log($"Updating Light {newLightOn}");
        this.currentRoom.lightOn = newLightOn;
        if (updateLightActions != null)
        {
            updateLightActions();
        }
    }
    public event Action updateFurnitureActions;
    public void updateFurniture()
    {
        if (updateFurnitureActions != null)
        {
            updateFurnitureActions();
        }
    }

}

public class Room
{
    public Furniture[] furniture;
    
    public Room()
    {
        furniture = new Furniture[] {
            new Furniture("lamp")
        };
        furniture[0].owned = true;
    }

    public bool lightOn = true;
    public bool isLightOn()
    {
        try
        {
            Furniture lamp = Array.Find(furniture, p => p.id == "lamp");
            return lightOn && lamp.owned && !lamp.broken;
        }
        catch(ArgumentNullException e)
        {
            Debug.LogError("Lamp does not exist");
            return false;
        }
    }

    public Furniture getFurnitureItem(string id)
    {
        try
        {
           return Array.Find(furniture, p => p.id == id);
        }
        catch (ArgumentNullException e)
        {
            return null;
        }
    }

    public void Load(SaveManager s, string preface = "")
    {
        foreach (Furniture f in furniture)
        {
            f.Load(s, preface);
        }
    }

    public void Save(SaveManager s, string preface = "")
    {
        foreach (Furniture f in furniture)
        {
            f.Save(s, preface);
        }
    }

    public void Clean()
    {
        foreach(Furniture f in furniture)
        {
            f.clean();
        }
    }

    public bool needsClean()
    {
        foreach (Furniture f in furniture)
        {
            if(f.needsCleaned())
            {
                return true;
            }
        }
        return false;
    }
}

public class Furniture
{
    public Furniture(string id)
    {
        this.id = id;
        this.owned = false;
        this.broken = false;
    }
    public bool broken;
    public bool owned;
    public string id;
    public void Load(SaveManager s,string preface = "")
    {
        this.broken = s.getKey(preface + id + "." + "broken");
        this.owned = s.getKey(preface + id + "." + "owned");
    }
    public void Save(SaveManager s, string preface = "")
    {
        s.setKey(preface + id + "." + "broken", broken);
        s.setKey(preface + id + "." + "owned", owned);
    }
    public void Purchase()
    {
        this.owned = true;
        this.broken = false;
    }
    public void Break()
    {
        if(this.owned)
        {
            this.broken = true;
        }
    }

    public bool needsCleaned()
    {
        return this.broken || this.id == "poop";
    }

    public void clean()
    {
        if (needsCleaned())
        {
            this.owned = false;
            this.broken = false;
        }
    }


}

public class Creature
{
    public int pawId;
    public string name;


    public float stat_boredom = 50f;
    public float stat_hungry = 50f;
    public float stat_happy = 50f;
    public float stat_tired = 50f;
    public float stat_dirty = 50f;

    public float start_werewolf_bathroom = 50f;
    public float stat_werewolf_happy = 50f;

    public long money = 20;

    public Creature(string name = "", int pawId = 0)
    {

    }


    public void Save(SaveManager s, string preface = "")
    {
        s.setKey(preface + "name", name);
        s.setKey(preface + "pawId", pawId);

        s.setKey(preface + "stat_boredom", stat_boredom);
        s.setKey(preface + "stat_hungry", stat_hungry);
        s.setKey(preface + "stat_happy", stat_happy);
        s.setKey(preface + "stat_tired", stat_tired);
        s.setKey(preface + "start_werewolf_bathroom", start_werewolf_bathroom);
        s.setKey(preface + "stat_werewolf_happy", stat_werewolf_happy);
        s.setKey(preface + "money", money);
    }

    public void Load(SaveManager s, string preface = "")
    {
        name = s.getKey(preface + "name");
        pawId = (int)s.getKey(preface + "pawId");

        stat_boredom = (float)s.getKey(preface + "stat_boredom");
        stat_hungry = (float)s.getKey(preface + "stat_boredom");
        stat_happy = (float)s.getKey(preface + "stat_happy");
        stat_tired = (float)s.getKey(preface + "stat_tired");
        stat_tired = (float)s.getKey(preface + "start_werewolf_bathroom");
        stat_tired = (float)s.getKey(preface + "stat_werewolf_happy");

        money = (long)s.getKey(preface + "money");
    }

    public bool isValid()
    {
        if (pawId <= 0)
        {
            return false;
        }
        if (name == null || name.Length < 1 || name.Length > 7)
        {
            return false;
        }
        return true;
    }

}
