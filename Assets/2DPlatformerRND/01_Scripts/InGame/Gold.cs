using UnityEngine;
using UnityEngine.InputSystem;

public class Gold : MonoBehaviour
{
    public void OnColliderEnter(Collider2D col)
    {
        LOG.trace(col.name + " : Gold +1");
        Destroy(gameObject);
    }
}
