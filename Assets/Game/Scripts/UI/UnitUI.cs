using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Wunderwunsch.HexMapLibrary;

namespace Andremani.DemoHexBattle.UI
{
    public class UnitUI : MonoBehaviour
    {
        [SerializeField] private GameObject unitInstance;
        [SerializeField] private TextMeshProUGUI amountOfCreaturesText;

        private void Start()
        {
            amountOfCreaturesText.text = "0"; //not OK value for creature stack anyway
        }

        private void OnEnable()
        {
            Unit.OnCreaturesAmountChanged += OnCreaturesAmountChanged;
        }

        private void OnDisable()
        {
            Unit.OnCreaturesAmountChanged -= OnCreaturesAmountChanged;
        }

        private void OnCreaturesAmountChanged(Unit unit, int newAmount)
        {
            if (HexConverter.CartesianCoordToTileCoord(unitInstance.transform.position) == unit.Coodinates)
            {
                amountOfCreaturesText.text = newAmount.ToString();
            }
        }
    }
}