using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupBase : MonoBehaviour
    {
        public UIInputHandler InputHandler { get; private set; }

        void Awake()
        {
            InputHandler = GetComponent<UIInputHandler>();
            InputHandler.EventCancel = OnCancel;
        }

        protected virtual void OnCancel()
        {
            PopupManager.Instance.Close(this);
        }


    }


}