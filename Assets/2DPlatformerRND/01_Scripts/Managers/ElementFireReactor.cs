using UnityEngine;
using UnityEngine.Events;

public class ElementFireReactor : MonoBehaviour, IReactableFire
{
    [SerializeField] UnityEvent OnReactFire = null;

    bool mIsReacted = false;

    void IReactableFire.OnReactFire(ElementAffector affector)
    {
        if (!mIsReacted)
        {
            mIsReacted = true;
            OnReactFire?.Invoke();
        }
    }
}