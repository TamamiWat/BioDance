using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class LineRender : MonoBehaviour
{
    #region set inspector
    [Range(2, 50)] public int vertexNum = 4;
    public Material material;
    #endregion

    #region private variables
    Vector3 mousePosition = Vector3.zero;
    bool isDrag = false;
    ComputeBuffer _inputBuffer;
    #endregion

    #region struct
    public struct User
    {
        public Vector3 position;
    }
    #endregion

    #region Monobehaviour function
    private void Start()
    {
        initBuffer();
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            _inputBuffer = null;
            initBuffer();
        }
    }

    private void OnDestroy()
    {
        _inputBuffer.Release();
        _inputBuffer = null;
    }

    private void OnRenderObject()
    {
       
        if(Input.GetMouseButton(0))
        {
            mousePosition = GetWorldPos(Input.mousePosition);
            material.SetInt("_VertexNum", vertexNum - 1);
            material.SetBuffer("_InputBuffer", _inputBuffer);
            material.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.LineStrip, vertexNum);
        }
       
    }


    #endregion

    #region self-define function
    Vector3 GetWorldPos(Vector3 pos)
    {
        pos.z = 100f;
        return Camera.main.ScreenToWorldPoint(pos);
    }

    void initBuffer()
    {
        _inputBuffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(User)));
    }

    #endregion
}
