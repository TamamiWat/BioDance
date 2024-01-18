/*
Function:
    > Set Parameters for Boids Simulation
    > Manage ComputeShader : Boids.compute

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Boids : MonoBehaviour
{
    //Boids Data struct
    [System.Serializable]
    struct BoidsData
    {
        public Vector3 Velocity;
        public Vector3 Position;
    }

    const int SIMULATION_BLOCK_SIZE = 256;
    
    #region Boids Parameters
    [Range(256, 32768), SerializeField]
    private int m_MaxObjectNum = 16384;

    // radius with other objects
    [Range(0f, 1f), SerializeField]
    private float m_CohesionNeighborRadius = 0.8f; //apply COHESION
    [Range(0f, 1f), SerializeField]
    private float m_AlignmentNeighborRadius = 0.5f;//apply ALIGNMENT
    [Range(0f, 1f), SerializeField]
    private float m_SeparationNeighborRadius = 0.03f;//apply SEPARATION

    [Range(0f, 10f), SerializeField]
    private float m_MaxSpeed = 5.0f;  //Max Speed
    [Range(0f, 1f), SerializeField]
    private float m_MaxSteerForce = 0.5f; //Max Steer Force
    [Range(0f, 1f), SerializeField]
    private float m_MinSpeed = 0.5f;  //Max Speed

    //Weight
    [Range(0f, 1f), SerializeField]
    private float m_CohesionWeight = 0.005f; //Cohesion force
    [Range(0f, 1f), SerializeField]
    private float m_AlignmentWeight = 0.01f; //Alignment force
    [Range(0f, 1f), SerializeField]
    private float m_SeparationWeight = 0.5f; //Separation force

    [Range(0f, 1f), SerializeField]
    private float m_AvoidFrameWeight = 0.2f; //Weight for avoiding frame

    //Angle
    [Range(0f, 10f), SerializeField]
    private float m_CohesionAngle = 1.5f; //Cohsion
    [Range(0f, 10f), SerializeField]
    private float m_AligmentAngle = 1.5f; //Aligment
    [Range(0f, 10f), SerializeField]
    private float m_SeparationAngle = 1.5f; //Separation

    [SerializeField]
    private Vector3 m_FrameCenter = Vector3.zero; //center position of frame
    [SerializeField]
    private Vector3 m_FrameSize = new Vector3(32.0f, 32.0f, 32.0f); //frame size
    #endregion

    [SerializeField] private ComputeShader BoidsCS; //reference ComputeSgader
    private ComputeBuffer _boidsForceBuffer;
    private ComputeBuffer _boidsDataBuffer;

    private ComputeBuffer GetBoidDataBuffer()
    {
        //if _boidDataBuffer != null -> return _boidDataBuffer
        //elif _boidDataBuffer == null -> return null
        return this._boidsDataBuffer != null ? this._boidsDataBuffer : null;
    }

    private int GetMaxObjectNum()
    {
        return this.m_MaxObjectNum;
    }

    private Vector3 GetSimulationAreaCenter()
    {
        return this.m_FrameCenter;
    }

    private Vector3 GetSimulationAreaSize()
    {
        return this.m_FrameSize;
    }


    #region MonoBehavior Function
    void Start()
    {
        //init Buffers
        InitBuffer();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Simulation
        Simulation();
        
    }

    void OnDestroy()
    {
        ReleaseBuffer();
    }
    #endregion

    #region Self-defined functions
    //init buffers
    void InitBuffer()
    {
        //init buffers
        _boidsDataBuffer = new ComputeBuffer(m_MaxObjectNum, Marshal.SizeOf(typeof(BoidsData)));
        _boidsForceBuffer = new ComputeBuffer(m_MaxObjectNum, Marshal.SizeOf(typeof(Vector3)));

        //init BoidsData, SteerForce Buffer
        Vector3[] steerForceArray = new Vector3[m_MaxObjectNum];
        BoidsData[] boidsDataArray = new BoidsData[m_MaxObjectNum];
        for (int i = 0; i < m_MaxObjectNum ; i++)
        {
            steerForceArray[i] = Vector3.zero;

            //Allow position and velocity to have random values
            boidsDataArray[i].Velocity = Random.insideUnitSphere * 0.1f;
            boidsDataArray[i].Position = Random.insideUnitSphere * 1.0f;
        }

        //set to buffer on ComputeShader
        _boidsForceBuffer.SetData(steerForceArray);
        _boidsDataBuffer.SetData(boidsDataArray);

        //Array release
        steerForceArray = null;
        boidsDataArray = null;

    }

    //use computeShader for simulation
    void Simulation()
    {
        ComputeShader boidsCS = BoidsCS;
        int id;
        int threadGroupSize = Mathf.CeilToInt(m_MaxObjectNum / SIMULATION_BLOCK_SIZE);

        //Calculate Steer Force
        id = boidsCS.FindKernel("MotionCalculator");
        boidsCS.SetInt("m_MaxBoidsObjectNum", m_MaxObjectNum);
        boidsCS.SetFloat("m_CohesionNeighborRadius", m_CohesionNeighborRadius);
        boidsCS.SetFloat("m_AlignmentNeighborRadius", m_AlignmentNeighborRadius);
        boidsCS.SetFloat("m_SeparationNeighborRadius", m_SeparationNeighborRadius);

        boidsCS.SetFloat("m_MaxSpeed", m_MaxSpeed);
        boidsCS.SetFloat("m_MaxSteerForce", m_MaxSteerForce);
        boidsCS.SetFloat("m_MinSpeed", m_MinSpeed);

        boidsCS.SetFloat("m_CohesionWeight", m_SeparationNeighborRadius);
        boidsCS.SetFloat("m_AlignmentWeight", m_AlignmentWeight);
        boidsCS.SetFloat("m_SeparationWeight", m_SeparationWeight);

        boidsCS.SetFloat("m_AvoidFrameWeight", m_AvoidFrameWeight);

        boidsCS.SetFloat("m_CohesionAngle", m_CohesionAngle);
        boidsCS.SetFloat("m_AlignmentAngle", m_AligmentAngle);
        boidsCS.SetFloat("m_SeparationAngle", m_SeparationAngle);

        boidsCS.SetVector("m_FrameCenter", m_FrameCenter);
        boidsCS.SetVector("m_FrameSize", m_FrameSize);

        boidsCS.SetBuffer(id, "_BoidsForceBufferReadWrite", _boidsForceBuffer);
        boidsCS.SetBuffer(id, "_BoidsDataBufferRead", _boidsDataBuffer);

        boidsCS.Dispatch(id, threadGroupSize, 1, 1);


        //Calculate velocity & position
        id = boidsCS.FindKernel("MotionCalculator");
        boidsCS.SetFloat("m_DeltaTime", Time.deltaTime);

        boidsCS.SetBuffer(id, "_BoidsDataBufferReadWrite", _boidsDataBuffer);
        boidsCS.SetBuffer(id, "_BoidsForceBufferRead", _boidsForceBuffer);

        boidsCS.Dispatch(id, threadGroupSize, 1, 1);



    }

    //release buffer
    void ReleaseBuffer()
    {
        if(_boidsDataBuffer != null)
        {
            _boidsDataBuffer.Release();
            _boidsDataBuffer = null;
        }

        if(_boidsForceBuffer != null)
        {
            _boidsForceBuffer.Release();
            _boidsForceBuffer = null;
        }

    }
    #endregion
}
