using TMPro;
using UnityEngine;

namespace PahlBit
{
    public class ToastManager : SingletonMono<ToastManager>
    {
        [SerializeField] Transform ContentRoot = null;
        [SerializeField] GameObject ToastMessageRow = null;

        public void ShowMessage(string message)
        {
            GameObject obj = Instantiate(ToastMessageRow, ContentRoot);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = message;
            Destroy(obj, 3.0f);
        }

    }


}