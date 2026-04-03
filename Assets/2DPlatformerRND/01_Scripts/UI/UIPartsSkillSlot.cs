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

        private TextMeshProUGUI mText = null;
        public SkillBase SKill { get; private set; } = null;

        void Awake()
        {
            mText = transform.GetComponentInChildren<TextMeshProUGUI>();
            mText.text = SkillID;

            SKill = InGameEngine.Instance.Player.GetComponentInChildren<SkillController>().GetSkill(SkillID);
        }
    }
}