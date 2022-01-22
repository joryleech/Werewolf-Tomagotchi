using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableMenuController : MonoBehaviour
{
    Vector3 openPosition;
    Vector3 closePosition;
    public GameObject background;
    public float timeToOpen = 0.5f;
    public string menuId;


    // Start is called before the first frame update
    void Start()
    {
        openPosition = background.transform.position;
        closePosition = openPosition + new Vector3(0, -10);
        background.transform.position = closePosition;
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.changeMenuActions += onChangeMenu;
    }

    private void OnDestroy()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.changeMenuActions -= onChangeMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("f1"))
        {
            Debug.Log("Pressed f1 Menu");
            WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
            gm.changeMenu(menuId);
        }
        if(Input.GetKeyDown("f2"))
        {
            WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
            gm.changeMenu("");
        }
    }

    private IEnumerator moveCoroutine;
    private void onChangeMenu(string id)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        
        moveCoroutine = moveToDestination(this.menuId == id ? openPosition : closePosition);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator moveToDestination(Vector3 destination)
    {
        Vector3 currentPosition = background.transform.position;
        while (Vector3.Distance(currentPosition, destination) > 0.01f)
        {
            currentPosition = background.transform.position;
            background.transform.position = Vector3.Lerp(currentPosition, destination, 1 - (Mathf.Pow(timeToOpen, Time.deltaTime)));
            yield return new WaitForEndOfFrame();
        }
    }
}
