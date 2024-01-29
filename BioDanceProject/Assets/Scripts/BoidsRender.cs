using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoidsSimulation
{
    [RequireComponent(typeof(Boids))]
    public class BoidsRender : MonoBehaviour
    {
        #region Paremeters
        //public Vector3 ObjectScale = new Vector3(0.1f, 0.2f, 0.5f);
        #endregion

        #region Script References
        public Boids BoidsCS;
        #endregion

        #region Built-in Resources
        public Mesh _mesh;
        public Material _material;
        #endregion

        #region Private Variables
        // Arguments for GPU instancing (for transfer to ComputeBuffer).
        // number of indexes per instance, number of instances,. 
        // Start index position, base vertex position, start position of instance
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        GraphicsBuffer _argsBuffer;
        #endregion

        #region MonoBehaviour Functions
        void Start()
        {
            _argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, args.Length * sizeof(uint));
        }

        void Update()
        {
            RenderInstancedMesh();
        }

        void OnDisable()
        {
            if (_argsBuffer != null)
                _argsBuffer.Release();
            _argsBuffer = null;
        }
        #endregion

        #region Private Functions
        void RenderInstancedMesh()
        {
            if (_material == null || BoidsCS == null ||
                !SystemInfo.supportsInstancing)
                return;

            // mesh index number
            uint meshIndex = (_mesh != null) ?
                (uint)_mesh.GetIndexCount(0) : 0;
            args[0] = meshIndex; // set mesh index
            args[1] = (uint)BoidsCS.GetMaxObjectNum(); // set instance number
            _argsBuffer.SetData(args); 

            // Set the buffer containing the Boid data to the material.
            _material.SetBuffer("_BoidDataBuffer",
                BoidsCS.GetBoidDataBuffer());
            // Set Boid object scale.
            //_material.SetVector("_ObjectScale", ObjectScale);
            // Define boundary area.
            var bounds = new Bounds
            (
                BoidsCS.GetSimulationAreaCenter(), 
                BoidsCS.GetSimulationAreaSize()    
            );
            
            //GPU instancing
            Graphics.DrawMeshInstancedIndirect
            (
                _mesh,           
                0,                      
                _material, 
                bounds,                 
                _argsBuffer              
            );
        }
        #endregion
    }
}