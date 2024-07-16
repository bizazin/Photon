using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Models
{
    public class WindowBindingInfoVo
    {
        public Type Type;
        public Object ViewPrefab;
        public Transform Parent;
        public int OrderNumber;
        public bool IsFocusable;
        public bool IsDontDestroyOnLoad;
    }
}