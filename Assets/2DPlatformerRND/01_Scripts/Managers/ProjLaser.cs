using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ProjLaser : ProjectileBase
    {
        [SerializeField] ProjectileBase _ReflectLaser = null;

        List<IReactableLaser> mLaserReactors = new List<IReactableLaser>();

        protected override void Update()
        {
            base.Update();
            AffectLaserReflect();
        }

        public override void DoEndProjectile()
        {
            base.DoEndProjectile();
        }

        void AffectLaserReflect()
        {
            mLaserReactors.Clear();
            UtilitiesPhy2D.OverlapBox(mBaseObj.Body.Rect, 1 << gameObject.layer, mLaserReactors);
            foreach (IReactableLaser laserRector in mLaserReactors)
            {
                laserRector.OnReactLaserReflection(this);

                // Vector2 pos = laserRector.ReflectPos;
                // Vector2 dir = laserRector.ReflectDir;
                // CreateReflectLaser(pos, dir);
            }
        }

        public ProjectileBase CreateReflectLaser(Vector2 pos, Vector2 dir)
        {
            // 스킬 오브젝트 생성
            ProjectileBase proj = ProjectileBase.Create(_ReflectLaser, pos, dir, mBaseObj.gameObject.layer);
            proj.OnHit.AddListener((col) =>
            {
                OnHit?.Invoke(col);
            });
            return proj;
        }
    }
}