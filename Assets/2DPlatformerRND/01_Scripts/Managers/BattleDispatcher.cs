using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class BattleDispatcher : MonoBehaviour
    {
        public UnityEvent<AttackResult> EventOnHitResult = new UnityEvent<AttackResult>();
        public UnityEvent<AttackResult> EventOnKillResult = new UnityEvent<AttackResult>();

        public void DispatchAttackResult(AttackResult result)
        {
            EventOnHitResult.Invoke(result);
            if (result.IsKilled)
            {
                EventOnKillResult.Invoke(result);
            }
        }
    }
}