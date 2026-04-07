using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupStats : PopupBase
    {
        [SerializeField] TextMeshProUGUI _LevelText;
        [SerializeField] TextMeshProUGUI _ExpValueText;
        [SerializeField] Image _ExpValueFill;
        [SerializeField] TextMeshProUGUI _RemainPointText;
        [SerializeField] TextMeshProUGUI _AttackPointText;
        [SerializeField] TextMeshProUGUI _DefensePointText;
        [SerializeField] TextMeshProUGUI _HealthPointText;
        [SerializeField] TextMeshProUGUI _ManaPointText;
        [SerializeField] UIPartsHandler[] _StatButtons;

        [SerializeField] Transform ContentsRoot;
        [SerializeField] UIPartsFieldRow FieldRow;

        private BaseObject mPlayer = null;
        private Experience mExperience = null;
        private List<FieldData> mDisplayFields = new List<FieldData>();

        void Start()
        {
            mPlayer = InGameEngine.Instance.Player;
            mExperience = mPlayer.GetComponentInChildren<Experience>();
            UpdateStatsPoints();

            foreach (var part in _StatButtons)
            {
                part.EventSelect += OnSelectButton;
                part.EventDeselect += OnDeselectButton;
            }

            _StatButtons[0].EventSubmit += (btn) => OnSubmitAttackPoint(1);
            _StatButtons[1].EventSubmit += (btn) => OnSubmitDefensePoint(1);
            _StatButtons[2].EventSubmit += (btn) => OnSubmitLifePoint(1);
            _StatButtons[3].EventSubmit += (btn) => OnSubmitManaPoint(1);

            UpdateDisplayInfo();
        }

        void OnSelectButton(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.green;
        }
        void OnDeselectButton(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.white;
        }

        void OnSubmitAttackPoint(int point)
        {
            mExperience.AddAttackPoint();
            UpdateStatsPoints();
        }
        void OnSubmitDefensePoint(int point)
        {
            mExperience.AddDefensePoint();
            UpdateStatsPoints();
        }
        void OnSubmitLifePoint(int point)
        {
            mExperience.AddHealthPoint();
            UpdateStatsPoints();
        }
        void OnSubmitManaPoint(int point)
        {
            mExperience.AddManaPoint();
            UpdateStatsPoints();
        }

        void UpdateStatsPoints()
        {
            const float ExpFullWidth = 580f;
            _LevelText.text = mExperience.CurrentLevel.ToString();
            _ExpValueText.text = mExperience.CurrentExp.ToString() + " / " + mExperience.ExpForNextLevel.ToString();
            _ExpValueFill.rectTransform.sizeDelta = new Vector2(mExperience.CurrentExpRate * ExpFullWidth, _ExpValueFill.rectTransform.sizeDelta.y);

            _RemainPointText.text = mExperience.RemainPoint.ToString();
            _AttackPointText.text = mExperience.AttackPoint.ToString();
            _DefensePointText.text = mExperience.DefensePoint.ToString();
            _HealthPointText.text = mExperience.HealthPoint.ToString();
            _ManaPointText.text = mExperience.ManaPoint.ToString();
        }


        void UpdateDisplayInfo()
        {
            ContentsRoot.ExDestroyAllChildren();

            SpecPlayer specPlayer = mPlayer.Spec as SpecPlayer;
            specPlayer.GetDisplayInfo(mDisplayFields);
            foreach (var field in mDisplayFields)
            {
                UIPartsFieldRow row = Instantiate(FieldRow, ContentsRoot);
                row.SetField(field.Name, field.Value);
            }
        }
    }
}