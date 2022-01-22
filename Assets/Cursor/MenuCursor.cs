using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnGUI()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        this.transform.position = mousePosition;
    }
}
