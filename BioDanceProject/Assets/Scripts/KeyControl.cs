using UnityEngine;

public class QuitOnKeyPress : MonoBehaviour
{
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}