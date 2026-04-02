using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public enum UIActionType { Learn, Use, UnUse, Enforce, Equip, UnEquip, Sell, Buy, Dump }

    public class UIPartsActions : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _FieldName;
        [SerializeField] TextMeshProUGUI _FieldValue;

        void Awake()
        {
        }

        public void SetField(string fieldName, string fieldValue)
        {
            _FieldName.text = fieldName;
            _FieldValue.text = fieldValue;
        }
    }
}