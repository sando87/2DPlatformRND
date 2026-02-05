using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountBar = null;

        public void OnDamaged(DamagedResultInfo resultInfo)
        {
            SetHealthBarRate(resultInfo.CurrentHealthRate);
        }

        public void OnDied()
        {
            SetHealthBarRate(0);
            gameObject.SetActive(false);
        }

        void SetHealthBarRate(float _rate)
        {
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountBar.transform.localScale = new Vector3(rate, 1, 1);
        }
    }
}
