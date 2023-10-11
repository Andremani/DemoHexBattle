using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.DemoHexBattle.AttackTypes
{
    [CreateAssetMenu(menuName = "Scriptables/Ranged Attack", fileName = "New Ranged Attack", order = 27)]
    public class RangedAttack : AttackType, IRangedAttack
    {
        [SerializeField] private int minRangedDamage;
        [SerializeField] private int maxRangedDamage;

        public override void Attack(Unit attacker, Unit defender, BattleGrid battleGrid, BattleManager battleManager, CallbackDelegate callback)
        {
            int damageFromAttacker = Random.Range(minRangedDamage, maxRangedDamage + 1) * attacker.CreaturesAmount;
            defender.ApplyDamage(attacker, damageFromAttacker, this);
            callback?.Invoke();
        }
    }
}