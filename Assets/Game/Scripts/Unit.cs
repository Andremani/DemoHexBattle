using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Andremani.DemoHexBattle.Enums;
using Andremani.DemoHexBattle.AttackTypes;

namespace Andremani.DemoHexBattle
{
    public class Unit
    {
        private CreatureType creatureType;
        private int creaturesAmount;
        private BattleSide battleSide;
        private List<Ability> abilities;
        private int actualCreatureHealth;
        private int currentHealthRemainder;
        private int actualMinDamage;
        private int actualMaxDamage;
        private int actualInitiative;
        private int actualSpeed;
        private Vector3Int coordinates; //cubic coordinates on hex grid

        public CreatureType CreatureType => creatureType;
        public int CreaturesAmount => creaturesAmount;
        public BattleSide BattleSide => battleSide;
        //public int ActualMinDamage => actualMinDamage;
        //public int ActualMaxDamage => actualMaxDamage;
        public int ActualInitiative => actualInitiative;
        public int ActualSpeed => actualSpeed;
        public Vector3Int Coodinates { get { return coordinates; } set { coordinates = value; } }

        public static event Action<Unit, int, AttackType> OnDamageRecieved;
        public static event Action<Unit, int> OnCreaturesAmountChanged;
        public static event Action<Unit> OnUnitDeath;

        public Unit(CreatureType creatureType, int creaturesAmount, BattleSide battleSide, Vector3Int unitCoordinates)
        {
            this.creatureType = creatureType;
            this.creaturesAmount = creaturesAmount;
            this.battleSide = battleSide;
            this.coordinates = unitCoordinates;
            //creating abilities from enum in CreatureType
            actualCreatureHealth = creatureType.Health;
            currentHealthRemainder = creatureType.Health;
            actualMinDamage = creatureType.MinDamage;
            actualMaxDamage = creatureType.MaxDamage;
            actualInitiative = creatureType.Initiative;
            actualSpeed = creatureType.Speed;

            //when init, apply permanent abilities to actual stats and set hooks?
        }

        public void UpdateCreaturesAmount()
        {
            OnCreaturesAmountChanged?.Invoke(this, creaturesAmount);
        }

        public int GetUnitAttackDamage()
        {
            return UnityEngine.Random.Range(actualMinDamage, actualMaxDamage + 1) * creaturesAmount;
        }

        public void ApplyDamage(Unit attacker, int damage, AttackType attackType)
        {
            OnDamageRecieved?.Invoke(this, damage, attackType);

            int damageRemainder = damage - currentHealthRemainder;

            if (damageRemainder < 0)
            {
                currentHealthRemainder -= damage;
                //damage has been less than healthRemainder, no 1 creature died
            }
            else
            {
                int killed = 1 + damageRemainder / actualCreatureHealth;
                int newHealthRemainder = actualCreatureHealth - damageRemainder % actualCreatureHealth;

                creaturesAmount -= killed;
                OnCreaturesAmountChanged?.Invoke(this, creaturesAmount);
                currentHealthRemainder = newHealthRemainder;
                if (creaturesAmount <= 0)
                {
                    UnitDeath();
                }
            }
        }

        private void UnitDeath()
        {
            //event?
            //disappearing from grid - here or in battleManager? (second better supposedly)
            OnUnitDeath?.Invoke(this);
        }
    }
}