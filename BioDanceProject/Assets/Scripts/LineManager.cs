using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DrawLine
{
    public class LineManager : MonoBehaviour
    {
        #region struct
        public struct User
        {
            public Vector3 position;
        }

        public struct OutputInfo
        {
            public Vector3 position;
        }

        public struct Line
        {
            public int currentNodeIdx;
        }

        public struct Node
        {
            public float time;
            public Vector3 pos;
        }

        public struct Emitter
        {
            public Vector3 pos;
        }
        #endregion

        #region set inspector
        public int lineNum;
        [Range(2, 50)] public int vertexNum = 4;
        public Material material;
        public ComputeShader cs;
        #endregion

        #region private variables
        int userPoint = 0;
        Vector3 mousePosition = Vector3.zero;
        bool isDrag = false;
        public ComputeBuffer _LineBuffer;
        public ComputeBuffer _nodeBuffer;
        public ComputeBuffer _EmitterBuffer;

        public int nodeNum {get; protected set;}
        public ComputeBuffer _inputBuffer;
        public ComputeBuffer _outputBuffer;
        List<User> userPositions = new List<User>();
        int kernelID;
        #endregion

        #region Monobehaviour function
        private void Start()
        {
            Application.targetFrameRate = 60;
            initBuffer();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                userPositions.Clear();
            }
            else if (Input.GetMouseButton(0))
            {
                isDrag = true;
                mousePosition = GetWorldPos(Input.mousePosition);
                userPositions.Add(new User { position = mousePosition });
                //Debug.Log(userPositions.Count);
                UpdateBuffer();
                cs.Dispatch(kernelID, 1, 1, 1);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDrag = false;
            }
        }


        private void OnDestroy()
        {
            if (_inputBuffer != null)
            {
                _inputBuffer.Release();
                _inputBuffer = null;
            }
            if (_outputBuffer != null)
            {
                _outputBuffer.Release();
                _outputBuffer = null;
            }
        }

        private void OnRenderObject()
        {
        
            if(isDrag)
            {
                material.SetInt("_VertexNum", vertexNum - 1);
                material.SetBuffer("_InputBuffer", _inputBuffer);
                material.SetPass(0);
                var bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
                Graphics.DrawProceduralNow(MeshTopology.Triangles, vertexNum);
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
            _outputBuffer = new ComputeBuffer(3, Marshal.SizeOf(typeof(OutputInfo)));
            kernelID = cs.FindKernel("CalcInput");
            cs.SetBuffer(kernelID, "_InputBuffer", _inputBuffer);
            cs.SetBuffer(kernelID, "_OutputBuffer", _outputBuffer);
        }

        void UpdateBuffer()
        {
            if (_inputBuffer.count != userPositions.Count)
            {
                _inputBuffer.Release();
                _inputBuffer = new ComputeBuffer(userPositions.Count, Marshal.SizeOf(typeof(User)));
                cs.SetBuffer(kernelID, "_InputBuffer", _inputBuffer);
            }

            _inputBuffer.SetData(userPositions.ToArray());
            vertexNum = userPositions.Count * 3; 
        }

        #endregion
    }


}
