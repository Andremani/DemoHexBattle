using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using Wunderwunsch.HexMapLibrary;
using Andremani.DemoHexBattle.Enums;
using Andremani.DemoHexBattle.AttackTypes;

namespace Andremani.DemoHexBattle
{
    public class BattleManager : MonoBehaviour
    {
        //хранить список юнитов (в инспекторе разбить на 2 армии двух игроков - листы с типом юнита и кол-вом)
        //размещать на карте в соответстии с выбранным построением (правое окружное, левое окружное, распеределённое)

        //передавать ходы

        //обрабатывать ввод и делать действия (говорить карте что делать и с кем)

        [SerializeField] private List<CreatureStack> leftSideStacks;
        [SerializeField] private List<CreatureStack> rightSideStacks;
        const int maxAmountOfUnitsFromOnOneSide = 7;

        [SerializeField] private BattleGrid battleGrid;
        [SerializeField] private Button skipTurn;

        private bool isGameOver;
        private List<Unit> unitsQueue;
        private int currentUnitIndexInQueue;
        private Unit currentUnit;

        private List<AttackType> avaliableAttackTypes;
        private AttackType currentAttackType;

        public delegate void CallbackDelegate();

        public static event Action<BattleSide> OnEndGame;

        /// <summary>
        /// There can be maximum %maxAmountOfUnits% units in army
        /// </summary>
        private void OnValidate()
        {
            if(leftSideStacks.Count > maxAmountOfUnitsFromOnOneSide)
            {
                leftSideStacks.RemoveAt(maxAmountOfUnitsFromOnOneSide);
            }
            if (rightSideStacks.Count > maxAmountOfUnitsFromOnOneSide)
            {
                rightSideStacks.RemoveAt(maxAmountOfUnitsFromOnOneSide);
            }
        }

        private void OnEnable()
        {
            Unit.OnUnitDeath += OnUnitDeath;
            skipTurn.onClick.AddListener(OnNextTurn);
        }

        private void OnDisable()
        {
            Unit.OnUnitDeath -= OnUnitDeath;
            skipTurn.onClick.RemoveListener(OnNextTurn);
        }

        void Start()
        {
            battleGrid.Init();
            DeployUnits();

            OnNextTurn();
        }

        void Update()
        {
            if(isGameOver)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (battleGrid.hexMouse.CursorIsOnMap && !battleGrid.IsMovementHappening)
                {
                    MoveOrAttack();
                }
            }
        }

        private void DeployUnits()
        {
            unitsQueue = new List<Unit>();
            Formation formation = new Formation(battleGrid.GetMapSizeData());

            for (int i = 0; i < leftSideStacks.Count; i++)
            {
                CreatureStack currentStack = leftSideStacks[i];
                Vector3Int newUnitPosition = battleGrid.CreateUnit(formation.GetPositionForLeftStack(i), currentStack.CreatureType.StackPrefab);

                Unit newUnit = new Unit(currentStack.CreatureType, currentStack.Amount, BattleSide.Left, newUnitPosition);
                unitsQueue.Add(newUnit);
            }
            for (int i = 0; i < rightSideStacks.Count; i++)
            {
                CreatureStack currentStack = rightSideStacks[i];
                Vector3Int newUnitPosition = battleGrid.CreateUnit(formation.GetPositionForRightStack(i), currentStack.CreatureType.StackPrefab);

                Unit newUnit = new Unit(currentStack.CreatureType, currentStack.Amount, BattleSide.Right, newUnitPosition);
                unitsQueue.Add(newUnit);
            }

            SortUnitsByInitiative();
            currentUnitIndexInQueue = -1; //'next turn' will make index 0, which means starting game with biggest initiative

            StartCoroutine(UpdateUnitAmountsUI()); //crutch for updating UI, but no time
        }

        private void SortUnitsByInitiative()
        {
            unitsQueue = unitsQueue.OrderByDescending(u => u.ActualInitiative).ToList();
        }

        private IEnumerator UpdateUnitAmountsUI()
        {
            yield return null;
            foreach(Unit unit in unitsQueue)
            {
                unit.UpdateCreaturesAmount();
            }
        }

        private void OnNextTurn()
        {
            if (isGameOver)
            {
                return;
            }

            currentUnitIndexInQueue++;
            if(currentUnitIndexInQueue >= unitsQueue.Count)
            {
                currentUnitIndexInQueue = 0;
            }
            currentUnit = unitsQueue[currentUnitIndexInQueue];

            Color colorOfBorders = currentUnit.BattleSide.GetColor();
            battleGrid.DrawReachableTiles(currentUnit.Coodinates, currentUnit.ActualSpeed, colorOfBorders);

            SetAttackType();
        }

        private void SetAttackType()
        {
            //установка текущего типа атаки из доступных. Проверка на то что кто-то стоит рядом - убирание дальнего боя
            avaliableAttackTypes = new List<AttackType>();
            if (currentUnit.CreatureType.AttackTypes == null)
            {
                currentAttackType = new MeleeAttack();
                return;
            }
            foreach (var attackType in currentUnit.CreatureType.AttackTypes)
            {
                if (attackType is IRangedAttack)
                {
                    bool blockedByEnemyUnit = false;

                    //проверка на рядом стоящих

                    if (!blockedByEnemyUnit)
                    {
                        avaliableAttackTypes.Add(attackType);
                    }
                    continue;
                }

                avaliableAttackTypes.Add(attackType);
            }
            if (avaliableAttackTypes.Count == 0)
            {
                avaliableAttackTypes.Add(new MeleeAttack());
            }

            AttackType rangedAttack = avaliableAttackTypes.FirstOrDefault(attackType => attackType is RangedAttack);
            if (rangedAttack != null)
            {
                currentAttackType = rangedAttack;
            }
            else
            {
                currentAttackType = avaliableAttackTypes[0];
            }
            //включение кнопки на переключение типов атаки если доступных несколько
        }

        private void MoveOrAttack()
        {
            Vector3Int selectedTile = battleGrid.hexMouse.TileCoord; //tile coords are cube coordinates
        
            bool isEnemyUnitOnSelectedTile = false;
            Unit enemyUnit = null;

            Unit unitInSelectedTile = unitsQueue.SingleOrDefault(u => u.Coodinates == selectedTile);
            if(unitInSelectedTile == null)
            {
                isEnemyUnitOnSelectedTile = false;
            }
            else
            {
                if(currentUnit.BattleSide == unitInSelectedTile.BattleSide)
                {
                    isEnemyUnitOnSelectedTile = false;
                }
                else
                {
                    isEnemyUnitOnSelectedTile = true;
                    enemyUnit = unitInSelectedTile;
                }
            }

            if (isEnemyUnitOnSelectedTile)
            {
                currentAttackType.Attack(currentUnit, enemyUnit, battleGrid, this, () => OnNextTurn());
            }
            else
            {
                MoveCurrentUnit(selectedTile, () => OnNextTurn());
            }
        }

        public void MoveCurrentUnit(Vector3Int selectedTile, CallbackDelegate callback = null)
        {
            Vector2Int offsetStartPos = HexConverter.TileCoordToOffsetTileCoord(currentUnit.Coodinates);
            Vector3Int startPos = new Vector3Int(offsetStartPos.x, offsetStartPos.y, 0);

            Vector2Int offsetSelectedCoord = HexConverter.TileCoordToOffsetTileCoord(selectedTile);
            int endPassability = battleGrid.IsTilePassable(selectedTile);
            Vector3Int endPos = new Vector3Int(offsetSelectedCoord.x, offsetSelectedCoord.y, endPassability);

            if (battleGrid.CreatePath(startPos, endPos, currentUnit.ActualSpeed))
            {
                //battleGrid.ClearPath();
                //battleGrid.DrawPath();
                battleGrid.MoveUnit(currentUnit.Coodinates, selectedTile, () =>
                {
                    callback?.Invoke();
                
                });
                currentUnit.Coodinates = selectedTile;
            }
        }

        private void OnUnitDeath(Unit deadUnit)
        {
            battleGrid.DeleteUnit(deadUnit.Coodinates);
            unitsQueue.Remove(deadUnit);

            CheckForEndGame();
        }

        private void CheckForEndGame()
        {
            bool thereAreUnitsOnLeftSide = false;
            bool thereAreUnitsOnRightSide = false;
            foreach (Unit unit in unitsQueue)
            {
                if (unit.BattleSide == BattleSide.Left)
                {
                    thereAreUnitsOnLeftSide = true;
                }
                if (unit.BattleSide == BattleSide.Right)
                {
                    thereAreUnitsOnRightSide = true;
                }
            }
            if (!(thereAreUnitsOnLeftSide && thereAreUnitsOnRightSide))
            {
                if (thereAreUnitsOnRightSide)
                {
                    GameOver(BattleSide.Right);
                }
                else
                {
                    GameOver(BattleSide.Left);
                }
            }
        }    

        private void GameOver(BattleSide winner)
        {
            isGameOver = true;
            OnEndGame?.Invoke(winner);
        }
    }
}