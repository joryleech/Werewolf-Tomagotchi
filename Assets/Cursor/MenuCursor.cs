using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor: MonoBehaviour
{
    SpriteRenderer sr;
    public Sprite unclicked;
    public Sprite clicked;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    private void OnGUI()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            sr.sprite = clicked;
        }
        else
        {
            sr.sprite = unclicked;
           
        }
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        this.transform.position = mousePosition;
    }
}
