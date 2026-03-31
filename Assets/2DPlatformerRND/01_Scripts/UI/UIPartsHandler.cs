using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        public UnityEvent EventSelect = new UnityEvent();
        public UnityEvent EventDeselect = new UnityEvent();
        public UnityEvent EventSubmit = new UnityEvent();

        public void OnDeselect(BaseEventData eventData)
        {
            EventDeselect.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            EventSelect.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            EventSubmit.Invoke();
        }
    }
}