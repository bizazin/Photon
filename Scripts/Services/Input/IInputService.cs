using System;
using UnityEngine;

namespace Services.Input
{
    public interface IInputService
    {
        event Action<bool> FireTriggeredEvent;

        void Enable();
        public Vector2 MoveAxis { get; }
        public Vector2 LookAxis { get; }
    }
}