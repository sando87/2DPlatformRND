using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsSkillSlot : UIPartsHandler
    {
        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _SkillID = "";
        public string SkillID => _SkillID;
        List<string> IDList { get => SkillResourceTable.Instance.GetAllInfo().Select(info => info.SkillID).ToList(); }

        private Image mIconBG = null;
        private Image mIcon = null;
        private TextMeshProUGUI mText = null;
        public SkillBase SKill { get; private set; } = null;

        void Awake()
        {
            SKill = InGameManager.Instance.Engine.Player.GetComponentInChildren<SkillController>().GetSkill(SkillID);

            mText = transform.GetComponentInChildren<TextMeshProUGUI>();
            mIconBG = transform.GetChild(0).GetComponent<Image>();
            mIcon = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            mIcon.sprite = SKill.Icon;

            UpdateSkillState();
        }

        public void UpdateSkillState()
        {
            if (SKill.IsLocked)
            {
                mIconBG.color = Color.white;
                mIcon.color = Color.black;
                mText.SetText("Locked");
            }
            else if (!SKill.IsLearned)
            {
                mIconBG.color = Color.white;
                mIcon.color = Color.gray;
                mText.SetText("Unlearned");
            }
            else if (SKill.IsEquipped)
            {
                mIconBG.color = Color.green;
                mIcon.color = Color.white;
                mText.SetText($"Lv.{SKill.Level} ({SKill.CurrentSubStep}/{SKill.MaxSubStep})");
            }
            else
            {
                mIconBG.color = Color.white;
                mIcon.color = Color.white;
                mText.SetText($"Lv.{SKill.Level} ({SKill.CurrentSubStep}/{SKill.MaxSubStep})");
            }
        }
    }
}