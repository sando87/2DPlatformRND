using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class Health : MonoBehaviour
    {
        public double CurrentHP { get; set; } = 0;
        public double CurrentMana { get; set; } = 0;
        public double CurrentShield { get; set; } = 0;
    }
}
