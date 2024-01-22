using UnityEngine;


namespace BoidsSimulation
{
    public class CameraRotation : MonoBehaviour
    {

        public float _speed = 1f;

        void Update()
        {
            var rot = transform.eulerAngles;
            rot.y += _speed * Time.deltaTime;
            rot.x = 90f * Mathf.Sin(Time.time / 45f);
            transform.rotation = Quaternion.Euler(rot);
        }
    }
}