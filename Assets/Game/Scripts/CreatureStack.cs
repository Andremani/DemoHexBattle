using UnityEngine;
using System;

namespace Andremani.DemoHexBattle
{
    [Serializable]
    public class CreatureStack
    {
        [field: SerializeField] public CreatureType CreatureType { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
    }
}