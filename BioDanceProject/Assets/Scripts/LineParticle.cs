using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DrawLine{
    [RequireComponent(typeof(LineManager))]
    public class LineParticle : MonoBehaviour
    {
        #region struct
        public struct Particle
        {
            public Vector3 position;
        }
        #endregion

        #region set from inspector
        public ComputeShader cs;
        
        #endregion

        #region private
        protected ComputeBuffer _ParticleBuffer;
        protected LineManager line;

        protected int particleNum => line.lineNum;
        #endregion

        #region Monobehaviour function
        void Start()
        {
            line = GetComponent<LineManager>();

            initBuffer();
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnDestroy()
        {
            _ParticleBuffer.Release();
        }
        #endregion

        #region self-define function
        void initBuffer()
        {
            _ParticleBuffer = new ComputeBuffer(particleNum, Marshal.SizeOf(typeof(Particle)));
            
        }
        #endregion
    }


}
