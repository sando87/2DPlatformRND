using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsSkillSlot : UIPartsHandler
    {
        private Image mImage = null;

        void Awake()
        {
            mImage = transform.GetChild(0).GetComponent<Image>();
        }
    }
}