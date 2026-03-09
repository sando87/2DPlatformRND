using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ProjFrozenOrb : ProjectileBase
    {
        [SerializeField] ProjectileBase _IceShard = null;
        [SerializeField] Transform _RotatePivot = null;
        [SerializeField] float _RotateSpeed = 700;
        [SerializeField] float _FireInterval = 0.2f;

        protected override void Awake()
        {
            base.Awake();

            mInteractCollider.LockInteract = true;
        }

        public override void StartProjectile()
        {
            base.StartProjectile();

            _RotatePivot.DORotate(new Vector3(0, 0, _RotateSpeed), 1, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

            this.ExRepeatCoroutine(_FireInterval, DoFireIceShard);
        }

        void DoFireIceShard()
        {
            CreateSkillProj();
        }

        public override void DoEndProjectile()
        {
            base.DoEndProjectile();

            _RotatePivot.DOKill();
            StopAllCoroutines();
        }

        public ProjectileBase CreateSkillProj()
        {
            // 스킬 오브젝트 생성
            Vector2 startPos = _RotatePivot.transform.position;
            ProjectileBase proj = ProjectileBase.Create(_IceShard, startPos, _RotatePivot.right, mBaseObj.gameObject.layer);
            proj.OnHit.AddListener((col) =>
            {
                OnHit?.Invoke(col);
                proj.DoEndProjectile();
            });
            return proj;
        }
    }
}