using PahlBit;
using UnityEngine;
using UnityEngine.Events;

public class ElementLaserReactor : MonoBehaviour, IReactableLaser
{
    [SerializeField] UnityEvent<ProjectileBase> _OnReflected = null;

    Vector2 IReactableLaser.ReflectPos => transform.position;
    Vector2 IReactableLaser.ReflectDir => transform.right;

    void IReactableLaser.OnReactLaserReflection(ProjectileBase affectorLaser)
    {
        _OnReflected?.Invoke(affectorLaser);
    }
}