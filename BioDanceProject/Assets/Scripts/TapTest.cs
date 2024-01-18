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
        if(Input.GetMouseButtonUp(0))
        {
            Debug.Log(mousePosition);
        }
    }
}
