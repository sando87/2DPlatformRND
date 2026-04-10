using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsViewer : MonoBehaviour
    {
        [SerializeField] Transform[] _FieldRows;

        public List<FieldData> Data { get; set; } = new List<FieldData>();

        public void Show()
        {
            gameObject.SetActive(true);

            for (int i = 0; i < _FieldRows.Length; i++)
            {
                if (i < Data.Count)
                {
                    _FieldRows[i].gameObject.SetActive(true);
                    SetFieldData(_FieldRows[i], Data[i]);
                }
                else
                {
                    _FieldRows[i].gameObject.SetActive(false);
                }
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void SetFieldData(Transform field, FieldData data)
        {
            field.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.Name;
            field.GetChild(1).GetComponent<TextMeshProUGUI>().text = data.Value;
        }
    }
}