using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecBase : MonoBehaviour
    {
        public virtual float MaxHealth { get; }
        public virtual float MaxMana { get; }
        public virtual float MaxShield { get; }

        public SpecOption Option { get; set; } = new SpecOption();
    }
}