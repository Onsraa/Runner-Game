using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemScript : MonoBehaviour
{
    public GameObject itemObject;
    private IUseable itemScript;

    private void Start()
    {
        if (itemObject)
        {
            itemScript = itemObject.GetComponent<IUseable>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (itemScript != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                itemScript.Use1();
            }
            
            if (Input.GetKey(KeyCode.Mouse1))
            {
                itemScript.Use2();
            }
        }
    }

    public void SetItem(GameObject item)
    {
        itemObject = item;
        itemScript = item.GetComponent<IUseable>();
    }
}
