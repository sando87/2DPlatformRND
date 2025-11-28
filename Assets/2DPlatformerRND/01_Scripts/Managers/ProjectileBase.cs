using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ProjectileBase : MonoBehaviour
    {
        private SkillObject mSkillObj = null;
        private Action<Collider2D> OnHit;

        void Awake()
        {
        } 

        void Start()
        {
        }

        public void DoCast(SkillObject skillObj, Action<Collider2D> onHit)
        {
            this.mSkillObj = skillObj;
            this.OnHit = onHit;
        }

    }
}