using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WerewolfTomagachiGamemode : GameModeBase
{
    public Creature currentCreature;
    public System.Random rng;

    public override void Start()
    {
        base.Start();
        rng = new System.Random();
        currentCreature = new Creature();
    }

    public void Load(bool file_op = true)
    {
        if(file_op)
        {
            this.saveManager.load();
        }
        currentCreature.Load(this.saveManager, "current_creature.");
        rng = new System.Random(currentCreature.pawId);

    }

    public void Save(bool file_op = true)
    {
        currentCreature.Save(this.saveManager, "current_creature.");
        if (file_op)
        {
            this.saveManager.save();
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

public class Creature
{
    public int pawId;
    public string name;

    public Creature(string name = "", int pawId = 0)
    {

    }

    public void Save(SaveManager s, string preface = "")
    {
        s.setKey(preface + "name", name);
        s.setKey(preface + "pawId", pawId);
    }

    public void Load(SaveManager s, string preface = "")
    {
        name = s.getKey(preface + "name");
        pawId = (int)s.getKey(preface + "pawId");
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
