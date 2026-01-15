using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlBit
{
    public class PlayerMain : MonoBehaviour
    {
        BaseObject mBaseObj = null;
        CharObject mSpec = null;
        PlayerUnitInput mPlayerInput = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mSpec = mBaseObj.GetComponentInChildren<CharObject>();
            mPlayerInput = mBaseObj.GetComponentInChildren<PlayerUnitInput>();
        }
        void Start()
        {
            mBaseObj.Health.InitHealth(mSpec.TotalStats.Health, mSpec.TotalStats.Mana, mSpec.TotalStats.Shield);
        }

    }
}