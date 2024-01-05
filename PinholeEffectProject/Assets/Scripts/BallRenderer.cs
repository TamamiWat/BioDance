using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

public class BallRenderer : MonoBehaviour
{
    //Parameters
    [Header("DrawMeshInstancedIndirectのパラメータ")]
    [SerializeField] private Mesh m_mesh; //描画するメッシュ
    [SerializeField] private Material m_instanceMaterial; //使用するマテリアル
    [SerializeField] private Bounds m_bounds; //描画するメッシュの境界
    [SerializeField] private ShadowCastingMode m_shadowCastingMode; //影のキャスティングモード
    [SerializeField] private bool m_receiveShadows; //影を受け取るかどうか

    [Space(20)]
    [SerializeField] private int m_instanceCount; //インスタンスの数
    [SerializeField] private ComputeShader m_particleCalclator; //パーティクルの位置計算に使用するコンピュートシェーダ

    [Header("パーティクル設定")]
    [Space(20)]
    [SerializeField] private float m_range; //範囲
    [SerializeField] private float m_scale; //スケール
    [SerializeField] private Color m_color; //色
    [SerializeField] private float m_particleVelocity; //速度

    private int m_calcParticlePositionKernel;
    private Vector3Int m_calcParticlePositionGroupSize;

    private ComputeBuffer m_argsBuffer;
    private ComputeBuffer m_particleBuffer;
    private ComputeBuffer m_particleVelocityBuffer;

    private MaterialPropertyBlock m_materialPropertyBlock;


    private readonly int m_DeltaTimePropId = Shader.PropertyToID("_DeltaTime");
    struct Particle
    {
        public Vector3 position;//位置
        public Vector4 color;//色
        public float scale;//スケール

    }


    // Start is called before the first frame update
    void Start()
    {
        m_materialPropertyBlock = new MaterialPropertyBlock();
        InitializeArgsBuffer();
        InitializeParticleBuffer();
        InitializeVelocityBuffer();
        SetUpParticleCalclator();


    }

    // Update is called once per frame
    void Update()
    {
        m_particleCalclator.SetFloat(m_DeltaTimePropId, Time.deltaTime);
        m_particleCalclator.Dispatch(m_calcParticlePositionKernel,
                                     m_calcParticlePositionGroupSize.x,
                                     m_calcParticlePositionGroupSize.y,
                                     m_calcParticlePositionGroupSize.z);
        m_materialPropertyBlock.SetVectorArray("_Color", );
    }

    // Update is called once per frame
    //Update関数が呼び出された後に実行される
    void LateUpdate()
    {
        //GPUインスタンシング
        Graphics.DrawMeshInstancedIndirect(
            m_mesh,
            0,
            m_instanceMaterial,
            m_bounds,
            m_argsBuffer,
            0,
            null,
            m_shadowCastingMode,
            m_receiveShadows
        );

    }
    
    //描画に必要なバッファを初期化
    private void InitializeArgsBuffer()
    {

        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        uint numIndices = (m_mesh != null) ? (uint)m_mesh.GetIndexCount(0) : 0;

        args[0] = numIndices;
        args[1] = (uint)m_instanceCount;

        m_argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        m_argsBuffer.SetData(args);

    }

    //パーティクルデータを保持するバッファを初期化
    private void InitializeParticleBuffer()
    {

        Particle[] particles = new Particle[m_instanceCount];

        for (int i = 0; i < m_instanceCount; ++i)
        {
            particles[i].position = RandomVector(-m_range, m_range);
            particles[i].color = m_color;
            particles[i].scale = m_scale;
        }

        m_particleBuffer = new ComputeBuffer(m_instanceCount, Marshal.SizeOf(typeof(Particle)));
        m_particleBuffer.SetData(particles);

        m_instanceMaterial.SetBuffer("_ParticleBuffer", m_particleBuffer);
    }

    //パーティクルの速度を保持するバッファを初期化
    private void InitializeVelocityBuffer()
    {

        Vector3[] velocities = new Vector3[m_instanceCount];

        for (int i = 0; i < m_instanceCount; ++i)
        {
            // Random.onUnitySphere:半径1の球面上のランダムな点を返す
            // つまり、大きさm_particleVelocityのランダムなベクトルを計算
            velocities[i] = Random.onUnitSphere * m_particleVelocity;
        }

        m_particleVelocityBuffer = new ComputeBuffer(m_instanceCount, Marshal.SizeOf(typeof(Vector3)));
        m_particleVelocityBuffer.SetData(velocities);


    }

    //コンピュートシェーダの設定
    private void SetUpParticleCalclator()
    {

        m_calcParticlePositionKernel = this.m_particleCalclator.FindKernel("CalcParticlePosition");

        m_particleCalclator.GetKernelThreadGroupSizes(m_calcParticlePositionKernel,
                                                      out uint x,
                                                      out uint y,
                                                      out uint z);

        m_calcParticlePositionGroupSize = new Vector3Int(m_instanceCount / (int)x, (int)y, (int)z);

        m_particleCalclator.SetFloat("_Range", m_range);
        m_particleCalclator.SetBuffer(m_calcParticlePositionKernel, "_Particle", m_particleBuffer);
        m_particleCalclator.SetBuffer(m_calcParticlePositionKernel, "_ParticleVelocity", m_particleVelocityBuffer);

    }

    private Vector3 RandomVector(float min, float max)
    {

        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max)
            );

    }

    // 領域の解放
    private void OnDisable()
    {

        m_particleBuffer?.Release();
        m_particleVelocityBuffer?.Release();
        m_argsBuffer?.Release();

    }

}
