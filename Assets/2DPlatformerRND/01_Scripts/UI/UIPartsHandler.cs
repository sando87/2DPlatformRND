using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        public UIInputHandler InputHandler { get; set; }

        public void OnSelect(BaseEventData eventData)
        {
            InputHandler.DispatchEventSelect(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            InputHandler.DispatchEventDeselect(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            InputHandler.DispatchEventSubmit(this);
        }
    }
}