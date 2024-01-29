using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
public class LineRender : MonoBehaviour
{
    public struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float lifetime;
    }
    const int SIMULATION_BLOCK_SIZE = 256;

    [Range(128, 32768)]public int m_objectNum = 256;
    public ComputeShader cs;
    public ComputeBuffer _outputBuffer;
    public Mesh m_mesh;
    public Material m_material;
    [Range(0.1f, 100f)]public float p = 10.0f;
    [Range(0.1f, 100f)]public float r = 28.0f;
    [Range(0.1f, 100f)]public float b = 8.0f / 3.0f;

    #region private
    private int kernelID;
    private float dragTime = 0f;
    private Vector3 dragPos;
    private bool isDragging = false;
    private List<Matrix4x4> matrices = new List<Matrix4x4>();
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    GraphicsBuffer argsBuffer;
    #endregion
    #region Monobehaviour function
    void Start()
    {
        initBuffer();
        argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, args.Length * sizeof(uint));
        UpdateArgsBuffer();
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragTime = 0f;
            dragPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        }
        else if(Input.GetMouseButton(0))
        {
            dragTime += Time.deltaTime;
            dragPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        }
        if(Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            dragTime = 0f;
        }
        if(isDragging){
            matrices.Add(Matrix4x4.TRS(dragPos, Quaternion.identity, Vector3.one));
            LorenzCalc();
            RenderParticle();

        }        
    }

    void OnDestroy()
    {
        ReleaseBuffer();
    }
    void OnDisable()
    {
        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }
    #endregion

    #region self-define function
    void initBuffer()
    {
        _outputBuffer = new ComputeBuffer(m_objectNum,
                                Marshal.SizeOf(typeof(Particle)));
        var ParticleArr = new Particle[m_objectNum];
        for(var i = 0; i < m_objectNum; i++)
        {
            ParticleArr[i].position =  Random.insideUnitSphere;
            ParticleArr[i].velocity = Random.insideUnitSphere;
            ParticleArr[i].lifetime = Random.Range(1f, 5f);
        }
        _outputBuffer.SetData(ParticleArr);
        ParticleArr = null;
        kernelID = cs.FindKernel("CalcLorenz");
    }

    void LorenzCalc()
    {
        cs.SetBuffer(kernelID, "_particle", _outputBuffer);
        cs.SetFloat("_deltaTime", Time.deltaTime);
        cs.SetFloat("_dragTime", dragTime);
        cs.SetVector("_dragPos", dragPos);

        cs.SetFloat("_p", p);
        cs.SetFloat("_r", r);
        cs.SetFloat("_b", b);
        int num =  Mathf.CeilToInt(m_objectNum / SIMULATION_BLOCK_SIZE);
        cs.Dispatch(kernelID, num, 1, 1);
    }

    void RenderParticle()
    {
        if (m_material == null || 
                !SystemInfo.supportsInstancing)
                return;
        
        UpdateArgsBuffer();
        
        if (matrices.Count > 0)
        {
            Graphics.DrawMeshInstanced(m_mesh, 0, m_material, matrices);
        }
        Debug.Log("called");
    }

    void ReleaseBuffer()
    {
        if(_outputBuffer != null){
            _outputBuffer.Release();
            _outputBuffer = null;
        }
    }

    void UpdateArgsBuffer()
    {
        uint meshIndex =  (m_mesh != null) ?
                (uint)m_mesh.GetIndexCount(0) : 0;

        args[0] = meshIndex;
        args[1] = (uint)m_objectNum;
        argsBuffer.SetData(args);
        
        m_material.SetBuffer("_particle", _outputBuffer);

    }
    public ComputeBuffer GetBuffer()
    {
        return this._outputBuffer != null ? this._outputBuffer : null;
    }
    #endregion
}
