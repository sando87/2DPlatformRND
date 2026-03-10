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
        [SerializeField] ProjLaser _ReflectLaser = null;

        public int UpdateIndex { get; private set; } = 0;
        public IReactableLaser ReflectedFrom { get; private set; } = null;

        Dictionary<IReactableLaser, ProjLaser> mReflectionPair = new Dictionary<IReactableLaser, ProjLaser>();

        List<IReactableLaser> mTempReactors = new List<IReactableLaser>();
        List<IReactableLaser> mRemoveKeys = new List<IReactableLaser>();
        private int mCurrentUpdateIndex = 0;

        protected override void Update()
        {
            base.Update();

            mCurrentUpdateIndex++;
            AffectLaserReflect();
            RemoveOldReflections();
        }

        public override void DoEndProjectile()
        {
            base.DoEndProjectile();

            foreach (var pair in mReflectionPair)
            {
                pair.Value.DoEndProjectile();
            }
            mReflectionPair.Clear();
        }

        void UpdateReflection(Vector2 pos, Vector2 dir, int updateIndex, IReactableLaser reflectedFrom)
        {
            mBaseObj.transform.position = pos;
            mBaseObj.transform.right = dir;
            UpdateIndex = updateIndex;
            ReflectedFrom = reflectedFrom;
        }

        void AffectLaserReflect()
        {
            mTempReactors.Clear();
            UtilitiesPhy2D.OverlapBox(mBaseObj.Body.Rect, 1 << gameObject.layer, mTempReactors);
            foreach (IReactableLaser laserRector in mTempReactors)
            {
                if (laserRector == ReflectedFrom)
                    continue;

                laserRector.OnReactLaserReflection(this);
                Vector2 pos = laserRector.ReflectPos;
                Vector2 dir = laserRector.ReflectDir;

                if (mReflectionPair.ContainsKey(laserRector))
                {
                    mReflectionPair[laserRector].UpdateReflection(pos, dir, mCurrentUpdateIndex, laserRector);
                }
                else
                {
                    ProjLaser newLaser = CreateReflectLaser(pos, dir);
                    mReflectionPair[laserRector] = newLaser;
                    newLaser.UpdateReflection(pos, dir, mCurrentUpdateIndex, laserRector);
                }
            }
        }

        void RemoveOldReflections()
        {
            mRemoveKeys.Clear();
            foreach (var pair in mReflectionPair)
            {
                if (pair.Value.UpdateIndex != mCurrentUpdateIndex)
                {
                    pair.Value.DoEndProjectile();
                    mRemoveKeys.Add(pair.Key);
                }
            }

            for (int i = 0; i < mRemoveKeys.Count; i++)
            {
                mReflectionPair.Remove(mRemoveKeys[i]);
            }
        }

        public ProjLaser CreateReflectLaser(Vector2 pos, Vector2 dir)
        {
            // 스킬 오브젝트 생성
            ProjectileBase proj = Create(_ReflectLaser, pos, dir, mBaseObj.gameObject.layer);
            proj.OnHit.AddListener((col) =>
            {
                OnHit?.Invoke(col);
            });
            return proj as ProjLaser;
        }
    }
}