using UnityEngine;

namespace Helpers
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start() => _mainCamera = Camera.main;

        private void Update()
        {
            if (_mainCamera != null)
                transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                    _mainCamera.transform.rotation * Vector3.up);
        }
    }
}