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
    [Range(256, 32768)]
    private int m_MaxObjectNum = 16384;

    // radius with other objects
    private float m_CohesionNeighborRadius = 2.0f; //apply COHESION
    private float m_AlignmentNeighborRadius = 2.0f;//apply ALIGNMENT
    private float m_SeparationNeighborRadius = 2.0f;//apply SEPARATION

    private float m_MaxSpeed = 5.0f;  //Max Speed
    private float m_MaxSteerForce = 0.5f; //Max Steer Force
    private float m_MinSpeed = 0.5f;  //Max Speed

    //Weight
    private float m_CohesionWeight = 1.0f; //Cohesion force
    private float m_AlignmentWeight = 1.0f; //Alignment force
    private float m_SeparationWeight = 1.0f; //Separation force

    private float m_AvoidFrameWeight = 10.0f; //Weight for avoiding frame

    //Angle
    private float m_CohesionAngle; //Cohsion
    private float m_AligmentAngle; //Aligment
    private float m_SeparationAngle; //Separation

    private Vector3 m_FrameCenter = Vector3.zero; //center position of frame
    private Vector3 m_FrameSize = new Vector3(32.0f, 32.0f, 32.0f); //frame size
    #endregion

    private ComputeShader BoidsCS; //reference ComputeSgader
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
