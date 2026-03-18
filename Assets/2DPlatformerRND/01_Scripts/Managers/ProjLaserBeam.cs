using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ProjLaserBeam : ProjectileBase
    {
        [SerializeField] ProjLaserBeam _ReflectLaser = null;

        List<IReactableLaser> mTempReactors = new List<IReactableLaser>();
        bool mIsReflected = false;
        public IReactableLaser ReflectedFrom { get; set; } = null;

        protected override void Update()
        {
            base.Update();

            if (!mIsReflected)
            {
                IReactableLaser reactableObj = FindReflectable();
                if (reactableObj != null)
                {
                    mIsReflected = true;

                    reactableObj.OnReactLaserReflection(this);
                    Vector2 dir = reactableObj.ReflectDir;

                    ProjLaserBeam newLaser = CreateReflectLaser(transform.position, dir);
                    newLaser.ReflectedFrom = reactableObj;

                    DoEndProjectile();
                }
            }
        }

        IReactableLaser FindReflectable()
        {
            mTempReactors.Clear();
            UtilitiesPhy2D.OverlapBox(mBaseObj.Body.Rect, 1 << gameObject.layer, mTempReactors);

            foreach (IReactableLaser laserRector in mTempReactors)
            {
                if (laserRector == ReflectedFrom)
                    continue;

                return laserRector;
            }

            return null;
        }

        public ProjLaserBeam CreateReflectLaser(Vector2 pos, Vector2 dir)
        {
            // 스킬 오브젝트 생성
            ProjLaserBeam proj = Create(_ReflectLaser, pos, dir, mBaseObj.gameObject.layer) as ProjLaserBeam;
            proj.transform.ExSetWorldPosZ(-0.1f);
            proj.OnHit.AddListener((col) =>
            {
                OnHit?.Invoke(col);
            });
            return proj;
        }
    }
}