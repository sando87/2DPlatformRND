using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{

    public class Equipment : MonoBehaviour
    {
        public PlayerRoot PlayerRoot => GetComponentInParent<PlayerRoot>();

        public BuffOption TotalItemOption { get; private set; } = new BuffOption();

        public void Init()
        {
        }

        public void UpdateState()
        {
        }

        public void EquipItem(ItemData data)
        {
            BuffOption option = GameSystem.CalculateOption(data);
            TotalItemOption.Add(option);
        }

        public void UnEquipItem(ItemData data)
        {
            BuffOption option = GameSystem.CalculateOption(data);
            TotalItemOption.Subtract(option);
        }


    }
}