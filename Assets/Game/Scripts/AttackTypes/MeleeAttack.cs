using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andremani.DemoHexBattle.AttackTypes
{
    [CreateAssetMenu(menuName = "Scriptables/Melee Attack", fileName = "Melee Attack", order = 26)]
    public class MeleeAttack : AttackType
    {
        public override void Attack(Unit attacker, Unit defender, BattleGrid battleGrid, BattleManager battleManager, CallbackDelegate callback)
        {
            //������� � ����������
            Vector3Int closestEdgeCoord = battleGrid.hexMouse.ClosestEdgeCoord;
            List<Vector3Int> tilesConnectedToClosestEdge = battleGrid.GetTilesOnEdge(closestEdgeCoord);
            tilesConnectedToClosestEdge.Remove(defender.Coodinates);
            Vector3Int movementTargetTile = attacker.Coodinates;
            if (tilesConnectedToClosestEdge.Count > 0)
            {
                movementTargetTile = tilesConnectedToClosestEdge[0];
            }
            else
            {
                return; //off the map, no melee attack possible
            }

            if (attacker.Coodinates == movementTargetTile)
            {
                MeleeAttacking(attacker, defender, callback);
            }
            else
            {
                battleManager.MoveCurrentUnit(movementTargetTile, () =>
                {
                    MeleeAttacking(attacker, defender, callback);
                });
            }
        }

        private void MeleeAttacking(Unit attacker, Unit defender, CallbackDelegate callback)
        {
            //��������� ������� ���� �����
            int damageFromAttacker = attacker.GetUnitAttackDamage();
            //���������� ����������� ���������� �� ������� ��� ��� ����������� ��� �����

            //���������� ����������� ������������� �� ������� ��� ��� ����������� ��� �������

            //���������� ����� ��������� -> //��������� ������ ������ -> //�������� ������ ����� �� ������
            defender.ApplyDamage(attacker, damageFromAttacker, this);
            callback?.Invoke();
        }
    }
}