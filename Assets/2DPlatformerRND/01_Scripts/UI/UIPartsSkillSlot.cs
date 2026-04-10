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

        private Image mIcon = null;
        private TextMeshProUGUI mText = null;
        public SkillBase SKill { get; private set; } = null;

        void Awake()
        {
            SKill = InGameManager.Instance.Engine.Player.GetComponentInChildren<SkillController>().GetSkill(SkillID);

            mText = transform.GetComponentInChildren<TextMeshProUGUI>();
            mIcon = transform.GetChild(0).GetComponent<Image>();
            mIcon.sprite = SKill.Icon;

            UpdateSkillState();
        }

        public void UpdateSkillState()
        {
            if (!SKill.IsLearned)
            {
                mIcon.color = Color.gray;
                mText.color = Color.gray;
            }
            else if (SKill.IsEquipped)
            {
                mIcon.color = Color.green;
                mText.color = Color.black;
            }
            else
            {
                mIcon.color = Color.white;
                mText.color = Color.black;
            }

            if (SKill.IsLearned)
                mText.SetText($"{SkillID}\nLv.{SKill.Level}\n({SKill.CurrentSubStep}/{SKill.MaxSubStep})");
            else
                mText.SetText(SkillID);
        }
    }
}