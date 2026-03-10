using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework.Constraints;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElementFireAffector : MonoBehaviour
{
    [SerializeField] BoxCollider2D _AffectArea = null;
    [SerializeField] LayerMask _AffectLayers = 0;

    List<IReactableFire> mFireReactors = new List<IReactableFire>();

    void Update()
    {
        AffectFireElement();
    }

    public void AffectFireElement()
    {
        UtilitiesPhy2D.OverlapBox(_AffectArea.ExToRect(), _AffectLayers, mFireReactors);
        foreach (IReactableFire fireRector in mFireReactors)
        {
            fireRector.OnReactFire(this);
        }
    }

}