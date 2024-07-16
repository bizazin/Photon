using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class FieldInfoVo
    {
        public string Key;
        [TextArea(1, 9999)] public string Value;
    }
}