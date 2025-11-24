using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class BuffController : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        public BuffOption TotalBuffEffect { get; private set; } = new BuffOption();

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