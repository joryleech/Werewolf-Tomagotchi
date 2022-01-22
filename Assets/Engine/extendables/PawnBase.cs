using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

public class PawnBase : MonoBehaviour
{
    [Inherits(typeof(ControllerBase))]
    [SerializeField] public TypeReference controllerType;

    protected ControllerBase controller;


    // Start is called before the first frame update
    void Start()
    {
        this.setController((ControllerBase)System.Activator.CreateInstance(controllerType));
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("controller:");
        Debug.Log(controller);
        if (controller != null)
        {
            controller.onUpdate(this);
        }
    }

    void FixedUpdate()
    {
        if(controller != null)
        {
            controller.onFixedUpdate(this);
        }
    }

    public void onPossess()
    {

    }

    public void onRemovePossess()
    {

    }


    public bool isPossessed()
    {
        return GameModeBase.current.currentPawn == this;
    }

    public void setController(ControllerBase newController)
    {
        if(controller != null)
        {
            controller.onRemoveFromPawn(this);
        }

        controller = newController;
        controller.onAddToPawn(this);
    }

}
