using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class InteractableCollider : MonoBehaviour
{
    [SerializeField] InteractMask _MyProperty = InteractMask.Everything;
    [SerializeField] InteractMask _InteractableWith = InteractMask.Everything;

    [SerializeField] UnityEvent<Collider2D> OnInteractEnter;
    [SerializeField] UnityEvent<Collider2D> OnInteractLeave;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInteractable(collision.collider))
        {
            DoInteractEnter(collision.collider);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (IsInteractable(collision.collider))
        {
            DoInteractLeave(collision.collider);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInteractable(collision))
        {
            DoInteractEnter(collision);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInteractable(collision))
        {
            DoInteractLeave(collision);
        }
    }
    
    private bool IsInteractable(Collider2D other)
    {
        InteractableCollider opp = other.GetComponent<InteractableCollider>();
        if (opp == null)
            return false;

        InteractMask mask = _InteractableWith & opp._MyProperty;
        return mask != InteractMask.Nothing;
    }
    private void DoInteractEnter(Collider2D other)
    {
        OnInteractEnter?.Invoke(other);
    }
    private void DoInteractLeave(Collider2D other)
    {
        OnInteractLeave?.Invoke(other);
    }

}

[System.Flags]
public enum InteractMask : uint
{
    Nothing = 0,
    Player = 1 << 0,
    Enemy = 1 << 1,
    Terrain = 1 << 2,
    Neutral = 1 << 3,
    Everything = 0xffffffff
}