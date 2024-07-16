using System.Collections;
using UnityEngine;

namespace Behaviours
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
    }

    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine routine);
    }
}