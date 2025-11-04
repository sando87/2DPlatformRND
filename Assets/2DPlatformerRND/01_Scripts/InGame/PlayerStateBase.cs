using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateBase : FiniteStateBase
    {
        public PlayerUnitInput PlayerInput { get => GetComponentInParent<PlayerUnitInput>(); }
        public PlayerMain PlayerMain { get => GetComponentInParent<PlayerMain>(); }
    }
}
