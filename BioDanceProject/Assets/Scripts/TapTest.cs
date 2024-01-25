using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapTest : MonoBehaviour
{ 
    Vector3 mousePosition = Vector3.zero;
    bool isDrag = false;

    // Update is called once per frame
    void Update()
    {
        //mousePosition = Input.mousePosition;

        //Debug.Log("Screen position : " + mousePosition);
        //mousePosition.z = 100f;

        //mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Debug.Log("World position : " + mousePosition);
        //transform.localPosition = mousePosition; 
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            mousePosition = Input.mousePosition;
            Debug.Log("Clicked!");
        }
        
        if(Input.GetMouseButton(0))
        {
            mousePosition = Input.mousePosition;
            Debug.Log(mousePosition);
        }

    }

}
