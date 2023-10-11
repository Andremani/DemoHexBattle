using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Andremani.DemoHexBattle.Enums;

namespace Andremani.DemoHexBattle.UI
{
    public class WinPanelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winMessageText;

        private void Start()
        {
            gameObject.SetActive(false);
            BattleManager.OnEndGame += OnEndGame;
        }

        private void OnEndGame(BattleSide winner)
        {
            gameObject.SetActive(true);
            winMessageText.text = "Game over! " + winner.ToString() + " player won!";
        }
    }
}