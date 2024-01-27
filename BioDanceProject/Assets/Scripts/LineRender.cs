using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DrawLine
{
    [RequireComponent(typeof(LineManager))]
    public class LineRender : MonoBehaviour
    {

        public Material _material;
        public float _life;
        LineManager _lines;
        // Start is called before the first frame update
        void Start()
        {
            _lines = GetComponent<LineManager>();
        }

        void OnRenderObject()
        {
            _material.SetPass(0);
        }
    }


}
