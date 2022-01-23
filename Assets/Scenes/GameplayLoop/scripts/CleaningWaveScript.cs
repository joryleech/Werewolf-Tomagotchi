using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningWaveScript : MonoBehaviour
{
    Vector3 openPosition;
    Vector3 closePosition;
    public GameObject background;
    public float timeToOpen = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        openPosition = background.transform.position;
        closePosition = openPosition + new Vector3(10, 0);
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        background.transform.position = closePosition;
        gm.cleanActions += onClean;
    }

    private void OnDestroy()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.cleanActions -= onClean;
    }

    private IEnumerator moveCoroutine;
    public void onClean()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        Debug.Log("Cleaning");
        moveCoroutine = cleanWave();
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator cleanWave()
    {
        Vector3 currentPosition = background.transform.position;
        while (Vector3.Distance(currentPosition, openPosition) > 0.01f)
        {
            currentPosition = background.transform.position;
            background.transform.position = Vector3.Lerp(currentPosition, openPosition, 1 - (Mathf.Pow(timeToOpen, Time.deltaTime)));
            yield return new WaitForEndOfFrame();
        }
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.updateFurniture();
        while (Vector3.Distance(currentPosition, closePosition) > 0.01f)
        {
            currentPosition = background.transform.position;
            background.transform.position = Vector3.Slerp(currentPosition, closePosition, 1 - (Mathf.Pow(timeToOpen, Time.deltaTime)));
            yield return new WaitForEndOfFrame();
        }
    }
}
