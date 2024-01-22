using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrailDrawing{
    [RequireComponent(typeof(TrailManager))]
    public class TrailEmit : MonoBehaviour
    {
        #region struct define
        public struct Emitter
        {
            public Vector3 position;
        }
        #endregion

        #region setting
        public ComputeShader emitCS;

        #endregion

        #region Monobehaviour functions
        void Start()
        {
            
        }

 
        void Update()
        {
            TrailUpdater();
            
        }
        #endregion

        #region self-define functions
        void TrailUpdater()
        {

        }
        #endregion
    }
}

