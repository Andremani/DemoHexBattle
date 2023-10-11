using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Andremani.DemoHexBattle.AttackTypes;

namespace Andremani.DemoHexBattle.UI
{
    public class BattleLog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI logTMP;
        [SerializeField] private float messagesShowDuration = 2f;

        private Coroutine currentClearLogCoroutine;

        private void Start()
        {
            logTMP.text = "";
        }

        private void OnEnable()
        {
            Unit.OnDamageRecieved += OnUnitGetsDamage;
        }

        private void OnDisable()
        {
            Unit.OnDamageRecieved -= OnUnitGetsDamage;
        }

        private void OnUnitGetsDamage(Unit unitGotDamage, int damage, AttackType attackType)
        {
            ShowMessageForSeconds(unitGotDamage.CreatureType.Name + " got " + damage + " points of damage (" + attackType.ToString() + ")", messagesShowDuration);
        }

        private void ShowMessageForSeconds(string message, float seconds)
        {
            logTMP.text = message;
            if (currentClearLogCoroutine != null)
            {
                StopCoroutine(currentClearLogCoroutine);
            }
            currentClearLogCoroutine = StartCoroutine(LogClearRoutine(seconds));
        }

        private IEnumerator LogClearRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            logTMP.text = "";
        }
    }
}