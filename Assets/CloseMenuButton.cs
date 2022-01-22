using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenuButton : MonoBehaviour
{
    bool raised = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && raised)
        {
            WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
            gm.changeMenu("");
        }
    }

    void OnMouseOver()
    {
        raised = true;
    }

    void OnMouseExit()
    {
        raised = false;
    }
}
