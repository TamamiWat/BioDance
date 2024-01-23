using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapTest : MonoBehaviour
{
    Vector3 mousePosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
<<<<<<< HEAD

        Debug.Log("Screen position : " + mousePosition);
        mousePosition.z = 100f;

        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Debug.Log("World position : " + mousePosition);
        transform.localPosition = mousePosition; 
        
        /*if(Input.GetMouseButtonUp(0))
        {
            Debug.Log(mousePosition);
        }*/
=======
        if(Input.GetMouseButtonUp(0))
        {
            Debug.Log(mousePosition);
        }
>>>>>>> origin/lab
    }
}
