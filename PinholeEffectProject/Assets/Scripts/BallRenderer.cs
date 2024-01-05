using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

public class BallRenderer : MonoBehaviour
{
    //Parameters
    [Header("DrawMeshInstancedIndirect�̃p�����[�^")]
    [SerializeField] private Mesh m_mesh; //�`�悷�郁�b�V��
    [SerializeField] private Material m_instanceMaterial; //�g�p����}�e���A��
    [SerializeField] private Bounds m_bounds; //�`�悷�郁�b�V���̋��E
    [SerializeField] private ShadowCastingMode m_shadowCastingMode; //�e�̃L���X�e�B���O���[�h
    [SerializeField] private bool m_receiveShadows; //�e���󂯎�邩�ǂ���

    [Space(20)]
    [SerializeField] private int m_instanceCount; //�C���X�^���X�̐�
    [SerializeField] private ComputeShader m_particleCalclator; //�p�[�e�B�N���̈ʒu�v�Z�Ɏg�p����R���s���[�g�V�F�[�_

    [Header("�p�[�e�B�N���ݒ�")]
    [Space(20)]
    [SerializeField] private float m_range; //�͈�
    [SerializeField] private float m_scale; //�X�P�[��
    [SerializeField] private Color m_color; //�F
    [SerializeField] private float m_particleVelocity; //���x

    private int m_calcParticlePositionKernel;
    private Vector3Int m_calcParticlePositionGroupSize;

    private ComputeBuffer m_argsBuffer;
    private ComputeBuffer m_particleBuffer;
    private ComputeBuffer m_particleVelocityBuffer;

    private MaterialPropertyBlock m_materialPropertyBlock;


    private readonly int m_DeltaTimePropId = Shader.PropertyToID("_DeltaTime");
    struct Particle
    {
        public Vector3 position;//�ʒu
        public Vector4 color;//�F
        public float scale;//�X�P�[��

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
    //Update�֐����Ăяo���ꂽ��Ɏ��s�����
    void LateUpdate()
    {
        //GPU�C���X�^���V���O
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
    
    //�`��ɕK�v�ȃo�b�t�@��������
    private void InitializeArgsBuffer()
    {

        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        uint numIndices = (m_mesh != null) ? (uint)m_mesh.GetIndexCount(0) : 0;

        args[0] = numIndices;
        args[1] = (uint)m_instanceCount;

        m_argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        m_argsBuffer.SetData(args);

    }

    //�p�[�e�B�N���f�[�^��ێ�����o�b�t�@��������
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

    //�p�[�e�B�N���̑��x��ێ�����o�b�t�@��������
    private void InitializeVelocityBuffer()
    {

        Vector3[] velocities = new Vector3[m_instanceCount];

        for (int i = 0; i < m_instanceCount; ++i)
        {
            // Random.onUnitySphere:���a1�̋��ʏ�̃����_���ȓ_��Ԃ�
            // �܂�A�傫��m_particleVelocity�̃����_���ȃx�N�g�����v�Z
            velocities[i] = Random.onUnitSphere * m_particleVelocity;
        }

        m_particleVelocityBuffer = new ComputeBuffer(m_instanceCount, Marshal.SizeOf(typeof(Vector3)));
        m_particleVelocityBuffer.SetData(velocities);


    }

    //�R���s���[�g�V�F�[�_�̐ݒ�
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

    // �̈�̉��
    private void OnDisable()
    {

        m_particleBuffer?.Release();
        m_particleVelocityBuffer?.Release();
        m_argsBuffer?.Release();

    }

}
