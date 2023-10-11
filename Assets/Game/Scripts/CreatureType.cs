using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andremani.DemoHexBattle.Enums;
using Andremani.DemoHexBattle.AttackTypes;

namespace Andremani.DemoHexBattle
{
    [CreateAssetMenu(menuName = "Scriptables/Creature", fileName = "New Creature", order = 25)]
    public class CreatureType : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int MinDamage { get; private set; }
        [field: SerializeField] public int MaxDamage { get; private set; }
        [field: SerializeField] public int Initiative { get; private set; }
        [field: SerializeField] public int Speed { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Race Race { get; private set; }
        [field: SerializeField] public List<AttackType> AttackTypes { get; private set; }
        [field: SerializeField] public List<AbilityType> Abilities { get; private set; }
        [field: SerializeField] public GameObject StackPrefab { get; private set; }
    }
}