using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIListViewer : MonoBehaviour
    {
        [SerializeField] Transform[] _FieldRows;

        List<string> mData = null;

        public static UIListViewer Show(UIListViewer prefab, Transform parent, List<string> data)
        {
            UIListViewer viewer = Instantiate(prefab, parent);
            viewer.mData = data;
            viewer.Show();
            return viewer;
        }

        public void Show()
        {
            for (int i = 0; i < _FieldRows.Length; i++)
            {
                if (i < mData.Count)
                {
                    SetFieldData(_FieldRows[i], mData[i]);
                }
                else
                {
                    _FieldRows[i].gameObject.SetActive(false);
                }
            }
        }

        public void Hide()
        {
            Destroy(gameObject);
        }

        void SetFieldData(Transform field, string data)
        {
            string[] cols = data.Split(',');
            TextMeshProUGUI[] texts = field.GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (i < cols.Length)
                {
                    texts[i].text = cols[i];
                }
                else
                {
                    texts[i].text = "";
                }
            }
        }
    }
}