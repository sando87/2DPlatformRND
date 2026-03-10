using PahlBit;
using UnityEngine;
using UnityEngine.Events;

public class ElementFireReactor : MonoBehaviour, IReactableFire
{
    [SerializeField] UnityEvent OnReactFire = null;

    bool mIsReacted = false;

    void IReactableFire.OnReactFire(ElementFireAffector affector)
    {
        if (!mIsReacted)
        {
            mIsReacted = true;
            OnReactFire?.Invoke();
        }
    }
}