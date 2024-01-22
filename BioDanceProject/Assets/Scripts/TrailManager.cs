using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using System.Linq;


namespace TrailDrawing
{
    public class TrailManager : MonoBehaviour
    {
        #region define Parameters
        public int m_trailNum;
        public int m_nodeNum;
        

        #endregion

        #region define struct
        public struct Trail
        {
            public int currentNodeIndex;
        }

        public struct Node
        {
            public float time;
            public Vector3 pos;
        }

        public struct Input
        {
            public Vector3 pos;
        }
        #endregion

        #region related-ComputeShader
        public ComputeShader TrailCS;
        public ComputeBuffer trailBuffer;
        public ComputeBuffer nodeBuffer;
        public ComputeBuffer inputBuffer;
        #endregion

        #region Monobehaviour functions

        void Start()
        {
            InitBuffer();
            
        }


        void LateUpdate()
        {
            
        }

        void OnDestroy()
        {
            //Release Buffer here.
            if(trailBuffer != null)
            {
                trailBuffer.Release();
                trailBuffer = null;
            }

            if(nodeBuffer != null)
            {
                nodeBuffer.Release();
                nodeBuffer = null;
            }

            if(inputBuffer != null)
            {
                inputBuffer.Release();
                inputBuffer = null;
            }
        }

        #endregion

        #region self-define functions
        void InitBuffer()
        {

            int allNodeNum = m_nodeNum * m_trailNum;
            //init buffers
            trailBuffer = new ComputeBuffer(m_trailNum,
                                 Marshal.SizeOf(typeof(Trail)));
            nodeBuffer = new ComputeBuffer(allNodeNum,
                                 Marshal.SizeOf(typeof(Node)));
            inputBuffer = new ComputeBuffer(m_trailNum,
                                 Marshal.SizeOf(typeof(Input)));

            //init All Buffers
            var firstTrail = new Trail(){currentNodeIndex = -1};
            var firstNode = new Node(){time = -1};

            var trailArr = Enumerable.Repeat(firstTrail, m_trailNum).ToArray();
            var nodeArr = Enumerable.Repeat(firstNode, allNodeNum).ToArray();

            trailBuffer.SetData(trailArr);
            nodeBuffer.SetData(nodeArr);
        }
        #endregion


    }

}

