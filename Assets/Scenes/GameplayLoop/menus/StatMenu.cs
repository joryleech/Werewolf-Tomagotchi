using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMenu : MonoBehaviour
{
    public TMPro.TMP_Text stat_boredom;
    public TMPro.TMP_Text stat_hungry;
    public TMPro.TMP_Text stat_happy;
    public TMPro.TMP_Text stat_tired;
    public TMPro.TMP_Text stat_dirty;

    // Start is called before the first frame update
    void Start()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);

        gm.changeMenuActions += onChangeMenu;
    }

    private void OnDestroy()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        gm.changeMenuActions -= onChangeMenu;
    }

    public void onChangeMenu(string id)
    {
        if(id == "stats")
        {
            updateStats();
        }
    }

    public void updateStats()
    {
        WerewolfTomagachiGamemode gm = ((WerewolfTomagachiGamemode)WerewolfTomagachiGamemode.current);
        stat_boredom.text = gm.currentCreature.stat_boredom.ToString("000");
        stat_hungry.text = gm.currentCreature.stat_hungry.ToString("000");
        stat_happy.text = gm.currentCreature.stat_happy.ToString("000");
        stat_tired.text = gm.currentCreature.stat_tired.ToString("000");
        stat_dirty.text = gm.currentCreature.stat_dirty.ToString("000");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
