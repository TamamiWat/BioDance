using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
namespace BoidsSimulation
{
    // 同GameObjectに、GPUBoidsコンポーネントがアタッチされていること保証
    [RequireComponent(typeof(Boids))]
    public class BoidsRender : MonoBehaviour
    {
        #region Paremeters
        // 描画するBoidsオブジェクトのスケール
        public Vector3 ObjectScale = new Vector3(0.1f, 0.2f, 0.5f);
        #endregion

        #region Script References
        // GPUBoidsスクリプトの参照
        public Boids BoidsCS;
        #endregion

        #region Built-in Resources
        // 描画するメッシュの参照
        public Mesh InstanceMesh;
        // 描画のためのマテリアルの参照
        public Material InstanceRenderMaterial;
        #endregion

        #region Private Variables
        // GPUインスタンシングのための引数（ComputeBufferへの転送用）
        // インスタンスあたりのインデックス数, インスタンス数, 
        // 開始インデックス位置, ベース頂点位置, インスタンスの開始位置
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        // GPUインスタンシングのための引数バッファ
        ComputeBuffer argsBuffer;
        #endregion

        #region MonoBehaviour Functions
        void Start()
        {
            // 引数バッファを初期化
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint),
                ComputeBufferType.IndirectArguments);
        }

        void Update()
        {
            // メッシュをインスタンシング
            RenderInstancedMesh();
        }

        void OnDisable()
        {
            // 引数バッファを解放
            if (argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;
        }
        #endregion

        #region Private Functions
        void RenderInstancedMesh()
        {
            // 描画用マテリアルがNull, または, GPUBoidsスクリプトがNull,
            // またはGPUインスタンシングがサポートされていなければ, 処理をしない
            if (InstanceRenderMaterial == null || BoidsCS == null ||
                !SystemInfo.supportsInstancing)
                return;

            // 指定したメッシュのインデックス数を取得
            uint numIndices = (InstanceMesh != null) ?
                (uint)InstanceMesh.GetIndexCount(0) : 0;
            args[0] = numIndices; // メッシュのインデックス数をセット
            args[1] = (uint)BoidsCS.GetMaxObjectNum(); // インスタンス数をセット
            argsBuffer.SetData(args); // バッファにセット

            // Boidデータを格納したバッファをマテリアルにセット
            InstanceRenderMaterial.SetBuffer("_BoidDataBuffer",
                BoidsCS.GetBoidDataBuffer());
            // Boidオブジェクトスケールをセット
            InstanceRenderMaterial.SetVector("_ObjectScale", ObjectScale);
            // 境界領域を定義
            var bounds = new Bounds
            (
                BoidsCS.GetSimulationAreaCenter(), // 中心
                BoidsCS.GetSimulationAreaSize()    // サイズ
            );
            // メッシュをGPUインスタンシングして描画
            Graphics.DrawMeshInstancedIndirect
            (
                InstanceMesh,           // インスタンシングするメッシュ
                0,                      // submeshのインデックス
                InstanceRenderMaterial, // 描画を行うマテリアル 
                bounds,                 // 境界領域
                argsBuffer              // GPUインスタンシングのための引数のバッファ 
            );
        }
        #endregion
    }
}
=======
public class BoidsRender : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
>>>>>>> origin/lab
