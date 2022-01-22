using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeBase : MonoBehaviour
{
    public PawnBase currentPawn;
    public static GameModeBase current;
    public bool debug = false;
    public Camera camera;
    protected SaveManager saveManager;


    public virtual void Start()
    {
        if(current && current != this)
        {
            Destroy(this);
            //throw new System.Exception("GameModeError: Multiple GameModes Exist");
        }else
        {
            current = this;
            saveManager = new SaveManager();
            DontDestroyOnLoad(this.gameObject);
        }
       
    }

    public virtual void Update()
    {
        if (Time.frameCount % 1000 == 0)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: false);
        }
    }

    public event Action<string, Dictionary<string, string>> levelActions;
    public void onLevelAction(string key, Dictionary<string, string> opts)
    {
        if (levelActions != null)
        {
            levelActions(key, opts);
        }
    }

    public void setPossessed(PawnBase p)
    {
        if (currentPawn != null)
        {
            currentPawn.onRemovePossess();
        }
        currentPawn = p;
        currentPawn.onPossess();
    }

    public Camera getCamera()
    {
        if(camera == null)
        {
            Debug.Log("finding Main Camera");
            this.camera = Camera.main;
        }
        return this.camera;
    }

    public SaveManager getSaveManager()
    {
        return this.saveManager;
    }
}
