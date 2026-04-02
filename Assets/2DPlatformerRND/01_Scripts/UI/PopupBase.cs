using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupBase : MonoBehaviour
    {
        public UIPartsHandler[] mUIParts;
        public PlayerUnitInput PlayerInput { get; set; }
        public int CharacterID { get => PlayerInput.ExGetBase().PlayerObj.CharacterID; }

        public UIPartsHandler CurrentSelectedPart { get; private set; }

        public void UpdateUIParts()
        {
            mUIParts = GetComponentsInChildren<UIPartsHandler>();
        }

        void Update()
        {
            if (PlayerInput != null && PlayerInput.JustPressed(PlayerUnitInputType.UIMove))
            {
                Vector2 moveDir = PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.UIMove);
                if (moveDir.magnitude > 0.1f)
                {
                    Move(moveDir.normalized);
                }
            }
        }

        void Move(Vector2 dir)
        {
            if (mUIParts == null || mUIParts.Length == 0)
                return;

            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current == null)
            {
                CurrentSelectedPart = mUIParts[0];
                EventSystem.current.SetSelectedGameObject(mUIParts[0].gameObject);
                return;
            }

            RectTransform currentRect = current.GetComponent<RectTransform>();
            Vector3 currentPos = currentRect.position;

            UIPartsHandler best = null;
            float bestScore = float.MaxValue;

            foreach (var btn in mUIParts)
            {
                if (btn.gameObject == current) continue;

                Vector3 dirToTarget = btn.transform.position - currentPos;

                // 방향 체크 (위쪽인지 등)
                if (Vector2.Dot(dir, dirToTarget.normalized) < 0.5f)
                    continue;

                float distance = dirToTarget.sqrMagnitude;

                if (distance < bestScore)
                {
                    bestScore = distance;
                    best = btn;
                }
            }

            if (best != null)
            {
                CurrentSelectedPart = best;
                EventSystem.current.SetSelectedGameObject(best.gameObject);
            }
        }
    }


}