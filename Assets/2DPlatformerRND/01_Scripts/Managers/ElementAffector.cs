using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElementAffector : MonoBehaviour
{
    Collider2D[] mTempColList = new Collider2D[16];

    public void AffectFireElement(BoxCollider2D area, int layerMask)
    {
        Rect hitBox = area.ExToRect();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = layerMask;
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;
        int ret = Physics2D.OverlapBox(hitBox.center, hitBox.size, 0, contactFilter, mTempColList);
        if (ret > 0)
        {
            for (int i = 0; i < ret; ++i)
            {
                Collider2D col = mTempColList[i];
                BaseObject baseObj = col.GetComponentInParent<BaseObject>();
                if (baseObj != null)
                {
                    IReactableFire[] skillFireables = baseObj.GetComponentsInChildren<IReactableFire>();
                    foreach (IReactableFire skillFireable in skillFireables)
                    {
                        skillFireable.OnReactFire(this);
                    }
                }
            }
        }
    }

    public void AffectLaserElement(BoxCollider2D area, int layerMask)
    {
        Rect hitBox = area.ExToRect();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = layerMask;
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;
        int ret = Physics2D.OverlapBox(hitBox.center, hitBox.size, 0, contactFilter, mTempColList);
        if (ret > 0)
        {
            for (int i = 0; i < ret; ++i)
            {
                Collider2D col = mTempColList[i];
                BaseObject baseObj = col.GetComponentInParent<BaseObject>();
                if (baseObj != null)
                {
                    IReactableLaser[] skillFireables = baseObj.GetComponentsInChildren<IReactableLaser>();
                    foreach (IReactableLaser skillFireable in skillFireables)
                    {
                        skillFireable.OnReactLaser(this);
                    }
                }
            }
        }
    }
}
public interface IReactableFire
{
    void OnReactFire(ElementAffector affector);
}

public interface IReactableLaser
{
    void OnReactLaser(ElementAffector affector);
}