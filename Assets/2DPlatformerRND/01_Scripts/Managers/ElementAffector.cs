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
    [SerializeField] BoxCollider2D _AffectArea = null;
    [SerializeField] LayerMask _AffectLayers = 0;

    Collider2D[] mTempColList = new Collider2D[16];

    void Update()
    {
        AffectElement();
    }

    public void AffectElement()
    {
        Rect hitBox = _AffectArea.ExToRect();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = _AffectLayers;
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
                    IReactableFire[] fireReactors = baseObj.GetComponentsInChildren<IReactableFire>();
                    foreach (IReactableFire fireRector in fireReactors)
                    {
                        fireRector.OnReactFire(this);
                    }

                    IReactableLaser[] laserReactors = baseObj.GetComponentsInChildren<IReactableLaser>();
                    foreach (IReactableLaser laserRector in laserReactors)
                    {
                        laserRector.OnReactLaser(this);
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