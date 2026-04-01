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
        [Foldout("Events")] public UnityEvent EventSelect = new UnityEvent();
        [Foldout("Events")] public UnityEvent EventDeselect = new UnityEvent();
        [Foldout("Events")] public UnityEvent EventSubmit = new UnityEvent();

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