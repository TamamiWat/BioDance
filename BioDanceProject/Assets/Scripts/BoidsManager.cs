using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

namespace BoidsSimulation
{
    public class Boids : MonoBehaviour
    {
        [System.Serializable]
        struct BoidData
        {
            public Vector3 Velocity; 
            public Vector3 Position; 
            public Vector4 Color;
            public Vector3 Scale;
        }

        struct Force
        {
            public Vector3 separation;
            public Vector3 aligment;
            public Vector3 cohesion;
            public Vector3 attraction;
            public Vector3 avoidance;
        }
   
        const int SIMULATION_BLOCK_SIZE = 256;

        #region Boids Parameters
        [Range(256, 32768)]
        public int m_MaxObjectNum = 16384;

        // radius with other objects
        [Range(0f, 10f)]
        public float m_CohesionNeighborRadius = 0.8f; //apply COHESION
        [Range(0f, 10f)]
        public float m_AlignmentNeighborRadius = 0.5f;//apply ALIGNMENT
        [Range(0f, 10f)]
        public float m_SeparationNeighborRadius = 0.03f;//apply SEPARATION

        [Range(0f, 10f)]
        public float m_MaxSpeed = 5.0f;  //Max Speed
        [Range(0f, 10f)]
        public float m_MaxSteerForce = 0.5f; //Max Steer Force
        [Range(0f, 1f)]
        public float m_MinSpeed = 0.5f;  //Min Speed

        //Weight
        [Range(0f, 100f)]
        public float m_CohesionWeight = 0.005f; //Cohesion force
        [Range(0f, 100f)]
        public float m_AlignmentWeight = 0.01f; //Alignment force
        [Range(0f, 100f)]
        public float m_SeparationWeight = 0.5f; //Separation force

        [Range(0f, 100f)]
        public float m_AvoidFrameWeight = 0.2f; //Weight for avoiding frame 

        //Angle
        [Range(0f, 10f)]
        public float m_CohesionAngle = 1.5f; //Cohsion
        [Range(0f, 10f)]
        public float m_AligmentAngle = 1.5f; //Aligment
        [Range(0f, 10f)]
        public float m_SeparationAngle = 1.5f; //Separation

        public Vector3 m_FrameCenter = Vector3.zero; //center position of frame
        public Vector3 m_FrameSize = new Vector3(32.0f, 32.0f, 32.0f); //frame size
        [Range(0f, 100f)] public float m_FrameRadius = 0f;
        [Range(0f, 100f)] public float m_AttractRange = 10f;
        [Range(0f, 1f)] public float m_hueMin = 0.0f;
        [Range(0f, 1f)] public float m_hueMax = 1.0f;
        [Range(0f, 1f)] public float m_satMin = 0.0f;
        [Range(0f, 1f)] public float m_satMax = 1.0f;
        [Range(0f, 1f)] public float m_valMin = 0.0f;
        [Range(0f, 1f)] public float m_valMax = 1.0f;
        public bool isUserDrag = false;
        public bool isUserInOut = false;
        #endregion

        #region Built-in Resources
        public ComputeShader BoidsCS;
        #endregion

        #region Private Resources
        ComputeBuffer _boidForceBuffer;
        ComputeBuffer _boidDataBuffer;
        ComputeBuffer _boidForceDataBuffer;
        private float m_range;

        Vector3 m_userPos;

        #endregion

        #region Accessors
        public ComputeBuffer GetBoidDataBuffer()
        {
            //if _boidDataBuffer != null -> return _boidDataBuffer
            //elif _boidDataBuffer == null -> return null
            return this._boidDataBuffer != null ? this._boidDataBuffer : null;
        }

        public int GetMaxObjectNum()
        {
            return this.m_MaxObjectNum;
        }

        public Vector3 GetSimulationAreaCenter()
        {
            return this.m_FrameCenter;
        }

        public Vector3 GetSimulationAreaSize()
        {
            return this.m_FrameSize;
        }
        #endregion

        #region MonoBehaviour Functions
        void Start()
        {
            //init Buffers
            InitBuffer();
        }

        void Update()
        {
            Simulation();
        }

        void OnDestroy()
        {
            ReleaseBuffer();
        }

        #endregion

        #region Self-defined functions
        // init buffers
        void InitBuffer()
        {
            // init buffers
            _boidDataBuffer = new ComputeBuffer(m_MaxObjectNum,
                Marshal.SizeOf(typeof(BoidData)));
            _boidForceBuffer = new ComputeBuffer(m_MaxObjectNum,
                Marshal.SizeOf(typeof(Vector3)));
            _boidForceDataBuffer = new ComputeBuffer(m_MaxObjectNum,
                Marshal.SizeOf(typeof(Force)));

            // init BoidsData, SteerForce Buffer
            var forceArr = new Vector3[m_MaxObjectNum];
            var boidDataArr = new BoidData[m_MaxObjectNum];
            var boidForceDataArr = new Force[m_MaxObjectNum];
            m_range = m_FrameSize.x / 2;

            for (var i = 0; i < m_MaxObjectNum; i++)
            {
                Vector4 initColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

                forceArr[i] = Vector3.zero;
                boidDataArr[i].Position = RandomVector(-m_range, m_range);
                boidDataArr[i].Velocity = Random.insideUnitSphere * 1.0f;
                boidDataArr[i].Color = initColor;
                boidDataArr[i].Scale = RandomVector(0.2f, 0.8f);
                boidForceDataArr[i].separation = Vector3.zero;
                boidForceDataArr[i].aligment = Vector3.zero;
                boidForceDataArr[i].cohesion = Vector3.zero;
                boidForceDataArr[i].attraction = Vector3.zero;
                boidForceDataArr[i].avoidance = Vector3.zero;
            }
            _boidForceBuffer.SetData(forceArr);
            _boidDataBuffer.SetData(boidDataArr);
            _boidForceDataBuffer.SetData(boidForceDataArr);
            forceArr = null;
            boidDataArr = null;
            boidForceDataArr = null;
        }

        // simulation
        void Simulation()
        {
            m_userPos = Input.mousePosition;
            m_userPos.z = 6.0f;
            m_userPos = Camera.main.ScreenToWorldPoint(m_userPos);
            if(Input.GetMouseButton(0))
            {
                isUserDrag = true;
            }
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                isUserInOut = true;
                Debug.Log(m_userPos); 
            }

            ComputeShader boidCS = BoidsCS;
            int id = -1;
            int threadGroupSize = Mathf.CeilToInt(m_MaxObjectNum / SIMULATION_BLOCK_SIZE);

            id = boidCS.FindKernel("SteerForceCalculator"); 
            boidCS.SetInt("_MaxBoidObjectNum", m_MaxObjectNum);
            boidCS.SetFloat("_CohesionNeighborhoodRadius", m_CohesionNeighborRadius);
            boidCS.SetFloat("_AlignmentNeighborhoodRadius", m_AlignmentNeighborRadius);
            boidCS.SetFloat("_SeparateNeighborhoodRadius", m_SeparationNeighborRadius);
            boidCS.SetFloat("_MaxSpeed", m_MaxSpeed);
            boidCS.SetFloat("_MaxSteerForce", m_MaxSteerForce);
            boidCS.SetFloat("_MinSpeed", m_MinSpeed);
            boidCS.SetFloat("_SeparationWeight", m_SeparationWeight);
            boidCS.SetFloat("_CohesionWeight", m_CohesionWeight);
            boidCS.SetFloat("_AlignmentWeight", m_AlignmentWeight);
            boidCS.SetVector("_FrameCenter", m_FrameCenter);
            boidCS.SetVector("_FrameSize", m_FrameSize);
            boidCS.SetFloat("_FrameRadius", m_FrameRadius);
            boidCS.SetFloat("_AttractRange", m_AttractRange);
            boidCS.SetVector("_DragPos", m_userPos);
            boidCS.SetVector("_TapPos", m_userPos);
            boidCS.SetBool("_userInOut", isUserInOut);  
            boidCS.SetBool("_userDrag", isUserDrag);            
            boidCS.SetFloat("_AvoidFrameWeight", m_AvoidFrameWeight);
            boidCS.SetFloat("_CohesionAngle", m_CohesionAngle);
            boidCS.SetFloat("_AlignmentAngle", m_AligmentAngle);
            boidCS.SetFloat("_SeparationAngle", m_SeparationAngle);
            boidCS.SetFloat("_hueMin", m_hueMin);
            boidCS.SetFloat("_hueMax", m_hueMax);
            boidCS.SetFloat("_satMin", m_satMin);
            boidCS.SetFloat("_satMax", m_satMax);
            boidCS.SetFloat("_valMin", m_valMin);
            boidCS.SetFloat("_valMax", m_valMax);
            boidCS.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
            boidCS.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
            boidCS.SetBuffer(id, "_BoidForceDataBufferWrite", _boidForceDataBuffer);
            boidCS.Dispatch(id, threadGroupSize, 1, 1); 

            
            id = boidCS.FindKernel("MotionCalculator");
            boidCS.SetFloat("_DeltaTime", Time.deltaTime);
            boidCS.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
            boidCS.SetBuffer(id, "_BoidForceDataBufferRead", _boidForceDataBuffer);
            boidCS.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
            boidCS.Dispatch(id, threadGroupSize, 1, 1);
            isUserInOut = false;
            isUserDrag = false; 
        }

        // Release Buffer
        void ReleaseBuffer()
        {
            if (_boidDataBuffer != null)
            {
                _boidDataBuffer.Release();
                _boidDataBuffer = null;
            }

            if (_boidForceBuffer != null)
            {
                _boidForceBuffer.Release();
                _boidForceBuffer = null;
            }

            if(_boidForceDataBuffer != null)
            {
                _boidForceDataBuffer.Release();
                _boidForceDataBuffer = null;
            }
        }

        private Vector3 RandomVector(float min,float max)
        {
            return new Vector3(
                Random.Range(min, max),
                Random.Range(min, max),
                Random.Range(min, max)
            );
        }
        #endregion
    } // class
} // namespace