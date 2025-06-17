using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    [CreateAssetMenu(menuName = "StaticData/OffsetsForUICarButtons", fileName = "OffsetsForUICarButtons")]
    public class OffsetsForUICarButtons : ScriptableObject
    {
        public List<Vector3> offsets;
    }
}
