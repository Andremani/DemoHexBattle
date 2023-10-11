using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.DemoHexBattle.AttackTypes
{
    //make attack types SO instead of trying to serialize them (and enums?)

    public abstract class AttackType : ScriptableObject
    {
        public delegate void CallbackDelegate();

        public abstract void Attack(Unit attacker, Unit defender, BattleGrid battleGrid, BattleManager battleManager, CallbackDelegate callback);
    }
}