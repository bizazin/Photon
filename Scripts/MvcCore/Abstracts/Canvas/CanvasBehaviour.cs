using UnityEngine;

namespace MvcCore.Abstracts.Canvas
{
    [RequireComponent(typeof(UnityEngine.Canvas))]
    public class CanvasBehaviour : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Canvas _canvas;

        public UnityEngine.Canvas Canvas => _canvas;
    }
}