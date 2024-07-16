using System;
using Zenject;
using UnityEngine;

namespace Services.Input
{
    public class InputService : IInputService, IDisposable
    {
        public event Action<bool> FireTriggeredEvent;
        public Vector2 MoveAxis => GetInputAxis(MoveHorizontal, MoveVertical);
        public Vector2 LookAxis => GetInputAxis(LookHorizontal, LookVertical);

        private const string MoveHorizontal = "Horizontal";
        private const string MoveVertical = "Vertical";
        private const string LookHorizontal = "ShootHorizontal";
        private const string LookVertical = "ShootVertical";

        public void Enable()
        {
            SimpleInput.FireTriggeredEvent += OnFireTriggeredEvent;
        }

        public void Dispose()
        {
            SimpleInput.FireTriggeredEvent -= OnFireTriggeredEvent;
        }

        private void OnFireTriggeredEvent(bool state) => FireTriggeredEvent?.Invoke(state);
        
        private static Vector2 GetInputAxis(string horizontal, string vertical) =>
            new(SimpleInput.GetAxis(horizontal), SimpleInput.GetAxis(vertical));
    }
}