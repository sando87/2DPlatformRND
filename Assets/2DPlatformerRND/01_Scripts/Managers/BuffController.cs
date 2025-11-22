using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class BuffController : MonoBehaviour
    {
        public PlayerRoot PlayerRoot => GetComponentInParent<PlayerRoot>();

        public PropertyEffect TotalBuffEffect { get; private set; } = new PropertyEffect();

        public void Init()
        {
        }

        public void UpdateState()
        {
        }

        public void EquipItem(string itemID)
        {

        }

        public void UnEquipItem(string itemID)
        {

        }


    }
}