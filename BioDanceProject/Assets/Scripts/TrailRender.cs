using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using Unity.Mathematics;

namespace TrailDrawing
{
    //[RequireComponent(typeof(TrailManager))]
    public class TrailRenderer : MonoBehaviour
    {
        #region Set from Inspector
        public Material m_renderMat;
        public ComputeShader cs;
        [Range(0f, 100f)]public float m_life;
        #endregion

        #region private
        private ComputeBuffer _PreCalc;
        private ComputeBuffer _PostCalc;
        private MaterialPropertyBlock m_ptoperties;
        private int kernelID;
        private vector3 m_DragPos;


        #endregion

        #region struct define
        public struct InputSource
        {
            public float3 position;
        }

        public struct OutputSource
        {
            public float3 position;
        } 
        #endregion
        
        #region Monobehaviour functions
        void Awake()
        {
            initBuffer();
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnDisable()
        {
            PreCalc.Release();
            PreCalc = null;
            PostCalc.Release();
            PostCalc = null;
            Destroy(Material);
            Material = null;
        }
        #endregion

        #region self-define funcitons
        void initBuffer()
        {
            _PreCalc = new ComputeBuffer(1, Marshal.SizeOf(InputSource));
            _PostCalc = new ComputeBuffer(3, Marshal.SizeOf(InputSource));

            kernelID = cs.FindKernel("CalcVert");
            cs.SetBuffer(kernelID, "_InputSource", _PreCalc);
            cs.SetBuffer(kernelID, "_OutputBuffer", _PostCalc);
            

        }
        #endregion
    }
}

